using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Npgsql;
using OzonEdu.MerchandiseService.Domain.SeedWork;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.ChangeTracker;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.DbConnection;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.Exceptions;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private NpgsqlTransaction _npgsqlTransaction;

        private readonly IDbConnectionFactory<NpgsqlConnection> _dbConnectionFactory;
        private readonly IPublisher _publisher;
        private readonly IChangeTracker _changeTracker;

        public UnitOfWork(
            IDbConnectionFactory<NpgsqlConnection> dbConnectionFactory,
            IPublisher publisher,
            IChangeTracker changeTracker)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
        }

        public async ValueTask StartTransaction(CancellationToken token)
        {
            if (_npgsqlTransaction is not null)
            {
                return;
            }

            var connection = await _dbConnectionFactory.CreateConnection(token);
            _npgsqlTransaction = await connection.BeginTransactionAsync(token);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (_npgsqlTransaction is null)
            {
                throw new NoActiveTransactionStartedException();
            }

            var domainEvents = new Queue<INotification>(
                _changeTracker.TrackedEntities
                    .SelectMany(x =>
                    {
                        var events = x.DomainEvents?.ToList() ?? new List<INotification>();
                        x.ClearDomainEvents();
                        return events;
                    }));

            while (domainEvents.TryDequeue(out var notification))
            {
                await _publisher.Publish(notification, cancellationToken);
            }

            await _npgsqlTransaction.CommitAsync(cancellationToken);
        }

        void IDisposable.Dispose()
        {
            _npgsqlTransaction?.Dispose();
            _dbConnectionFactory?.Dispose();
        }
    }
}
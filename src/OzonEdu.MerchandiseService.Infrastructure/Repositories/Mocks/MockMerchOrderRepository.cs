using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Mocks
{
    public class MockMerchOrderRepository : MockRepository<MerchOrder>, IMerchOrderRepository
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MockMerchOrderRepository> _logger;

        public MockMerchOrderRepository(IMediator mediator, ILogger<MockMerchOrderRepository> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;

            FillByFakedData();
            FillByFakedData();
            FillByFakedData();
        }

        public async Task<IEnumerable<OrderItemWithIssueDate>> GetCompletedByEmployeeIdAsync(int employeeId,
            CancellationToken token = default)
        {
            await Task.Delay(500);
            var items = MockSet.Where(o =>
                o.Status.Equals(OrderStatus.Completed) && o.Receiver.Id == employeeId
            ).Select(o => new OrderItemWithIssueDate(o.Item, o.StatusDateTime));

            return items;
        }

        public async Task SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var domainEvents = MockSet.SelectMany(x => x.DomainEvents).ToList();

                MockSet.ForEach(entity => entity.ClearDomainEvents());

                foreach (var domainEvent in domainEvents)
                    await _mediator.Publish(domainEvent, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }

        private readonly Random _random = new();
        private string[] firstNames = new[] {"Ivan", "Petr", "Oleg", "Alex", "Anton", "Elena", "Olga", "Svetlana"};

        private string[] lastNames = new[]
            {"Ivanov", "Petrov", "Olegin", "Alexoff", "Antonov", "Elenin", "Olgunin", "Svetlov"};

        private string[] middleNames = new[]
            {"Ivanovich", "Petrovich", "Olegovich", "Al.", "Antonovna", "", "Olgovich", "Svetlanovna"};

        private string[] skuDescriptions = new[]
            {"TShirt black XL", "Paper Bag Eco", "Silver Fork", "No name shit", "Hoody XXXL"};

        private void FillByFakedData()
        {
            int i = 20;
            while (i-- > 0)
            {
                Employee employee = Employee.Create(i, firstNames[_random.Next(firstNames.Length)],
                    lastNames[_random.Next(lastNames.Length)],
                    middleNames[_random.Next(middleNames.Length)],
                    $"{firstNames[_random.Next(firstNames.Length)]}@{lastNames[_random.Next(lastNames.Length)]}.mail");

                int sku = _random.Next(skuDescriptions.Length);
                OrderItem orderItem = OrderItem.Create(sku, skuDescriptions[sku], 1 + _random.Next(3));

                Employee manager = Employee.Create(20 + i, firstNames[_random.Next(firstNames.Length)],
                    lastNames[_random.Next(lastNames.Length)],
                    middleNames[_random.Next(middleNames.Length)],
                    $"{firstNames[_random.Next(firstNames.Length)]}@{lastNames[_random.Next(lastNames.Length)]}.mail");

                MerchOrder order = new MerchOrder(employee, orderItem);
                order.AssignTo(manager, DateTime.UtcNow);
                order.SetInProgressStatus(DateTime.UtcNow);
                order.SetReservedStatus(DateTime.UtcNow);
                order.SetCompletedStatus(DateTime.UtcNow);
                order.ClearDomainEvents();
                MockSet.Add(order);
            }
        }
    }
}
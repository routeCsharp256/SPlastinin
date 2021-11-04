using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Commands;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Handlers
{
    public class ReserveOrderInStockCommandHandler : IRequestHandler<ReserveOrderInStockCommand>
    {
        private readonly ILogger<ReserveOrderInStockCommandHandler> _logger;
        private readonly IMerchOrderRepository _merchOrderRepository;

        public ReserveOrderInStockCommandHandler(ILogger<ReserveOrderInStockCommandHandler> logger, IMerchOrderRepository merchOrderRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _merchOrderRepository = merchOrderRepository ?? throw new ArgumentNullException(nameof(merchOrderRepository));
        }

        public async Task<Unit> Handle(ReserveOrderInStockCommand request, CancellationToken cancellationToken)
        {
            var order = request.Order;

            _logger.LogInformation($"Grpc call to stock-api to reserve {order.Item}. Order Id: {order.Id}");

            await Task.Delay(3000);

            bool reserved = (new Random().Next(2) == 1);
            
            if(reserved) order.SetReservedStatus(DateTime.UtcNow);
            else order.SetDeferredStatus(DateTime.UtcNow);
            await _merchOrderRepository.UpdateAsync(order, cancellationToken);
            await _merchOrderRepository.SaveEntitiesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}
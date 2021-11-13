using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public ReserveOrderInStockCommandHandler(ILogger<ReserveOrderInStockCommandHandler> logger,
            IMerchOrderRepository merchOrderRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _merchOrderRepository =
                merchOrderRepository ?? throw new ArgumentNullException(nameof(merchOrderRepository));
        }

        public async Task<Unit> Handle(ReserveOrderInStockCommand request, CancellationToken cancellationToken)
        {
            var order = request.Order;
            var pendingItems = order.OrderItems.Where(item => item.Status.Equals(OrderItemStatus.Pending));

            foreach (var item in pendingItems)
            {
                await Task.Delay(300);

                bool reserved = (new Random().Next(2) == 1);

                _logger.LogInformation(
                    $"Grpc call to stock-api to reserve {item.Sku}. Order Id: {order.Id}. Reserved: {reserved}");

                if (reserved)
                {
                    item.SetStatusTo(OrderItemStatus.Reserved, DateTime.UtcNow);
                }
            }

            // check if there are still Pending items
            var pendingItemsCount = order.OrderItems.Count(item => item.Status.Equals(OrderItemStatus.Pending));

            if (pendingItemsCount == 0) order.SetReservedStatus(DateTime.UtcNow);
            else order.SetDeferredStatus(DateTime.UtcNow);

            await _merchOrderRepository.UpdateAsync(order, cancellationToken);
            await _merchOrderRepository.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
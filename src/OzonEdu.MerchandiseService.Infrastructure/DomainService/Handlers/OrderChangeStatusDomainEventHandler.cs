using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.Events.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Events;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Handlers
{
    public class OrderChangeStatusDomainEventHandler : INotificationHandler<OrderChangedStatusDomainEvent>
    {
        private readonly ILogger<OrderChangeStatusDomainEventHandler> _logger;
        private readonly IMerchOrderRepository _repository;
        private readonly IMediator _mediator;

        public OrderChangeStatusDomainEventHandler(ILogger<OrderChangeStatusDomainEventHandler> logger,
            IMerchOrderRepository repository, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mediator = mediator;
        }

        public async Task Handle(OrderChangedStatusDomainEvent notification,
            CancellationToken cancellationToken = default)
        {
            var order = notification.Order;

            var updatedOrder = await _repository.UpdateAsync(order, cancellationToken);
            await _repository.SaveEntitiesAsync(cancellationToken);

            _logger.LogInformation("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
                updatedOrder.Id, updatedOrder.Status.Name, updatedOrder.Status.Id);

            var orderStatus = updatedOrder.Status;
            if (orderStatus.Equals(OrderStatus.Canceled))
            {
                await _mediator.Publish(new NotifyWhenOrderCanceledEvent(order), cancellationToken);
            }
            else if (orderStatus.Equals(OrderStatus.Reserved))
            {
                await _mediator.Publish(new NotifyWhenOrderReservedEvent(order), cancellationToken);
            }
            else if (orderStatus.Equals(OrderStatus.Deferred))
            {
                await _mediator.Publish(new NotifyWhenOrderDeferredEvent(order), cancellationToken);
            }
            else if (orderStatus.Equals(OrderStatus.Assigned))
            {
                await _mediator.Publish(new NotifyWhenOrderAssignedEvent(order), cancellationToken);
                
                updatedOrder.SetInProgressStatus(DateTime.UtcNow);
                await _repository.UpdateAsync(updatedOrder, cancellationToken);
                await _repository.SaveEntitiesAsync(cancellationToken);
            }
            else if (orderStatus.Equals(OrderStatus.InProgress))
            {
                await _mediator.Send(new ReserveOrderInStockCommand(order), cancellationToken);
            }
        }
    }
}
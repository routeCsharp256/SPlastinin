using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.Events.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Events;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Handlers
{
    public class OrderChangeStatusDomainEventHandler : INotificationHandler<OrderChangedStatusDomainEvent>
    {
        private readonly ILogger<OrderChangeStatusDomainEventHandler> _logger;
        private readonly ITracer _tracer;
        private readonly IMerchOrderRepository _repository;
        private readonly IMediator _mediator;

        public OrderChangeStatusDomainEventHandler(
            IMerchOrderRepository repository,
            IMediator mediator,
            ILogger<OrderChangeStatusDomainEventHandler> logger,
            ITracer tracer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Handle(OrderChangedStatusDomainEvent notification,
            CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan(nameof(OrderChangeStatusDomainEventHandler)).StartActive();
            
            var order = notification.Order;
            
            var updatedOrder = await _repository.UpdateAsync(order, cancellationToken);

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
            else if (orderStatus.Equals(OrderStatus.InProgress))
            {
                await _mediator.Send(new ReserveOrderInStockCommand(order), cancellationToken);
            }
        }
    }
}
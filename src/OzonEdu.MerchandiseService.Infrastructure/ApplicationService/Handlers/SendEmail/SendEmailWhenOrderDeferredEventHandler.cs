using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Events;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Handlers.SendEmail
{
    public class SendEmailWhenOrderDeferredEventHandler : INotificationHandler<NotifyWhenOrderDeferredEvent>
    {
        private readonly ILogger<SendEmailWhenOrderDeferredEventHandler> _logger;

        public SendEmailWhenOrderDeferredEventHandler(ILogger<SendEmailWhenOrderDeferredEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(NotifyWhenOrderDeferredEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var order = notification.Order;
                var pendingItems = order.OrderItems.Where(i => i.Status.Equals(OrderItemStatus.Pending));

                StringBuilder sb = new();
                sb.AppendLine($"Email To Manager: {order.Manager.PersonName} ({order.Manager.Email.Value})");
                sb.AppendLine($"Order (Id:{order.Id}) has been deferred until the supply of the merch.");
                sb.AppendLine($"Receiver: {order.Receiver.PersonName} ({order.Receiver.Email.Value})");
                sb.AppendLine("Pending items:");
                sb.AppendLine(string.Join(Environment.NewLine, pendingItems.Select(i => i.ToString())));

                _logger.LogInformation(sb.ToString());

                await Task.Delay(100);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }
    }
}
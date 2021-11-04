using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Events;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Handlers.SendEmail
{
    public class SendEmailWhenOrderCanceledEventHandler : INotificationHandler<NotifyWhenOrderCanceledEvent>
    {
        private readonly ILogger<SendEmailWhenOrderCanceledEventHandler> _logger;

        public SendEmailWhenOrderCanceledEventHandler(ILogger<SendEmailWhenOrderCanceledEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(NotifyWhenOrderCanceledEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var order = notification.Order;
                
                StringBuilder sb = new();
                if (order.Receiver != null)
                {
                    sb.AppendLine($"Email To Employee: {order.Receiver.PersonName} ({order.Receiver.Email.Value})");
                    sb.AppendLine($"Your order has been canceled. OrderId: {order.Id}");
                    sb.AppendLine($"Reason: {order.Description}");
                    sb.AppendLine($"Merch: {order.Item}");
                    sb.AppendLine();
                }

                if (order.Manager != null)
                {
                    sb.AppendLine($"Email To Manager: {order.Manager.PersonName} ({order.Manager.Email.Value})");
                    sb.AppendLine($"Order ({order.Id}) has been canceled.");
                    sb.AppendLine($"Reason: {order.Description}");
                    sb.AppendLine($"Employee: {order.Receiver.PersonName} ({order.Receiver.Email.Value})");
                    sb.AppendLine($"Merch: {order.Item}");
                }

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
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Events;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Handlers.SendEmail
{
    public class SendEmailWhenOrderReservedEventHandler : INotificationHandler<NotifyWhenOrderReservedEvent>
    {
        private readonly ILogger<SendEmailWhenOrderReservedEventHandler> _logger;

        public SendEmailWhenOrderReservedEventHandler(ILogger<SendEmailWhenOrderReservedEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(NotifyWhenOrderReservedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var order = notification.Order;
                StringBuilder sb = new();
                sb.AppendLine($"Email To Employee: {order.Receiver.PersonName} ({order.Receiver.Email.Value})");
                sb.AppendLine($"Merch has been reserved for you!");
                sb.AppendLine($"OrderId: {order.Id}");
                sb.AppendLine($"Merch: {order.Item}");
                sb.AppendLine();
                sb.AppendLine($"Email To Manager: {order.Manager.PersonName} ({order.Manager.Email.Value})");
                sb.AppendLine($"Merch has been reserved!");
                sb.AppendLine($"OrderId: {order.Id}");
                sb.AppendLine($"Employee: {order.Receiver.PersonName} ({order.Receiver.Email.Value})");
                sb.AppendLine($"Merch: {order.Item}");

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
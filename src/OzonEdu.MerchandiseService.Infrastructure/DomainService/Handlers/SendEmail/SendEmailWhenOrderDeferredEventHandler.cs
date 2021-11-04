using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Events;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Handlers.SendEmail
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
                StringBuilder sb = new();
                sb.AppendLine($"Email To Manager: {order.Manager.PersonName} ({order.Manager.Email.Value})");
                sb.AppendLine($"Order (Id:{order.Id}) has been deferred until the supply of the merch.");
                sb.AppendLine($"Receiver: {order.Receiver.PersonName} ({order.Receiver.Email.Value})");
                sb.AppendLine($"Ordered Merch: {order.Item}");

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
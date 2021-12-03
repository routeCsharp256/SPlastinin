using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CSharpCourse.Core.Lib.Enums;
using CSharpCourse.Core.Lib.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Events;
using OzonEdu.MerchandiseService.Infrastructure.MessageBroker;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Handlers.SendEmail
{
    public class SendEmailWhenOrderReservedEventHandler : INotificationHandler<NotifyWhenOrderReservedEvent>
    {
        private readonly ILogger<SendEmailWhenOrderReservedEventHandler> _logger;
        private readonly IProducerBuilderWrapper _producerBuilderWrapper;

        public SendEmailWhenOrderReservedEventHandler(ILogger<SendEmailWhenOrderReservedEventHandler> logger,
            IProducerBuilderWrapper producerBuilderWrapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _producerBuilderWrapper =
                producerBuilderWrapper ?? throw new ArgumentNullException(nameof(producerBuilderWrapper));
        }

        public Task Handle(NotifyWhenOrderReservedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var order = notification.Order;
                string itemsInOrder = string.Join(Environment.NewLine, order.OrderItems.Select(i => i.ToString()));
                
                _producerBuilderWrapper.Producer.Produce(_producerBuilderWrapper.EmailNotificationTopic,
                    new Message<string, string>()
                    {
                        Key = order.Id.ToString(),
                        Value = JsonSerializer.Serialize(new NotificationEvent()
                            {
                                EmployeeEmail = order.Receiver.Email.Value,
                                EmployeeName = order.Receiver.PersonName.ToString(),
                                EventType = EmployeeEventType.MerchDelivery,
                                ManagerEmail = order.Manager.Email.Value,
                                ManagerName = order.Manager.PersonName.ToString(),
                                Payload = order.MerchPackType
                            }
                        )
                    });

                _logger.LogInformation(
                    "Sent notification to {EmployeeEmail}. Order {Id} reserved. Merch items: {Items}",
                    order.Receiver.Email.Value, order.Id.ToString(), itemsInOrder);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            
            return Task.CompletedTask;
        }
    }
}
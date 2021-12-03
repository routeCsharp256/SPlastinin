using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CSharpCourse.Core.Lib.Enums;
using CSharpCourse.Core.Lib.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;

namespace OzonEdu.MerchandiseService.BackgroundServices
{
    public class EmployeeNotificationTopicMessageHandler : ITopicMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly KafkaOptions _options;
        private readonly ILogger<EmployeeNotificationTopicMessageHandler> _logger;

        public EmployeeNotificationTopicMessageHandler(IMediator mediator, IOptions<KafkaOptions> options,
            ILogger<EmployeeNotificationTopicMessageHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ProcessTopicMessage(ConsumeResult<Ignore, string> topicMessage,
            CancellationToken cancellationToken = default)
        {
            var message = JsonSerializer.Deserialize<NotificationEvent>(topicMessage.Message.Value);
            if (message == null) return;

            if (message.EventType == EmployeeEventType.Hiring ||
                message.EventType == EmployeeEventType.ConferenceAttendance ||
                message.EventType == EmployeeEventType.ProbationPeriodEnding)
            {
                var command = new CreateMerchOrderCommand()
                {
                    EmployeeFirstName = message.EmployeeName,
                    EmployeeEmail = message.EmployeeEmail,
                    ManagerFirstName = message.ManagerName,
                    ManagerEmail = message.ManagerEmail,
                    MerchPackType = GetMerchPackTypeByEventType(message.EventType)
                };

                var commandResponse = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation("Create merch order from topic {TopicName}. EventType {EventType}. Response: {@Response}", 
                    _options.EmployeeNotificationTopic, message.EventType, commandResponse);
            }
        }

        private MerchType GetMerchPackTypeByEventType(EmployeeEventType eventType)
        {
            MerchType merchPackType = default(MerchType);

            switch (eventType)
            {
                case EmployeeEventType.Hiring:
                    merchPackType = MerchType.WelcomePack;
                    break;
                case EmployeeEventType.ProbationPeriodEnding:
                    merchPackType = MerchType.ProbationPeriodEndingPack;
                    break;
                case EmployeeEventType.ConferenceAttendance:
                    break;
            }
            
            return merchPackType;
        }

        public bool CanProcess(ConsumeResult<Ignore, string> message)
        {
            return message?.Topic == _options.EmployeeNotificationTopic;
        }
    }
}
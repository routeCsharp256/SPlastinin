﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.Events.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Handlers.SendEmail
{
    public class SendEmailWhenOrderAssignedEventHandler : INotificationHandler<OrderAssignedDomainEvent>
    {
        private readonly ILogger<SendEmailWhenOrderAssignedEventHandler> _logger;

        public SendEmailWhenOrderAssignedEventHandler(ILogger<SendEmailWhenOrderAssignedEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderAssignedDomainEvent notification, CancellationToken cancellationToken)
        {
            await SendEmail(notification.Order);
        }

        private async Task SendEmail(MerchOrder order)
        {
            try
            {
                StringBuilder sb = new();
                sb.AppendLine($"Email To Manager: {order.Manager.PersonName} ({order.Manager.Email.Value})");
                sb.AppendLine($"You are assigned to control the merch order (Id:{order.Id})");
                sb.AppendLine($"Receiver: {order.Receiver.PersonName} ({order.Receiver.Email.Value})");

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
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CSharpCourse.Core.Lib.Events;
using CSharpCourse.Core.Lib.Models;
using MediatR;
using Microsoft.Extensions.Options;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;

namespace OzonEdu.MerchandiseService.BackgroundServices
{
    public class StockReplenishedTopicMessageHandler : ITopicMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly KafkaOptions _options;

        public StockReplenishedTopicMessageHandler(IMediator mediator, IOptions<KafkaOptions> options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }
        
        public async Task ProcessTopicMessage(ConsumeResult<Ignore, string> topicMessage, 
            CancellationToken cancellationToken = default)
        {
            var message = JsonSerializer.Deserialize<StockReplenishedEvent>(topicMessage.Message.Value);
            if (message == null) return;

            var items = message.Type.Select(item =>
            {
                return new StockReplenishedItem()
                {
                    ClothingSize = item.ClothingSize,
                    ItemTypeId = item.ItemTypeId,
                    ItemTypeName = item.ItemTypeName,
                    Sku = item.Sku
                };
            });

            var command = new ProcessStockReplenishedCommand(items);
            
            await _mediator.Send(command, cancellationToken);
        }

        public bool CanProcess(ConsumeResult<Ignore, string> message)
        {
            return message?.Topic == _options.StockReplenishedTopic;
        }
    }
}
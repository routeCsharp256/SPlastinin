using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;

namespace OzonEdu.MerchandiseService.BackgroundServices
{
    public class MerchandiseKafkaConsumerService : BackgroundService
    {
        private readonly ILogger<MerchandiseKafkaConsumerService> _logger;
        private readonly IEnumerable<ITopicMessageHandler> _messageHandlers;
        private readonly KafkaOptions _options;

        public MerchandiseKafkaConsumerService(IOptions<KafkaOptions> options,
            ILogger<MerchandiseKafkaConsumerService> logger,
            IEnumerable<ITopicMessageHandler> messageHandlers)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageHandlers = messageHandlers ?? throw new ArgumentNullException(nameof(messageHandlers));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = _options.ConsumerGroupId,
                BootstrapServers = _options.BootstrapServers,
                AllowAutoCreateTopics = true
            };

            string[] topicNames = new string[] {_options.EmployeeNotificationTopic, _options.StockReplenishedTopic};
            
            await CreateTopicAsync(_options.BootstrapServers, topicNames);

            try
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

                consumer.Subscribe(topicNames);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(stoppingToken);
                        if (result != null)
                        {
                            foreach (var handler in _messageHandlers)
                            {
                                if (handler.CanProcess(result))
                                {
                                    await handler.ProcessTopicMessage(result, stoppingToken);
                                }
                            }
                            
                            consumer.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error while get consume.");
                        await Task.Delay(60000);
                    }
                }
                
                consumer.Close();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while consumer subscription.");
            }
        }
        
        private async Task CreateTopicAsync(string bootstrapServers, string[] topicNames)
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(
                        topicNames.Select(topicName =>
                            new TopicSpecification {Name = topicName, ReplicationFactor = 1, NumPartitions = 1})
                    );
                }
                catch (CreateTopicsException e)
                {
                    foreach (var report in e.Results)
                    {
                        _logger.LogError(e,"An error occured creating topic {TopicName}: {Reason}", 
                            report.Topic, report.Error.Reason);   
                    }
                }
            }
        }
    }
}
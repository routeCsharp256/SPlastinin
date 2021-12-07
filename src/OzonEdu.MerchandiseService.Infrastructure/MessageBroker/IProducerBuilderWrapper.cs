using Confluent.Kafka;

namespace OzonEdu.MerchandiseService.Infrastructure.MessageBroker
{
    public interface IProducerBuilderWrapper
    {
        IProducer<string, string> Producer { get; set; }
        
        string EmailNotificationTopic { get; set; }
        string StockReplenishedTopic { get; set; }
        string EmployeeNotificationTopic { get; set; }
    }
}
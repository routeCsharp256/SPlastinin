using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace OzonEdu.MerchandiseService.BackgroundServices
{
    public interface ITopicMessageHandler
    {
        Task ProcessTopicMessage(ConsumeResult<Ignore, string> topicMessage, 
            CancellationToken cancellationToken = default);
        bool CanProcess(ConsumeResult<Ignore, string> message);
    }
}
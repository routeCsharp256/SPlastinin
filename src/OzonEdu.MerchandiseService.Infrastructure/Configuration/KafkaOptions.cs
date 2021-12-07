namespace OzonEdu.MerchandiseService.Infrastructure.Configuration
{
    public class KafkaOptions
    {
        public string BootstrapServers { get; set; }
        
        public string ConsumerGroupId { get; set; }
        
        public string EmailNotificationTopic { get; set; }
        public string StockReplenishedTopic { get; set; }
        public string EmployeeNotificationTopic { get; set; }
    }
}
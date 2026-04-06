namespace PilotIceService.Infrastructure.Kafka
{
    public interface IQueue
    {
        Task<QueueDeliveryResult> ProduceAsync<T>(string topicName, T message, CancellationToken ct) where T : class;
    }
}
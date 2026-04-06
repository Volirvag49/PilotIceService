namespace PilotIceService.Infrastructure.Kafka
{
    public class QueueDeliveryResult
    {
        public bool Success { get; set; }

        public DateTime? UtcDateTime { get; set; }
    }
}
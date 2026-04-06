using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace PilotIceService.Infrastructure.Kafka
{
    public class KafkaQueue : IQueue
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;
        public KafkaQueue(IProducer<string, string> producer, IConfiguration configuration)
        {
            _producer = producer;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<QueueDeliveryResult> ProduceAsync<T>(string topicName, T message, CancellationToken ct) where T : class
        {
            var json = JsonSerializer.Serialize(message);
            var msg = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = json,
            };
            var result = await _producer.ProduceAsync(topicName, msg, ct);

            return new QueueDeliveryResult
            {
                Success = result != null,
                UtcDateTime = result?.Timestamp.UtcDateTime,
            };
        }
    }
}
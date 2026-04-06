using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PilotIceService.Infrastructure.Kafka
{
    public class KafkaQueue : IQueue
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaQueue> _logger;
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;

        public KafkaQueue(IProducer<string, string> producer, IConfiguration configuration, ILogger<KafkaQueue> logger)
        {
            _producer = producer;
            _configuration = configuration;
            _logger = logger;
            
            // Настройки retry из конфигурации
            _maxRetries = configuration.GetValue<int>("KafkaSettings:MaxRetries", 3);
            _initialDelay = TimeSpan.FromMilliseconds(configuration.GetValue<int>("KafkaSettings:InitialDelayMs", 500));
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

            return await ProduceWithRetry(topicName, msg, ct);
        }

        private async Task<QueueDeliveryResult> ProduceWithRetry(string topicName, Message<string, string> message, CancellationToken ct, int attempt = 1)
        {
            try
            {
                var result = await _producer.ProduceAsync(topicName, message, ct);
                return new QueueDeliveryResult
                {
                    Success = result != null,
                    UtcDateTime = result?.Timestamp.UtcDateTime,
                };
            }
            catch (ProduceException<string, string> ex) when (attempt <= _maxRetries)
            {
                var delay = _initialDelay * (attempt - 1); // Линейное увеличение задержки
                _logger.LogWarning(ex, "Ошибка при отправке сообщения в Kafka (попытка {Attempt}/{MaxRetries}). Повтор через {Delay}мс", attempt, _maxRetries, delay.TotalMilliseconds);
                
                await Task.Delay(delay, ct);
                return await ProduceWithRetry(topicName, message, ct, attempt + 1);
            }
            catch (Exception ex) when (attempt <= _maxRetries)
            {
                var delay = _initialDelay * (attempt - 1);
                _logger.LogWarning(ex, "Ошибка при отправке сообщения в Kafka (попытка {Attempt}/{MaxRetries}). Повтор через {Delay}мs", attempt, _maxRetries, delay.TotalMilliseconds);
                
                await Task.Delay(delay, ct);
                return await ProduceWithRetry(topicName, message, ct, attempt + 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось отправить сообщение в Kafka после {MaxRetries} попыток", _maxRetries);
                return new QueueDeliveryResult
                {
                    Success = false,
                    UtcDateTime = null,
                };
            }
        }
    }
}
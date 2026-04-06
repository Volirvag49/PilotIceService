using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PilotIceService.Infrastructure.Kafka
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddKafkaQueue(this IServiceCollection services, IConfiguration configuration)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = configuration.GetValue<string>("KafkaSettings:BootstrapServers"),
                AllowAutoCreateTopics = true,
                Acks = Acks.All,
                SecurityProtocol = SecurityProtocol.Plaintext,

                MessageTimeoutMs = configuration.GetValue<int?>("KafkaSettings:MessageTimeoutMs") ?? 5000,
                MessageSendMaxRetries = 3,
                LingerMs = 5,
            };

            // 2. Регистрация IProducer как Singleton
            // Мы используем Builder, чтобы создать и собрать клиент
            services.AddSingleton<IProducer<string, string>>(sp => new ProducerBuilder<string, string>(producerConfig).Build());


            services.AddSingleton<IQueue, KafkaQueue>();

            return services;
        }
    }
}
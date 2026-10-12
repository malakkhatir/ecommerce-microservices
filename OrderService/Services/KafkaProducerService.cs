using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using OrderService.Models;
using System.Text.Json;

namespace OrderService.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(IConfiguration configuration)
        {
            var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

            var config = new ProducerConfig
            {
                // ✅ "kafka:29092" dans Docker, "localhost:9092" en local
                BootstrapServers = bootstrapServers
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
            Console.WriteLine($"[Kafka] ✅ Producer connecté → {bootstrapServers}");
        }

        public async Task PublishOrderCreated(Order order)
        {
            var message = JsonSerializer.Serialize(order);

            await _producer.ProduceAsync("order-created", new Message<Null, string>
            {
                Value = message
            });

            Console.WriteLine($"[Kafka] 📤 Order publié : {message}");
        }
    }
}
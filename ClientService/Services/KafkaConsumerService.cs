using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ClientService.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly string _bootstrapServers;

        public KafkaConsumerService(IConfiguration configuration)
        {
            // ✅ "kafka:29092" dans Docker, "localhost:9092" en local
            _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId          = "client-group",
                AutoOffsetReset  = AutoOffsetReset.Earliest
            };

            try
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe("order-created");

                Console.WriteLine($"[Kafka] 🎧 En écoute sur {_bootstrapServers}");

                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(stoppingToken);
                    Console.WriteLine($"[Kafka] 📨 Message reçu : {result.Message.Value}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[Kafka] 🔴 Consumer arrêté proprement");
            }
            catch (Exception ex)
            {
                // ✅ Ne crash pas le service si Kafka est indisponible
                Console.WriteLine($"[Kafka] ⚠️ Connexion impossible : {ex.Message}");
            }
        }
    }
}
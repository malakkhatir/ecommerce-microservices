using Consul;
using ClientService.Services;
using ClientService.Settings;
using Prometheus;   

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var consulAddress = builder.Configuration["Consul:Address"]     ?? "http://localhost:8500";
var serviceHost   = builder.Configuration["Consul:ServiceHost"] ?? "127.0.0.1";
var servicePort   = int.Parse(builder.Configuration["Consul:ServicePort"] ?? "8080");
var kafkaServers  = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

// MongoDB Settings
builder.Services.AddSingleton(sp =>
    builder.Configuration
        .GetSection("MongoDbSettings")
        .Get<MongoDbSettings>()!
);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ClientMongoService>();

builder.Configuration["Kafka:BootstrapServers"] = kafkaServers;
builder.Services.AddHostedService<KafkaConsumerService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// ========== MÉTRIQUES PROMETHEUS ==========
app.UseHttpMetrics();   // capture les requêtes HTTP
app.MapMetrics();       // expose /metrics

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Health check
app.MapHealthChecks("/health");
app.MapControllers();

// ========== ENREGISTREMENT CONSUL ==========
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
var consulClient = new ConsulClient(c =>
{
    c.Address = new Uri(consulAddress);
});

var registration = new AgentServiceRegistration
{
    ID      = "client-service",
    Name    = "ClientService",
    Address = serviceHost,
    Port    = servicePort,
    Tags    = new[] { "clients", "api" },
    Check   = new AgentServiceCheck
    {
        HTTP                           = $"http://{serviceHost}:{servicePort}/health",
        Interval                       = TimeSpan.FromSeconds(10),
        Timeout                        = TimeSpan.FromSeconds(5),
        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)
    }
};

lifetime.ApplicationStarted.Register(async () =>
{
    await consulClient.Agent.ServiceRegister(registration);
    Console.WriteLine($"[Consul] ✅ ClientService enregistré → {serviceHost}:{servicePort}");
});

lifetime.ApplicationStopping.Register(async () =>
{
    await consulClient.Agent.ServiceDeregister("client-service");
    Console.WriteLine("[Consul] 🔴 ClientService désenregistré");
});

app.Run();
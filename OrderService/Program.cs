using Consul;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Services;
using Prometheus;   // 👈 Ajout pour les métriques

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var consulAddress = builder.Configuration["Consul:Address"]     ?? "http://localhost:8500";
var serviceHost   = builder.Configuration["Consul:ServiceHost"] ?? "127.0.0.1";
var servicePort   = int.Parse(builder.Configuration["Consul:ServicePort"] ?? "8080");
var kafkaServers  = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

builder.Configuration["Kafka:BootstrapServers"] = kafkaServers;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<KafkaProducerService>();

builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    )
);

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
    ID      = "order-service",
    Name    = "OrderService",
    Address = serviceHost,
    Port    = servicePort,
    Tags    = new[] { "orders", "api" },
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
    Console.WriteLine($"[Consul] ✅ OrderService enregistré → {serviceHost}:{servicePort}");
});

lifetime.ApplicationStopping.Register(async () =>
{
    await consulClient.Agent.ServiceDeregister("order-service");
    Console.WriteLine("[Consul] 🔴 OrderService désenregistré");
});

app.Run();
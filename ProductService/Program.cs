using Consul;
using ProductService.Services;
using ProductService.Settings;
using Prometheus;   // 👈 Ajout pour les métriques

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var consulAddress = builder.Configuration["Consul:Address"]     ?? "http://localhost:8500";
var serviceHost   = builder.Configuration["Consul:ServiceHost"] ?? "127.0.0.1";
var servicePort   = int.Parse(builder.Configuration["Consul:ServicePort"] ?? "8080");

builder.Services.AddSingleton(sp =>
    builder.Configuration
        .GetSection("MongoDbSettings")
        .Get<MongoDbSettings>()!
);

builder.Services.AddSingleton<ProductMongoService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    ID      = "product-service",
    Name    = "ProductService",
    Address = serviceHost,
    Port    = servicePort,
    Tags    = new[] { "products", "api" },
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
    Console.WriteLine($"[Consul] ✅ ProductService enregistré → {serviceHost}:{servicePort}");
});

lifetime.ApplicationStopping.Register(async () =>
{
    await consulClient.Agent.ServiceDeregister("product-service");
    Console.WriteLine("[Consul] 🔴 ProductService désenregistré");
});

app.Run();
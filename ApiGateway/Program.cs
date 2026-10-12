using Consul;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var consulAddress = builder.Configuration["Consul:Address"]     ?? "http://consul:8500";
var serviceHost   = builder.Configuration["Consul:ServiceHost"] ?? "api-gateway";
var servicePort   = int.Parse(builder.Configuration["Consul:ServicePort"] ?? "8080");

builder.WebHost.UseUrls($"http://0.0.0.0:{servicePort}");

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHealthChecks();
builder.Services.AddOcelot(builder.Configuration).AddConsul();

var app = builder.Build();

// ✅ CORS AVANT tout le reste
app.UseCors("AllowAngular");

// ✅ Intercepter /health et /metrics AVANT Ocelot
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/health"))
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("Healthy");
        return;
    }

    if (context.Request.Path.StartsWithSegments("/metrics"))
    {
        var registry = Prometheus.Metrics.DefaultRegistry;
        context.Response.ContentType = "text/plain; version=0.0.4; charset=utf-8";
        context.Response.StatusCode  = 200;
        await registry.CollectAndExportAsTextAsync(context.Response.Body);
        return;
    }

    await next();
});

app.UseHttpMetrics();

// ✅ Ocelot EN DERNIER
await app.UseOcelot();

var lifetime     = app.Services.GetRequiredService<IHostApplicationLifetime>();
var consulClient = new ConsulClient(c => c.Address = new Uri(consulAddress));

var registration = new AgentServiceRegistration
{
    ID      = "api-gateway",
    Name    = "ApiGateway",
    Address = serviceHost,
    Port    = servicePort,
    Tags    = new[] { "gateway", "ocelot" },
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
    Console.WriteLine($"[Consul] ✅ ApiGateway enregistré → {serviceHost}:{servicePort}");
});

lifetime.ApplicationStopping.Register(async () =>
{
    await consulClient.Agent.ServiceDeregister("api-gateway");
    Console.WriteLine("[Consul] 🔴 ApiGateway désenregistré");
});

app.Run();
using Esport.Domain;
using Esport.Infrastructure;
using Esport.Kafka.Common;
using Esport.Kafka.Subscriber;
using Esport.Kafka.Subscriber.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((context, config) =>
{
    config.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();
});

builder.ConfigureServices((context, services) =>
{
    services.Configure<KafkaConfiguration>(context.Configuration.GetSection("Kafka"));
    services.Configure<ApiConnectionConfiguration>(context.Configuration.GetSection("ApiConnection"));
    services.AddHostedService<KafkaSubscriberService>();
    services.AddScoped<IEsportRepository, EsportRepository>();
    services.AddDbContext<EsportDbContext>(options =>
        options.UseNpgsql(context.Configuration.GetConnectionString("PostgresConnection"),
            b => b.MigrationsAssembly("Esport.Kafka.Subscriber")));
    services.AddHttpClient();
});

var app = builder.Build();

await app.RunAsync();
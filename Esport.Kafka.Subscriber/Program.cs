using Esport.Kafka.Common;
using Esport.Kafka.Subscriber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Esport.Domain;
using Esport.Domain.Models;
using Esport.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var kafkaConfig = builder.Configuration.GetSection("Kafka").Get<KafkaConfiguration>();

builder.Services.AddSingleton(kafkaConfig);
builder.Services.AddHostedService<KafkaSubscriberService>();  // Регистрация хост-сервиса
builder.Services.AddScoped<IEsportRepository<EsportEvent>, EsportRepository<EsportEvent>>();  // Регистрация репозитория

builder.Services.AddDbContext<EsportDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"),
        b => b.MigrationsAssembly("Esport.Kafka.Subscriber")));

var app = builder.Build();
await app.RunAsync();
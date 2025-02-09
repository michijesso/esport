using System.Text.Json;
using Esport.GeneratorService.Core.Interfaces;
using Esport.GeneratorService.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<IEsportGenerator, EsportGenerator>();
        services.AddTransient<IEsportSender, EsportSender>();
        services.AddLogging(configure => configure.AddConsole());
    })
    .Build();

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

while (true)
{
    var modelToSend = host.Services.GetRequiredService<IEsportGenerator>().GenerateEsportData();
    logger.LogInformation(JsonSerializer.Serialize(modelToSend));
    await host.Services.GetRequiredService<IEsportSender>().SendAsync(modelToSend);
    await Task.Delay(10000);

    var modelToUpdate = host.Services.GetRequiredService<IEsportGenerator>().UpdateEsportData(modelToSend);
    logger.LogInformation(JsonSerializer.Serialize(modelToUpdate));
    await host.Services.GetRequiredService<IEsportSender>().SendAsync(modelToUpdate);
    await Task.Delay(10000);
}
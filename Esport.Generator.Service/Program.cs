using System.Text.Json;
using Esport.GeneratorService.Core.Interfaces;
using Esport.GeneratorService.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<IEsportGenerator, EsportGenerator>();
        services.AddTransient<IEsportSender, EsportSender>();
    })
    .Build();

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

while (true)
{
    var modelToSend = host.Services.GetRequiredService<IEsportGenerator>().GenerateAsync();
    Console.WriteLine(JsonSerializer.Serialize(modelToSend));
    await host.Services.GetRequiredService<IEsportSender>().SendAsync(modelToSend);
    await Task.Delay(10000);
    var modelToUpdate = host.Services.GetRequiredService<IEsportGenerator>().UpdateEsportData(modelToSend);
    Console.WriteLine(JsonSerializer.Serialize(modelToUpdate));
    await host.Services.GetRequiredService<IEsportSender>().SendAsync(modelToUpdate);
    await Task.Delay(10000);
}
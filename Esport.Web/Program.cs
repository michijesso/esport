using System.Net.WebSockets;
using Esport.Domain;
using Esport.Domain.Models;
using Esport.Infrastructure;
using Esport.Web;
using Esport.Web.Mappings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IEsportRepository<EsportEvent>, EsportRepository<EsportEvent>>();
builder.Services.AddAutoMapper(typeof(EsportEventMappingProfile));
builder.Services.AddControllers();
builder.Services.AddSingleton<IWebSocketService, WebSocketService>();

builder.Services.AddDbContext<EsportDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseWebSockets();
app.UseRouting();

// app.Map("/ws", async context =>
// {
//     if (context.WebSockets.IsWebSocketRequest)
//     {
//         var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
//         var webSocket = await context.WebSockets.AcceptWebSocketAsync();
//         await webSocketHandler.HandleWebSocketAsync(webSocket);
//     }
//     else
//     {
//         context.Response.StatusCode = 400;
//     }
// });

app.MapControllers();

await app.RunAsync();






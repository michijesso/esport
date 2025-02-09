using Esport.Domain;
using Esport.Infrastructure;
using Esport.Web.Implementations;
using Esport.Web.Interfaces;
using Esport.Web.Mappings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IEsportRepository, EsportRepository>();
builder.Services.AddAutoMapper(typeof(EsportEventMappingProfile));
builder.Services.AddSingleton<IWebSocketAllEventsService, WebSocketAllEventService>();
builder.Services.AddSingleton<IWebSocketSpecifiedEventService, WebSocketSpecifiedEventService>();

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

app.MapControllers();

await app.RunAsync();






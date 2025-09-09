using Application.Interfaces;
using Application.Options;
using Application.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection("RabbitMqConfiguration"));

builder.Services.AddSingleton<IQueryService, QueryService>();
builder.Services.AddHostedService<RabbitMqListener>();


builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

using var scope = app.Services.CreateScope();


app.MapControllers();

app.Run();
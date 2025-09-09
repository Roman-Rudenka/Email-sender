using Application.Interfaces;
using Application.Options;
using Application.Services;
using Infrastructure;
using Infrastructure.Options;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbOptions>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var dbOptions = serviceProvider.GetRequiredService<IOptions<DbOptions>>().Value;
    options.UseNpgsql(dbOptions.DefaultConnection);
});

builder.Services.AddControllers();
builder.Services.AddScoped<IUserDataService, UserDataService>();
builder.Services.AddScoped<IOutboxMessageService, OutboxMessageService>();
builder.Services.AddScoped<IUserDataRepository, UserDataRepository>();
builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
builder.Services.AddHttpClient();
builder.Services.Configure<EmailApiOptions>(builder.Configuration.GetSection("EmailApi"));
builder.Services.Configure<PhoneNumberApiOptions>(builder.Configuration.GetSection("PhoneApi"));
builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection("RabbitMqConfiguration"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiExceptionMiddleware>();
app.MapControllers();

app.Run();
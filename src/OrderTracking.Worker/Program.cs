using Microsoft.EntityFrameworkCore;
using OrderTracking.Application.Interfaces;
using OrderTracking.Infrastructure.Configuration;
using OrderTracking.Infrastructure.Messaging;
using OrderTracking.Infrastructure.Persistence;
using OrderTracking.Infrastructure.Repositories;
using OrderTracking.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// RabbitMQ Settings
builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMqSettings"));

// Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Messaging - Singleton porque mantém conexão persistente
builder.Services.AddSingleton<IMessageConsumer, RabbitMqConsumer>();

// Worker Service
builder.Services.AddHostedService<OrderProcessorWorker>();

var host = builder.Build();
host.Run();

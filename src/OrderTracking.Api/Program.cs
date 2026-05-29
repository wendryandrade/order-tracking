using Microsoft.EntityFrameworkCore;
using OrderTracking.Api.Middleware;
using OrderTracking.Application.Interfaces;
using OrderTracking.Application.Services;
using OrderTracking.Infrastructure.Configuration;
using OrderTracking.Infrastructure.Messaging;
using OrderTracking.Infrastructure.Persistence;
using OrderTracking.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configuração de Controllers
builder.Services.AddControllers();

    // Swagger/OpenAPI tradicional
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Order Tracking API",
            Version = "v1",
            Description = "API para gerenciamento de pedidos com processamento assíncrono via RabbitMQ",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Order Tracking Team",
                Email = "suporte@ordertracking.com"
            }
        });

        // Incluir comentários XML
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    // CORS para React
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReact", policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    // Database
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));

    // RabbitMQ Settings
    builder.Services.Configure<RabbitMqSettings>(
        builder.Configuration.GetSection("RabbitMQ"));

    // Repositories
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();

    // Services
    builder.Services.AddScoped<IOrderPublisher, RabbitMqOrderPublisher>();
    builder.Services.AddScoped<IOrderService, OrderService>();

    // Exception Handler
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    // Aplicar migrations e seed (apenas em desenvolvimento)
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<AppDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("📦 Aplicando migrations...");
                db.Database.Migrate();
                logger.LogInformation("✅ Migrations aplicadas com sucesso!");

                // Fazer seed dos dados iniciais
                DbInitializer.Initialize(db, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Erro ao aplicar migrations ou seed");
                throw;
            }
        }

        // Configure middleware pipeline
        app.UseExceptionHandler();

        // CORS deve vir antes de outros middlewares
        app.UseCors("AllowReact");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Tracking API v1");
                c.RoutePrefix = "swagger"; // Swagger em /swagger
                c.DocumentTitle = "Order Tracking API - Swagger UI";
                c.DisplayRequestDuration();
            });

            // Redirect root to Swagger
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
        }
        else
        {
            // HTTPS redirect apenas em produção
            app.UseHttpsRedirection();
        }

        app.MapControllers();

        app.Run();

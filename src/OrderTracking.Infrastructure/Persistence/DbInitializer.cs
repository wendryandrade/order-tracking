using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderTracking.Domain.Entities;
using OrderTracking.Domain.Enums;

namespace OrderTracking.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context, ILogger logger)
        {
            try
            {
                // Verifica se já existem dados
                if (context.Orders.Any())
                {
                    logger.LogInformation("✅ Banco já possui dados. Seed ignorado.");
                    return;
                }

                logger.LogInformation("🌱 Iniciando seed de dados...");

                var orders = new List<Order>
                {
                    new Order(
                        customerName: "João Silva",
                        amount: 150.50m,
                        orderDate: DateTime.UtcNow.AddDays(-5)
                    ),
                    new Order(
                        customerName: "Maria Santos",
                        amount: 320.00m,
                        orderDate: DateTime.UtcNow.AddDays(-3)
                    ),
                    new Order(
                        customerName: "Pedro Costa",
                        amount: 89.90m,
                        orderDate: DateTime.UtcNow.AddDays(-1)
                    ),
                    new Order(
                        customerName: "Ana Oliveira",
                        amount: 550.00m,
                        orderDate: DateTime.UtcNow
                    )
                };

                // Marca todos os pedidos como processados
                foreach (var order in orders)
                {
                    order.MarkAsProcessed();
                }

                context.Orders.AddRange(orders);
                context.SaveChanges();

                logger.LogInformation($"✅ Seed concluído! {orders.Count} pedidos criados.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Erro ao fazer seed dos dados");
                throw;
            }
        }
    }
}

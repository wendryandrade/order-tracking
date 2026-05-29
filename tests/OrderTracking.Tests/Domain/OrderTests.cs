using FluentAssertions;
using OrderTracking.Domain.Entities;
using OrderTracking.Domain.Enums;
using Xunit;

namespace OrderTracking.Tests.Domain;

public class OrderTests
{
    // Testa a criação de um pedido com dados válidos
    [Fact]
    public void Order_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var orderDate = DateTime.UtcNow;
        var order = new Order("João Silva", 150.50m, orderDate);

        // Assert
        order.Should().NotBeNull();
        order.CustomerName.Should().Be("João Silva");
        order.Amount.Should().Be(150.50m);
        order.OrderDate.Should().Be(orderDate);
        order.Status.Should().Be(OrderStatus.Pending);
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    // Testa a atualização do status do pedido para "Processado"
    [Fact]
    public void MarkAsProcessed_ShouldUpdateStatus()
    {
        // Arrange
        var order = new Order("João Silva", 150.50m, DateTime.UtcNow);

        // Act
        order.MarkAsProcessed();

        // Assert
        order.Status.Should().Be(OrderStatus.Processed);
    }
}

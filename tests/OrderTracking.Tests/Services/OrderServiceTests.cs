using FluentAssertions;
using Moq;
using OrderTracking.Application.DTOs;
using OrderTracking.Application.Interfaces;
using OrderTracking.Application.Services;
using OrderTracking.Domain.Entities;
using OrderTracking.Domain.Enums;
using Xunit;

namespace OrderTracking.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IOrderPublisher> _mockPublisher;
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockPublisher = new Mock<IOrderPublisher>();
        _sut = new OrderService(_mockRepository.Object, _mockPublisher.Object);
    }

    // Testa a criação de um pedido com dados válidos e a publicação   
    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldCreateOrderAndPublish()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerName = "João Silva",
            Amount = 150.50m
            // OrderDate será DateTime.UtcNow automaticamente
        };

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.Should().NotBeEmpty();

        _mockPublisher.Verify(
            x => x.PublishAsync(It.Is<Order>(o => 
                o.CustomerName == request.CustomerName && 
                o.Amount == request.Amount)),
            Times.Once);
    }

    // Testa obter um pedido existente por ID
    [Fact]
    public async Task GetByIdAsync_WithExistingOrder_ShouldReturnOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order("João Silva", 150.50m, DateTime.UtcNow);
        typeof(Order).GetProperty("Id")!.SetValue(order, orderId);

        _mockRepository
            .Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _sut.GetByIdAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        result.CustomerName.Should().Be("João Silva");
        result.Amount.Should().Be(150.50m);
    }

    // Testa obter um pedido inexistente por ID
    [Fact]
    public async Task GetByIdAsync_WithNonExistingOrder_ShouldReturnNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mockRepository
            .Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _sut.GetByIdAsync(orderId);

        // Assert
        result.Should().BeNull();
    }

    // Testa obter todos os pedidos
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order("João Silva", 150.50m, DateTime.UtcNow),
            new Order("Maria Santos", 200.00m, DateTime.UtcNow)
        };

        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(orders);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().CustomerName.Should().Be("João Silva");
        result.Last().CustomerName.Should().Be("Maria Santos");
    }
}

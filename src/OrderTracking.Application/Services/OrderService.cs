using OrderTracking.Application.DTOs;
using OrderTracking.Application.Interfaces;
using OrderTracking.Domain.Entities;

namespace OrderTracking.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IOrderPublisher _publisher;

        public OrderService(
            IOrderRepository repository,
            IOrderPublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task<Guid> CreateAsync(CreateOrderRequest request)
        {
            // Validar request
            if (string.IsNullOrWhiteSpace(request.CustomerName))
                throw new ArgumentException("Nome do cliente é obrigatório", nameof(request.CustomerName));

            if (request.Amount <= 0)
                throw new ArgumentException("Valor do pedido deve ser maior que zero", nameof(request.Amount));

            // Criar entidade Order com data atual
            var order = new Order(
                customerName: request.CustomerName,
                amount: request.Amount,
                orderDate: DateTime.UtcNow);

            // 1. Salvar no banco PRIMEIRO (Status = Pending)
            await _repository.AddAsync(order);

            // 2. Publicar mensagem no RabbitMQ para processamento assíncrono
            await _publisher.PublishAsync(order);

            return order.Id;
        }

        public async Task<OrderResponse?> GetByIdAsync(Guid id)
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
                return null;

            return new OrderResponse
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Amount = order.Amount,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString()
            };
        }

        public async Task<List<OrderResponse>> GetAllAsync()
        {
            var orders = await _repository.GetAllAsync();

            return orders.Select(order => new OrderResponse
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Amount = order.Amount,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString()
            }).ToList();
        }
    }
}

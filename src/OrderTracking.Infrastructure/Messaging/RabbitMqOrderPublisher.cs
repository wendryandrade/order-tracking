using Microsoft.Extensions.Options;
using OrderTracking.Application.Interfaces;
using OrderTracking.Domain.Entities;
using OrderTracking.Infrastructure.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderTracking.Infrastructure.Messaging
{
    public class RabbitMqOrderPublisher : IOrderPublisher, IDisposable
    {
        private readonly RabbitMqSettings _settings;
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public RabbitMqOrderPublisher(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            // Declarar a fila de forma durável
            _channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null).GetAwaiter().GetResult();
        }

        public async Task PublishAsync(Order order)
        {
            try
            {
                var message = new
                {
                    order.Id,
                    order.CustomerName,
                    order.Amount,
                    order.OrderDate,
                    order.Status,
                    order.CreatedAt
                };

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json"
                };

                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: _settings.QueueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao publicar mensagem no RabbitMQ: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}

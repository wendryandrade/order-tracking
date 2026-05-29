using Microsoft.Extensions.Options;
using OrderTracking.Application.Interfaces;
using OrderTracking.Domain.Entities;
using OrderTracking.Domain.Enums;
using OrderTracking.Infrastructure.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace OrderTracking.Infrastructure.Messaging
{
    public class RabbitMqConsumer : IMessageConsumer, IDisposable
    {
        private readonly RabbitMqSettings _settings;
        private IConnection? _connection;
        private IChannel? _channel;
        private AsyncEventingBasicConsumer? _consumer;
        private readonly Channel<Order> _orderChannel;
        private bool _initialized = false;
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public RabbitMqConsumer(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;
            _orderChannel = Channel.CreateUnbounded<Order>();
        }

        private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
        {
            if (_initialized)
                return;

            await _initLock.WaitAsync(cancellationToken);
            try
            {
                if (_initialized)
                    return;

                var factory = new ConnectionFactory
                {
                    HostName = _settings.HostName,
                    Port = _settings.Port,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };

                // Retry logic para conectar ao RabbitMQ
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        _connection = await factory.CreateConnectionAsync(cancellationToken);
                        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
                        break;
                    }
                    catch (Exception)
                    {
                        if (i == 9)
                            throw;

                        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    }
                }

                if (_channel == null)
                    throw new InvalidOperationException("Failed to create RabbitMQ channel");

                // Declarar a fila de forma durável
                await _channel.QueueDeclareAsync(
                    queue: _settings.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken);

                // Configurar QoS para processar uma mensagem por vez
                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);

                // Criar e configurar o consumer
                _consumer = new AsyncEventingBasicConsumer(_channel);
                _consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        var orderData = JsonSerializer.Deserialize<OrderMessage>(message);

                        if (orderData != null)
                        {
                            // Reconstruir a entidade Order usando reflexão para acessar construtor privado
                            var order = (Order)Activator.CreateInstance(
                                typeof(Order),
                                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                                null,
                                new object[] { },
                                null)!;

                            // Usar reflexão para setar propriedades privadas
                            typeof(Order).GetProperty("Id")!.SetValue(order, orderData.Id);
                            typeof(Order).GetProperty("CustomerName")!.SetValue(order, orderData.CustomerName);
                            typeof(Order).GetProperty("Amount")!.SetValue(order, orderData.Amount);
                            typeof(Order).GetProperty("OrderDate")!.SetValue(order, orderData.OrderDate);
                            typeof(Order).GetProperty("Status")!.SetValue(order, orderData.Status);
                            typeof(Order).GetProperty("CreatedAt")!.SetValue(order, orderData.CreatedAt);

                            // Fazer ACK manualmente
                            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                            // Enviar para o canal
                            await _orderChannel.Writer.WriteAsync(order);
                        }
                        else
                        {
                            // Rejeitar a mensagem sem requeue
                            await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                        }
                    }
                    catch (Exception)
                    {
                        // Em caso de erro, rejeitar e reenviar para a fila
                        await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                // Iniciar o consumo
                await _channel.BasicConsumeAsync(
                    queue: _settings.QueueName,
                    autoAck: false,
                    consumer: _consumer,
                    cancellationToken: cancellationToken);

                _initialized = true;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<Order?> ConsumeOrderAsync(CancellationToken cancellationToken)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                // Ler do canal com timeout
                var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(5));

                return await _orderChannel.Reader.ReadAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Timeout ou cancelamento - retornar null
                return null;
            }
        }

        public void Dispose()
        {
            _orderChannel.Writer.Complete();
            _channel?.Dispose();
            _connection?.Dispose();
            _initLock?.Dispose();
        }

        private class OrderMessage
        {
            public Guid Id { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public DateTime OrderDate { get; set; }
            public OrderStatus Status { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}

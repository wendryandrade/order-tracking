using OrderTracking.Application.Interfaces;

namespace OrderTracking.Worker
{
    public class OrderProcessorWorker : BackgroundService
    {
        private readonly ILogger<OrderProcessorWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageConsumer _consumer;

        public OrderProcessorWorker(
            ILogger<OrderProcessorWorker> logger,
            IServiceProvider serviceProvider,
            IMessageConsumer consumer)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderProcessorWorker iniciado em: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                    _logger.LogInformation("Aguardando mensagens da fila...");

                    var order = await _consumer.ConsumeOrderAsync(stoppingToken);

                    if (order != null)
                    {
                        _logger.LogInformation(
                            "Pedido {OrderId} recebido da fila. Cliente: {CustomerName}, Valor: {Amount}",
                            order.Id,
                            order.CustomerName,
                            order.Amount);

                        // Marcar como processado ANTES de salvar
                        order.MarkAsProcessed();

                        // Verificar se o pedido já existe no banco
                        var existingOrder = await repository.GetByIdAsync(order.Id);

                        if (existingOrder != null)
                        {
                            // Atualizar pedido existente
                            existingOrder.MarkAsProcessed();
                            await repository.UpdateAsync(existingOrder);
                            _logger.LogInformation(
                                "Pedido {OrderId} atualizado no banco de dados com sucesso",
                                order.Id);
                        }
                        else
                        {
                            // Inserir novo pedido
                            await repository.AddAsync(order);
                            _logger.LogInformation(
                                "Pedido {OrderId} inserido no banco de dados com sucesso",
                                order.Id);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("OrderProcessorWorker está sendo cancelado");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem da fila: {Message}", ex.Message);

                    // Aguardar antes de tentar novamente em caso de erro
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("OrderProcessorWorker parado em: {time}", DateTimeOffset.Now);
        }
    }
}

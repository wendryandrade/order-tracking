using Microsoft.AspNetCore.Mvc;
using OrderTracking.Application.DTOs;
using OrderTracking.Application.Interfaces;

namespace OrderTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Cria um novo pedido e envia para a fila de processamento
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var orderId = await _orderService.CreateAsync(request);

                _logger.LogInformation("Pedido {OrderId} criado e enviado para fila", orderId);

                return CreatedAtAction(
                    nameof(GetOrderById),
                    new { id = orderId },
                    new CreateOrderResponse { Id = orderId, Message = "Pedido enviado para processamento" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao criar pedido");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido");
                return StatusCode(500, new { error = "Erro interno ao processar pedido" });
            }
        }

        /// <summary>
        /// Consulta um pedido específico por ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);

                if (order == null)
                {
                    _logger.LogInformation("Pedido {OrderId} não encontrado", id);
                    return NotFound(new { error = "Pedido não encontrado" });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar pedido {OrderId}", id);
                return StatusCode(500, new { error = "Erro interno ao consultar pedido" });
            }
        }

        /// <summary>
        /// Lista todos os pedidos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllAsync();

                _logger.LogInformation("Retornando {Count} pedidos", orders.Count);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar pedidos");
                return StatusCode(500, new { error = "Erro interno ao listar pedidos" });
            }
        }
    }

    public class CreateOrderResponse
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

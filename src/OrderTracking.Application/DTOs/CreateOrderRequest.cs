using System.ComponentModel.DataAnnotations;

namespace OrderTracking.Application.DTOs
{
    public class CreateOrderRequest
    {
        /// <summary>
        /// Nome do cliente
        /// </summary>
        [Required(ErrorMessage = "Nome do cliente é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome do cliente deve ter entre 3 e 200 caracteres")]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Valor do pedido
        /// </summary>
        [Required(ErrorMessage = "Valor do pedido é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor do pedido deve ser maior que zero")]
        public decimal Amount { get; set; }

        // OrderDate será gerado automaticamente pelo Service com DateTime.UtcNow
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTracking.Application.DTOs
{
    public sealed class OrderResponse
    {
        public Guid Id { get; set; }

        // Nome do cliente
        public string CustomerName { get; set; } = string.Empty;

        // Valor do pedido
        public decimal Amount { get; set; }

        // Data do pedido
        public DateTime OrderDate { get; set; }

        // Status do pedido
        public string Status { get; set; } = string.Empty;
    }
}

using OrderTracking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTracking.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }

        public string CustomerName { get; private set; }

        public decimal Amount { get; private set; }

        public DateTime OrderDate { get; private set; }

        public OrderStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        // Necessário para o Entity Framework
        private Order()
        {
        }

        public Order(
            string customerName,
            decimal amount,
            DateTime orderDate)
        {
            Id = Guid.NewGuid();
            CustomerName = customerName;
            Amount = amount;
            OrderDate = orderDate;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsProcessed()
        {
            Status = OrderStatus.Processed;
        }
    }
}

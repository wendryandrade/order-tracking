using OrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTracking.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);

        Task<List<Order>> GetAllAsync();

        Task AddAsync(Order order);

        Task UpdateAsync(Order order);
    }
}

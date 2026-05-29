using OrderTracking.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTracking.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Guid> CreateAsync(CreateOrderRequest request);

        Task<OrderResponse?> GetByIdAsync(Guid id);

        Task<List<OrderResponse>> GetAllAsync();
    }
}

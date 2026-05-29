using OrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTracking.Application.Interfaces
{
    public interface IOrderPublisher
    {
        Task PublishAsync(Order order);
    }
}

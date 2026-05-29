using OrderTracking.Domain.Entities;

namespace OrderTracking.Application.Interfaces
{
    public interface IMessageConsumer
    {
        Task<Order?> ConsumeOrderAsync(CancellationToken cancellationToken);
    }
}

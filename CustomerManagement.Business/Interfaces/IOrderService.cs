using CustomerManagement.Business.DTOs.Orders;

namespace CustomerManagement.Business.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderResponse>> GetAllAsync();

    Task<OrderResponse?> GetByIdAsync(int id);

    Task<OrderResponse> CreateAsync(
        CreateOrderRequest request);

    Task<bool> UpdateStatusAsync(
    int id,
    UpdateOrderStatusRequest request);

    Task<bool> DeleteAsync(int id);
}
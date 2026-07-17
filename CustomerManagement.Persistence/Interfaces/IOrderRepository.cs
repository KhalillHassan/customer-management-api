using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;

namespace CustomerManagement.Persistence.Repositories.Interfaces;

public interface IOrderRepository : IRepository<Order>
{

    Task<Order?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Order>> GetAllWithDetailsAsync();
}
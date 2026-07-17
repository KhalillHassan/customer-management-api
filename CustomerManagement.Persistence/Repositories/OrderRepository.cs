using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Data;
using CustomerManagement.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Persistence.Repositories;

public class OrderRepository
    : Repository<Order>, IOrderRepository  
{
    public OrderRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Order>>
        GetAllWithDetailsAsync()
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
                .ThenInclude(item => item.Product)
            .Where(order => !order.IsDeleted)
            .ToListAsync();
    }

    public async Task<Order?>
        GetByIdWithDetailsAsync(int id)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(order =>
                order.Id == id &&
                !order.IsDeleted);
    }
}
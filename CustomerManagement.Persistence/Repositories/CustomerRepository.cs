using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Data;
using CustomerManagement.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Persistence.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Customers
                .AsNoTracking()
            .AnyAsync(customer => customer.Email == email && !customer.IsDeleted);
    }
}
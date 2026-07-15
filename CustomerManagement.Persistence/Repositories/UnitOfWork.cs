using CustomerManagement.Persistence.Data;
using CustomerManagement.Persistence.Interfaces;

namespace CustomerManagement.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public ICustomerRepository Customers { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(AppDbContext context, ICustomerRepository customerRepository, IUserRepository userRepository)
    {
        _context = context;
        Customers = customerRepository;
        Users = userRepository;
    }

    public async Task<int> SaveChangesAsync()   
    {
        return await _context.SaveChangesAsync();
    }
}
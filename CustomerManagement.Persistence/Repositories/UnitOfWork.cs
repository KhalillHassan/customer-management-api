using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Data;
using CustomerManagement.Persistence.Interfaces;
using CustomerManagement.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace CustomerManagement.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    public ICustomerRepository Customers { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(
       ICustomerRepository customerRepository,
       IUserRepository userRepository,
       IProductRepository productRepository,
       IOrderRepository orderRepository,
       AppDbContext context)
    {
        Customers = customerRepository;

        Users = userRepository;

        Products = productRepository;

        _context = context;

        Orders = orderRepository;
    }

    public async Task<int> SaveChangesAsync()   
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

}
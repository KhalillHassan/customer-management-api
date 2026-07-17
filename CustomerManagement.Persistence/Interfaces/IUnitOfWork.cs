using CustomerManagement.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace CustomerManagement.Persistence.Interfaces;

public interface IUnitOfWork
{
    ICustomerRepository Customers { get; }
    IUserRepository Users { get; }

    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    Task<IDbContextTransaction> BeginTransactionAsync();

    Task<int> SaveChangesAsync();
}
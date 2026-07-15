namespace CustomerManagement.Persistence.Interfaces;

public interface IUnitOfWork
{
    ICustomerRepository Customers { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync();
}
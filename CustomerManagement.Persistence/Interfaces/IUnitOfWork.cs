namespace CustomerManagement.Persistence.Interfaces;

public interface IUnitOfWork
{
    ICustomerRepository Customers { get; }

    Task<int> SaveChangesAsync();
}
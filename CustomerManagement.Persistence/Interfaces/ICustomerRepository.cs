using CustomerManagement.Domain.Entities;

namespace CustomerManagement.Persistence.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<bool> EmailExistsAsync(string email);
}
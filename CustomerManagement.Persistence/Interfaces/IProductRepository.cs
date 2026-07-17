using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;

namespace CustomerManagement.Persistence.Repositories.Interfaces;

public interface IProductRepository
    : IRepository<Product>
{

}
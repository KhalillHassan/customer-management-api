using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Data;
using CustomerManagement.Persistence.Repositories.Interfaces;

namespace CustomerManagement.Persistence.Repositories;

public class ProductRepository
    : Repository<Product>, IProductRepository
{
    public ProductRepository(
        AppDbContext context)
        : base(context)
    {
    }
}
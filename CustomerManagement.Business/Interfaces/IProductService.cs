using CustomerManagement.Business.DTOs.Products;

namespace CustomerManagement.Business.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllAsync();

    Task<ProductResponse?> GetByIdAsync(int id);

    Task<ProductResponse> CreateAsync(
        CreateProductRequest request);

    Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request);

    Task<bool> DeleteAsync(int id);
}
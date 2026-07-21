using CustomerManagement.Business.DTOs.Products;
using CustomerManagement.Business.Interfaces;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;
using CustomerManagement.Persistence.Repositories.Interfaces;

namespace CustomerManagement.Business.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductResponse> CreateAsync(
    CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            IsDeleted = false
        };

        await _unitOfWork.Products.AddAsync(product);

        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(product);
    }

    private static ProductResponse MapToResponse(
    Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CreatedDate = product.CreatedDate
        };
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products =
            await _unitOfWork.Products.GetAllAsync();

        return products
            .Where(product => !product.IsDeleted)
            .Select(MapToResponse);
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product =
            await _unitOfWork.Products.GetByIdAsync(id);

        if (product is null || product.IsDeleted)
        {
            return null;
        }

        return MapToResponse(product);
    }

    public async Task<bool> UpdateAsync(
    int id,
    UpdateProductRequest request)
    {
        var product =
            await _unitOfWork.Products.GetByIdAsync(id);

        if (product is null || product.IsDeleted)
        {
            return false;
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.IsActive = request.IsActive;
        product.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Products.Update(product);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product =
            await _unitOfWork.Products.GetByIdAsync(id);

        if (product is null || product.IsDeleted)
        {
            return false;
        }

        product.IsDeleted = true;
        product.IsActive = false;
        product.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Products.Update(product);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
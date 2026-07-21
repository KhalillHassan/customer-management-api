using CustomerManagement.Business.DTOs.Products;
using CustomerManagement.Business.Services;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;
using CustomerManagement.Persistence.Repositories.Interfaces;
using Moq;
using Xunit;

namespace CustomerManagement.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.Products)
            .Returns(_productRepositoryMock.Object);

        _productService =
            new ProductService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_CreatesProduct()
    {
        var request = new CreateProductRequest
        {
            Name = "Laptop",
            Description = "Business laptop",
            Price = 1200m,
            StockQuantity = 10
        };

        Product? addedProduct = null;

        _productRepositoryMock
            .Setup(repository =>
                repository.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(product =>
            {
                product.Id = 1;
                addedProduct = product;
            })
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        var result =
            await _productService.CreateAsync(request);

        var savedProduct =
            Assert.IsType<Product>(addedProduct);

        Assert.Equal(1, result.Id);
        Assert.Equal("Laptop", result.Name);
        Assert.Equal("Business laptop", result.Description);
        Assert.Equal(1200m, result.Price);
        Assert.Equal(10, result.StockQuantity);
        Assert.True(result.IsActive);

        Assert.Equal("Laptop", savedProduct.Name);
        Assert.Equal("Business laptop", savedProduct.Description);
        Assert.Equal(1200m, savedProduct.Price);
        Assert.Equal(10, savedProduct.StockQuantity);
        Assert.True(savedProduct.IsActive);
        Assert.False(savedProduct.IsDeleted);
        Assert.True(savedProduct.CreatedDate != default);

        _productRepositoryMock.Verify(
            repository =>
                repository.AddAsync(It.IsAny<Product>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenProductsExist_ReturnsOnlyNonDeletedProducts()
    {
        var products = new List<Product>
        {
            new()
            {
                Id = 1,
                Name = "Laptop",
                Description = "Business laptop",
                Price = 1200m,
                StockQuantity = 10,
                IsActive = true,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                Name = "Deleted Product",
                Description = "Deleted product",
                Price = 100m,
                StockQuantity = 2,
                IsActive = false,
                IsDeleted = true,
                CreatedDate = DateTime.UtcNow
            }
        };

        _productRepositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(products);

        var result =
            (await _productService.GetAllAsync()).ToList();

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Laptop", result[0].Name);
        Assert.Equal(1200m, result[0].Price);

        _productRepositoryMock.Verify(
            repository => repository.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoProductsExist_ReturnsEmptyCollection()
    {
        _productRepositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<Product>());

        var result =
            await _productService.GetAllAsync();

        Assert.Empty(result);

        _productRepositoryMock.Verify(
            repository => repository.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProductResponse()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Description = "Business laptop",
            Price = 1200m,
            StockQuantity = 10,
            IsActive = true,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow
        };

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        var result =
            await _productService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Laptop", result.Name);
        Assert.Equal("Business laptop", result.Description);
        Assert.Equal(1200m, result.Price);
        Assert.Equal(10, result.StockQuantity);
        Assert.True(result.IsActive);

        _productRepositoryMock.Verify(
            repository => repository.GetByIdAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Product?)null);

        var result =
            await _productService.GetByIdAsync(99);

        Assert.Null(result);

        _productRepositoryMock.Verify(
            repository => repository.GetByIdAsync(99),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductIsDeleted_ReturnsNull()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Deleted Product",
            IsDeleted = true
        };

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        var result =
            await _productService.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductExists_UpdatesProduct()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Old Laptop",
            Description = "Old description",
            Price = 900m,
            StockQuantity = 5,
            IsActive = true,
            IsDeleted = false
        };

        var request = new UpdateProductRequest
        {
            Name = "Updated Laptop",
            Description = "Updated description",
            Price = 1100m,
            StockQuantity = 15,
            IsActive = false
        };

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        var result =
            await _productService.UpdateAsync(1, request);

        Assert.True(result);
        Assert.Equal("Updated Laptop", product.Name);
        Assert.Equal("Updated description", product.Description);
        Assert.Equal(1100m, product.Price);
        Assert.Equal(15, product.StockQuantity);
        Assert.False(product.IsActive);
        Assert.True(product.UpdatedDate != default);

        _productRepositoryMock.Verify(
            repository => repository.Update(product),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        var request = new UpdateProductRequest
        {
            Name = "Updated Laptop",
            Description = "Updated description",
            Price = 1100m,
            StockQuantity = 15,
            IsActive = true
        };

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Product?)null);

        var result =
            await _productService.UpdateAsync(99, request);

        Assert.False(result);

        _productRepositoryMock.Verify(
            repository => repository.Update(It.IsAny<Product>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_SoftDeletesProduct()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            IsActive = true,
            IsDeleted = false
        };

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        var result =
            await _productService.DeleteAsync(1);

        Assert.True(result);
        Assert.True(product.IsDeleted);
        Assert.False(product.IsActive);
        Assert.True(product.UpdatedDate != default);

        _productRepositoryMock.Verify(
            repository => repository.Update(product),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Product?)null);

        var result =
            await _productService.DeleteAsync(99);

        Assert.False(result);

        _productRepositoryMock.Verify(
            repository => repository.Update(It.IsAny<Product>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductIsAlreadyDeleted_ReturnsFalse()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Deleted Product",
            IsActive = false,
            IsDeleted = true
        };

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        var result =
            await _productService.DeleteAsync(1);

        Assert.False(result);

        _productRepositoryMock.Verify(
            repository => repository.Update(It.IsAny<Product>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Never);
    }
}
using CustomerManagement.Business.DTOs.Orders;
using CustomerManagement.Business.Services;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;
using CustomerManagement.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace CustomerManagement.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _transactionMock = new Mock<IDbContextTransaction>();

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.Orders)
            .Returns(_orderRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.Products)
            .Returns(_productRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.Customers)
            .Returns(_customerRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.BeginTransactionAsync())
            .ReturnsAsync(_transactionMock.Object);

        _transactionMock
            .Setup(transaction =>
                transaction.CommitAsync(
                    It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _transactionMock
            .Setup(transaction =>
                transaction.RollbackAsync(
                    It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _orderService =
            new OrderService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOrder()
    {
        var order = CreateDetailedOrder();

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(order);

        var result =
            await _orderService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Ali Hassan", result.CustomerName);
        Assert.Equal(100m, result.TotalAmount);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdWithDetailsAsync(99))
            .ReturnsAsync((Order?)null);

        var result =
            await _orderService.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_CommitsTransaction()
    {
        var customer = new Customer
        {
            Id = 1,
            FullName = "Ali Hassan",
            IsDeleted = false
        };

        var product = new Product
        {
            Id = 5,
            Name = "Laptop",
            Price = 50m,
            StockQuantity = 10,
            IsActive = true,
            IsDeleted = false
        };

        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = 5,
                    Quantity = 2
                }
            ]
        };

        Order? createdOrder = null;

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(5))
            .ReturnsAsync(product);

        _orderRepositoryMock
            .Setup(repository =>
                repository.AddAsync(It.IsAny<Order>()))
            .Callback<Order>(order =>
            {
                order.Id = 100;
                createdOrder = order;
            })
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        _orderRepositoryMock
            .Setup(repository =>
                repository.GetByIdWithDetailsAsync(100))
            .ReturnsAsync(() => createdOrder);

        var result =
            await _orderService.CreateAsync(request);

        Assert.Equal(100, result.Id);
        Assert.Equal(100m, result.TotalAmount);
        Assert.Equal(8, product.StockQuantity);

        _transactionMock.Verify(
            transaction =>
                transaction.CommitAsync(
                    It.IsAny<CancellationToken>()),
            Times.Once);

        _transactionMock.Verify(
            transaction =>
                transaction.RollbackAsync(
                    It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenStockIsInsufficient_RollsBackTransaction()
    {
        var customer = new Customer
        {
            Id = 1,
            FullName = "Ali Hassan",
            IsDeleted = false
        };

        var product = new Product
        {
            Id = 5,
            Name = "Laptop",
            Price = 50m,
            StockQuantity = 1,
            IsActive = true,
            IsDeleted = false
        };

        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = 5,
                    Quantity = 3
                }
            ]
        };

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(5))
            .ReturnsAsync(product);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _orderService.CreateAsync(request));

        _transactionMock.Verify(
            transaction =>
                transaction.RollbackAsync(
                    It.IsAny<CancellationToken>()),
            Times.Once);

        _transactionMock.Verify(
            transaction =>
                transaction.CommitAsync(
                    It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenStatusIsValid_UpdatesOrder()
    {
        var order = new Order
        {
            Id = 1,
            Status = "Pending",
            IsDeleted = false
        };

        var request = new UpdateOrderStatusRequest
        {
            Status = "Completed"
        };

        _orderRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(order);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        var result =
            await _orderService.UpdateStatusAsync(1, request);

        Assert.True(result);
        Assert.Equal("Completed", order.Status);

        _orderRepositoryMock.Verify(
            repository => repository.Update(order),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenStatusIsInvalid_ThrowsException()
    {
        var request = new UpdateOrderStatusRequest
        {
            Status = "Shipped"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _orderService.UpdateStatusAsync(1, request));

        _orderRepositoryMock.Verify(
            repository =>
                repository.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_SoftDeletesOrder()
    {
        var order = new Order
        {
            Id = 1,
            IsActive = true,
            IsDeleted = false
        };

        _orderRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(order);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        var result =
            await _orderService.DeleteAsync(1);

        Assert.True(result);
        Assert.True(order.IsDeleted);
        Assert.False(order.IsActive);

        _orderRepositoryMock.Verify(
            repository => repository.Update(order),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderDoesNotExist_ReturnsFalse()
    {
        _orderRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Order?)null);

        var result =
            await _orderService.DeleteAsync(99);

        Assert.False(result);

        _orderRepositoryMock.Verify(
            repository =>
                repository.Update(It.IsAny<Order>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(),
            Times.Never);
    }

    private static Order CreateDetailedOrder()
    {
        var customer = new Customer
        {
            Id = 1,
            FullName = "Ali Hassan"
        };

        var product = new Product
        {
            Id = 5,
            Name = "Laptop",
            Price = 50m
        };

        var order = new Order
        {
            Id = 1,
            CustomerId = 1,
            Customer = customer,
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            TotalAmount = 100m,
            IsActive = true,
            IsDeleted = false
        };

        order.OrderItems.Add(new OrderItem
        {
            Id = 10,
            OrderId = 1,
            Order = order,
            ProductId = 5,
            Product = product,
            Quantity = 2,
            UnitPrice = 50m
        });

        return order;
    }
}
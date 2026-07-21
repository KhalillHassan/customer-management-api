using CustomerManagement.Business.DTOs;
using CustomerManagement.Business.Services;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;
using Moq;
using Xunit;

namespace CustomerManagement.Tests.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.Customers)
            .Returns(_customerRepositoryMock.Object);

        _customerService =
            new CustomerService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenCustomersExist_ReturnsMappedCustomers()
    {
        var customers = new List<Customer>
        {
            new()
            {
                Id = 1,
                FullName = "Ali Hassan",
                Email = "ali@example.com",
                PhoneNumber = "70123456",
                IsActive = true,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                FullName = "Sara Ahmad",
                Email = "sara@example.com",
                PhoneNumber = "71123456",
                IsActive = true,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow
            }
        };

        _customerRepositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(customers);

        var result =
            (await _customerService.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Ali Hassan", result[0].FullName);
        Assert.Equal("ali@example.com", result[0].Email);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("Sara Ahmad", result[1].FullName);
        Assert.Equal("sara@example.com", result[1].Email);

        _customerRepositoryMock.Verify(
            repository => repository.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoCustomersExist_ReturnsEmptyCollection()
    {
        _customerRepositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<Customer>());

        var result =
            await _customerService.GetAllAsync();

        Assert.Empty(result);

        _customerRepositoryMock.Verify(
            repository => repository.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsCustomerResponse()
    {
        var customer = new Customer
        {
            Id = 1,
            FullName = "Ali Hassan",
            Email = "ali@example.com",
            PhoneNumber = "70123456",
            IsActive = true,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow
        };

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(customer);

        var result =
            await _customerService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Ali Hassan", result.FullName);
        Assert.Equal("ali@example.com", result.Email);
        Assert.Equal("70123456", result.PhoneNumber);
        Assert.True(result.IsActive);

        _customerRepositoryMock.Verify(
            repository => repository.GetByIdAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Customer?)null);

        var result =
            await _customerService.GetByIdAsync(99);

        Assert.Null(result);

        _customerRepositoryMock.Verify(
            repository => repository.GetByIdAsync(99),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailDoesNotExist_CreatesCustomer()
    {
        var dto = new CreateCustomerDto
        {
            FullName = "Maya Khalil",
            Email = "maya@example.com",
            PhoneNumber = "76123456"
        };

        Customer? addedCustomer = null;

        _customerRepositoryMock
            .Setup(repository =>
                repository.EmailExistsAsync(dto.Email))
            .ReturnsAsync(false);

        _customerRepositoryMock
            .Setup(repository =>
                repository.AddAsync(It.IsAny<Customer>()))
            .Callback<Customer>(customer =>
            {
                customer.Id = 10;
                addedCustomer = customer;
            })
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        var result =
            await _customerService.CreateAsync(dto);

        var savedCustomer =
            Assert.IsType<Customer>(addedCustomer);

        Assert.Equal(10, result.Id);
        Assert.Equal("Maya Khalil", result.FullName);
        Assert.Equal("maya@example.com", result.Email);
        Assert.Equal("76123456", result.PhoneNumber);
        Assert.True(result.IsActive);

        Assert.Equal("Maya Khalil", savedCustomer.FullName);
        Assert.Equal("maya@example.com", savedCustomer.Email);
        Assert.Equal("System", savedCustomer.CreatedBy);
        Assert.True(savedCustomer.IsActive);
        Assert.False(savedCustomer.IsDeleted);
        Assert.True(savedCustomer.CreatedDate != default);

        _customerRepositoryMock.Verify(
            repository =>
                repository.EmailExistsAsync(dto.Email),
            Times.Once);

        _customerRepositoryMock.Verify(
            repository =>
                repository.AddAsync(It.IsAny<Customer>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailAlreadyExists_ThrowsException()
    {
        var dto = new CreateCustomerDto
        {
            FullName = "Maya Khalil",
            Email = "existing@example.com",
            PhoneNumber = "76123456"
        };

        _customerRepositoryMock
            .Setup(repository =>
                repository.EmailExistsAsync(dto.Email))
            .ReturnsAsync(true);

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _customerService.CreateAsync(dto));

        Assert.Equal(
            "A customer with this email already exists.",
            exception.Message);

        _customerRepositoryMock.Verify(
            repository =>
                repository.AddAsync(It.IsAny<Customer>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerExists_UpdatesCustomer()
    {
        var customer = new Customer
        {
            Id = 1,
            FullName = "Old Name",
            Email = "old@example.com",
            PhoneNumber = "70111111",
            IsActive = true,
            IsDeleted = false
        };

        var dto = new UpdateCustomerDto
        {
            FullName = "Updated Name",
            Email = "updated@example.com",
            PhoneNumber = "70222222",
            IsActive = false
        };

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(customer);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        var result =
            await _customerService.UpdateAsync(1, dto);

        Assert.True(result);
        Assert.Equal("Updated Name", customer.FullName);
        Assert.Equal("updated@example.com", customer.Email);
        Assert.Equal("70222222", customer.PhoneNumber);
        Assert.False(customer.IsActive);
        Assert.Equal("System", customer.UpdatedBy);
        Assert.True(customer.UpdatedDate != default);

        _customerRepositoryMock.Verify(
            repository => repository.Update(customer),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerDoesNotExist_ReturnsFalse()
    {
        var dto = new UpdateCustomerDto
        {
            FullName = "Updated Name",
            Email = "updated@example.com",
            PhoneNumber = "70222222",
            IsActive = true
        };

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Customer?)null);

        var result =
            await _customerService.UpdateAsync(99, dto);

        Assert.False(result);

        _customerRepositoryMock.Verify(
            repository =>
                repository.Update(It.IsAny<Customer>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_DeletesCustomer()
    {
        var customer = new Customer
        {
            Id = 1,
            FullName = "Ali Hassan",
            Email = "ali@example.com",
            IsActive = true,
            IsDeleted = false
        };

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(customer);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync())
            .ReturnsAsync(1);

        var result =
            await _customerService.DeleteAsync(1);

        Assert.True(result);

        _customerRepositoryMock.Verify(
            repository => repository.Delete(customer),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerDoesNotExist_ReturnsFalse()
    {
        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Customer?)null);

        var result =
            await _customerService.DeleteAsync(99);

        Assert.False(result);

        _customerRepositoryMock.Verify(
            repository =>
                repository.Delete(It.IsAny<Customer>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(),
            Times.Never);
    }
}
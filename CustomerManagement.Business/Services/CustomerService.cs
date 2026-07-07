using CustomerManagement.Business.DTOs;
using CustomerManagement.Business.Interfaces;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;

namespace CustomerManagement.Business.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CustomerResponseDto>> GetAllAsync()
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();

        return customers.Select(MapToResponseDto);
    }

    public async Task<CustomerResponseDto?> GetByIdAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);

        if (customer == null)
        {
            return null;
        }

        return MapToResponseDto(customer);
    }

    public async Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto)
    {
        var emailExists = await _unitOfWork.Customers.EmailExistsAsync(dto.Email);

        if (emailExists)
        {
            throw new InvalidOperationException("A customer with this email already exists.");
        }

        var customer = new Customer
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "System",
            IsActive = true,
            IsDeleted = false
        };

        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(customer);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);

        if (customer == null)
        {
            return false;
        }

        customer.FullName = dto.FullName;
        customer.Email = dto.Email;
        customer.PhoneNumber = dto.PhoneNumber;
        customer.IsActive = dto.IsActive;
        customer.UpdatedDate = DateTime.UtcNow;
        customer.UpdatedBy = "System";

        _unitOfWork.Customers.Update(customer);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);

        if (customer == null)
        {
            return false;
        }

        _unitOfWork.Customers.Delete(customer);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static CustomerResponseDto MapToResponseDto(Customer customer)
    {
        return new CustomerResponseDto
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            IsActive = customer.IsActive,
            CreatedDate = customer.CreatedDate
        };
    }
}
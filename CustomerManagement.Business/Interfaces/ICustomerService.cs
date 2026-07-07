using CustomerManagement.Business.DTOs;

namespace CustomerManagement.Business.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseDto>> GetAllAsync();

    Task<CustomerResponseDto?> GetByIdAsync(int id);

    Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto);

    Task<bool> UpdateAsync(int id, UpdateCustomerDto dto);

    Task<bool> DeleteAsync(int id);
}
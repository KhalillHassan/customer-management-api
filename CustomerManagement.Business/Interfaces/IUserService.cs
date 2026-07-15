using CustomerManagement.Business.DTOs.Users;

namespace CustomerManagement.Business.Interfaces;

public interface IUserService
{
    Task CreateAsync(CreateUserRequest request);
}
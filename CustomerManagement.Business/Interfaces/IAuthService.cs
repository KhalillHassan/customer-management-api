using CustomerManagement.Business.DTOs.Auth;
namespace CustomerManagement.Business.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}

using CustomerManagement.Business.DTOs.Users;
using CustomerManagement.Business.Interfaces;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CustomerManagement.Business.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    public UserService(
    IUnitOfWork unitOfWork,
    IPasswordHasher<User> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task CreateAsync(
    CreateUserRequest request)
    {
        var existingUser =
            await _unitOfWork.Users
                .GetByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Role = request.Role
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                request.Password);

        await _unitOfWork.Users.AddAsync(user);

        await _unitOfWork.SaveChangesAsync();
    }
}
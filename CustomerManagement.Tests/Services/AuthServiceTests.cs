using CustomerManagement.Business.DTOs.Auth;
using CustomerManagement.Business.Services;
using CustomerManagement.Business.Settings;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;
using CustomerManagement.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CustomerManagement.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.Users)
            .Returns(_userRepositoryMock.Object);

        var jwtSettings = new JwtSettings
        {
            SecretKey =
                "this-is-a-long-test-secret-key-with-at-least-32-bytes",
            Issuer = "CustomerManagementApi",
            Audience = "CustomerManagementClient",
            ExpirationMinutes = 60
        };

        _authService = new AuthService(
            Options.Create(jwtSettings),
            _passwordHasherMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsToken()
    {
        var user = new User
        {
            Id = 1,
            FullName = "Admin User",
            Email = "admin@example.com",
            PasswordHash = "hashed-password",
            Role = "Admin"
        };

        var request = new LoginRequest
        {
            Email = "admin@example.com",
            Password = "Admin123!"
        };

        _userRepositoryMock
            .Setup(repository =>
                repository.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(hasher =>
                hasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password))
            .Returns(PasswordVerificationResult.Success);

        var result = await _authService.LoginAsync(request);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.True(result.ExpiresAt > DateTime.UtcNow);

        _userRepositoryMock.Verify(
            repository =>
                repository.GetByEmailAsync(request.Email),
            Times.Once);

        _passwordHasherMock.Verify(
            hasher =>
                hasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var request = new LoginRequest
        {
            Email = "missing@example.com",
            Password = "Password123!"
        };

        _userRepositoryMock
            .Setup(repository =>
                repository.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        var result = await _authService.LoginAsync(request);

        Assert.Null(result);

        _passwordHasherMock.Verify(
            hasher =>
                hasher.VerifyHashedPassword(
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsIncorrect_ReturnsNull()
    {
        var user = new User
        {
            Id = 1,
            FullName = "Admin User",
            Email = "admin@example.com",
            PasswordHash = "hashed-password",
            Role = "Admin"
        };

        var request = new LoginRequest
        {
            Email = "admin@example.com",
            Password = "WrongPassword"
        };

        _userRepositoryMock
            .Setup(repository =>
                repository.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(hasher =>
                hasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password))
            .Returns(PasswordVerificationResult.Failed);

        var result = await _authService.LoginAsync(request);

        Assert.Null(result);

        _passwordHasherMock.Verify(
            hasher =>
                hasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password),
            Times.Once);
    }
}
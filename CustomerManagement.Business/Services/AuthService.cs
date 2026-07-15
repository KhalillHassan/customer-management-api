using CustomerManagement.Business.DTOs.Auth;
using CustomerManagement.Business.Interfaces;
using CustomerManagement.Business.Settings;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace CustomerManagement.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(IOptions<JwtSettings> jwtOptions, IPasswordHasher<User> passwordHasher, IUnitOfWork unitOfWork)
        {
            _jwtSettings = jwtOptions.Value;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }
        public async Task<LoginResponse?> LoginAsync(
     LoginRequest request)
        {
            var user = await _unitOfWork.Users
                .GetByEmailAsync(request.Email);

            if (user is null)
            {
                return null;
            }

            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(ClaimTypes.Name, user.FullName)
    };

            var expiresAt = DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpirationMinutes);

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var signingCredentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: signingCredentials);

            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return new LoginResponse
            {
                Token = tokenString,
                ExpiresAt = expiresAt
            };
        }
    }
}
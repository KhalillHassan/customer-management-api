using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Data;
using CustomerManagement.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

public class UserRepository
    : Repository<User>, IUserRepository
{
    public UserRepository(
        AppDbContext context)
        : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(
        string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user =>
                user.Email == email &&
                !user.IsDeleted &&
                user.IsActive);
    }
}
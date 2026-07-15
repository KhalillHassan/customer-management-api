using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Data;
using CustomerManagement.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Persistence.Repositories
{
    public class UserRepository
     : Repository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
    : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
            .FirstOrDefaultAsync(user =>
             user.Email == email &&
             !user.IsDeleted &&
             user.IsActive);
        }
    }
}

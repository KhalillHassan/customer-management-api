using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
using LuxMedTest.Infrastructure.Data;
using LuxMedTest.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using LuxMedTest.Domain.Models;

namespace LuxMedTest.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<User?> GetByUsernameAsync(string username) => await context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task AddUserAsync(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}

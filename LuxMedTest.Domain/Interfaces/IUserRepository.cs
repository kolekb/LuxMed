using LuxMedTest.Domain.Models;

namespace LuxMedTest.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}

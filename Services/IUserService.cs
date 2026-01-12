using BarmanBank.Models;

namespace BarmanBank.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> RegisterAsync(string username, string password);
        Task<User> GetUserAsync(int id);
    }
}

using BarmanBank.Data;
using BarmanBank.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace BarmanBank.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _repo;
        private readonly AppDbContext _context;

        public UserService(IRepository<User> repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var users = await _repo.FindAsync(u => u.Username == username);
            var user = users.FirstOrDefault();
            if (user == null) return null;

            if (VerifyPassword(password, user.PasswordHash)) return user;
            return null;
        }

        public async Task<User> RegisterAsync(string username, string password)
        {
            var exists = (await _repo.FindAsync(u => u.Username == username)).Any();
            if (exists) throw new Exception("User already exists");

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password)
            };
            await _repo.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserAsync(int id) => await _repo.GetAsync(id);

        private string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8));

            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        private bool VerifyPassword(string password, string stored)
        {
            var parts = stored.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            string attempted = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8));

            return attempted == hash;
        }
    }
}

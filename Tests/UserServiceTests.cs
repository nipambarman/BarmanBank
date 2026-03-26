using BarmanBank.Models;
using BarmanBank.Services;
using BarmanBank.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly AppDbContext _context;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb").Options;
        _context = new AppDbContext(options);

        var userRepo = new Repository<User>(_context);
        _userService = new UserService(userRepo, _context);
    }

    [Fact]
    public async Task Register_Then_Authenticate_User()
    {
        var user = await _userService.RegisterAsync("testuser", "Password123");
        var auth = await _userService.AuthenticateAsync("testuser", "Password123");
        Assert.NotNull(auth);
        Assert.Equal(user.Username, auth.Username);
    }
}

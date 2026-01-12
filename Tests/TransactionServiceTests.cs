using BarmanBank.Models;
using BarmanBank.Services;
using BarmanBank.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TransactionServiceTests
{
    private readonly TransactionService _txnService;
    private readonly AppDbContext _context;
    private readonly IRepository<User> _userRepo;

    public TransactionServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TxnTestDb").Options;
        _context = new AppDbContext(options);

        _userRepo = new Repository<User>(_context);
        var txnRepo = new Repository<Transaction>(_context);
        _txnService = new TransactionService(_context, txnRepo, _userRepo);
    }

    [Fact]
    public async Task Deposit_Updates_Balance()
    {
        var user = new User { Username = "txnuser", PasswordHash = "hash" };
        await _userRepo.AddAsync(user);
        await _context.SaveChangesAsync();

        var txn = await _txnService.DepositAsync(user.Id, 100);
        var updatedUser = await _userRepo.GetAsync(user.Id);

        Assert.Equal(100, updatedUser.Balance);
        Assert.Equal(TransactionType.Deposit, txn.Type);
    }
}

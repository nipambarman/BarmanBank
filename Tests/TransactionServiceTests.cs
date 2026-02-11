using BarmanBank.Models;
using BarmanBank.Services;
using BarmanBank.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;
using Razorpay.Api; // only if you actually use RazorpayClient

public class TransactionServiceTests
{
    private readonly TransactionService _txnService;
    private readonly AppDbContext _context;
    private readonly IRepository<User> _userRepo;

    public TransactionServiceTests()
    {
        // 1. Initialize in-memory DbContext
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TxnTestDb")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new AppDbContext(options);

        // 2. Repositories
        _userRepo = new Repository<User>(_context);
        var txnRepo = new Repository<Transaction>(_context);

        // 3. TransactionService
        _txnService = new TransactionService(_context, txnRepo, _userRepo);
    }


    [Fact]
    public async Task Deposit_Updates_Balance()
    {
        // Arrange
        var user = new User { Username = "txnuser", PasswordHash = "hash" };
        await _userRepo.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var txn = await _txnService.DepositAsync(user.Id, 100);
        var updatedUser = await _userRepo.GetAsync(user.Id);

        // Assert
        Assert.Equal(100, updatedUser.Balance);
        Assert.Equal(TransactionType.Deposit, txn.Type);
        Assert.NotNull(txn.ReferenceId); // extra sanity check
    }
}

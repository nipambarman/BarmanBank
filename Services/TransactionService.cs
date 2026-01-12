using BarmanBank.Data;
using BarmanBank.Models;

namespace BarmanBank.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Transaction> _repo;
        private readonly IRepository<User> _userRepo;

        public TransactionService(AppDbContext context,
            IRepository<Transaction> repo, IRepository<User> userRepo)
        {
            _context = context;
            _repo = repo;
            _userRepo = userRepo;
        }

        public async Task<Transaction> DepositAsync(int userId, decimal amount, string paymentId = null)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            var user = await _userRepo.GetAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.Balance += amount;
            await _userRepo.UpdateAsync(user);
            await _context.SaveChangesAsync();

            var txn = new Transaction
            {
                UserId = userId,
                Amount = amount,
                Type = TransactionType.Deposit,
                PaymentGatewayId = paymentId
            };

            await _repo.AddAsync(txn);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return txn;
        }

        public async Task<Transaction> WithdrawAsync(int userId, decimal amount)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            var user = await _userRepo.GetAsync(userId);
            if (user == null) throw new Exception("User not found");
            if (user.Balance < amount) throw new Exception("Insufficient funds");

            user.Balance -= amount;
            await _userRepo.UpdateAsync(user);

            var txn = new Transaction
            {
                UserId = userId,
                Amount = amount,
                Type = TransactionType.Withdrawal
            };

            await _repo.AddAsync(txn);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return txn;
        }

        public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId)
            => await _repo.FindAsync(t => t.UserId == userId);
    }
}

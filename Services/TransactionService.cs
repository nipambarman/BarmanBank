using BarmanBank.Data;
using BarmanBank.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarmanBank.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Transaction> _repo;
        private readonly IRepository<User> _userRepo;

        public TransactionService(
            AppDbContext context,
            IRepository<Transaction> repo,
            IRepository<User> userRepo)
        {
            _context = context;
            _repo = repo;
            _userRepo = userRepo;
        }

        // Deposit updates balance and creates a transaction
        public async Task<Transaction> DepositAsync(int userId, decimal amount, string? paymentId = null)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var user = await _userRepo.GetAsync(userId)
                ?? throw new Exception("User not found");

            user.Balance += amount;

            var txn = new Transaction
            {
                UserId = userId,
                Type = TransactionType.Deposit,
                Amount = amount,
                BalanceAfter = user.Balance,
                Status = TransactionStatus.Completed,
                ReferenceId = paymentId ?? $"DEP_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}",
                RazorpayOrderId = null, // placeholder, no Razorpay
                PaymentGatewayId = null
            };

            await _userRepo.UpdateAsync(user);
            await _repo.AddAsync(txn);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return txn;
        }

        // Withdraw updates balance and creates a transaction
        public async Task<Transaction> WithdrawAsync(int userId, decimal amount, string? paymentId = null)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var user = await _userRepo.GetAsync(userId)
                ?? throw new Exception("User not found");

            if (user.Balance < amount)
                throw new Exception("Insufficient funds");

            user.Balance -= amount;

            var txn = new Transaction
            {
                UserId = userId,
                Type = TransactionType.Withdrawal,
                Amount = amount,
                BalanceAfter = user.Balance,
                Status = TransactionStatus.Completed,
                RazorpayOrderId = null, // placeholder, no Razorpay
                PaymentGatewayId = null,
                ReferenceId = paymentId ?? $"WD_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}"
            };

            await _userRepo.UpdateAsync(user);
            await _repo.AddAsync(txn);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return txn;
        }

        // Get transactions for a user
        public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId)
            => await _repo.FindAsync(t => t.UserId == userId);
    }
}

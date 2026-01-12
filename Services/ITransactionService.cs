using BarmanBank.Models;

namespace BarmanBank.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId);
        Task<Transaction> DepositAsync(int userId, decimal amount, string paymentId = null);
        Task<Transaction> WithdrawAsync(int userId, decimal amount);
    }
}

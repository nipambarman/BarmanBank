using BarmanBank.Models;

namespace BarmanBank.ViewModels
{
    public class TransactionsIndexViewModel
    {
        public decimal Balance { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}

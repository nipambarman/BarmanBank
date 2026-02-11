using BarmanBank.Models;
using System.Collections.Generic;


namespace BarmanBank.ViewModels
{
    public class TransactionsIndexViewModel
    {
        // Make required so compiler knows these are always set
        public required IEnumerable<Transaction> Transactions { get; set; }
        public required string DailyInsight { get; set; }

        public TransactionsIndexViewModel()
        {
            Transactions = new List<Transaction>();
            DailyInsight = string.Empty;

        }

        public decimal Balance { get; set; } = 0m;
    }
}

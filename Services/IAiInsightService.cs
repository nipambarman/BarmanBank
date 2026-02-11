using BarmanBank.Models;
public interface IAiInsightService
{
    string GenerateDailyInsight(IEnumerable<Transaction> transactions);
}

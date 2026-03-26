using BarmanBank.Models;
public class AiInsightService : IAiInsightService
{
    public string GenerateDailyInsight(IEnumerable<Transaction> txns)
    {
        var today = DateTime.UtcNow.Date;
        var todayTxns = txns.Where(t => t.CreatedAt.Date == today);

        var deposits = todayTxns
            .Where(t => t.Type == TransactionType.Deposit)
            .Sum(t => t.Amount);

        var withdrawals = todayTxns
            .Where(t => t.Type == TransactionType.Withdrawal)
            .Sum(t => t.Amount);

        if (!todayTxns.Any())
            return "No transactions today. Good day to plan your savings!";

        if (withdrawals > deposits)
            return "You spent more than you added today. Consider limiting discretionary spending.";

        return "Great job! You maintained a positive balance today.";
    }
}

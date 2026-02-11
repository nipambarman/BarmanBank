namespace BarmanBank.Models
{
    public enum TransactionType { Deposit, Withdrawal }
    //public enum TransactionStatus { Pending, Completed, Failed }
    public enum TransactionStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2
    }


    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public TransactionStatus Status { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // ADD THIS
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;
        //public string RazorpayOrderId { get; set; } = string.Empty; // Required
        public string? RazorpayOrderId { get; set; } // can be null until payment is created

        //public string PaymentGatewayId { get; set; } // Required for deposits
        public string? PaymentGatewayId { get; set; } // optional for demo deposits


    }

}

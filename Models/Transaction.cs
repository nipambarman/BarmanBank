using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarmanBank.Models
{
    public enum TransactionType { Deposit, Withdrawal }

    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public TransactionType Type { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string PaymentGatewayId { get; set; }

        public User User { get; set; }
    }
}

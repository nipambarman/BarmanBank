namespace BarmanBank.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Initialized
        public decimal Balance { get; set; }

        // Optional: Navigation property
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}

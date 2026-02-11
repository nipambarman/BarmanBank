namespace BarmanBank.Models
{
    public class AuditLogs
    {
        public int Id { get; set; }

        // Mark required or initialize
        public required string Event { get; set; }
        public required string ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

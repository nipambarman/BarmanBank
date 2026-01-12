using System.ComponentModel.DataAnnotations;

namespace BarmanBank.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public required string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public decimal Balance { get; set; } = 0;
    }
}

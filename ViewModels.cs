using System.ComponentModel.DataAnnotations;


namespace BarmanBank.ViewModels
{
    public class LoginViewModel
    {
        [Required] public required string Username { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required] public required string Username { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
    }

    public class DepositViewModel
    {
        [Required, Range(1, double.MaxValue)]
        public decimal Amount { get; set; }
        public string RazorpayOrderId { get; set; }
    }
}

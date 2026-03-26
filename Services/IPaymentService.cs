namespace BarmanBank.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentOrderAsync(int userId, decimal amount);
        Task<bool> VerifyPaymentWebhookAsync(string payload, string signature);
    }
}

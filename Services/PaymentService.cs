using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace BarmanBank.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        public PaymentService(IConfiguration config)
        {
            _config = config;
        }

        public Task<string> CreatePaymentOrderAsync(int userId, decimal amount)
        {
            // Normally you'd call Razorpay's REST API here. For demo, return a fake order id
            return Task.FromResult($"order_{Guid.NewGuid():N}");
        }

        public Task<bool> VerifyPaymentWebhookAsync(string payload, string signature)
        {
            var secret = _config["Razorpay:WebhookSecret"] ?? "demo_secret";
            var hash = new HMACSHA256(Encoding.UTF8.GetBytes(secret))
                        .ComputeHash(Encoding.UTF8.GetBytes(payload));
            var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return Task.FromResult(computed == signature.ToLower());
        }
    }
}

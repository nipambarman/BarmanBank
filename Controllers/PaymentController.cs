using BarmanBank.Services;
using BarmanBank.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace BarmanBank.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ITransactionService _txnService;

        public PaymentController(IPaymentService paymentService, ITransactionService txnService)
        {
            _paymentService = paymentService;
            _txnService = txnService;
        }

        [HttpGet]
        public IActionResult Deposit() => View(new DepositViewModel());

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositViewModel vm)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            // Create Razorpay Order
            var orderId = await _paymentService.CreatePaymentOrderAsync(userId.Value, vm.Amount);
            vm.RazorpayOrderId = orderId;

            if (!ModelState.IsValid) 
            {
                try
                {
                    await _txnService.DepositAsync(userId.Value, vm.Amount, vm.RazorpayOrderId);

                    TempData["Success"] = "Deposit successful";
                    return RedirectToAction("Index", "Transactions");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(vm);
                }

            }
            
            
            
            return View("Checkout", vm);// Show Razorpay checkout page
        }

        [HttpPost]
        public async Task<IActionResult> Webhook()
        {
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();
            var signature = Request.Headers["X-Razorpay-Signature"];

            bool isValid = await _paymentService.VerifyPaymentWebhookAsync(payload, signature);

            if (!isValid) return BadRequest();

            // Parse payload and update DB transaction
            dynamic json = System.Text.Json.JsonSerializer.Deserialize<dynamic>(payload);
            int userId = int.Parse(json["payload"]["payment"]["entity"]["notes"]["user_id"].ToString());
            decimal amount = int.Parse(json["payload"]["payment"]["entity"]["amount"].ToString()) / 100m;
            string paymentId = json["payload"]["payment"]["entity"]["id"].ToString();

            await _txnService.DepositAsync(userId, amount, paymentId);
            return Ok();
        }
    }
}

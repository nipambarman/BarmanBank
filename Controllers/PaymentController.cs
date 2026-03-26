using BarmanBank.Services;
using BarmanBank.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace BarmanBank.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ITransactionService _txnService;
        private readonly ILogger<PaymentController> _logger;

        private const decimal MaxDepositAmount = 200_000m; // max per transaction

        public PaymentController(IPaymentService paymentService, ITransactionService txnService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _txnService = txnService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Deposit() => View(new DepositViewModel());

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositViewModel vm)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid) return View(vm);

            try
            {
                if (vm.Amount > MaxDepositAmount)
                {
                    ModelState.AddModelError("", $"Maximum deposit per transaction is ₹{MaxDepositAmount:N0}");
                    _logger.LogWarning("User {UserId} tried to deposit {Amount} exceeding max limit {MaxAmount}", userId, vm.Amount, MaxDepositAmount);
                    return View(vm);
                }

                // Process deposit
                var txn = await _txnService.DepositAsync(userId.Value, vm.Amount);

                TempData["Success"] = $"Deposit successful: {txn.Amount:C}";
                _logger.LogInformation(
    "Transaction completed | Ref={ReferenceId} | Type={Type}",
    txn.ReferenceId,
    txn.Type
);


                return RedirectToAction("Index", "Transactions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during deposit for user {UserId}", userId);
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        [HttpGet]
        public IActionResult Withdraw() => View(new WithdrawViewModel());

        [HttpPost]
        public async Task<IActionResult> Withdraw(WithdrawViewModel wm)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid) return View(wm);

            try
            {
                var txn = await _txnService.WithdrawAsync(userId.Value, wm.Amount);
                TempData["Success"] = $"Withdrawal successful: {txn.Amount:C}";
                _logger.LogInformation(
    "Transaction completed | Ref={ReferenceId} | Type={Type}",
    txn.ReferenceId,
    txn.Type
);

                return RedirectToAction("Index", "Transactions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during withdrawal for user {UserId}", userId);
                ModelState.AddModelError("", ex.Message);
                return View(wm);
            }
        }
    }
}

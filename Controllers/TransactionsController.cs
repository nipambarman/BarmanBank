using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BarmanBank.Services;
using BarmanBank.ViewModels;
using BarmanBank.Models;

namespace BarmanBank.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _txnService;
        private readonly IUserService _userService;
        private readonly IAiInsightService _ai;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(
            ITransactionService txnService,
            IUserService userService,
            IAiInsightService ai,
            ILogger<TransactionsController> logger)
        {
            _txnService = txnService ?? throw new ArgumentNullException(nameof(txnService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _ai = ai ?? throw new ArgumentNullException(nameof(ai));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: /Transactions
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user == null)
                {
                    _logger.LogWarning("TransactionsController.Index: User not found.");
                    return RedirectToAction("Login", "Account");
                }

                // Fetch transactions safely (parameterized in service layer)
                //var txns = await _txnService.GetTransactionsAsync(user.Id);
                var txns = await _txnService.GetUserTransactionsAsync(user.Id);

                // Generate AI daily insight
                var insight = _ai.GenerateDailyInsight(txns);

                // Prepare ViewModel
                var vm = new TransactionsIndexViewModel
                {
                    Balance = user.Balance,
                    Transactions = txns,
                    DailyInsight = insight
                };

                _logger.LogInformation("Transactions loaded successfully for user {UserId}", user.Id);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading transactions for current user.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /Transactions/Deposit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(decimal amount)
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            // Returns transaction with RazorpayOrderId
            var txn = await _txnService.DepositAsync(user.Id, amount);

            // You can now pass txn.RazorpayOrderId to the frontend for payment completion
            TempData["RazorpayOrderId"] = txn.RazorpayOrderId;

            return RedirectToAction(nameof(Index));
        }


        // POST: /Transactions/Withdraw
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(decimal amount)
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user == null)
                    return Unauthorized();

                await _txnService.WithdrawAsync(user.Id, amount);

                _logger.LogInformation("Withdraw operation executed for user {UserId}", user.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing withdrawal for user {UserId}", User.Identity.Name);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

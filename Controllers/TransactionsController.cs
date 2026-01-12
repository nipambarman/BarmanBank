using BarmanBank.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarmanBank.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _txnService;

        public TransactionsController(ITransactionService txnService)
        {
            _txnService = txnService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var txns = await _txnService.GetUserTransactionsAsync(userId.Value);
            return View(txns);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var txns = await _txnService.GetUserTransactionsAsync(userId.Value);
            var txn = txns.FirstOrDefault(t => t.Id == id);
            if (txn == null) return NotFound();

            return View(txn);
        }
    }
}

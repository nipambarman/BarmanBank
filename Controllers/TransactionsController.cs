using BarmanBank.Services;
using Microsoft.AspNetCore.Mvc;
using BarmanBank.ViewModels;


namespace BarmanBank.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _txnService;
        private readonly IUserService _userService;

        public TransactionsController(ITransactionService txnService, IUserService userService)
        {
            _txnService = txnService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var txns = await _txnService.GetUserTransactionsAsync(userId.Value);
            var user = await _userService.GetUserAsync(userId.Value);
            var vm = new TransactionsIndexViewModel
            {
                Balance = user.Balance,
                Transactions = txns
            };

            return View(vm);
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

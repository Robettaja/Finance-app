using finance.Models;
using finance.Models.Tables;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace finance.Controllers
{

    public class EconomyController : Controller
    {
        [Authorize]
        [Route("/Expense")]
        public async Task<IActionResult> Expense()
        {
            ExpenseViewModel model = new();
            User user = await DatabaseManipulator.GetSingle<User>(u => u.Username == User.Identity!.Name);

            model.Expenses = await DatabaseManipulator.GetMany<Expense>(e => e.UserId == user.Id);
            model.Sum = model.Expenses.Sum(x => x.Amount);
            model.UserId = user.Id;
            return View(model);
        }
        [Authorize]
        [RequireAntiforgeryToken]
        public async Task<IActionResult> _ExpenseList()
        {
            User user = await DatabaseManipulator.GetSingle<User>(u => u.Username == User.Identity!.Name);
            List<Expense> expenses = await DatabaseManipulator.GetMany<Expense>(e => e.UserId == user.Id);
            return PartialView(expenses);
        }


        [Authorize]
        [Route("/Income")]
        public async Task<IActionResult> Income()
        {
            List<Income> incomes = [];
            return View(incomes);
        }

        [Authorize]
        [IgnoreAntiforgeryToken]
        [Route("/Expense/List")]
        public async Task<IActionResult> ExpenseList(string? search)
        {
            User user = await DatabaseManipulator.GetSingle<User>(u => u.Username == User.Identity!.Name);
            var expenses = await DatabaseManipulator.GetMany<Expense>(e =>
                e.UserId == user.Id &&
                (string.IsNullOrEmpty(search) || e.ExpenseName.Contains(search)));
            return PartialView("_ExpenseList", expenses);
        }

        [Authorize]
        [HttpGet]
        [RequireAntiforgeryToken]
        [Route("Expense/DeleteConfirm")]
        public async Task<IActionResult> _DeleteConfirm(string id)
        {
            Console.WriteLine("Delete confirmed");
            return PartialView("_DeleteConfirm", id);

        }
        [HttpPost]
        [RequireAntiforgeryToken]
        [Authorize]
        [Route("Economy/DeleteExpense")]
        public async Task<IActionResult> DeleteExpense(ObjectId id)
        {
            await DatabaseManipulator.DeleteOne<Expense>(e => e.Id == id);
            return RedirectToAction("_ExpenseList");

        }

        [HttpPost]
        [Authorize]
        [RequireAntiforgeryToken]
        [Route("/Expense")]
        public async Task<IActionResult> Expense(ExpenseViewModel model)
        {
            User user = await DatabaseManipulator.GetSingle<User>(u => u.Username == User.Identity!.Name);
            Expense expense = new()
            {
                ExpenseName = model.ExpenseName,
                Amount = model.Amount,
                Gategory = model.Gategory,
                UserId = user.Id

            };
            await DatabaseManipulator.Save(expense);
            return RedirectToAction("Expense");

        }
    }
}

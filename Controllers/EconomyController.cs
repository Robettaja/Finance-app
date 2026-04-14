using finance.Models;
using finance.Models.Tables;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace finance.Controllers
{

    public class EconomyController : Controller
    {
        [Authorize]
        [Route("/Expense")]
        public async Task<IActionResult> Expense()
        {
            User user = await GetUser();
            ExpenseViewModel model = new() { UserId = user.Id };
            await model.Initialize();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [RequireAntiforgeryToken]
        [Route("/Expense")]
        public async Task<IActionResult> Expense(ExpenseViewModel model)
        {
            User user = await GetUser();
            model.UserId = user.Id;
            await model.CreateExpense();
            return RedirectToAction("Expense");

        }


        [Authorize]
        [Route("/Income")]
        public async Task<IActionResult> Income()
        {
            User user = await GetUser();
            IncomeViewModel model = new() { UserId = user.Id };
            await model.Initialize();

            return View(model);
        }
        [Authorize]
        [RequireAntiforgeryToken]
        [HttpPost]
        [Route("/Income")]
        public async Task<IActionResult> Income(IncomeViewModel model)
        {
            User user = await GetUser();
            model.UserId = user.Id;
            await model.CreateIncome();

            return RedirectToAction("Income");
        }

        [Authorize]
        [Route("/Expense/List")]
        [RequireAntiforgeryToken]
        public async Task<IActionResult> _ExpenseList(string? search)
        {
            User user = await GetUser();
            var expenses = await DatabaseManipulator.GetMany<Expense>(e =>
                e.UserId == user.Id &&
                (string.IsNullOrEmpty(search) || e.ExpenseName.ToLower().Contains(search.ToLower())));
            return PartialView("_ExpenseList", expenses);
        }

        [Authorize]
        [IgnoreAntiforgeryToken]
        [Route("/Income/List")]
        public async Task<IActionResult> IncomeList(string? search)
        {
            User user = await GetUser();
            var incomes = await DatabaseManipulator.GetMany<Income>(e =>
                e.UserId == user.Id &&
                (string.IsNullOrEmpty(search) || e.IncomeName.ToLower().Contains(search.ToLower())));
            return PartialView("_IncomeList", incomes);
        }


        [Authorize]
        [HttpGet]
        [RequireAntiforgeryToken]
        [Route("Expense/DeleteConfirm")]
        public async Task<IActionResult> _DeleteConfirm(ConfirmDeleteModel model)
        {
            return PartialView("_DeleteConfirm", model);
        }
        [HttpPost]
        [RequireAntiforgeryToken]
        [Authorize]
        [Route("Economy/DeleteExpense")]
        public async Task<IActionResult> DeleteExpense(ObjectId id)
        {
            await DatabaseManipulator.DeleteOne<Expense>(e => e.Id == id);
            User user = await GetUser();
            var expenses = await DatabaseManipulator.GetMany<Expense>(e => e.UserId == user.Id);
            return PartialView("_ExpenseList", expenses);

        }
        [HttpPost]
        [RequireAntiforgeryToken]
        [Authorize]
        [Route("Economy/DeleteIncome")]
        public async Task<IActionResult> DeleteIncome(ObjectId id)
        {
            await DatabaseManipulator.DeleteOne<Income>(e => e.Id == id);
            User user = await GetUser();
            var incomes = await DatabaseManipulator.GetMany<Income>(e => e.UserId == user.Id);
            return PartialView("_IncomeList", incomes);

        }

        public async Task<User> GetUser()
        {
            return await DatabaseManipulator.GetSingle<User>(u => u.Username == User.Identity!.Name);

        }
    }
}

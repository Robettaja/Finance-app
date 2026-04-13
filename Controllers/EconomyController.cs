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
            ExpenseViewModel model = new();
            User user = await DatabaseManipulator.GetSingle<User>(u => u.Username == User.Identity!.Name);

            model.Expenses = await DatabaseManipulator.GetMany<Expense>(e => e.UserId == user.Id);
            model.Sum = model.Expenses.Sum(x => x.Amount);
            model.MostExpensiveGategory = await DatabaseManipulator.GetTopCategory(user.Id);
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
            IncomeViewModel model = new();
            User user = await GetUser();
            model.UserId = user.Id;

            var incomes = await DatabaseManipulator.GetMany<Income>(e => e.UserId == user.Id);

            foreach (var income in incomes.Where(i => i.IsRecurring && i.Payday < DateTime.Now))
            {
                income.Payday = income.Payday!.Value.AddMonths(
                    (int)Math.Ceiling((DateTime.Now - income.Payday.Value).TotalDays / 30));
                await DatabaseManipulator.Update(income, i => i.Id == income.Id);
            }

            model.Incomes = await DatabaseManipulator.GetMany<Income>(e => e.UserId == user.Id);
            return View(model);
        }
        [Authorize]
        [HttpPost]
        [Route("/Income")]
        public async Task<IActionResult> Income(IncomeViewModel model)
        {
            User user = await GetUser();
            Income income = new()
            {
                IncomeName = model.IncomeName,
                Amount = model.Amount,
                Payday = model.Payday,
                IsRecurring = model.IsRecuring,
                UserId = user.Id
            };
            await DatabaseManipulator.Save(income);


            return View(model);
        }

        [Authorize]
        [IgnoreAntiforgeryToken]
        [Route("/Expense/List")]
        public async Task<IActionResult> IncomeList(string? search)
        {
            User user = await GetUser();
            var expenses = await DatabaseManipulator.GetMany<Income>(e =>
                e.UserId == user.Id &&
                (string.IsNullOrEmpty(search) || e.IncomeName.Contains(search.ToLower())));
            return PartialView("_IncomeList", expenses);
        }

        [Authorize]
        [IgnoreAntiforgeryToken]
        [Route("/Expense/List")]
        public async Task<IActionResult> ExpenseList(string? search)
        {
            User user = await GetUser();
            var expenses = await DatabaseManipulator.GetMany<Expense>(e =>
                e.UserId == user.Id &&
                (string.IsNullOrEmpty(search) || e.ExpenseName.Contains(search.ToLower())));
            return PartialView("_ExpenseList", expenses);
        }

        [Authorize]
        [HttpGet]
        [RequireAntiforgeryToken]
        [Route("Expense/DeleteConfirm")]
        public async Task<IActionResult> _DeleteConfirm(string id)
        {
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
            User user = await GetUser();
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

        public async Task<User> GetUser()
        {
            return await DatabaseManipulator.GetSingle<User>(u => u.Username == User.Identity!.Name);

        }
    }
}

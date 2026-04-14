using finance.Models.Tables;
using MongoDB.Bson;

namespace finance.Models
{
    public class ExpenseViewModel
    {
        public string? ExpenseName { get; set; }
        public int Amount { get; set; }
        public string? Gategory { get; set; }
        public List<Expense>? Expenses { get; set; } = [];
        public int Sum { get; set; }
        public string? MostExpensiveGategory { get; set; }
        public required ObjectId UserId { get; set; }

        public async Task Initialize()
        {
            Expenses = await DatabaseManipulator.GetMany<Expense>(e => e.UserId == UserId);
            Sum = Expenses.Sum(x => x.Amount);
            MostExpensiveGategory = await DatabaseManipulator.GetTopCategory(UserId);

        }

        public async Task CreateExpense()
        {
            Expense expense = new()
            {
                ExpenseName = ExpenseName,
                Amount = Amount,
                Gategory = Gategory,
                UserId = UserId
            };
            await DatabaseManipulator.Save(expense);
            Expenses = await DatabaseManipulator.GetMany<Expense>(e => e.UserId == UserId);
        }
    }
}

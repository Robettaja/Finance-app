using finance.Models.Tables;
using MongoDB.Bson;

namespace finance.Models
{
    public class ExpenseViewModel
    {
        public string? ExpenseName { get; set; }
        public float Amount { get; set; }
        public string? Gategory { get; set; }
        public List<Expense>? Expenses { get; set; } = [];
        public float Sum { get; set; } = 0;
        public string? MostExpensiveGategory { get; set; }
        public required ObjectId UserId { get; set; }

        public async Task Initialize()
        {

            Expenses = (await DatabaseManipulator.GetMany<Expense>(e => e.UserId == UserId)).OrderByDescending(e => e.Timestamp).ToList();
            Sum = Expenses
                .Where(e => e.Timestamp.Month == DateTime.Now.Month &&
                            e.Timestamp.Year == DateTime.Now.Year)
                .Sum(x => x.Amount);

            MostExpensiveGategory = await DatabaseManipulator.GetTopCategory(UserId);

        }

        public async Task CreateExpense()
        {
            Expense expense = new()
            {
                ExpenseName = ExpenseName,
                Amount = Amount,
                Gategory = Gategory,
                Timestamp = DateTime.Now,
                UserId = UserId

            };
            await DatabaseManipulator.Save(expense);
            Expenses = await DatabaseManipulator.GetMany<Expense>(e => e.UserId == UserId);
        }
    }
}

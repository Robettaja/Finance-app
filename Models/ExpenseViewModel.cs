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
        public ObjectId UserId { get; set; }

    }
}

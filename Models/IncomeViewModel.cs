using finance.Models.Tables;
using MongoDB.Bson;

namespace finance.Models
{
    public class IncomeViewModel
    {
        public string? IncomeName { get; set; }
        public int Amount { get; set; }
        public DateTime Payday { get; set; } = DateTime.Now;
        public bool IsRecuring { get; set; }
        public int Sum { get; set; }
        public int DaysUntilPayday { get; set; }
        public ObjectId UserId { get; set; }
        public List<Income> Incomes { get; set; } = [];

    }
}

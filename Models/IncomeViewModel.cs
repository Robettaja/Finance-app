using System.Security.Claims;
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
        public float Sum { get; set; }
        public required ObjectId UserId { get; set; }
        public List<Income> Incomes { get; set; } = [];

        public async Task Initialize()
        {
            Incomes = (await DatabaseManipulator.GetMany<Income>(e => e.UserId == UserId)).OrderByDescending(e => e.Timestamp).ToList();
            Sum = Incomes
                    .Where(e => e.Timestamp.Month == DateTime.Now.Month && e.Timestamp.Year == DateTime.Now.Year)
                    .Sum(x => x.Amount);


        }
        public async Task CreateIncome()
        {
            Income income = new()
            {
                IncomeName = IncomeName,
                Amount = Amount,
                Timestamp = DateTime.Now,
                Payday = Payday,
                IsRecurring = IsRecuring,
                UserId = UserId
            };
            await DatabaseManipulator.Save(income);
            Incomes = await DatabaseManipulator.GetMany<Income>(e => e.UserId == UserId);

        }

    }
}

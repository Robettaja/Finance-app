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
        public int Sum { get; set; }
        public int DaysUntilPayday { get; set; }
        public required ObjectId UserId { get; set; }
        public List<Income> Incomes { get; set; } = [];

        public async Task Initialize()
        {
            Incomes = await DatabaseManipulator.GetMany<Income>(e => e.UserId == UserId);

            foreach (var income in Incomes.Where(i => i.IsRecurring && i.Payday < DateTime.Now))
            {
                income.Payday = income.Payday!.Value.AddMonths(
                    (int)Math.Ceiling((DateTime.Now - income.Payday.Value).TotalDays / 30));
                await DatabaseManipulator.Update(income, i => i.Id == income.Id);
            }
            Incomes = await DatabaseManipulator.GetMany<Income>(e => e.UserId == UserId);

        }
        public async Task CreateIncome()
        {
            Income income = new()
            {
                IncomeName = IncomeName,
                Amount = Amount,
                Payday = Payday,
                IsRecurring = IsRecuring,
                UserId = UserId
            };
            await DatabaseManipulator.Save(income);
            Incomes = await DatabaseManipulator.GetMany<Income>(e => e.UserId == UserId);

        }

    }
}


using finance.Models.Tables;
using MongoDB.Bson;

namespace finance.Models
{
    public class HomeViewModel
    {

        public ChartData? ExpenseMonthlies { get; set; }
        public ExpenseViewModel? ExpenseModel { get; set; }
        public IncomeViewModel? IncomeModel { get; set; }
        public float Savings { get; set; }
        public float SavingGoal { get; set; }
        public float SavingGoalDaily { get; set; }
        public int TimeToPayday { get; set; }

        public float AverageCostDaily { get; set; }
        public required ObjectId UserId { get; set; }

        public async Task Initialize()
        {
            ExpenseMonthlies = await DatabaseManipulator.GetLast6MonthsChart(UserId);

            // vähän tyhmää tehdä kaksi kanta hakua mutta äänyväys
            SavingGoal = (await DatabaseManipulator.GetSingle<User>(u => u.Id == UserId)).SavingGoal;

            ExpenseModel = new() { UserId = UserId };
            await ExpenseModel.Initialize();
            IncomeModel = new() { UserId = UserId };
            await IncomeModel.Initialize();
            Savings = IncomeModel.Sum - ExpenseModel.Sum;


            SavingGoalDaily = (IncomeModel.Sum - ExpenseModel.Sum - SavingGoal) / 30;

            var nextPayday = IncomeModel.Incomes
                .Where(e => e.Payday > DateTime.Now)
                .OrderBy(e => e.Payday)
                .FirstOrDefault();
            TimeToPayday = nextPayday != null
                ? (int)(nextPayday.Payday - DateTime.Now).TotalDays
                : 0;

            AverageCostDaily = ExpenseModel.Sum / 30;

        }
    }
    public class ChartData
    {
        public List<string> Labels { get; set; } = [];
        public List<float> Data { get; set; } = [];
    }
}

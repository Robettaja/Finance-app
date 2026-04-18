using System.Linq.Expressions;
using finance.Models.Tables;
using MongoDB.Bson;
using MongoDB.Driver;

namespace finance.Models;

public static class DatabaseManipulator
{
    private static IConfiguration? config;
    private static string? databaseName;
    private static string? host;
    private static MongoServerAddress? address;
    private static MongoClientSettings? settings;
    private static MongoClient? client;
    private static IMongoDatabase? database;

    public static void Initialize(IConfiguration configuration)
    {
        config = configuration;
        IConfigurationSection sections = config.GetSection("ConnectionStrings");
        databaseName = sections.GetValue<string>("DatabaseName");
        host = sections.GetValue<string>("MongoConnection");
        address = new MongoServerAddress(host);
        settings = new MongoClientSettings() { Server = address };
        client = new MongoClient(settings);
        database = client.GetDatabase(databaseName);
    }
    public static async Task Save<T>(T saveableObject)
    {
        var table = database.GetCollection<T>(saveableObject.GetType().Name);
        try
        {
            await table.InsertOneAsync(saveableObject);

        }
        catch
        {
            Console.WriteLine("Error saving object");

        }

    }
    public static async Task Update<T>(T saveableObject, Expression<Func<T, bool>> filter)
    {
        var table = database.GetCollection<T>(saveableObject.GetType().Name);
        try
        {
            await table.ReplaceOneAsync(filter, saveableObject);

        }
        catch
        {

        }

    }

    public static async Task SaveMany<T>(List<T> items)
    {
        var table = database.GetCollection<T>(typeof(T).Name);

        try
        {
            await table.InsertManyAsync(items);
        }
        catch
        {

        }
    }
    public static async Task<T> GetSingle<T>(Expression<Func<T, bool>> filter)
    {
        var table = database.GetCollection<T>(typeof(T).Name);
        try
        {
            return await table.Find(filter).FirstOrDefaultAsync();
        }
        catch
        {
            Console.WriteLine("Error retrieving object");
            return default(T);
        }
    }
    public static async Task<List<T>> GetMany<T>(Expression<Func<T, bool>> filter)
    {

        var table = database.GetCollection<T>(typeof(T).Name);
        try
        {
            List<T> list = await table.Find(filter).ToListAsync();
            return list;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving objects: {ex.Message}");
        }
        return [];

    }
    public static async Task<ChartData> GetLast6MonthsChart(ObjectId userId)
    {
        var table = database.GetCollection<Expense>(nameof(Expense));

        var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
            .AddMonths(-5);

        var raw = await table.Aggregate()
            .Match(x => x.UserId == userId && x.Timestamp >= startDate)
            .Group(
                x => new { x.Timestamp.Year, x.Timestamp.Month },
                g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Total = g.Sum(x => x.Amount)
                }
            )
            .ToListAsync();

        var result = new ChartData();

        for (int i = 0; i < 6; i++)
        {
            var date = startDate.AddMonths(i);

            var match = raw.FirstOrDefault(x =>
                x.Year == date.Year && x.Month == date.Month);

            result.Labels.Add(date.ToString("MMM"));

            result.Data.Add(match?.Total ?? 0);
        }

        return result;
    }
    public static async Task<string?> GetTopCategory(ObjectId userId)
    {
        var table = database.GetCollection<Expense>("Expense");

        var result = await table.Aggregate()
            .Match(x => x.UserId == userId)
            .Group(x => x.Gategory, g => new
            {
                Category = g.Key,
                Total = g.Sum(x => x.Amount)
            })
            .SortByDescending(x => x.Total)
            .FirstOrDefaultAsync();

        return result?.Category;
    }
    public static async Task DeleteOne<T>(Expression<Func<T, bool>> filter)
    {
        var table = database.GetCollection<T>(typeof(T).Name);
        try
        {
            await table.DeleteOneAsync(filter);
        }
        catch
        {
            Console.WriteLine("Error retrieving objects");
        }

    }

    public class MonthlyExpense
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Total { get; set; }
    }
}

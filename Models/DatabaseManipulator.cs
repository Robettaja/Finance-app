using System.Linq.Expressions;
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

    public static async Task SaveMany<T, TKey>(
        List<T> saveableObjects,
        Func<T, TKey> keySelector,
        FilterDefinition<T> duplicateFilter = null)
    {
        if (saveableObjects == null || saveableObjects.Count == 0)
            return;

        var table = database.GetCollection<T>(typeof(T).Name);

        try
        {
            var uniqueItems = saveableObjects
                .GroupBy(keySelector)
                .Select(g => g.First())
                .ToList();

            if (duplicateFilter != null)
            {
                var existingItems = await table
                    .Find(duplicateFilter)
                    .ToListAsync();

                var existingKeys = existingItems
                    .Select(keySelector)
                    .ToHashSet();

                uniqueItems = uniqueItems
                    .Where(x => !existingKeys.Contains(keySelector(x)))
                    .ToList();
            }

            if (uniqueItems.Count == 0)
                return;

            await table.InsertManyAsync(uniqueItems);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving objects: {ex.Message}");
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
            return await table.Find(filter).ToListAsync();
        }
        catch
        {
            Console.WriteLine("Error retrieving objects");
        }
        return [];

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
}

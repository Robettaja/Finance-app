using MongoDB.Bson;

namespace finance.Models.Tables
{
    public class Expense : SaveableObject
    {
        public string? ExpenseName { get; set; }
        public int Amount { get; set; }
        public string? Gategory { get; set; }
        public ObjectId UserId { get; set; }
    }
}

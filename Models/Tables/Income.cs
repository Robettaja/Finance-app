
using MongoDB.Bson;

namespace finance.Models.Tables
{
    public class Income : SaveableObject
    {
        public string? IncomeName { get; set; }
        public float Amount { get; set; }
        public DateTime Payday { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRecurring { get; set; }
        public ObjectId UserId { get; set; }
    }
}

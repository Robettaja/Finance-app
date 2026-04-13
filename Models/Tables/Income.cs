
using MongoDB.Bson;

namespace finance.Models.Tables
{
    public class Income : SaveableObject
    {
        public string? IncomeName { get; set; }
        public int Amount { get; set; }
        public DateTime Payday { get; set; }
        public bool IsRecuring { get; set; }
        public ObjectId UserId { get; set; }
    }
}

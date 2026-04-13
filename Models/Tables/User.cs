using MongoDB.Bson;

namespace finance.Models.Tables
{
    class User : SaveableObject
    {
        public string? Username { get; set; }
        public byte[]? Salt { get; set; }
        public byte[]? Password { get; set; }

    }
}

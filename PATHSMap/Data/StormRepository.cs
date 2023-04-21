using Dapper;
using System.Data;

namespace PATHSMap.Data
{
    public class StormRepository
    {
        private readonly IDbConnection _conn;
        public StormRepository(IDbConnection conn)
        {
            _conn = conn;
        }
        public void CreateStorm(string id, string headline, string areaDesc, DateTime expiration, string description, string messageType, string motion, string eventType)
        {
            _conn.Execute("INSERT INTO Storm (headline, areaDesc, expiration, id, description, motion, messageType, eventType) VALUES (@headline, @areaDesc, @expiration, @id, @description, @motion, @messageType, @eventType);",
                new { id = id, headline = headline, expiration = expiration, areaDesc = areaDesc, description = description, messageType = messageType, motion = motion, eventType = eventType});
        }
        public IEnumerable<Storm> GetAllStorms()
        {
            return _conn.Query<Storm>("SELECT * FROM Storm;");
        }
    }
}

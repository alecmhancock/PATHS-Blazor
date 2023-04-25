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



        public void CreateStorm(Storm storm)
        {
            _conn.Execute("INSERT INTO Storm (headline, areaDesc, expiration, id, description, motion, messageType, eventType, geometry) VALUES (@headline, @areaDesc, @expiration, @id, @description, @motion, @messageType, @eventType, @geometry);",
                new { id = storm.id, headline = storm.headline, expiration = storm.expiration, areaDesc = storm.areaDesc, description = storm.description, messageType = storm.messageType, motion = storm.motion, eventType = storm.@event, geometry = storm.geometry});
        }

        public IEnumerable<Storm> GetAllStorms()
        {
            return _conn.Query<Storm>("SELECT * FROM Storm;");
        }

        public void UpdateStorm(Storm storm)
        {
            _conn.Execute("UPDATE Storm SET headline = @headline, areaDesc = @areaDesc, expiration = @expiration, id = @id, description = @description, motion = @motion, messageType = @messageType, eventType = @eventType, geometry = @geometry WHERE id = @id;",
                               new { id = storm.id, headline = storm.headline, expiration = storm.expiration, areaDesc = storm.areaDesc, description = storm.description, messageType = storm.messageType, motion = storm.motion, eventType = storm.@event, geometry = storm.geometry });
        }
        
        public void DeleteStorm(Storm storm) 
        { 
            _conn.Execute("DELETE FROM Storm WHERE id = @id;", new { id = storm.id });
        }
        public Storm GetStormById(string id)
        {
            return _conn.QueryFirstOrDefault<Storm>("SELECT * FROM Storm WHERE id = @id;", new { id });
        }


    }
}

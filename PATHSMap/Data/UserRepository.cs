using Dapper;
using System.Data;

namespace PATHSMap.Data
{
    public class UserRepository
    {
        private readonly IDbConnection _conn;
        public UserRepository(IDbConnection conn)
        {
            _conn = conn;
        }
        public void CreateUser(UserData userdata)
        {
            DateTime timecreated = DateTime.Now;
            _conn.Execute("INSERT INTO UserData (zip, units, language, timecreated) VALUES (@zip, @units, @language, @timecreated);",
                new {zip = userdata.zip, units = userdata.units, language = userdata.language, timecreated = userdata.timecreated});
        }
        public IEnumerable<UserData> GetAllUsers()
        {
            return _conn.Query<UserData>("SELECT * FROM UserData;");
        }
        public void DeleteUser(UserData userdata)
        {
            _conn.Execute("DELETE FROM UserData WHERE timecreated = @t;", new { zip = userdata.zip, units = userdata.units, language = userdata.language});
        }
    }
}

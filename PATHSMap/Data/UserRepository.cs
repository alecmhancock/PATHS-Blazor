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
            _conn.Execute("INSERT INTO UserData (zip, units, language) VALUES (@zip, @units, @language);",
                new {zip = userdata.zip, units = userdata.units, language = userdata.language});
        }
        public IEnumerable<UserData> GetAllUsers()
        {
            return _conn.Query<UserData>("SELECT * FROM UserData;");
        }
        public void DeleteUser(UserData userdata)
        {
            _conn.Execute("DELETE FROM UserData WHERE zip = @zip, units = @units, language = @language;", new { zip = userdata.zip, units = userdata.units, language = userdata.language});
        }
    }
}

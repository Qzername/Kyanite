using LiteDB;
using System.IO;

namespace Whiteboard.Services
{
    public class LiteDbConnection
    {
        LiteDatabase _database;
        public LiteDatabase Database => _database;

        public LiteDbConnection()
        {
            if (!Directory.Exists("./Database/"))
                Directory.CreateDirectory("./Database/");

            _database = new LiteDatabase("./Database/database.db");
        }
    }
}

using LiteDB;
using System;
using System.Diagnostics;
using System.IO;

namespace Whiteboard.Services
{
    public class LiteDbConnection
    {
        LiteDatabase _database;
        public LiteDatabase Database => _database;

        public void OpenConnection()
        {
            string path= 
                Environment.GetFolderPath(Environment.SpecialFolder.Personal)+
                "/Database/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = path + "database.db";

            _database = new LiteDatabase(path);
        }

        public void CloseConnection()
        {
            _database = null;
        }
    }
}

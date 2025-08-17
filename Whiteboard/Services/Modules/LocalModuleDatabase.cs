using LiteDB;
using System;
using System.Linq;
using Whiteboard.Models;

namespace Whiteboard.Services.Modules
{
    /// <summary>
    /// database is localy stored in nosql LiteDB database.
    /// </summary>
    public class LocalModuleDatabase : IModuleDatabase
    {
        LiteDbConnection connection;

        public LocalModuleDatabase(LiteDbConnection connection)
        {
            this.connection = connection;
        }

        public ModuleInfo[] GetModules()
        {
            var collection = connection.Database.GetCollection<ModuleInfo>("Modules");
            return collection.FindAll().ToArray();
        }

        public bool ExistModule(int moduleId)
        {
            return connection.Database.GetCollection<ModuleInfo>("Modules").Exists("$._id = " + moduleId);
        }

        public void AddModule(ModuleInfo moduleInfo)
        {
            moduleInfo.ID = 0;

            var collection = connection.Database.GetCollection<ModuleInfo>("Modules");
            collection.Insert(moduleInfo);
            connection.Database.Checkpoint();
        }

        public void DeleteModule(int moduleId)
        {
            bool tableExist = connection.Database.CollectionExists("module" + moduleId.ToString());
            bool infoExist = connection.Database.GetCollection<ModuleInfo>("Modules").Exists("$._id = " + moduleId);

            if (!infoExist && !tableExist)
                throw new Exception("Table and info doesnt exist");

            if (infoExist)
            {
                var collection = connection.Database.GetCollection<ModuleInfo>("Modules");
                collection.Delete(moduleId);
            }

            if (tableExist)
                connection.Database.DropCollection("module" + moduleId.ToString());
            connection.Database.Checkpoint();
        }

        public void UpdateModule(int moduleId, ModuleInfo name)
        {
            var valuesTable = connection.Database.GetCollection<ModuleInfo>("Modules");
            var moduleList = valuesTable.Find(x => x.ID == moduleId);

            if (moduleList.Count() == 0)
                throw new Exception("Module not found");

            var tempModule = moduleList.First();
            tempModule.Name = name.Name;

            valuesTable.Update(tempModule);
            connection.Database.Checkpoint();
        }
    }
}

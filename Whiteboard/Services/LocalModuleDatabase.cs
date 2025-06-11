using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whiteboard.Models;

namespace Whiteboard.Services
{
    public class LocalModuleDatabase : IModuleDatabase
    {
        LiteDatabase database;

        public LocalModuleDatabase(LiteDbConnection connection)
        {
            database = connection.Database;
        }

        public ModuleInfo[] GetModules()
        {
            var collection = database.GetCollection<ModuleInfo>("Modules");
            return collection.FindAll().ToArray();
        }

        public bool ExistModule(int moduleId)
        {
            return database.GetCollection<ModuleInfo>("Modules").Exists("$._id = " + moduleId);
        }

        public void AddModule(ModuleInfo moduleInfo)
        {
            moduleInfo.ID = 0;

            var collection = database.GetCollection<ModuleInfo>("Modules");
            collection.Insert(moduleInfo);
        }

        public void DeleteModule(int moduleId)
        {
            bool tableExist = database.CollectionExists("module" + moduleId.ToString());
            bool infoExist = database.GetCollection<ModuleInfo>("Modules").Exists("$._id = " + moduleId);

            if (!infoExist && !tableExist)
                throw new Exception("Table and info doesnt exist");

            if (infoExist)
            {
                var collection = database.GetCollection<ModuleInfo>("Modules");
                collection.Delete(moduleId);
            }

            if (tableExist)
                database.DropCollection("module" + moduleId.ToString());
        }

        public void UpdateModule(int moduleId, ModuleInfo name)
        {
            var valuesTable = database.GetCollection<ModuleInfo>("Modules");
            var moduleList = valuesTable.Find(x => x.ID == moduleId);

            if (moduleList.Count() == 0)
                throw new Exception("Module not found");

            var tempModule = moduleList.First();
            tempModule.Name = name.Name;

            valuesTable.Update(tempModule);
        }
    }
}

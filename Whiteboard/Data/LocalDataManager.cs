using LiteDB;
using System;
using System.Linq;
using Whiteboard.Models;

namespace Whiteboard.Data
{
    public class LocalDataManager : IDataManager
    {
        LiteDatabase database;

        public LocalDataManager()
        {
            database = new LiteDatabase("./Database/database.db");
        }

        #region Module
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
        #endregion

        #region Data item
        public DataItem GetDataItem(int moduleId, string name)
        {
            var valuesTable = database.GetCollection<DataItem>("module" + moduleId.ToString());
            var dataItem = valuesTable.Find(x => x.Name == name);

            if (dataItem.Count() == 0)
                throw new Exception("Data item not found");

            return dataItem.First();
        }

        public bool ExistDataItem(int moduleId, string name)
        {
            var valuesTable = database.GetCollection<DataItem>("module" + moduleId.ToString());
            var dataItem = valuesTable.Find(x => x.Name == name);

            return dataItem.Count() > 0;    
        }

        public void AddDataItem(int moduleId, DataItem item)
        {
            var valuesTable = database.GetCollection<DataItem>("module" + moduleId.ToString());

            if (!valuesTable.EnsureIndex(x => x.Name, true))
                throw new Exception("Data item not unique");

            valuesTable.Insert(item);
        }

        public void UpdateDataItem(int moduleId, DataItem item)
        {
            var valuesTable = database.GetCollection<DataItem>("module" + moduleId.ToString());

            var dataItem = valuesTable.Find(x => x.Name == item.Name);

            if (dataItem.Count() == 0)
                throw new Exception("Data item not found");

            item.Id = dataItem.First().Id;

            valuesTable.Update(item);
        }
        #endregion
    }
}

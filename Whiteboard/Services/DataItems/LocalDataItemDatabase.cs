using LiteDB;
using System;
using System.Linq;
using Whiteboard.Models;

namespace Whiteboard.Services.DataItems
{
    /// <summary>
    /// database is localy stored in nosql LiteDB database.
    /// </summary>
    public class LocalDataItemDatabase : IDataItemDatabase
    {
        LiteDbConnection connection;

        public LocalDataItemDatabase(LiteDbConnection connection)
        {
            this.connection = connection;
        }

        public DataItem GetDataItem(int moduleId, string name)
        {
            var valuesTable = connection.Database.GetCollection<DataItem>("module" + moduleId.ToString());
            var dataItem = valuesTable.Find(x => x.Name == name);

            if (dataItem.Count() == 0)
                throw new Exception("Data item not found");

            return dataItem.First();
        }

        public bool ExistDataItem(int moduleId, string name)
        {
            var valuesTable = connection.Database.GetCollection<DataItem>("module" + moduleId.ToString());
            var dataItem = valuesTable.Find(x => x.Name == name);

            return dataItem.Count() > 0;
        }

        public void AddDataItem(int moduleId, DataItem item)
        {
            var valuesTable = connection.Database.GetCollection<DataItem>("module" + moduleId.ToString());

            if (!valuesTable.EnsureIndex(x => x.Name, true))
                throw new Exception("Data item not unique");

            valuesTable.Insert(item);
            connection.Database.Checkpoint();
        }

        public void UpdateDataItem(int moduleId, DataItem item)
        {
            var valuesTable = connection.Database.GetCollection<DataItem>("module" + moduleId.ToString());

            var dataItem = valuesTable.Find(x => x.Name == item.Name);

            if (dataItem.Count() == 0)
                throw new Exception("Data item not found");

            item.Id = dataItem.First().Id;

            valuesTable.Update(item);
            connection.Database.Checkpoint();
        }
    }
}

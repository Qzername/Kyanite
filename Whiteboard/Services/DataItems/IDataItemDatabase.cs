using Whiteboard.Models;

namespace Whiteboard.Services.DataItems
{
    /// <summary>
    /// Resposible for reading dataitems
    /// </summary>
    public interface IDataItemDatabase
    {
        public DataItem GetDataItem(int moduleId, string name);
        public bool ExistDataItem(int moduleId, string name);
        public void AddDataItem(int moduleId, DataItem item);
        public void UpdateDataItem(int moduleId, DataItem item);
    }
}

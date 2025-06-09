using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whiteboard.Models;

namespace Whiteboard.Data
{
    public interface IDataManager
    {
        public ModuleInfo[] GetModules();
        public bool ExistModule(int moduleId);
        public void AddModule(ModuleInfo moduleInfo);
        public void DeleteModule(int moduleId);

        public DataItem GetDataItem(int moduleId, string name);
        public bool ExistDataItem(int moduleId, string name);
        public void AddDataItem(int moduleId, DataItem item);
        public void UpdateDataItem(int moduleId, DataItem item);
    }
}

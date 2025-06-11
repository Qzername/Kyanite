using Whiteboard.Models;

namespace Whiteboard.Services.Modules
{
    /// <summary>
    /// Resposible for reading data about modules
    /// </summary>
    public interface IModuleDatabase
    {
        public ModuleInfo[] GetModules();
        public bool ExistModule(int moduleId);
        public void AddModule(ModuleInfo moduleInfo);
        public void UpdateModule(int moduleId, ModuleInfo moduleInfo);
        public void DeleteModule(int moduleId);
    }
}

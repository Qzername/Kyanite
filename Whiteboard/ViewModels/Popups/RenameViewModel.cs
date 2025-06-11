using ReactiveUI.Fody.Helpers;
using Whiteboard.Models;
using Whiteboard.Services;

namespace Whiteboard.ViewModels.Popups;

public class RenameViewModel : ViewModelBase
{
    ModuleInfo _moduleInfo;

    [Reactive] string newName { get; set; }

    public RenameViewModel(ModuleInfo moduleInfo)
    {
        _moduleInfo = moduleInfo;
    }

    public void Confirm()
    {
        var manager = GetService<IDataManager>();
        _moduleInfo.Name = newName;
        manager.UpdateModule(_moduleInfo.ID!.Value, _moduleInfo);
    }
}

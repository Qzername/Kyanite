using ReactiveUI.Fody.Helpers;
using System;
using Whiteboard.Modules;
using Whiteboard.Modules.Reminder;
using Whiteboard.Modules.Text;
using Whiteboard.Services;
using Whiteboard.Services.Modules;

namespace Whiteboard.ViewModels.Popups;

internal class ModuleCreationViewModel : ViewModelBase
{
    //TODO: this should be automated
    string[] ModuleNamesText => [
        "Text module",
        "Reminder module"
    ];

    [Reactive] int SelectedModuleIndex { get; set; } = 0;
    [Reactive] string ModuleName { get; set; } = string.Empty;

    public void Confirm()
    {
        string selectedType = SelectedModuleIndex switch
        {
            0 => nameof(TextModule),
            1 => nameof(ReminderModule),
            _ => throw new ArgumentOutOfRangeException(nameof(SelectedModuleIndex), "Invalid module index selected.")
        };

        GetService<IModuleDatabase>().AddModule(new Models.ModuleInfo()
        {
            Name = ModuleName,
            Type = selectedType
        });
        GetService<PopupService>().ClosePopup();
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.Modules;
using Kyanite.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace Kyanite.Controls.ModulePicker;

internal partial class ModulePickerViewModel : ViewModelBase
{
    [ObservableProperty] string _moduleName = string.Empty;

    //TODO: make it not hardcoded
    public ObservableCollection<string> ModuleList { get; }
    [ObservableProperty] string _selectedModule = string.Empty;

    public ModulePickerViewModel(ModuleManager moduleManager)
    {
        if (!moduleManager.IsStackPrepared)
            throw new Exception("Module manager is not initialized, even though it is expected to be");

        string[] moduleTypeNames = [.. moduleManager.ModuleTypes.Keys];
        ModuleList = new ObservableCollection<string>(moduleTypeNames);
    }
}

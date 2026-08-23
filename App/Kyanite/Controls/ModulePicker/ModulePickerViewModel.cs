using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.ViewModels;
using System.Collections.ObjectModel;

namespace Kyanite.Controls.ModulePicker;

internal partial class ModulePickerViewModel : ViewModelBase
{
    //TODO: make it not hardcoded
    public ObservableCollection<string> ModuleList { get; } = ["Note"];
    [ObservableProperty] string _selectedModule = string.Empty;
}

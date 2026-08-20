using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.Database;

namespace Kyanite.Modules.Note;

public partial class NoteViewModel(ModuleInformation moduleInformation) : Module(moduleInformation)
{
    [Synchronize]
    [ObservableProperty]
    string _text = string.Empty;
}

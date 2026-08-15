using CommunityToolkit.Mvvm.ComponentModel;

namespace Kyanite.ModuleHandling.Modules.Note;

public partial class NoteViewModel(Guid moduleId) : Module(moduleId)
{
    [Synchronize]
    [ObservableProperty]
    string text = string.Empty;
}

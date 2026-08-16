using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Kyanite.Modules.Note;

public partial class NoteViewModel(Guid moduleId) : Module(moduleId)
{
    [Synchronize]
    [ObservableProperty]
    string text = string.Empty;
}

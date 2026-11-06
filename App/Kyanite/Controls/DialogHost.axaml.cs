using Avalonia.Controls;
using Avalonia.Input;
using Kyanite.ViewModels;

namespace Kyanite.Controls;

public partial class DialogHost : UserControl
{
    public DialogHost()
    {
        InitializeComponent();
    }

    void OnScrimPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is ShellViewModel shellViewModel)
            shellViewModel.CloseDialogCommand.Execute(null);
    }
}

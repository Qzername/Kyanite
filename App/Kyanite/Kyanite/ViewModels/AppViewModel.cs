using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using Kyanite.Views;

namespace Kyanite.ViewModels;

internal partial class AppViewModel : ViewModelBase
{
    [RelayCommand]
    void OpenApplication()
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        
        if (desktop.MainWindow!.IsVisible)
            return;

        desktop.MainWindow = new MainWindow()
        {
            DataContext = new MainViewModel()
        };
        desktop.MainWindow.Show();
    }

    [RelayCommand]
    void CloseApplication()
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        desktop.Shutdown();
    }
}
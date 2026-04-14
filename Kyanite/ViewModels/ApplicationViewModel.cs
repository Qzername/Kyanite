using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Kyanite.Views;

namespace Kyanite.ViewModels;

public class ApplicationViewModel : ViewModelBase
{
    public void OpenApplication()
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

    public void CloseApplication()
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        desktop.Shutdown();
    }
}

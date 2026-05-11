using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Kyanite.DatabaseConnection;
using Kyanite.Views;

namespace Kyanite.ViewModels;

public class ApplicationViewModel : ViewModelBase
{
    readonly ServerHandler _currentServerHandler;

    public ApplicationViewModel()
    {
        _currentServerHandler = GetService<ServerHandler>();

        var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        desktop.MainWindow!.Closed += MainWindow_Closed;
    }

    public void OpenApplication()
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;

        if (desktop.MainWindow!.IsVisible)
            return;

        _currentServerHandler.OnApplicationOpen();

        desktop.MainWindow = new MainWindow()
        {
            DataContext = new MainViewModel()
        };
        desktop.MainWindow.Closed += MainWindow_Closed;
        desktop.MainWindow.Show();
    }

    public void CloseApplication()
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        desktop.Shutdown();
    }

    void MainWindow_Closed(object? sender, System.EventArgs e) => _currentServerHandler.OnApplicationClose();
}

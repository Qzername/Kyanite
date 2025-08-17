using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Whiteboard.Views;

namespace Whiteboard.ViewModels
{
    public class ApplicationViewModel : ViewModelBase
    {
        //handle tray

        public static void OpenApplication()
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

        public static void CloseApplication()
        {
            var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            desktop.Shutdown();
        }
    }
}

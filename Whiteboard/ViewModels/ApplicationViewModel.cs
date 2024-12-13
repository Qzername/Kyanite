using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Diagnostics;

namespace Whiteboard.ViewModels
{
    public class ApplicationViewModel : ViewModelBase
    {
        public static void CloseApplication()
        {
            var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            desktop.Shutdown();
        }
    }
}

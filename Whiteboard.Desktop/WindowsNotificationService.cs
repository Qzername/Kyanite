using Microsoft.Toolkit.Uwp.Notifications;
using Whiteboard.Services;

namespace Whiteboard.Desktop;

internal class WindowsNotificationService : INotificationService
{
    public void ShowNotification(string title, string message)
    {
        new ToastContentBuilder()
               .AddText(title)     
               .AddText(message) 
               .Show();              
    }
}

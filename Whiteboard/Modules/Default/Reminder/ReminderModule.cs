using ReactiveUI.Fody.Helpers;
using Whiteboard.Services;

namespace Whiteboard.Modules.Reminder
{
    public class ReminderModule : Module
    {
        [Reactive] string title { get; set; }
        [Reactive] string description { get; set; }

        public void Send()
        {
            GetService<INotificationService>().ShowNotification(title, description);
        }
    }
}

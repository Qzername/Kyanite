using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Whiteboard.Services;

namespace Whiteboard.Android;

internal class AndroidNotificationService : INotificationService
{
    private const string CHANNEL_ID = "default_channel";
    private readonly Context _context;

    public AndroidNotificationService(Context context)
    {
        _context = context;
        CreateNotificationChannel();
    }

    private void CreateNotificationChannel()
    {
        var channel = new NotificationChannel(CHANNEL_ID, "Default Channel", NotificationImportance.Default)
        {
            Description = "Default desc"
        };

        var notificationManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
        notificationManager.CreateNotificationChannel(channel);
    }

    public void ShowNotification(string title, string message)
    {
        var builder = new NotificationCompat.Builder(_context, CHANNEL_ID)
            .SetSmallIcon(Android.Resource.Drawable.notification_icon_background)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetPriority(NotificationCompat.PriorityHigh)
            .SetDefaults(NotificationCompat.DefaultAll)
            .SetAutoCancel(true);

        var notificationManager = NotificationManagerCompat.From(_context);
        notificationManager.Notify(1001, builder.Build());
    }
}
using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Kyanite.Services;

namespace Kyanite.Android;

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
        var channel = new NotificationChannel(CHANNEL_ID, "kyanite Channel", NotificationImportance.Default)
        {
            Description = "Kyanite - Simple work managment tool"
        };

        var notificationManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
        notificationManager.CreateNotificationChannel(channel);
    }

    public void Show(string title, string message)
    {
        var builder = new NotificationCompat.Builder(_context, CHANNEL_ID)
            .SetSmallIcon(Android.Resource.Drawable.avalonia_anim)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetPriority(NotificationCompat.PriorityHigh)
            .SetDefaults(NotificationCompat.DefaultAll)
            .SetAutoCancel(true);

        var notificationManager = NotificationManagerCompat.From(_context);
        notificationManager.Notify(1001, builder.Build());
    }
}
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Avalonia.Android;

namespace Kyanite.Android;

[Activity(
    Label = "Kyanite",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) // API 33 = Android 13
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) != Permission.Granted)
                ActivityCompat.RequestPermissions(this, [Manifest.Permission.PostNotifications], 0);

        var app = (App)App.Current!;
        app.RegisterNotificationService(new AndroidNotificationService(this));
    }
}

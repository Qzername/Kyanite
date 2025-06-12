using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using And = Android;

namespace Whiteboard.Android;

[Activity(
    Label = "Whiteboard.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) // API 33 = Android 13
            if (ContextCompat.CheckSelfPermission(this, And.Manifest.Permission.PostNotifications) != Permission.Granted)
                ActivityCompat.RequestPermissions(this, [And.Manifest.Permission.PostNotifications], 0);

        var app = (App)App.Current;
        app.RegisterNotificationService(new AndroidNotificationService(this));
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseReactiveUI();
    }
}

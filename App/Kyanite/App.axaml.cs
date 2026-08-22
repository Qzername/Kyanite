using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Kyanite.ViewModels;
using Kyanite.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Kyanite;

public partial class App : Application
{
     public override void Initialize()
     {
          AvaloniaXamlLoader.Load(this);
     }

     public override void OnFrameworkInitializationCompleted()
     {
          var collection = new ServiceCollection();
          collection.AddCommonServices();

          var services = collection.BuildServiceProvider();

          DataContext = services.GetRequiredService<AppViewModel>();
          var mainVm = services.GetRequiredService<MainViewModel>();

          if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
          {
               desktop.MainWindow = new MainWindow
               {
                    DataContext = mainVm
               };

               desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
          }
          else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
          {
               singleViewFactoryApplicationLifetime.MainViewFactory = () => new MainView { DataContext = mainVm };
          }
          else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
          {
               singleViewPlatform.MainView = new MainView
               {
                    DataContext = mainVm
               };
          }

          base.OnFrameworkInitializationCompleted();
     }
}
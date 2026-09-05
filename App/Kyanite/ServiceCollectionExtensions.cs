using Kyanite.Database;
using Kyanite.Core.Dialogs;
using Kyanite.Modules;
using Kyanite.Services;
using Kyanite.ViewModels;
using Kyanite.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;

namespace Kyanite;

internal static class ServiceCollectionExtensions
{
    internal static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<AppSettingsService>();

        // --- database ---

        collection.AddSingleton<DatabaseStackLoader>();
        collection.AddSingleton<DatabaseStackProvider>();

        // --- modules --- 

        collection.AddSingleton<ModuleManager>();
        collection.AddSingleton<IDialogService, DialogService>();
        collection.AddSingleton<NotificationServiceProvider>();
   
        // --- app ---

        collection.AddSingleton<AppViewModel>();
        collection.AddSingleton<ShellViewModelProvider>();

        collection.AddTransient<MainViewModel>();
    }
}

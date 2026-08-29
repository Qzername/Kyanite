using Kyanite.Database;
using Kyanite.Database.Default.GoogleDrive;
using Kyanite.Dialogs;
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
        collection.AddSingleton<DatabaseStackLoader>();
        collection.AddSingleton<DatabaseStackProvider>();
        collection.AddSingleton<ModuleManager>();
        collection.AddSingleton<DialogService>();

        collection.AddSingleton<AppViewModel>();
        collection.AddSingleton<ShellViewModel>();

        collection.AddTransient<MainViewModel>();
    }
}

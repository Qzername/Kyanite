using Kyanite.Database;
using Kyanite.Database.Default.GoogleDrive;
using Kyanite.Dialogs;
using Kyanite.Modules;
using Kyanite.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Kyanite;

internal static class ServiceCollectionExtensions
{
    internal static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<DatabaseStack, GoogleDriveDatabaseStack>();
        collection.AddSingleton<ModuleManager>();
        collection.AddSingleton<DialogService>();

        collection.AddSingleton<AppViewModel>();

        collection.AddTransient<MainViewModel>();
    }
}

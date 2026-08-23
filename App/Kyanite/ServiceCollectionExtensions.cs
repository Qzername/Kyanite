using Kyanite.Database;
using Kyanite.Database.Default.Local;
using Kyanite.Dialogs;
using Kyanite.Modules;
using Kyanite.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Kyanite;

internal static class ServiceCollectionExtensions
{
    internal static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<DatabaseStack, LocalDatabaseStack>();
        collection.AddSingleton<ModuleManager>();
        collection.AddSingleton<DialogService>();

        collection.AddSingleton<AppViewModel>();

        collection.AddTransient<MainViewModel>();
    }
}

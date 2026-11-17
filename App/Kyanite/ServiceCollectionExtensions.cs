using Kyanite.Core.Dialogs;
using Kyanite.Database;
using Kyanite.Modules;
using Kyanite.Services;
using Kyanite.ViewModels;
using Kyanite.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;

namespace Kyanite;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<AppSettingsService>();

        // --- database ---

        collection.AddSingleton<DatabaseStackLoader>();
        collection.AddSingleton<DatabaseStackProvider>();

        // --- modules --- 

        collection.AddSingleton<ModuleManager>();
        collection.AddSingleton<IDialogService, DialogService>();

        // --- app ---

        collection.AddSingleton<AppViewModel>();
        collection.AddSingleton<ShellViewModelProvider>();

        collection.AddTransient<MainViewModel>();

        return collection;
    }
}

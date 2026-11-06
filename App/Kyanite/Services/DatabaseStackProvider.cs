using Kyanite.Database;
using Kyanite.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kyanite.Services;

internal class DatabaseStackProvider
{
    public DatabaseStack? ActiveStack { get; private set; }

    public DatabaseStackProvider(IServiceProvider serviceProvider, AppSettingsService appSettingsService, DatabaseStackLoader databaseStackLoader)
    {
        if (appSettingsService.CurrentAppSettings is null)
            throw new AppSettingsNotInitializedException();

        if (!appSettingsService.CurrentAppSettings.DatabaseinformationInitialized)
            return;

        if (string.IsNullOrEmpty(appSettingsService.CurrentAppSettings.DatabaseStackType))
            throw new InvalidOperationException("DatabaseStackType is expected to be set in app settings.");

        if (!databaseStackLoader.DatabaseStackTypes.TryGetValue(
                appSettingsService.CurrentAppSettings.DatabaseStackType,
                out var stackType))
        {
            throw new InvalidOperationException(
                $"Database stack '{appSettingsService.CurrentAppSettings.DatabaseStackType}' is not available.");
        }

        var instance = ActivatorUtilities.CreateInstance(serviceProvider, stackType);
        SetStack((DatabaseStack)instance);
    }

    public void SetStack(DatabaseStack databaseStack)
    {
        ActiveStack = databaseStack ?? throw new ArgumentNullException(nameof(databaseStack));
    }

    public void ClearStack() => ActiveStack = null;
}

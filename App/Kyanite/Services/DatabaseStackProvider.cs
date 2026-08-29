using Kyanite.Database;
using Kyanite.Exceptions;
using System;

namespace Kyanite.Services;

internal class DatabaseStackProvider
{
    public DatabaseStack? ActiveStack { get; private set; }

    public DatabaseStackProvider(AppSettingsService appSettingsService, DatabaseStackLoader databaseStackLoader)
    {
        if (appSettingsService.CurrentAppSettings is null)
            throw new AppSettingsNotInitializedException();

        if (!appSettingsService.CurrentAppSettings.DatabaseinformationInitialized)
            return;

        if (string.IsNullOrEmpty(appSettingsService.CurrentAppSettings.DatabaseStackType))
            throw new Exception("DatabaseStackType expected to be set in appSettings");

        var stackType = databaseStackLoader.DatabaseStackTypes[appSettingsService.CurrentAppSettings.DatabaseStackType];
        var instance = Activator.CreateInstance(stackType);
        SetStack((DatabaseStack)instance);
    }

    public void SetStack(DatabaseStack databaseStack)
    {
        ActiveStack = databaseStack ?? throw new ArgumentNullException(nameof(databaseStack));
    }
}

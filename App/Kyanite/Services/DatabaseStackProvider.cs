using Kyanite.Core.Services;
using Kyanite.Database;
using Kyanite.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kyanite.Services;

internal class DatabaseStackProvider
{
    public DatabaseStack? ActiveStack { get; private set; }

    public DatabaseStackProvider(IServiceProvider serviceProvider, IAppSettingsService appSettingsService, DatabaseStackLoader databaseStackLoader)
    {
        if (!appSettingsService.CurrentAppSettings.DatabaseInformationInitialized)
            return;

        if (string.IsNullOrEmpty(appSettingsService.CurrentAppSettings.DatabaseStackType))
            throw new Exception("DatabaseStackType expected to be set in appSettings");

        var stackType = databaseStackLoader.DatabaseStackTypes[appSettingsService.CurrentAppSettings.DatabaseStackType];
        var instance = ActivatorUtilities.CreateInstance(serviceProvider, stackType);
        SetStack((DatabaseStack)instance);
    }

    public void SetStack(DatabaseStack databaseStack)
    {
        ActiveStack = databaseStack ?? throw new ArgumentNullException(nameof(databaseStack));
    }
}

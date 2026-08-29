using Kyanite.Database;
using System;

namespace Kyanite.Services;

internal class DatabaseStackProvider
{
    public DatabaseStack? ActiveStack { get; private set; }
    public void SetStack(DatabaseStack databaseStack)
    {
        ActiveStack = databaseStack ?? throw new ArgumentNullException(nameof(databaseStack));
    }
}

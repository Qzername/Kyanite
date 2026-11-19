using System.Reflection;

namespace Kyanite.Database;

/// <summary>
/// reads and loads available databasestack implementation
/// </summary>
public class DatabaseStackLoader
{
    const string DefaultDatabaseStackNamespace = "Kyanite.Database.Default";

    public Dictionary<string, Type> DatabaseStackTypes { get; private set; } = [];

    public DatabaseStackLoader()
    {
        LoadDatabaseStacksFromAssembly();
    }

    void LoadDatabaseStacksFromAssembly()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var foundStacks = from t in Assembly.GetExecutingAssembly().GetTypes()
                          where typeof(DatabaseStack).IsAssignableFrom(t) &&
                                t != typeof(DatabaseStack) &&
                                t.Namespace is not null &&
                                t.Namespace.StartsWith(DefaultDatabaseStackNamespace)
                          select t;

        foreach (var databaseStackType in foundStacks)
            DatabaseStackTypes[databaseStackType.Name.Replace("ViewModel", string.Empty)] = databaseStackType;
    }
}

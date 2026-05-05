using LiteDB;

namespace KyaniteAPI.Contexts;

public class DatabaseContext
{
    public ILiteDatabase Database { get; }

    public DatabaseContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("LiteDbOptions:DatabasePath").Value;

        string currentDirectory = Directory.GetCurrentDirectory();
        currentDirectory += Path.DirectorySeparatorChar + Path.GetDirectoryName(connectionString);

        if (!Directory.Exists(currentDirectory))
            Directory.CreateDirectory(currentDirectory);

        Database = new LiteDatabase(connectionString);
    }
}

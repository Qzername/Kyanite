using Kyanite.DatabaseConnection;
using Kyanite.DatabaseConnection.GoogleDrive;
using Kyanite.Modules;
using Splat;

namespace Kyanite;

internal static class Bootstrapper
{
    public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton<ServerHandler>(() => new GoogleDriveServerHandler());
        services.RegisterLazySingleton(() => new ModuleManager(GetService<ServerHandler>(resolver)));
    }

    static T GetService<T>(IReadonlyDependencyResolver resolver) => resolver.GetService<T>()!;
}

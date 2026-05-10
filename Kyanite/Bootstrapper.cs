using Kyanite.DatabaseConnection.GoogleDrive;
using Kyanite.DatabaseConnection.Local;
using Kyanite.Modules;
using Splat;

namespace Kyanite;

internal static class Bootstrapper
{
    public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton(() => new GoogleDriveServerHandler());
        services.RegisterLazySingleton(() => new ModuleManager(GetService<GoogleDriveServerHandler>(resolver)));
    }

    static T GetService<T>(IReadonlyDependencyResolver resolver) => resolver.GetService<T>()!;
}

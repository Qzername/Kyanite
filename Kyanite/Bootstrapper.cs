using Kyanite.DatabaseConnection.StandardAPI;
using Kyanite.Modules;
using Splat;

namespace Kyanite;

internal static class Bootstrapper
{
    public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton(() => new APIServerHandler());
        services.RegisterLazySingleton(() => new ModuleManager(GetService<APIServerHandler>(resolver)));
    }

    static T GetService<T>(IReadonlyDependencyResolver resolver) => resolver.GetService<T>()!;
}

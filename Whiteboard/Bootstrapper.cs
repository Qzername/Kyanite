using Splat;
using Whiteboard.Services;

namespace Whiteboard;

internal static class Bootstrapper
{
    public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        // --- DATA MANAGING ---
        services.RegisterLazySingleton(() => new LiteDbConnection());
        services.RegisterLazySingleton(() => new LocalDataItemDatabase(GetService<LiteDbConnection>(resolver)), typeof(IDataItemDatabase));
        services.RegisterLazySingleton(() => new LocalModuleDatabase(GetService<LiteDbConnection>(resolver)), typeof(IModuleDatabase));
    }

    /*
     * - This method is for later use in Register method
     * - Shortens the code for getting a service from the resolver
     */
    static T GetService<T>(IReadonlyDependencyResolver resolver) => resolver.GetService<T>()!;
}
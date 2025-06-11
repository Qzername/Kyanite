using Splat;
using Whiteboard.Services;

namespace Whiteboard;

internal static class Bootstrapper
{
    public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton(() => new LocalDataManager(), typeof(IDataManager));
    }

    /*
     * - This method is for later use in Register method
     * - Shortens the code for getting a service from the resolver
     */
    static T GetService<T>(IReadonlyDependencyResolver resolver) => resolver.GetService<T>()!;
}
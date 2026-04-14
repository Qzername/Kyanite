using Splat;

namespace Kyanite;

internal static class Bootstrapper
{
    public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
    }

    static T GetService<T>(IReadonlyDependencyResolver resolver) => resolver.GetService<T>()!;
}

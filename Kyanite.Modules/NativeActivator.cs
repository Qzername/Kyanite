namespace Kyanite.Modules;

internal static class NativeActivator
{
    public static object CreateInstance(Type type, IServiceProvider provider, params object[] customArgs)
    {
        var constructor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();

        var parameters = constructor.GetParameters();
        var resolvedArgs = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var paramType = parameters[i].ParameterType;

            var customArg = customArgs.FirstOrDefault(arg => arg != null && paramType.IsAssignableFrom(arg.GetType()));

            if (customArg != null)
                resolvedArgs[i] = customArg;
            else
                resolvedArgs[i] = provider.GetService(paramType) ?? throw new InvalidOperationException($"Unable to resolve service for type {paramType}");
        }

        return constructor.Invoke(resolvedArgs);
    }
}
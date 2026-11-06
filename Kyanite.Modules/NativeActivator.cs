namespace Kyanite.Modules;

internal static class NativeActivator
{
    public static object CreateInstance(Type type, IServiceProvider provider, params object[] customArgs)
    {
        var constructor = type.GetConstructors()
            .OrderByDescending(candidate => candidate.GetParameters().Length)
            .FirstOrDefault()
            ?? throw new InvalidOperationException($"Type '{type}' does not have a public constructor.");

        var parameters = constructor.GetParameters();
        var resolvedArgs = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var parameterType = parameters[i].ParameterType;

            var customArg = customArgs.FirstOrDefault(
                argument => argument is not null && parameterType.IsInstanceOfType(argument));

            if (customArg is not null)
            {
                resolvedArgs[i] = customArg;
                continue;
            }

            resolvedArgs[i] = provider.GetService(parameterType)
                ?? throw new InvalidOperationException($"Unable to resolve service for type {parameterType}.");
        }

        return constructor.Invoke(resolvedArgs);
    }
}

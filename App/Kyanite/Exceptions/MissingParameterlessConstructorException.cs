using System;

namespace Kyanite.Exceptions;

internal class MissingParameterlessConstructorException : Exception
{
    public MissingParameterlessConstructorException()
        : base("The specified DatabaseStack does not contain a parameterless constructor") { }
}

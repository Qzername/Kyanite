using System;

namespace Whiteboard.Modules
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class SynchronizeAttribute : Attribute
    {
        string _variableName;
        string _defaultValue;

        public SynchronizeAttribute(string variableName, string defaultValue)
        {
            _variableName = variableName;
            _defaultValue = defaultValue;
        }

        public string VariableName =>  _variableName;
        public string DefaultValue => _defaultValue;
    }
}

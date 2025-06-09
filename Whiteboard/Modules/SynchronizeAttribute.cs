using System;

namespace Whiteboard.ModuleManagement
{
    [AttributeUsage(AttributeTargets.Property)]
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

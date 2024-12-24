using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard.Modules
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class SynchronizeAttribute : Attribute
    {
        string _variableName;

        public SynchronizeAttribute(string variableName)
        {
            _variableName = variableName;
        }

        public string VariableName =>  _variableName;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISonesGQLFunction.Structure
{
    /// <summary>
    /// A Function parameter containing the parameter name and the type.
    /// This is used by the parameter definition in the function implementation.
    /// </summary>
    public struct ParameterValue
    {

        public readonly String Name;
        public readonly Object Value;
        public readonly Boolean VariableNumOfParams;

        /// <summary>
        /// A single parameter
        /// </summary>
        public ParameterValue(String myParameterName, Object myParameterValue)
        {
            Name = myParameterName;
            Value = myParameterValue;
            VariableNumOfParams = false;
        }

        /// <summary>
        /// A single parameter
        /// </summary>
        /// <param name="myVariableNumOfParams">True if this parameter occurs 1 or more time. This have to be the last parameter!</param>
        public ParameterValue(String myParameterName, Object myParameterValue, Boolean myVariableNumOfParams)
        {
            Name = myParameterName;
            Value = myParameterValue;
            VariableNumOfParams = myVariableNumOfParams;
        }

    }
}

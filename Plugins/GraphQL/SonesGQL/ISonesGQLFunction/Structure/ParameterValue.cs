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
        public readonly Type Type;
        public readonly Boolean VariableNumOfParams;

        /// <summary>
        /// A single parameter
        /// </summary>
        /// <param name="myParameterName">The name of the parameter. Currently this is not used.</param>
        /// <param name="myParameterDBType">The Type of the parameter</param>
        public ParameterValue(String myParameterName, Type myParameterType)
            : this()
        {
            Name = myParameterName;
            Type = myParameterType;
            VariableNumOfParams = false;
        }

        /// <summary>
        /// A single parameter
        /// </summary>
        /// <param name="myParameterName">The name of the parameter. Currently this is not used.</param>
        /// <param name="myParameterDBType">The Type of the parameter</param>
        /// <param name="myVariableNumOfParams">True if this parameter occurs 1 or more time. This have to be the last parameter!</param>
        public ParameterValue(String myParameterName, Type myParameterType, Boolean myVariableNumOfParams)
            : this()
        {
            Name = myParameterName;
            Type = myParameterType;
            VariableNumOfParams = myVariableNumOfParams;
        }

    }
}

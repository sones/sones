
#region Usings

using System;
using sones.GraphDB.TypeManagement.BasicTypes;

#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// A Function parameter containing the parameter name and the type.
    /// This is used by the parameter definition in the function implementation.
    /// </summary>
    public struct ParameterValue
    {

        public String        Name                { get; private set; }
        public ADBBaseObject DBType              { get; private set; }
        public Boolean       VariableNumOfParams { get; private set; }

        /// <summary>
        /// A single parameter
        /// </summary>
        /// <param name="myParameterName">The name of the parameter. Currently this is not used.</param>
        /// <param name="myParameterDBType">The DBType of the parameter</param>
        public ParameterValue(String myParameterName, ADBBaseObject myParameterDBType)
            : this()
        {
            Name                = myParameterName;
            DBType              = myParameterDBType;
            VariableNumOfParams = false;
        }

        /// <summary>
        /// A single parameter
        /// </summary>
        /// <param name="myParameterName">The name of the parameter. Currently this is not used.</param>
        /// <param name="myParameterDBType">The DBType of the parameter</param>
        /// <param name="myVariableNumOfParams">True if this parameter occurs 1 or more time. This have to be the last parameter!</param>
        public ParameterValue(String myParameterName, ADBBaseObject myParameterDBType, Boolean myVariableNumOfParams)
            : this()
        {
            Name                = myParameterName;
            DBType              = myParameterDBType;
            VariableNumOfParams = myVariableNumOfParams;
        }

    }

}
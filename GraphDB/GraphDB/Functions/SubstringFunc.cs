
#region Usings

using System;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// This function represents the well known substring function with a start position and a length
    /// </summary>
    public class SubstringFunc : ABaseFunction
    {

        public override string FunctionName
        {
            get { return "SUBSTRING"; }
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "Retrieves a substring from the attribute value. The substring starts at a specified character position and has a specified length.";
        }

        #endregion

        public SubstringFunc()
        {
            Parameters.Add(new ParameterValue("StartPosition", new DBInt32()));
            Parameters.Add(new ParameterValue("Length", new DBInt32()));
        }

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                if (workingBase is ADBBaseObject)
                {
                    return true;
                }
                else if ((workingBase is DBTypeAttribute) && (workingBase as DBTypeAttribute).GetValue().GetDBType(typeManager).IsUserDefined)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            if (CallingObject is ADBBaseObject)
            {
                var substring = (CallingObject as ADBBaseObject).Value.ToString().Substring((myParams[0].Value as DBInt32).GetValue(), (myParams[1].Value as DBInt32).GetValue());
                return new Exceptional<FuncParameter>(new FuncParameter(new DBString(substring)));
            }
            else
            {
                return new Exceptional<FuncParameter>(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
        }

    }

}

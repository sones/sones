
#region Usings

using System;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;


#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// This function represents the well known ToUpper function.
    /// </summary>
    public class ToUpperFunc : ABaseFunction
    {

        public override string FunctionName
        {
            get { return "TOUPPER"; }
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "Returns a copy of this attribute value converted to uppercase.";
        }

        #endregion


        public ToUpperFunc()
        {
        }

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                if (workingBase is DBTypeAttribute)
                {
                    if ((workingBase as DBTypeAttribute).GetValue().GetDBType(typeManager).IsUserDefined)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }

            throw new NotImplementedException();
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            if (CallingObject is ADBBaseObject)
                return new Exceptional<FuncParameter>(new FuncParameter(new DBString((CallingObject as ADBBaseObject).Value.ToString().ToUpper())));
            else
                return new Exceptional<FuncParameter>(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

    }

}

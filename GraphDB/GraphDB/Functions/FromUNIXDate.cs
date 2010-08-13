
#region Usings

using System;
using sones.GraphDB.Errors;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// Convert from unix datime format to DBDateTime format.
    /// </summary>
    public class FromUNIXDate : ABaseFunction
    {

        #region constructors

        public FromUNIXDate()
        { }

        #endregion

        #region ABaseFunction

        public override string FunctionName
        {
            get { return "FromUNIXDate"; }
        }

        public override string GetDescribeOutput()
        {
            return "Convert from unix datime format to DBDateTime format.";
        }

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
        {
            if (workingBase is DBInt64)
            {
                return true;
            }
            else if ((workingBase is DBTypeAttribute) && (workingBase as DBTypeAttribute).GetValue().GetDBType(typeManager).UUID == DBInt64.UUID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            Exceptional<FuncParameter> result = new Exceptional<FuncParameter>();

            if (CallingObject != null)
            {
                if (CallingObject is DBInt64)
                {
                    result.Value = new FuncParameter(new DBDateTime(((Int64)((DBInt64)CallingObject).Value).FromUnixTimeStamp()));
                }
                else
                {
                    result = new Exceptional<FuncParameter>(new Error_FunctionParameterTypeMismatch(typeof(DBInt64), CallingObject.GetType()));
                }
            }
            else 
                return new Exceptional<FuncParameter>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

            return result;
        }

        #endregion
    
    }

}

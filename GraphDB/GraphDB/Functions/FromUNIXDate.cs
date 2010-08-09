#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.Functions
{
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

        public override bool ValidateWorkingBase(TypeAttribute workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                if (workingBase.GetDBType(typeManager).IsUserDefined)
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

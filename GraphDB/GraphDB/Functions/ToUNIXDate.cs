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
using sones.Lib.DataStructures.Timestamp;

#endregion

namespace sones.GraphDB.Functions
{
    public class ToUNIXDate : ABaseFunction
    {

        #region constructors 

        public ToUNIXDate()
        { }

        #endregion

        #region ABaseFunction

        public override string FunctionName
        {
            get { return "ToUNIXDate"; }
        }

        public override string GetDescribeOutput()
        {
            return "Convert the datetime value to the unix datime format.";
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
                if (CallingObject is DBUInt64)
                {
                    try
                    {
                        var dtValue = new DateTime(System.Convert.ToInt64(((DBUInt64)CallingObject).Value));
                        result.Value = new FuncParameter(new DBInt64(dtValue.ToUnixTimeStamp()));
                    }
                    catch (Exception e)
                    {
                        return new Exceptional<FuncParameter>(new Error_UnknownDBError(e.Message));
                    }

                    return result;
                }
                else if (CallingObject is DBDateTime)
                {
                    result.Value = new FuncParameter(new DBInt64(((DateTime)((DBDateTime)CallingObject).Value).ToUnixTimeStamp()));
                }
                else
                {
                    return new Exceptional<FuncParameter>(new Error_FunctionParameterTypeMismatch(typeof(DBDateTime), CallingObject.GetType()));
                }
            }
            else
                return new Exceptional<FuncParameter>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

            return result;
        }

        #endregion
    }
}

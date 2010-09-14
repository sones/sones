#region Usings

using System;
using System.Linq;
using System.Text;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// This function inserts one or more strings at the given position.
    /// </summary>
    public class InsertFunc : ABaseFunction
    {

        public override string FunctionName
        {
            get { return "INSERT"; }
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "This function inserts one or more strings at the given position.";
        }

        #endregion

        public InsertFunc()
        {
            Parameters.Add(new ParameterValue("Position", new DBInt32()));
            Parameters.Add(new ParameterValue("StringPart", new DBString(), true));
        }

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                if ((workingBase is DBTypeAttribute) && (workingBase as DBTypeAttribute).GetValue().GetDBType(typeManager).IsUserDefined)
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
            var result = new Exceptional<FuncParameter>();

            if (!(CallingObject is ADBBaseObject))
            {
                return new Exceptional<FuncParameter>(new Errors.Error_InvalidFunctionParameter("CallingObject", "ADBBaseObject", CallingObject.GetType()));
            }

            var pos = (myParams[0].Value as DBInt32).GetValue();

            StringBuilder resString = new StringBuilder((CallingObject as ADBBaseObject).Value.ToString().Substring(0, pos));
            foreach (FuncParameter fp in myParams.Skip(1))
            {
                resString.Append((fp.Value as DBString).GetValue());
            }
            resString.Append((CallingObject as ADBBaseObject).Value.ToString().Substring(pos));

            result = new Exceptional<FuncParameter>(new FuncParameter(new DBString(resString.ToString())));

            return result;
        }

    }

}

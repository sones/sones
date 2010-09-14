
#region Usings

using System;
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
    /// This will concatenate some strings. This function can be used as type independent to concatenate
    /// string values or as type dependent to concatenate an attribute output with other strings.
    /// </summary>
    public class ConcatFunc : ABaseFunction
    {

        public override string FunctionName
        {
            get { return "CONCAT"; }
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "This will concatenate some strings. This function can be used as type independent to concatenate string values or as type dependent to concatenate an attribute output with other strings.";
        }
        
        #endregion

        public ConcatFunc()
        {
            Parameters.Add(new ParameterValue("StringPart", new DBString(), true));
        }

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
        {
            if (workingBase is DBString || ((workingBase is DBTypeAttribute) && (workingBase as DBTypeAttribute).GetValue().GetDBType(typeManager).UUID == DBString.UUID))
            {
                return true; // valid for string
            }
            else if (workingBase == null)
            {
                return true; // valid without a workingBase
            }
            else
            {
                return false;
            }
        }

        public override IObject GetReturnType(IObject myWorkingBase, DBTypeManager myTypeManager)
        {
            return new DBString();
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            var result = new Exceptional<FuncParameter>();

            StringBuilder resString = new StringBuilder();

            if (CallingObject != null)
            {
                if (CallingObject is DBString)
                {
                    resString.Append((CallingObject as DBString).GetValue());
                }
            }

            foreach (FuncParameter fp in myParams)
            {
                resString.Append(fp.Value);
            }


            result = new Exceptional<FuncParameter>(new FuncParameter(new DBString(resString.ToString())));

            return result;
        }

    }

}

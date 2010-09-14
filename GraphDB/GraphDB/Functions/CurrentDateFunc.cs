
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
    /// Returns the current date and time.
    /// </summary>
    public class CurrentDateFunc : ABaseFunction
    {

        public override string FunctionName
        {
            get { return "CURRENTDATE"; }
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "Returns the current date and time.";
        }

        #endregion

        public CurrentDateFunc()
        {
           
        }

        public override IObject GetReturnType(IObject myWorkingBase, DBTypeManager myTypeManager)
        {
            return new DBDateTime();
        }

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            return new Exceptional<FuncParameter>(new FuncParameter(new DBDateTime(DateTime.Now)));
        }

    }

}

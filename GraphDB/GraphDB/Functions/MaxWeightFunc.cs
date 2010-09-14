
#region Usings

using System;
using sones.GraphDB.Errors;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// This function is valid for weighted edges and will return the maximum weight.
    /// </summary>
    public class MaxWeightFunc : ABaseFunction
    {

        public MaxWeightFunc()
        {
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "This function is valid for weighted edges and will return the maximum weight.";
        }

        #endregion

        public override string FunctionName
        {
            get { return "MAXWEIGHT"; }
        }

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
        {
            if ((workingBase is DBTypeAttribute) && (workingBase as DBTypeAttribute).GetValue().EdgeType is EdgeTypeWeighted)
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
            var result = new Exceptional<FuncParameter>();

            if (!(CallingObject is EdgeTypeWeighted))
            {
                return result.PushIErrorT(new Error_FunctionParameterTypeMismatch(typeof(EdgeTypeWeighted), CallingObject.GetType()));
            }

            result.Value = new FuncParameter(((EdgeTypeWeighted)CallingObject).GetMaxWeight());

            return result;
        }

    }

}

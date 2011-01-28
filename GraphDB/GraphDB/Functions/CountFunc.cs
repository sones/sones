
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// This will count the elements of an edge and return them as UInt64 value.
    /// </summary>
    public class CountFunc : ABaseFunction
    {

        public override string FunctionName
        {
            get { return "COUNT"; }
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "This will count the elements of an edge and return them as UInt64 value.";
        }

        #endregion

        public CountFunc()
        {
        }

        public override TypeManagement.IObject GetReturnType(TypeManagement.IObject myWorkingBase, TypeManagement.DBTypeManager myTypeManager)
        {
            return new DBUInt64();
        }

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                if ((workingBase is DBTypeAttribute) && (workingBase as DBTypeAttribute).GetValue().EdgeType is IEdgeType)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            var pResult = new Exceptional<FuncParameter>();

            if (CallingObject is IListOrSetEdgeType)
            {
                pResult.Value = new FuncParameter(new DBUInt64(((IListOrSetEdgeType)CallingObject).Count()));
            }
            else if (CallingObject is ASingleReferenceEdgeType)
            {
                pResult.Value = new FuncParameter(new DBUInt64(1));
            }
            else if (CallingObject is IEnumerable<ObjectUUID>)
            {
                pResult.Value = new FuncParameter(new DBUInt64((CallingObject as IEnumerable<ObjectUUID>).LongCount()));
            }
            else
            {
                return pResult.PushIErrorT(new Error_FunctionParameterTypeMismatch(typeof(IEdgeType), CallingObject.GetType()));
            }

            return pResult;
        }

    }

}

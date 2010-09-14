
#region Usings

using System;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// Will return the top elements of an edge.
    /// </summary>
    public class TopFunc : ABaseFunction
    {

        public override string FunctionName
        {
            get { return "TOP"; }
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "Will return the top elements of an edge.";
        }

        #endregion


        public TopFunc()
        {
            Parameters.Add(new ParameterValue("NumOfEntries", new DBUInt64()));
        }

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
        {

            if (((workingBase is DBTypeAttribute) && (workingBase as DBTypeAttribute).GetValue().EdgeType is IListOrSetEdgeType)
                || workingBase is IListOrSetEdgeType)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public override IObject GetReturnType(IObject workingBase, DBTypeManager typeManager)
        {
            return workingBase;
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {

            var pResult = new Exceptional<FuncParameter>();

            ulong numOfEntries = (myParams[0].Value as DBUInt64).GetValue();

            var retVal = (IEdgeType)(CallingObject as IListOrSetEdgeType).GetTopAsEdge(numOfEntries);
            pResult.Value = new FuncParameter(retVal, CallingAttribute);

            return pResult;

        }

    }

}

/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/



#region
using System;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.ErrorHandling;

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

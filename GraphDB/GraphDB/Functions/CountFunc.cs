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
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
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
                return pResult.PushT(new Error_FunctionParameterTypeMismatch(typeof(IEdgeType), CallingObject.GetType()));
            }

            return pResult;
        }

    }

}

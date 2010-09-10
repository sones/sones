/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/


#region Usings

using System;
using sones.GraphDB.Errors;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDBInterface.TypeManagement;
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
                return result.PushT(new Error_FunctionParameterTypeMismatch(typeof(EdgeTypeWeighted), CallingObject.GetType()));
            }

            result.Value = new FuncParameter(((EdgeTypeWeighted)CallingObject).GetMaxWeight());

            return result;
        }

    }

}

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.ObjectManagement;

using sones.GraphDB.Errors;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.Session;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions
{
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

        public override bool ValidateWorkingBase(TypeAttribute workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                if (workingBase.EdgeType is AListEdgeType)
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

            if (CallingObject is AListEdgeType)
            {
                pResult.Value = new FuncParameter(new DBUInt64(((AListEdgeType)CallingObject).Count()));
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
                return pResult.PushT(new Error_FunctionParameterTypeMismatch(typeof(AListEdgeType), CallingObject.GetType()));
            }

            return pResult;
        }

    }
}

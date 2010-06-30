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

        public override bool ValidateWorkingBase(TypeAttribute workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null && workingBase.EdgeType is AListEdgeType)
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
            var pResult = new Exceptional<FuncParameter>();

            ulong numOfEntries = (myParams[0].Value as DBUInt64).GetValue();

            if (CallingObject is EdgeTypeWeightedList)
            {
                pResult.Value = new FuncParameter(((EdgeTypeWeightedList)CallingObject).GetTopAsWeightedSet(numOfEntries), CallingAttribute);
            }
            else if (CallingObject is EdgeTypeSetOfReferences)
            {
                var retVal = new EdgeTypeSetOfReferences((CallingObject as EdgeTypeSetOfReferences).GetTop(numOfEntries) as IEnumerable<ObjectUUID>);
                pResult.Value = new FuncParameter(retVal, CallingAttribute);
            }
            else
            {
                return pResult.PushT(new Error_FunctionParameterTypeMismatch(typeof(ASetReferenceEdgeType), CallingObject.GetType()));
            }

            return pResult;
        }

    }
}

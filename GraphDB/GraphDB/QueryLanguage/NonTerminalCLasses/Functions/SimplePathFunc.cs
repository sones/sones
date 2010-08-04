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


using sones.Lib.ErrorHandling;

using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Session;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.Session;
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions
{
    public class SimplePathFunc : ABaseFunction
    {
        public override string FunctionName
        {
            get { return "SIMPLEPATH"; } // The name of the function which is the same you use in the select
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "Just a dummy simple path function. It does nothing.";
        }

        #endregion

        public SimplePathFunc()
        {
            Parameters.Add(new ParameterValue("TargetDBO", new DBEdge())); // a list of target DB objects
            Parameters.Add(new ParameterValue("MaxDepth", new DBInt64())); // a int64 parameter for the max depth
        }

        public override bool ValidateWorkingBase(TypeAttribute workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                if (workingBase.GetDBType(typeManager).IsUserDefined)
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

            // The edge we starting of (Friends)
            var typeAttribute = CallingAttribute;

            // Get the source Objects from the CallingObject, if this is not a reference this function is used in a wrong context and return a error
            IEnumerable<ObjectUUID> sourceDBOs = null;
            if (CallingObject is IReferenceEdge)
            {
                sourceDBOs = (CallingObject as ASetReferenceEdgeType).GetAllReferenceIDs();
            }
            else
            {
                throw new GraphDBException(new Error_InvalidEdgeType(CallingAttribute.EdgeType.GetType(), typeof(ASetReferenceEdgeType), typeof(ASingleReferenceEdgeType)));
            }

            // The destination DBObjects which are passed with the first parameter
            var destDBOs = (myParams[0].Value as DBEdge).GetDBObjects();
            if (destDBOs == null)
            {
                throw new GraphDBException(new Error_InvalidEdgeType(CallingAttribute.EdgeType.GetType(), typeof(ASetReferenceEdgeType), typeof(ASingleReferenceEdgeType)));
            }

            // The depth which is passed as the second parameter
            Int64 mayDepth = (myParams[1].Value as DBInt64).GetValue();

            // Call your own implementation of a path function and return the result, in this case it is a list of DB object (uuids)
            var resultOfPathFunction = new List<ObjectUUID>();
            
            pResult.Value = new FuncParameter(new EdgeTypeSetOfReferences(resultOfPathFunction, CallingAttribute.DBTypeUUID));

            return pResult;
        }

    }
}

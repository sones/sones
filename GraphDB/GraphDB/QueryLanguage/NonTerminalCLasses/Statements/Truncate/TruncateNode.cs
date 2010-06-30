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


#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Managers;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Drop
{
    public class TruncateNode : AStatement
    {
        #region Data

        private ObjectManipulationManager _ObjectManipulationManager;
        private String _TypeName = ""; //the name of the type that should be dropped

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "Truncate"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            #region get Name

            _TypeName = parseNode.ChildNodes[1].Token.ValueString;

            #endregion

            GraphDBType pandoraType = typeManager.GetTypeByName(_TypeName);
            if (pandoraType == null)
            {
                throw new GraphDBException(new Error_TypeDoesNotExist(_TypeName));
            }

            if (typeManager.GetAllSubtypes(pandoraType, false).Count > 0)
            {
                throw new GraphDBException(new Error_TruncateNotAllowedOnInheritedType(_TypeName));
            }

            _ObjectManipulationManager = new ObjectManipulationManager(dbContext.SessionSettings, null, dbContext, this);

        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession, DBContext dbContext)
        {

            using (var transaction = graphDBSession.BeginTransaction())
            {

                var dbInnerContext = transaction.GetDBContext();
                GraphDBType graphDBType = dbContext.DBTypeManager.GetTypeByName(_TypeName);

                if (graphDBType == null)
                {
                    var aError = new Error_TypeDoesNotExist(_TypeName);

                    return new QueryResult(aError);
                }

                #region Remove

                #region remove dbobjects

                var listOfAffectedDBObjects = dbContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(graphDBType), dbInnerContext).ToList();

                foreach (var aDBO in listOfAffectedDBObjects)
                {
                    if (!aDBO.Success)
                    {
                        return new QueryResult(aDBO.Errors);
                    }

                    var dbObjID = aDBO.Value.ObjectUUID;
                    var dbBackwardEdges = aDBO.Value.BackwardEdges;

                    if (dbBackwardEdges == null)
                    {
                        var dbBackwardEdgeLoadExcept = dbContext.DBObjectManager.LoadBackwardEdge(aDBO.Value.ObjectLocation);

                        if (!dbBackwardEdgeLoadExcept.Success)
                            return new QueryResult(dbBackwardEdgeLoadExcept.Errors);

                        dbBackwardEdges = dbBackwardEdgeLoadExcept.Value;
                    }

                    var result = dbContext.DBObjectManager.RemoveDBObject(graphDBType, aDBO.Value, dbContext.DBObjectCache, dbContext.SessionSettings);

                    if (!result.Success)
                    {
                        return new QueryResult(result.Errors);
                    }

                    if (dbBackwardEdges != null)
                    {
                        var deleteReferences = _ObjectManipulationManager.DeleteObjectReferences(dbObjID, dbBackwardEdges, dbContext);

                        if (!deleteReferences.Success)
                            return new QueryResult(deleteReferences.Errors);
                    }
                }

                #endregion

                #region remove indexe

                dbContext.DBIndexManager.RemoveGuidIndexEntriesOfParentTypes(graphDBType, dbContext.DBIndexManager);
                
                Parallel.ForEach(graphDBType.GetAllAttributeIndices(), aIdx =>
                    {
                        var idxRef = aIdx.GetIndexReference(dbContext.DBIndexManager);
                        if (!idxRef.Success)
                        {
                            throw new GraphDBException(idxRef.Errors);
                        }
                        idxRef.Value.Clear();
                    });

                #endregion

                #endregion

                #region Commit transaction and add all Warnings and Errors

                var queryResult = new QueryResult(transaction.Commit());

                #endregion

                return queryResult;

            }
        }
    }
}

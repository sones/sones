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
using System.Linq;
using System.Threading.Tasks;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.ErrorHandling;
using System.Collections.Generic;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Drop
{

    public class TruncateNode : AStatement
    {

        #region Data

        private ObjectManipulationManager _ObjectManipulationManager;
        private String _TypeName = ""; //the name of the type that should be dropped
        private List<IWarning> _Warnings = new List<IWarning>();

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "Truncate"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            var _DBContext   = myCompilerContext.IContext as DBContext;
            var _TypeManager = _DBContext.DBTypeManager;
            var grammar = GetGraphQLGrammar(myCompilerContext);

            // get Name
            _TypeName = myParseTreeNode.ChildNodes.Last().Token.ValueString;
            if (myParseTreeNode.ChildNodes[1].Token == null || myParseTreeNode.ChildNodes[1].Token.AsSymbol != grammar.S_TYPE)
            {
                _Warnings.Add(new Warnings.Warning_ObsoleteGQL(
                    String.Format("{0} {1}", grammar.S_TRUNCATE.ToUpperString(), _TypeName),
                    String.Format("{0} {1} {2}", grammar.S_TRUNCATE.ToUpperString(), grammar.S_TYPE.ToUpperString(), _TypeName)));
            }

            var _GraphDBType = _TypeManager.GetTypeByName(_TypeName);

            if (_GraphDBType == null)
            {
                throw new GraphDBException(new Error_TypeDoesNotExist(_TypeName));
            }

            if (_TypeManager.GetAllSubtypes(_GraphDBType, false).Count > 0)
            {
                throw new GraphDBException(new Error_TruncateNotAllowedOnInheritedType(_TypeName));
            }

            _ObjectManipulationManager = new ObjectManipulationManager(_DBContext.SessionSettings, null, _DBContext, this);

        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="myIGraphDBSession">The DBSession to start new transactions</param>
        /// <param name="myDBContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="myIGraphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession myIGraphDBSession, DBContext myDBContext)
        {

            using (var _Transaction = myIGraphDBSession.BeginTransaction())
            {

                var _DBInnerContext = _Transaction.GetDBContext();

                var _GraphDBType = myDBContext.DBTypeManager.GetTypeByName(_TypeName);

                if (_GraphDBType == null)
                {

                    var aError = new Error_TypeDoesNotExist(_TypeName);

                    return new QueryResult(aError);

                }

                #region Remove

                #region remove dbobjects

                var listOfAffectedDBObjects = myDBContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(_GraphDBType), _DBInnerContext).ToList();

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
                        var dbBackwardEdgeLoadExcept = myDBContext.DBObjectManager.LoadBackwardEdge(aDBO.Value.ObjectLocation);

                        if (!dbBackwardEdgeLoadExcept.Success)
                            return new QueryResult(dbBackwardEdgeLoadExcept.Errors);

                        dbBackwardEdges = dbBackwardEdgeLoadExcept.Value;
                    }

                    var result = myDBContext.DBObjectManager.RemoveDBObject(_GraphDBType, aDBO.Value, myDBContext.DBObjectCache, myDBContext.SessionSettings);

                    if (!result.Success)
                    {
                        return new QueryResult(result.Errors);
                    }

                    if (dbBackwardEdges != null)
                    {
                        var deleteReferences = _ObjectManipulationManager.DeleteObjectReferences(dbObjID, dbBackwardEdges, myDBContext);

                        if (!deleteReferences.Success)
                            return new QueryResult(deleteReferences.Errors);
                    }

                }

                #endregion

                #region remove indices

                myDBContext.DBIndexManager.RemoveGuidIndexEntriesOfParentTypes(_GraphDBType, myDBContext.DBIndexManager);
                
                Parallel.ForEach(_GraphDBType.GetAllAttributeIndices(), aIdx =>
                    {
                        var idxRef = aIdx.GetIndexReference(myDBContext.DBIndexManager);
                        if (!idxRef.Success)
                        {
                            throw new GraphDBException(idxRef.Errors);
                        }
                        idxRef.Value.Clear();
                    });

                #endregion

                #endregion

                #region Commit transaction and add all Warnings and Errors

                var queryResult = new QueryResult(_Transaction.Commit());

                #endregion

                queryResult.AddWarnings(_Warnings);

                return queryResult;

            }
        }
    }
}

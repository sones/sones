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


/* <id name="sones GraphDB – ReplaceNode" />
 * <copyright file="ReplaceNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphFS.Session;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Errors;

using sones.Lib.ErrorHandling;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Managers;
using sones.GraphDB.Exceptions;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Replace
{
    public class ReplaceNode : AStatement
    {
        private ObjectManipulationManager _ObjectManipulationManager;
        private BinaryExpressionNode _whereExpression;

        public override string StatementName
        {
            get { return "Replace"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            if (parseNode.HasChildNodes())
            {
                //get type
                if (parseNode.ChildNodes[1] != null && parseNode.ChildNodes[1].AstNode != null)
                {
                    var _Type = ((ATypeNode)(parseNode.ChildNodes[1].AstNode)).DBTypeStream;
                    _ObjectManipulationManager = new ObjectManipulationManager(dbContext.SessionSettings, _Type, dbContext, this);
                }
                

                if (parseNode.ChildNodes[3] != null)
                {
                    if (parseNode.ChildNodes[3].HasChildNodes())
                    {
                        if (parseNode.ChildNodes[3].HasChildNodes())
                        {
                            var result = _ObjectManipulationManager.GetRecursiveAttributes(parseNode.ChildNodes[3], dbContext);
                            if (result.Failed)
                            {
                                throw new GraphDBException(result.Errors);
                            }

                            _ObjectManipulationManager.CheckMandatoryAttributes(dbContext);
                        }
                    }
                }

                if (parseNode.ChildNodes[5] != null)
                    _whereExpression = (BinaryExpressionNode)parseNode.ChildNodes[5].AstNode;
            }
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession, DBContext dbContext)
        {

            #region Data

            IEnumerable<Exceptional<DBObjectStream>> _dbObjects = null;
            List<IWarning> warnings = new List<IWarning>();

            #endregion

            using (var transaction = graphDBSession.BeginTransaction())
            {

                var dbInnerContext = transaction.GetDBContext();


                var _whereGraphResult = _whereExpression.Calculon(dbContext, new CommonUsageGraph(dbContext), false);

                if (_whereGraphResult.Success)
                {
                    _dbObjects = _whereGraphResult.Value.Select(new LevelKey(_ObjectManipulationManager.Type), null, true);
                }
                else
                {
                    return new QueryResult(_whereGraphResult.Errors);
                }

                #region expressionGraph error handling

                warnings.AddRange(_whereGraphResult.Value.GetWarnings());

                #endregion

                if (_dbObjects.Count() > 1)
                {
                    return new QueryResult(new Error_MultipleResults());
                }

                if (_dbObjects.Count() == 0)
                {
                    warnings.Add(new Warnings.Warning_NoObjectsToReplace());
                    return new QueryResult(myErrors: new List<IError>(), myWarnings: warnings);
                }

                IEnumerable<GraphDBType> parentTypes = dbContext.DBTypeManager.GetAllParentTypes(_ObjectManipulationManager.Type, true, false);
                Exceptional<Boolean> checkUniqueVal = null;

                var assingedAttrs = _ObjectManipulationManager.Attributes.ToDictionary(key => key.Key.Definition.UUID, value => value.Value);

                checkUniqueVal = dbContext.DBIndexManager.CheckUniqueConstraint(_ObjectManipulationManager.Type, parentTypes, assingedAttrs);
                if (!checkUniqueVal.Success)
                    return new QueryResult(checkUniqueVal);

                var DeleteResult = _ObjectManipulationManager.DeleteDBObjects(_ObjectManipulationManager.Type, null, _dbObjects, dbContext);

                if (!DeleteResult.Success)
                {
                    return new QueryResult(DeleteResult);
                }

                var result = _ObjectManipulationManager.Insert(dbContext, false);

                result.AddWarnings(warnings);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;

            }

        }
    }
}

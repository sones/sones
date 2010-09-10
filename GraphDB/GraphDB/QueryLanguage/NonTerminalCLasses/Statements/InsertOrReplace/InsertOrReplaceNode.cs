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

/* <id name="PandoraDB – InsertOrReplaceNode" />
 * <copyright file="InsertOrReplaceNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphFS.Session;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;

using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Managers;
using sones.GraphDB.Exceptions;


#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.InsertOrReplace
{
    public class InsertOrReplaceNode : AStatement
    {
        private ObjectManipulationManager _ObjectManipulationManager;
        private BinaryExpressionNode _whereExpression;

        #region Constructor

        public InsertOrReplaceNode()
        { }

        #endregion

        public override string StatementName
        {
            get { return "InsertOrReplace"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;
            GraphDBType type;

            if (parseNode.HasChildNodes())
            {
                //get type
                if (parseNode.ChildNodes[1] != null && parseNode.ChildNodes[1].AstNode != null)
                {
                    type = ((ATypeNode)(parseNode.ChildNodes[1].AstNode)).DBTypeStream;
                    _ObjectManipulationManager = new ObjectManipulationManager(dbContext.SessionSettings, type, dbContext, this);
                }
                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                if (parseNode.ChildNodes[3] != null && parseNode.ChildNodes[3].HasChildNodes())
                {
                    var result = _ObjectManipulationManager.GetRecursiveAttributes(parseNode.ChildNodes[3], dbContext);
                    if (result.Failed)
                    {
                        throw new GraphDBException(result.Errors);
                    }

                    _ObjectManipulationManager.CheckMandatoryAttributes(dbContext);
                }

                if (parseNode.ChildNodes[4] != null && ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinExprNode != null)
                {
                    _whereExpression = ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinExprNode;

                    Exceptional validateResult = _whereExpression.Validate(dbContext, type);
                    if (!validateResult.Success)
                    {
                        throw new GraphDBException(validateResult.Errors);
                    }
                }
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

            IEnumerable<Exceptional<DBObjectStream>> extractedDBOs = null;
            List<IWarning> warnings = new List<IWarning>();

            #endregion


            using (var transaction = graphDBSession.BeginTransaction())
            {

                var dbInnerContext = transaction.GetDBContext();


                if (_whereExpression != null)
                {
                    var _whereGraphResult = _whereExpression.Calculon(dbContext, new CommonUsageGraph(dbContext), false);

                    if (_whereGraphResult.Success)
                    {
                        extractedDBOs = _whereGraphResult.Value.Select(new LevelKey(_ObjectManipulationManager.Type), null, true);
                    }
                    else
                    {
                        return new QueryResult(_whereGraphResult.Errors);
                    }

                    #region expressionGraph error handling

                    warnings.AddRange(_whereGraphResult.Value.GetWarnings());

                    #endregion

                }
                else
                {
                    extractedDBOs = new List<Exceptional<DBObjectStream>>();
                }

                var count = extractedDBOs.Count();

                if (count > 1)
                {
                    return new QueryResult(new Error_MultipleResults());
                }
                else
                {
                    if (count == 1)
                    {
                        #region delete

                        IEnumerable<GraphDBType> parentTypes = dbContext.DBTypeManager.GetAllParentTypes(_ObjectManipulationManager.Type, true, false);
                        Exceptional<Boolean> checkUniqueVal = null;

                        var assingedAttrs = _ObjectManipulationManager.Attributes.ToDictionary(key => key.Key.Definition.UUID, value => value.Value);

                        checkUniqueVal = dbContext.DBIndexManager.CheckUniqueConstraint(_ObjectManipulationManager.Type, parentTypes, assingedAttrs);
                        if (checkUniqueVal.Failed)
                            return new QueryResult(checkUniqueVal.Errors);

                        var DeleteResult = _ObjectManipulationManager.DeleteDBObjects(_ObjectManipulationManager.Type, null, extractedDBOs, dbContext);

                        if (DeleteResult.Failed)
                        {
                            return new QueryResult(DeleteResult.Errors);
                        }

                        #endregion
                    }
                }

                //Insert
                var result = _ObjectManipulationManager.Insert(dbContext, false);

                //if there were any warnings during this process, the need to be added
                result.AddWarnings(warnings);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;

            }

        }
    }
}

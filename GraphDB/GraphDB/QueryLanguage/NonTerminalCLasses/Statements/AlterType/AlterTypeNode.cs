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


/* <id name="sones GraphDB – AlterTypeNode astnode" />
 * <copyright file="AlterTypeNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of an Alter Type statement.</summary>
 */

#region Usings

using System;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers;
using sones.GraphDB.Managers.AlterType;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.Lib.Frameworks.Irony.Parsing;
using System.Collections.Generic;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements
{
    /// <summary>
    /// This node is requested in case of an Alter Type statement.
    /// </summary>
    class AlterTypeNode : AStatement
    {

        #region Properties - Statement information

        public override String StatementName { get { return "AlterType"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region Data

        String _TypeName = ""; //the name of the type that should be altered
        List<AAlterTypeCommand> _AlterTypeCommand;

        #endregion

        #region GetContent

        /// <summary>
        /// Gets the content of a AlterTypeNode.
        /// </summary>
        /// <param name="myCompilerContext">CompilerContext of Irony.</param>
        /// <param name="myParseTreeNode">The current ParseNode.</param>
        /// <param name="myTypeManager">The TypeManager of the PandoraDB.</param>
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {
            
            try
            {
                _AlterTypeCommand = new List<AAlterTypeCommand>();
                
                _TypeName = myParseTreeNode.ChildNodes[2].Token.ValueString;

                #region Get the AlterTypeCommand

                foreach (var alterCmds in myParseTreeNode.ChildNodes[3].ChildNodes)
                {
                    if (alterCmds.AstNode != null)
                    {
                        var alterCommand = (AlterCommandNode)alterCmds.AstNode;

                        if (alterCommand.AlterTypeCommand != null)
                        {
                            _AlterTypeCommand.Add(alterCommand.AlterTypeCommand);
                        }
                    }                    
                }

                if (myParseTreeNode.ChildNodes[4].HasChildNodes())
                {
                    _AlterTypeCommand.Add(new AlterType_SetUnique() { UniqueAttributes = ((UniqueAttributesOptNode)myParseTreeNode.ChildNodes[4].AstNode).UniqueAttributes });
                }

                if (myParseTreeNode.ChildNodes[5].HasChildNodes())
                {
                    _AlterTypeCommand.Add(new AlterType_SetMandatory() { MandatoryAttributes = ((MandatoryOptNode)myParseTreeNode.ChildNodes[5].AstNode).MandatoryAttribs });
                }

                #endregion

            }
            catch (GraphDBException e)
            {
                throw new GraphDBException(e.GraphDBErrors);
            }

        }

        #endregion

        #region Execute

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

                #region check pandoraType

                var dbType = dbInnerContext.DBTypeManager.GetTypeByName(_TypeName);
                if (dbType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(_TypeName));
                }

                #endregion

                QueryResult qr = new QueryResult();

                foreach (var alterTypeCmd in _AlterTypeCommand)
                {
                    var result = dbInnerContext.DBTypeManager.AlterType(dbInnerContext, dbType, alterTypeCmd);
                    qr.AddErrorsAndWarnings(result);
                    
                    if(result.Value != null)
                    {
                        foreach (var resVal in result.Value.Results)
                            qr.AddResult(resVal);
                    }
                }

                #region Commit transaction and add all Warnings and Errors

                qr.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return qr;
            }
        }

        #endregion

    }
}

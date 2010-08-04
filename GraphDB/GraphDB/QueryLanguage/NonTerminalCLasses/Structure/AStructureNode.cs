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


/* <id name="sones GraphDB – abstract structure node" />
 * <copyright file="AStructureNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Abstract class for all structure nodes.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;

using sones.GraphDB.ObjectManagement;

using sones.Lib.DataStructures;
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.ErrorHandling;

using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Objects;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.Operators;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.Lib.Frameworks.Irony.Parsing;
using System.Diagnostics;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// Abstract class for all structure nodes.
    /// </summary>
    public abstract class AStructureNode
    {

        #region protected helper methods

        /// <summary>
        /// Extracts a AExpressionDefinition from the ParseTreeNode
        /// </summary>
        /// <param name="myParseTreeNode"></param>
        /// <returns></returns>
        protected AExpressionDefinition GetExpressionDefinition(ParseTreeNode myParseTreeNode)
        {
            AExpressionDefinition retVal = null;
            if (myParseTreeNode.Term is NonTerminal)
            {
                #region left is NonTerminal

                if (myParseTreeNode.AstNode is IDNode)
                {
                    retVal = (myParseTreeNode.AstNode as IDNode).IDChainDefinition;
                }
                else if (myParseTreeNode.AstNode is TupleNode)
                {
                    retVal = (myParseTreeNode.AstNode as TupleNode).TupleDefinition;
                }
                else if (myParseTreeNode.AstNode is BinaryExpressionNode)
                {
                    retVal = (myParseTreeNode.AstNode as BinaryExpressionNode).BinaryExpressionDefinition;
                }
                else if (myParseTreeNode.AstNode is UnaryExpressionNode)
                {
                    retVal = (myParseTreeNode.AstNode as UnaryExpressionNode).UnaryExpressionDefinition;
                }
                else if (myParseTreeNode.AstNode is AggregateNode)
                {
                    retVal = (myParseTreeNode.AstNode as AggregateNode).AggregateDefinition;
                }

                #endregion
            }
            else
            {
                #region No NonTerminal

                retVal = new ValueDefinition(myParseTreeNode.Token.Value);

                #endregion
            }

            return retVal;
        }

        protected List<TypeReferenceDefinition> GetTypeReferenceDefinitions(CompilerContext context)
        {
            return ((List<TypeReferenceDefinition>)context.PandoraListOfReferences);
        }

        #endregion

        #region public methods

        public GraphQL GetGraphQLGrammar(CompilerContext context)
        {
            return context.Compiler.Language.Grammar as GraphQL;
        }

        #endregion

    }
}

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

/* <id name="GraphDB – abstract structure node" />
 * <copyright file="AStructureNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Abstract class for all structure nodes.</summary>
 */

#region Usings

using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// Abstract class for all structure nodes.
    /// </summary>
    public abstract class AStructureNode
    {

        #region Protected properties

        public Exceptional ParsingResult { get; private set; }

        #endregion

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
            return ((List<TypeReferenceDefinition>)context.GraphListOfReferences);
        }

        #endregion

        #region public methods

        public GraphQL.GraphQueryLanguage GetGraphQLGrammar(CompilerContext context)
        {
            return context.Compiler.Language.Grammar as GraphQL.GraphQueryLanguage;
        }

        #endregion

        #region Ctor

        public AStructureNode()
        {
            ParsingResult = new Exceptional();
        }

        #endregion

    }

}

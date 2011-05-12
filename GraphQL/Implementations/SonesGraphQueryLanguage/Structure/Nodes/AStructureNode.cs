/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using System;
using Irony.Parsing;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;
namespace sones.GraphQL.Structure.Nodes
{
    public abstract class AStructureNode
    {
        protected bool HasChildNodes(ParseTreeNode parseTreeNode)
        {
            return parseTreeNode.ChildNodes != null && parseTreeNode.ChildNodes.Count > 0;
        }

        protected List<TypeReferenceDefinition> GetTypeReferenceDefinitions(ParsingContext context)
        {
            if (context.Values.ContainsKey(SonesGQLConstants.GraphListOfReferences))
            {
                return ((List<TypeReferenceDefinition>)context.Values[SonesGQLConstants.GraphListOfReferences]);
            }
            else
            {
                return new List<TypeReferenceDefinition>();
            }
        }

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
    }
}

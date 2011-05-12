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

using System;
using Irony.Ast;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using Irony.Parsing;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.Structure.Nodes.DML;

namespace sones.GraphQL.Structure.Nodes.Expressions
{

    /// <summary>
    /// This node is requested in case of an tuple statement.
    /// </summary>
    public sealed class TupleNode : AStructureNode, IAstNodeInit
    {
        public TupleDefinition TupleDefinition { get; private set; }

        #region Constructor

        public TupleNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            //TupleDefinition = new TupleDefinition(GetKindOfTuple(context, parseNode));
            TupleDefinition = new TupleDefinition();

            ParseTreeNodeList childNodes;
            if (parseNode.ChildNodes[0].AstNode == null && parseNode.ChildNodes[0].ChildNodes.Count > 0) // this is a not resolved node and has childNodes
            {
                childNodes = parseNode.ChildNodes[0].ChildNodes;
            }

            else // single expression tuple - SetRefNode seems MarkTransient the tuple
            {
                childNodes = parseNode.ChildNodes;
            }

            #region get tuple elements

            foreach (ParseTreeNode aExpressionNode in childNodes)
            {

                #region Data

                Type typeOfExpression = null;
                TupleElement aElement = null;

                #endregion

                #region Check the AstNode type and set the typeOfExpression. In case of a tuple just add them.

                if (aExpressionNode.Term != null)
                {
                    if ((aExpressionNode.Term.Name == SonesGQLConstants.BracketLeft) || (aExpressionNode.Term.Name == SonesGQLConstants.BracketRight))
                    {
                        continue;
                    }
                }

                if (aExpressionNode.AstNode is ExpressionOfAListNode)
                {
                    typeOfExpression = ((ExpressionOfAListNode)aExpressionNode.AstNode).GetType();
                }
                else if (aExpressionNode.AstNode is ExpressionNode)
                {
                    typeOfExpression = ((ExpressionNode)aExpressionNode.AstNode).GetType();
                }
                else if (aExpressionNode.AstNode is BinaryExpressionNode)
                {
                    typeOfExpression = aExpressionNode.Term.GetType();
                }
                else if (aExpressionNode.AstNode is UnaryExpressionNode)
                {
                    typeOfExpression = aExpressionNode.Term.GetType();
                }
                else if (aExpressionNode.AstNode is TupleNode)
                {
                    TupleDefinition = ((TupleNode)aExpressionNode.AstNode).TupleDefinition;
                    continue;
                }
                else if (aExpressionNode.AstNode is PartialSelectStmtNode)
                {
                    typeOfExpression = aExpressionNode.Term.GetType();
                }
                else if (aExpressionNode.AstNode is IDNode)
                {
                    throw new NotImplementedQLException("");
                }
                else
                {
                    var val = new ValueDefinition(aExpressionNode.Token.Value);
                    TupleDefinition.AddElement(new TupleElement(val));
                    continue;
                }

                #endregion

                if (aExpressionNode.AstNode is ExpressionNode)
                {
                    aElement = new TupleElement(((ExpressionNode)aExpressionNode.AstNode).ExpressionDefinition);
                }
                else if (aExpressionNode.AstNode is ExpressionOfAListNode)
                {
                    aElement = new TupleElement((((ExpressionOfAListNode)aExpressionNode.AstNode).ExpressionDefinition));

                    if (((ExpressionOfAListNode)aExpressionNode.AstNode).Parameters != null)
                    {
                        aElement.Parameters = ((ExpressionOfAListNode)aExpressionNode.AstNode).Parameters;
                    }
                }
                else if (aExpressionNode.AstNode is BinaryExpressionNode)
                {
                    aElement = new TupleElement((aExpressionNode.AstNode as BinaryExpressionNode).BinaryExpressionDefinition);
                }
                else if (aExpressionNode.AstNode is UnaryExpressionNode)
                {
                    aElement = new TupleElement((aExpressionNode.AstNode as UnaryExpressionNode).UnaryExpressionDefinition);
                }
                else if (aExpressionNode.AstNode is PartialSelectStmtNode)
                {
                    #region partial select

                    aElement = new TupleElement(((PartialSelectStmtNode)aExpressionNode.AstNode).SelectDefinition);

                    #endregion
                }

                else if (aExpressionNode.AstNode is ExpressionNode)
                {
                    aElement = new TupleElement(((ExpressionNode)aExpressionNode.AstNode).ExpressionDefinition);
                }
                else if (aExpressionNode.AstNode is ExpressionOfAListNode)
                {
                    aElement = new TupleElement(((ExpressionOfAListNode)aExpressionNode.AstNode).ExpressionDefinition);

                    if (((ExpressionOfAListNode)aExpressionNode.AstNode).Parameters != null)
                    {
                        aElement.Parameters = ((ExpressionOfAListNode)aExpressionNode.AstNode).Parameters;
                    }
                }
                else
                {
                    throw new NotImplementedQLException("");
                }

                TupleDefinition.AddElement(aElement);

            }

            #endregion
        }

        #endregion
    }
}

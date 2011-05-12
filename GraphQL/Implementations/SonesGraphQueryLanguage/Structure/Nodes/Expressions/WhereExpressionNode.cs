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
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.Expressions
{
    /// <summary>
    /// This node is requested in case of where clause.
    /// </summary>
    public sealed class WhereExpressionNode : AStructureNode, IAstNodeInit
    {
        public BinaryExpressionDefinition BinaryExpressionDefinition { get; private set; }

        public WhereExpressionNode()
        { }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {

                if (parseNode.ChildNodes[1].AstNode is TupleNode && (parseNode.ChildNodes[1].AstNode as TupleNode).TupleDefinition.TupleElements.Count == 1)
                {
                    var tuple = (parseNode.ChildNodes[1].AstNode as TupleNode).TupleDefinition.Simplyfy();
                    BinaryExpressionDefinition = (tuple.TupleElements[0].Value as BinaryExpressionDefinition);
                }
                else if (parseNode.ChildNodes[1].AstNode is BinaryExpressionNode)
                {
                    BinaryExpressionDefinition = ((BinaryExpressionNode)parseNode.ChildNodes[1].AstNode).BinaryExpressionDefinition;
                }
            }
        }

        #endregion
    }
}

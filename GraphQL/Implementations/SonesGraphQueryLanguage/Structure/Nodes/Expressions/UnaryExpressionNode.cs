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
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.Expressions
{
    public sealed class UnaryExpressionNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private String _OperatorSymbol;
        private Object _Term;

        #endregion

        public UnaryExpressionDefinition UnaryExpressionDefinition { get; private set; }

        #region Accessor

        public String OperatorSymbol { get; private set; }
        public AExpressionDefinition Expression { get; private set; }

        #endregion

        #region Constructor

        public UnaryExpressionNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {

                if (parseNode.ChildNodes[1].AstNode == null)
                {
                    throw new NotImplementedQLException("");
                }

                _OperatorSymbol = parseNode.ChildNodes[0].Token.Text;
                Expression = GetExpressionDefinition(parseNode.ChildNodes[1]);

            }

            System.Diagnostics.Debug.Assert(Expression != null);

            UnaryExpressionDefinition = new UnaryExpressionDefinition(_OperatorSymbol, Expression);
        }

        #endregion
    }
}

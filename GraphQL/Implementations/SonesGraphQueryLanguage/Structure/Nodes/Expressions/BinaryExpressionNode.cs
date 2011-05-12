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
    public sealed class BinaryExpressionNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private String _OperatorSymbol;
        private AExpressionDefinition _left = null;
        private AExpressionDefinition _right = null;
        private String OriginalString = String.Empty;

        #endregion

        #region Properties

        public BinaryExpressionDefinition BinaryExpressionDefinition { get; private set; }

        #endregion

        #region constructor

        public BinaryExpressionNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region set type of binary expression

            _OperatorSymbol = parseNode.ChildNodes[1].FirstChild.Token.ValueString;
            _left = GetExpressionDefinition(parseNode.ChildNodes[0]);
            _right = GetExpressionDefinition(parseNode.ChildNodes[2]);

            #endregion

            BinaryExpressionDefinition = new BinaryExpressionDefinition(_OperatorSymbol, _left, _right);

            OriginalString += _left.ToString() + " ";
            OriginalString += _OperatorSymbol + " ";
            OriginalString += _right.ToString();
        }

        #endregion
    }
}

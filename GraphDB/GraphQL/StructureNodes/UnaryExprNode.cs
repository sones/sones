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


#region Usings

using System;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Structures.Operators;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class UnaryExpressionNode : AStructureNode
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

        #region GetContent(myCompilerContext, myParseTreeNode)

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            if (myParseTreeNode.HasChildNodes())
            {

                if (myParseTreeNode.ChildNodes[1].AstNode == null)
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                _OperatorSymbol = myParseTreeNode.ChildNodes[0].Token.Text;
                Expression = GetExpressionDefinition(myParseTreeNode.ChildNodes[1]);

            }

            System.Diagnostics.Debug.Assert(Expression != null);

            UnaryExpressionDefinition = new UnaryExpressionDefinition(_OperatorSymbol, Expression);

        }

        #endregion

    }

}

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

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.Operators;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class UnaryExpressionNode : AStructureNode
    {

        #region Data

        private String _OperatorSymbol;
        private Object _Term;

        #endregion

        #region constructor

        public UnaryExpressionNode()
        { }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var pluginManager = dbContext.DBPluginManager;

            if (parseNode.HasChildNodes())
            {
                _OperatorSymbol = parseNode.ChildNodes[0].Token.Text;
                var binaryOperator = pluginManager.GetBinaryOperator(_OperatorSymbol);

                if (binaryOperator.Label != BinaryOperator.Addition || binaryOperator.Label != BinaryOperator.Subtraction)
                {
                    if (parseNode.ChildNodes[1].AstNode == null)
                        throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                    _Term = parseNode.ChildNodes[1].AstNode;
                }
                else
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
        }

        #region Accessor

        public String OperatorSymbol { get { return _OperatorSymbol; } }
        public Object Term { get { return _Term; } }

        #endregion
    }
}

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
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
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

                    Expression = GetExpressionDefinition(parseNode.ChildNodes[1]);
                }
                else
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            UnaryExpressionDefinition = new UnaryExpressionDefinition(_OperatorSymbol, Expression);
        }
 
    }
}

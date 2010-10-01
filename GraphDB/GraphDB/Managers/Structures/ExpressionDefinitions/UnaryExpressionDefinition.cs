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

/*
 * UnaryExpressionDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Operators;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Structures.Enums;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    public class UnaryExpressionDefinition : AExpressionDefinition
    {
        private AExpressionDefinition _Expression;
        private String _OperatorSymbol;

        public UnaryExpressionDefinition(string myOperatorSymbol, AExpressionDefinition myExpression)
        {
            _OperatorSymbol = myOperatorSymbol;
            _Expression = myExpression;
        }

        public Exceptional<BinaryExpressionDefinition> GetBinaryExpression(DBContext myDBContext)
        {
            
            AExpressionDefinition right;
            var op = myDBContext.DBPluginManager.GetBinaryOperator(_OperatorSymbol);
            if (op == null)
            {
                return new Exceptional<BinaryExpressionDefinition>(new Error_OperatorDoesNotExist(_OperatorSymbol));
            }


            if (op.Label == BinaryOperator.Subtraction)
                right = new ValueDefinition(new DBInt32(-1));
            else
                right = new ValueDefinition(new DBInt32(1));


            var binExpr = new BinaryExpressionDefinition("*", _Expression, right);
            binExpr.Validate(myDBContext);

            return new Exceptional<BinaryExpressionDefinition>(binExpr, binExpr.ValidateResult);
        }

    }
}

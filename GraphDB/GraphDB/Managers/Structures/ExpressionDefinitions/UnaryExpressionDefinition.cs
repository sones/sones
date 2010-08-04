/*
 * UnaryExpressionDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Operators;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage.Enums;
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

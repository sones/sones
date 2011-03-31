using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request.Helper.Operator.Logic;

namespace sones.GraphDB.Request.Helper.Expression
{
    public sealed class UnaryExpression : IExpression
    {
        public readonly ILogicOperator Operator;

        public readonly IExpression Expression;
    }
}

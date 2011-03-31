using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request.Helper.Operator;

namespace sones.GraphDB.Request.Helper.Expression
{
    public sealed class BinaryExpression : IExpression
    {
        public readonly IExpression Left;

        public readonly IOperator Operator;

        public readonly IExpression Right;
    }
}

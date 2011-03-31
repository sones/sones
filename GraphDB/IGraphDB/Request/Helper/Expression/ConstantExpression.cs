using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request.Helper.Expression
{
    public sealed class ConstantExpression : IExpression
    {
        public readonly Object Constant;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public abstract class AExpressionDefinition
    {
        protected ABinaryOperator GetOperatorBySymbol(string _OperatorSymbol)
        {
            throw new NotImplementedException();
        }
    }
}

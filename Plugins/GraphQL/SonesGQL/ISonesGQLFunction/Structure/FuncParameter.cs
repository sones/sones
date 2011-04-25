using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace ISonesGQLFunction.Structure
{
    public sealed class FuncParameter
    {
        public readonly Object Value;

        public readonly IAttributeDefinition AttributeDefinition;

        public FuncParameter(Object myValue, IAttributeDefinition myAttributeDefinition)
        {
            Value = myValue;
            AttributeDefinition = myAttributeDefinition;
        }

        public FuncParameter(Object myValue)
            : this(myValue, null)
        { }
    }

}

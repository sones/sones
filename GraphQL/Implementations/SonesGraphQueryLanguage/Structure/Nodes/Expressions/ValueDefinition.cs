using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public sealed class ValueDefinition : AOperationDefinition
    {

        #region Properties

        public Object Value { get; private set; }

        #endregion

        #region Ctors

        public ValueDefinition(Object myValue)
        {
            Value = myValue;
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is ValueDefinition)
            {
                return (obj as ValueDefinition).Value.Equals(Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public sealed class TupleElement : IEqualityComparer<TupleElement>
    {

        #region Data

        AExpressionDefinition _Value = null;
        public List<Object> Parameters { get; set; }

        #endregion

        public TupleElement(AExpressionDefinition Value)
        {
            _Value = Value;
            Parameters = new List<Object>();
        }

        public AExpressionDefinition Value { get { return _Value; } }


        public override bool Equals(object obj)
        {
            if (obj is TupleElement)
            {
                return Equals((obj as TupleElement), this);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        #region IEqualityComparer<TupleElement> Members

        public bool Equals(TupleElement x, TupleElement y)
        {
            return 
                x.Value.Equals(y.Value) && 
                ((x.Parameters == null) == (y.Parameters == null)) && 
                (x.Parameters.Count == y.Parameters.Count);
        }

        public int GetHashCode(TupleElement obj)
        {
            return obj._Value.GetHashCode();
        }

        #endregion
    }
}

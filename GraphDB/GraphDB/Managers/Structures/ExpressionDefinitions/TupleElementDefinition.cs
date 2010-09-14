/*
 * TupleElement
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;

using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.Lib;

#endregion

namespace sones.GraphDB.Managers.Structures
{

    public class TupleElement : IEqualityComparer<TupleElement>
    {

        #region Data

        AExpressionDefinition _Value = null;
        BasicType _TypeOfValue;
        //public HashSet<ObjectUUID> CorrespondingDBObjectUUIDS { get; set; }
        //public AEdgeType Edges { get; set; }
        public List<ADBBaseObject> Parameters { get; set; }

        #endregion

        public TupleElement(BasicType TypeOfValue, AExpressionDefinition Value)
        {
            _TypeOfValue = TypeOfValue;
            _Value = Value;
            Parameters = new List<ADBBaseObject>();
        }

        public TupleElement(AExpressionDefinition Value)
        {
            _Value = Value;
            Parameters = new List<ADBBaseObject>();
        }
        public AExpressionDefinition Value { get { return _Value; } }

        public BasicType TypeOfValue { get { return _TypeOfValue; } }

        public override string ToString()
        {
            return String.Concat("[", _TypeOfValue.ToString(), "] ", _Value.ToString(), "{", _Value.GetType().Name, "}");
        }

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
            return x.Value.Equals(y.Value) && (x.Parameters.IsNotNullOrEmpty() == y.Parameters.IsNotNullOrEmpty()) && (x.Parameters.Count == y.Parameters.Count);
        }

        public int GetHashCode(TupleElement obj)
        {
            return obj._Value.GetHashCode();
        }

        #endregion
    }
}

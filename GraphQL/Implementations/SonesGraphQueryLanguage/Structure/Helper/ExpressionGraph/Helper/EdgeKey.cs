using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper
{
    /// <summary>
    /// The Edge Key carries information about backward edges of DBObjects.
    /// </summary>
    public sealed class EdgeKey : IComparable, IComparable<EdgeKey>
    {

        #region properties

        public readonly Boolean IsAttributeSet;

        #region typeUUID

        public readonly Int64 VertexTypeID;

        #endregion

        #region attrUUID

        public readonly Int64 AttributeID;

        #endregion

        #endregion

        #region Constructor

        #region EdgeKey(attr, objectUUID)

        public EdgeKey(Int64 myVertexTypeID, Int64 myAttributeID)
        {
            VertexTypeID = myVertexTypeID;
            AttributeID = myAttributeID;
            IsAttributeSet = true;
        }

        #endregion

        public EdgeKey(Int64 myVertexTypeID)
        {
            VertexTypeID = myVertexTypeID;
            IsAttributeSet = false;
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return CompareTo((EdgeKey)obj);
        }

        #endregion

        #region IComparable<EdgeKey> Members

        public int CompareTo(EdgeKey other)
        {
            if (this.VertexTypeID.CompareTo(other.VertexTypeID) == 0)
            {
                if (this.AttributeID.CompareTo(other.AttributeID) == 0)
                {
                    return 0;
                }
            }

            return -1;
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is EdgeKey)
            {
                return Equals((EdgeKey)obj);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(EdgeKey p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return (this.VertexTypeID == p.VertexTypeID) && (this.AttributeID == p.AttributeID);
        }

        public static Boolean operator ==(EdgeKey a, EdgeKey b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(EdgeKey a, EdgeKey b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return VertexTypeID.GetHashCode() ^ AttributeID.GetHashCode();
        }

        #endregion
    }
}

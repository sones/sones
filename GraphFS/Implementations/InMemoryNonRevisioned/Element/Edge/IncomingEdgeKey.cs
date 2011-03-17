using System;

namespace sones.GraphFS.Element.Edge
{
    /// <summary>
    /// The definition of an incoming edge
    /// </summary>
    public struct IncomingEdgeKey
    {
        #region data

        /// <summary>
        /// The id of the edge property
        /// </summary>
        public readonly Int64 EdgePropertyID;

        /// <summary>
        /// The id of the vertex type
        /// </summary>
        public readonly Int64 VertexTypeID;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new incoming edge key
        /// </summary>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myEdgePropertyID">The id of the edge property</param>
        public IncomingEdgeKey(Int64 myVertexTypeID, Int64 myEdgePropertyID)
        {
            EdgePropertyID = myEdgePropertyID;

            VertexTypeID = myVertexTypeID;
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            if (obj is IncomingEdgeKey)
            {
                return Equals((IncomingEdgeKey)obj);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(IncomingEdgeKey p)
        {
            return (EdgePropertyID == p.EdgePropertyID) && (VertexTypeID == p.VertexTypeID);
        }

        public static Boolean operator ==(IncomingEdgeKey a, IncomingEdgeKey b)
        {
            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(IncomingEdgeKey a, IncomingEdgeKey b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return EdgePropertyID.GetHashCode() ^ VertexTypeID.GetHashCode();
        }

        #endregion
    }
}
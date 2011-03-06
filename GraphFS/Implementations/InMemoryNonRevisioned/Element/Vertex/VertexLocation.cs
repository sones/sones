using System;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The in memory location of a vertex
    /// </summary>
    public struct VertexLocation
    {
        #region data

        /// <summary>
        /// The id of the vertex
        /// </summary>
        public readonly Int64 VertexID;

        /// <summary>
        /// The id of the vertex type
        /// </summary>
        public readonly Int64 VertexTypeID;

        #endregion

        #region Constructor

        /// <summary>
        /// creates a new vertex location
        /// </summary>
        /// <param name="myVertexTypeID">The type id of the vertex</param>
        /// <param name="myVertexID">The id of the vertex</param>
        public VertexLocation(Int64 myVertexTypeID, Int64 myVertexID)
        {
            VertexID = myVertexID;

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

            if (obj is VertexLocation)
            {
                return Equals((VertexLocation) obj);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(VertexLocation p)
        {
            return (VertexID == p.VertexID) && (VertexTypeID == p.VertexTypeID);
        }

        public static Boolean operator ==(VertexLocation a, VertexLocation b)
        {
            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(VertexLocation a, VertexLocation b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return VertexID.GetHashCode() ^ VertexTypeID.GetHashCode();
        }

        #endregion
    }
}
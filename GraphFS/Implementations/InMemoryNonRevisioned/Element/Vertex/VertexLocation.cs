using System;
using System.Collections.Generic;
using System.IO;
using sones.GraphFS.ErrorHandling;
using sones.PropertyHyperGraph;

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
        public readonly UInt64 VertexID;

        /// <summary>
        /// The id of the vertex type
        /// </summary>
        public readonly UInt64 VertexTypeID;

        #endregion

        #region Constructor

        /// <summary>
        /// creates a new vertex location
        /// </summary>
        /// <param name="myVertexTypeID">The type id of the vertex</param>
        /// <param name="myVertexID">The id of the vertex</param>
        public VertexLocation (UInt64 myVertexTypeID, UInt64 myVertexID)
        {
            VertexID = myVertexID;

            VertexTypeID = myVertexTypeID;
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

            if (obj is VertexLocation)
            {
                return Equals((VertexLocation)obj);
            }
            else
            {
                return false;
            }


        }

        public Boolean Equals(VertexLocation p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return (this.VertexID == p.VertexID) && (this.VertexTypeID == p.VertexTypeID);
        }

        public static Boolean operator ==(VertexLocation a, VertexLocation b)
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

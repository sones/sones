using System;
using sones.Library.PropertyHyperGraph;

namespace sones.Library.Commons.VertexStore.Definitions
{
    /// <summary>
    /// The information / loaction definition of a vertex
    /// </summary>
    public struct VertexInformation
    {
        #region data

        /// <summary>
        /// The vertex edition name
        /// </summary>
        public readonly String VertexEditionName;

        /// <summary>
        /// The id of this vertex
        /// </summary>
        public readonly Int64 VertexID;

        /// <summary>
        /// The vertex revision id
        /// </summary>
        public readonly Int64 VertexRevisionID;

        /// <summary>
        /// The type id of this vertex
        /// </summary>
        public readonly Int64 VertexTypeID;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new vertex information/location
        /// </summary>
        /// <param name="myVertexTypeID">The type id of this vertex</param>
        /// <param name="myVertexID">The id of this vertex</param>
        /// <param name="myVertexRevisionID">The vertex revision id</param>
        /// <param name="myVertexEditionName">The vertex edition name</param>
        public VertexInformation(
            Int64 myVertexTypeID,
            Int64 myVertexID,
            Int64 myVertexRevisionID = 0L,
            String myVertexEditionName = null)
        {
            VertexTypeID = myVertexTypeID;
            VertexID = myVertexID;
            VertexRevisionID = myVertexRevisionID;
            VertexEditionName = myVertexEditionName;
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
            if (obj is VertexInformation)
            {
                return Equals((VertexInformation)obj);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(VertexInformation p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return
                (this.VertexID == p.VertexID) &&
                (this.VertexTypeID == p.VertexTypeID) &&
                (this.VertexEditionName == p.VertexEditionName) &&
                (this.VertexRevisionID == p.VertexRevisionID);
        }

        public static Boolean operator ==(VertexInformation a, VertexInformation b)
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

        public static Boolean operator !=(VertexInformation a, VertexInformation b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return VertexID.GetHashCode() ^ VertexTypeID.GetHashCode() ^ VertexEditionName.GetHashCode() ^ VertexRevisionID.GetHashCode();
        }

        #endregion
    }
}
using System;

namespace sones.PropertyHyperGraph
{
    /// <summary>
    /// The id of a vertex
    /// </summary>
    public sealed class VertexID : IGraphElementID
    {
        #region Data

        /// <summary>
        /// The type-global-id of the vertex
        /// </summary>
        readonly public String Name;

        /// <summary>
        /// The id of the vertex type
        /// </summary>
        readonly private UInt64 _vertexTypeID;

        /// <summary>
        /// The hashcode of the VertexID
        /// </summary>
        readonly private int _hashCode = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new VertexID.
        /// A VertexID consists of a vertex type name and a vertex name
        /// </summary>
        /// <param name="myVertexTypeID">The vertex type id</param>
        /// <param name="myVertexID">The vertex name (if left out, a name will be generated)</param>
        public VertexID(UInt64 myVertexTypeID, String myName = null)
        {
            if (myName != null)
            {
                Name = myName;
            }
            else
            {
                Name = Guid.NewGuid().ToString();
            }

            _vertexTypeID = myVertexTypeID;

            _hashCode = Name.GetHashCode() ^ _vertexTypeID.GetHashCode();
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            VertexID v = obj as VertexID;

            if (v != null)
            {
                return Equals(v);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(VertexID myVertex)
        {
            if ((object)myVertex == null)
            {
                return false;
            }

            return (this.Name == myVertex.Name) && (this._vertexTypeID == myVertex._vertexTypeID);
        }

        public static Boolean operator ==(VertexID aVertex, VertexID bVertex)
        {
            if (Object.ReferenceEquals(aVertex, bVertex))
            {
                return true;
            }

            if (((object)aVertex == null) || ((object)bVertex == null))
            {
                return false;
            }

            return aVertex.Equals(bVertex);
        }

        public static Boolean operator !=(VertexID aVertex, VertexID bVertex)
        {
            return !(aVertex == bVertex);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        #endregion

        #region IGraphElementID Members

        public UInt64 TypeID
        {
            get { return _vertexTypeID; }
        }

        #endregion
    }
}

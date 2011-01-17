using System;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The id of a vertex. It consists of a Guid and an vertex-type-name
    /// </summary>
    public sealed class VertexID
    {
        #region Data

        /// <summary>
        /// The type-global-id of the vertex
        /// </summary>
        readonly public Guid VertexGuid;

        /// <summary>
        /// The name of the vertex type
        /// </summary>
        readonly public String TypeName;

        /// <summary>
        /// The hashcode of the VertexID
        /// </summary>
        readonly private int _hashCode = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new VertexID
        /// </summary>
        /// <param name="myVertexGuid">The Guid of the vertex</param>
        /// <param name="myTypeName">The type name of the vertex</param>
        public VertexID(Guid myVertexGuid, String myTypeName)
        {
            VertexGuid = myVertexGuid;

            TypeName = myTypeName;

            _hashCode = VertexGuid.GetHashCode() ^ TypeName.GetHashCode();
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

            return (this.VertexGuid == myVertex.VertexGuid) && (this.TypeName == myVertex.TypeName);
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
    }
}

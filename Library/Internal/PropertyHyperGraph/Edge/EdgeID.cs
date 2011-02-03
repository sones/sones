using System;

namespace sones.PropertyHyperGraph
{
    /// <summary>
    /// The id of an edge
    /// </summary>
    public sealed class EdgeID : IGraphElementID
    {
        #region Data

        /// <summary>
        /// The id of the vertex type
        /// </summary>
        readonly private UInt64 _vertexTypeID;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new EdgeID.
        /// </summary>
        /// <param name="myEdgeTypeID">The edge type id</param>
        public EdgeID(UInt64 myEdgeTypeID)
        {
            _vertexTypeID = myEdgeTypeID;
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            EdgeID v = obj as EdgeID;

            if (v != null)
            {
                return Equals(v);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(EdgeID myVertex)
        {
            if ((object)myVertex == null)
            {
                return false;
            }

            return this.TypeID == myVertex.TypeID;
        }

        public static Boolean operator ==(EdgeID aVertex, EdgeID bVertex)
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

        public static Boolean operator !=(EdgeID aVertex, EdgeID bVertex)
        {
            return !(aVertex == bVertex);
        }

        public override int GetHashCode()
        {
            return _vertexTypeID.GetHashCode();
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

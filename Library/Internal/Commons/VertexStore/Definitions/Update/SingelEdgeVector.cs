using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The definition of the direction of a single edge
    /// </summary>
    public sealed class SingelEdgeVector
    {
        #region data

        /// <summary>
        /// The vertex where the edge begins
        /// </summary>
        public readonly VertexInformation FromVertex;
        
        /// <summary>
        /// The vertex where the edge ends
        /// </summary>
        public readonly VertexInformation ToVertex;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new singleEdgeVector
        /// </summary>
        /// <param name="myFromVertex">The vertex where the edge begins</param>
        /// <param name="myToVertex">The vertex where the edge ends</param>
        public SingelEdgeVector(VertexInformation myFromVertex, VertexInformation myToVertex)
        {
            FromVertex = myFromVertex;
            ToVertex = myToVertex;
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
            if (obj is SingelEdgeVector)
            {
                return Equals((SingelEdgeVector)obj);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(SingelEdgeVector p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return (this.FromVertex == p.FromVertex) && (this.ToVertex == p.ToVertex);
        }

        public static Boolean operator ==(SingelEdgeVector a, SingelEdgeVector b)
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

        public static Boolean operator !=(SingelEdgeVector a, SingelEdgeVector b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return FromVertex.GetHashCode() ^ ToVertex.GetHashCode();
        }

        #endregion

    }
}

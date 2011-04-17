using System;
using System.Collections.Generic;
using System.IO;

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// Static filter class
    /// </summary>
    public static class PropertyHyperGraphFilter
    {
        #region IVertex

        /// <summary>
        /// Filters the incoming edges of a vertex
        /// </summary>
        /// <param name="myIncomingVertexType">The id of the incoming vertex type</param>
        /// <param name="myIncomingEdgePropertyID">The id of the incoming edge property</param>
        /// <param name="myIncomingSingleEdges">The incoming single edges</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool IncomingEdgeFilter(Int64 myIncomingVertexType, Int64 myIncomingEdgePropertyID, IEnumerable<ISingleEdge> myIncomingSingleEdges);

        /// <summary>
        /// Filters all outgoing edges of a vertex
        /// </summary>
        /// <param name="myEdgePropertyID">The edge property id</param>
        /// <param name="myOutgoingEdge">The outgoing edge</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool OutgoingEdgeFilter(Int64 myEdgePropertyID, IEdge myOutgoingEdge);
        
        /// <summary>
        /// Filters all outgoing hyper edges of a vertex
        /// </summary>
        /// <param name="myEdgePropertyID">The edge property id</param>
        /// <param name="myOutgoingEdge">The outgoing hyper edge</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool OutgoingHyperEdgeFilter(Int64 myEdgePropertyID, IHyperEdge myOutgoingEdge);

        /// <summary>
        /// Filters all outgoing single edges
        /// </summary>
        /// <param name="myEdgePropertyID">The edge property id</param>
        /// <param name="myOutgoingEdge">The outgoing single edge</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool OutgoingSingleEdgeFilter(Int64 myEdgePropertyID, ISingleEdge myOutgoingEdge);

        /// <summary>
        /// Filters all binary properties of a vertex
        /// </summary>
        /// <param name="myBinaryPropertyID">The binary property id</param>
        /// <param name="myBinaryStream">The stream</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool BinaryPropertyFilter(Int64 myBinaryPropertyID, Stream myBinaryStream);

        #endregion

        #region IGraphElement

        /// <summary>
        /// Filters a graph element property
        /// </summary>
        /// <param name="myStructuredPropertyID">The id of the property</param>
        /// <param name="myProperty">The property</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool GraphElementStructuredPropertyFilter(Int64 myStructuredPropertyID, IComparable myProperty);

        /// <summary>
        /// Filters an unstructured graph element property
        /// </summary>
        /// <param name="myUnstructuredPropertyName">The name of the unstructured property</param>
        /// <param name="myProperty">The property</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool GraphElementUnStructuredPropertyFilter(String myUnstructuredPropertyName, Object myProperty);

        #endregion

        #region IHyperEdge

        /// <summary>
        /// A filter for a single edge of a hyperedge
        /// </summary>
        /// <param name="mySingleEdge">The single edge</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool SingleEdgeFilter(ISingleEdge mySingleEdge);

        #endregion

        #region IEdge

        /// <summary>
        /// Filter a target vertex
        /// </summary>
        /// <param name="myVertex">The vertex</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool TargetVertexFilter(IVertex myVertex);

        #endregion
    }
}

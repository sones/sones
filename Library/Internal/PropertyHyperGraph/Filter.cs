using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// Static filter class
    /// </summary>
    public static class Filter
    {
        /// <summary>
        /// Filters the incoming edges of a vertex
        /// </summary>
        /// <param name="myIncomingVertexType">The id of the incoming vertex type</param>
        /// <param name="myIncomingEdgePropertyID">The id of the incoming edge property</param>
        /// <param name="myIncomingSingleEdges">The incoming single edges</param>
        /// <returns>True or false</returns>
        public delegate bool IncomingEdgeFilter(Int64 myIncomingVertexType, Int64 myIncomingEdgePropertyID, IEnumerable<ISingleEdge> myIncomingSingleEdges);

        /// <summary>
        /// Filters all outgoing edges of a vertex
        /// </summary>
        /// <param name="myEdgePropertyID">The edge property id</param>
        /// <param name="myOutgoingEdge">The outgoing edge</param>
        /// <returns>True or false</returns>
        public delegate bool OutgoingEdgeFilter(Int64 myEdgePropertyID, IEdge myOutgoingEdge);
        
        /// <summary>
        /// Filters all outgoing hyper edges of a vertex
        /// </summary>
        /// <param name="myEdgePropertyID">The edge property id</param>
        /// <param name="myOutgoingEdge">The outgoing hyper edge</param>
        /// <returns>True or false</returns>
        public delegate bool OutgoingHyperEdgeFilter(Int64 myEdgePropertyID, IHyperEdge myOutgoingEdge);

        /// <summary>
        /// Filters all outgoing single edges
        /// </summary>
        /// <param name="myEdgePropertyID">The edge property id</param>
        /// <param name="myOutgoingEdge">The outgoing single edge</param>
        /// <returns>True or false</returns>
        public delegate bool OutgoingSingleEdgeFilter(Int64 myEdgePropertyID, ISingleEdge myOutgoingEdge);

        /// <summary>
        /// Filters all binary properties of a vertex
        /// </summary>
        /// <param name="myBinaryPropertyID">The binary property id</param>
        /// <param name="myBinaryStream">The stream</param>
        /// <returns>True or false</returns>
        public delegate bool BinaryPropertyFilter(Int64 myBinaryPropertyID, Stream myBinaryStream);
    }
}

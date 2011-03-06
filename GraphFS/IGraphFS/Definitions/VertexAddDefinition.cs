using System;
using System.Collections.Generic;
using System.IO;

namespace sones.GraphFS.Definitions
{
    /// <summary>
    /// This struct represents the filesystem definition for a vertex
    /// </summary>
    public struct VertexAddDefinition
    {
        #region data

        /// <summary>
        /// The binary properties
        /// </summary>
        public readonly Dictionary<Int64, Stream> BinaryProperties;

        /// <summary>
        /// The edition of the vertex
        /// </summary>
        public readonly string Edition;

        /// <summary>
        /// The graph element properties
        /// </summary>
        public readonly GraphElementInformation GraphElementInformation;

        /// <summary>
        /// The definition of the outgoing hyper edges
        /// </summary>
        public readonly Dictionary<Int64, HyperEdgeAddDefinition> OutgoingHyperEdges;

        /// <summary>
        /// The definition of the outgoing hyper edges
        /// </summary>
        public readonly Dictionary<Int64, SingleEdgeAddDefinition> OutgoingSingleEdges;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new vertex add definition
        /// </summary>
        /// <param name="myGraphElementInformation">The graph element properties</param>
        /// <param name="myEdition">The edition of the new vertex</param>
        /// <param name="myOutgoingHyperEdges">The outgoing hyper edge definitions</param>
        /// <param name="myOutgoingSingleEdges">The outgoing single edge definitions</param>
        /// <param name="myBinaryProperties">The binary properties of the new vertex</param>
        public VertexAddDefinition(
            GraphElementInformation myGraphElementInformation,
            String myEdition,
            Dictionary<Int64, HyperEdgeAddDefinition> myOutgoingHyperEdges,
            Dictionary<Int64, SingleEdgeAddDefinition> myOutgoingSingleEdges,
            Dictionary<Int64, Stream> myBinaryProperties)
        {
            Edition = myEdition;
            OutgoingHyperEdges = myOutgoingHyperEdges;
            OutgoingSingleEdges = myOutgoingSingleEdges;
            BinaryProperties = myBinaryProperties;
            GraphElementInformation = myGraphElementInformation;
        }

        #endregion
    }
}
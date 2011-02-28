using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.PropertyHyperGraph;
using System.IO;

namespace sones.GraphFS
{
    /// <summary>
    /// This struct represents the filesystem definition for a vertex
    /// </summary>
    public struct VertexAddDefinition
    {
        #region data

        /// <summary>
        /// The edition of the vertex
        /// </summary>
        public readonly string Edition;

        /// <summary>
        /// The definition of the outgoing hyper edges
        /// </summary>
        public readonly Dictionary<UInt64, HyperEdgeAddDefinition> OutgoingHyperEdges;

        /// <summary>
        /// The definition of the outgoing hyper edges
        /// </summary>
        public readonly Dictionary<UInt64, SingleEdgeAddDefinition> OutgoingSingleEdges;

        /// <summary>
        /// The binary properties
        /// </summary>
        public readonly Dictionary<UInt64, Stream> BinaryProperties;

        /// <summary>
        /// The graph element properties
        /// </summary>
        public readonly GraphElementInformation GraphElementInformation;

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
            Dictionary<UInt64, HyperEdgeAddDefinition> myOutgoingHyperEdges,
            Dictionary<UInt64, SingleEdgeAddDefinition> myOutgoingSingleEdges,
            Dictionary<UInt64, Stream> myBinaryProperties)
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

using System;

namespace sones.GraphDB.Request.CreateVertexTypes
{
    public class IncomingEdgePredefinition
    {
        public String SourceTypeName { get; private set; }
        public String SourceEdgeName { get; private set; }
        public String EdgeName { get; private set; }

        /// <summary>
        /// Creates a definition for an incoming edge
        /// </summary>
        /// <param name="myEdgeName">The name of the edge</param>
        public IncomingEdgePredefinition(String myEdgeName)
        {
            EdgeName = myEdgeName;
            SourceTypeName = String.Empty;
            SourceEdgeName = String.Empty;
        }

        /// <summary>
        /// Sets the outgoing edge, this incoming edge is the backward version for.
        /// </summary>
        /// <param name="myVertexType">A vertex type pre-definition.</param>
        /// <param name="myOutgoingEdge">An outgoing edge pre-definition.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(VertexTypePredefinition myVertexType, OutgoingEdgePredefinition myOutgoingEdge)
        {
            SourceTypeName = myVertexType.VertexTypeName;
            SourceEdgeName = myOutgoingEdge.EdgeName;
            return this;
        }

        /// <summary>
        /// Sets the outgoing edge, this incoming edge is the backward version for.
        /// </summary>
        /// <param name="myVertexType">A vertex type pre-definition.</param>
        /// <param name="myOutgoingEdgeName">The name of the edge on the vertex type.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(VertexTypePredefinition myVertexType, String myOutgoingEdgeName)
        {
            SourceTypeName = myVertexType.VertexTypeName;
            SourceEdgeName = myOutgoingEdgeName;
            return this;            
        }

        /// <summary>
        /// Sets the outgoing edge, this incoming edge is the backward version for.
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type that declares the outgoing edge.</param>
        /// <param name="myOutgoingEdgeName">The name of the edge on the vertex type.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(String myVertexTypeName, String myOutgoingEdgeName)
        {
            SourceTypeName = myVertexTypeName;
            SourceEdgeName = myOutgoingEdgeName;
            return this;
        }
    }
}

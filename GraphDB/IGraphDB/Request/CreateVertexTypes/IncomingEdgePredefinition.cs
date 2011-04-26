using System;

namespace sones.GraphDB.Request
{
    public class IncomingEdgePredefinition
    {
        public String SourceTypeName { get; private set; }
        public String SourceEdgeName { get; private set; }
        public String EdgeName { get; private set; }
        public String Comment { get; private set; }

        /// <summary>
        /// Creates a definition for an incoming IncomingEdge
        /// </summary>
        /// <param name="myEdgeName">The name of the IncomingEdge</param>
        public IncomingEdgePredefinition(String myEdgeName)
        {
            EdgeName = myEdgeName;
            SourceTypeName = String.Empty;
            SourceEdgeName = String.Empty;
        }

        /// <summary>
        /// Sets the outgoing IncomingEdge, this incoming IncomingEdge is the backward version for.
        /// </summary>
        /// <param name="myVertexType">A vertex type pre-definition.</param>
        /// <param name="myOutgoingEdge">An outgoing IncomingEdge pre-definition.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(VertexTypePredefinition myVertexType, OutgoingEdgePredefinition myOutgoingEdge)
        {
            SourceTypeName = myVertexType.VertexTypeName;
            SourceEdgeName = myOutgoingEdge.EdgeName;
            return this;
        }

        /// <summary>
        /// Sets the outgoing IncomingEdge, this incoming IncomingEdge is the backward version for.
        /// </summary>
        /// <param name="myVertexType">A vertex type pre-definition.</param>
        /// <param name="myOutgoingEdgeName">The name of the IncomingEdge on the vertex type.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(VertexTypePredefinition myVertexType, String myOutgoingEdgeName)
        {
            SourceTypeName = myVertexType.VertexTypeName;
            SourceEdgeName = myOutgoingEdgeName;
            return this;            
        }

        /// <summary>
        /// Sets the outgoing IncomingEdge, this incoming IncomingEdge is the backward version for.
        /// </summary>
        /// <param name="VertexTypeName">The name of the vertex type that declares the outgoing IncomingEdge.</param>
        /// <param name="myOutgoingEdgeName">The name of the IncomingEdge on the vertex type.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(String myVertexTypeName, String myOutgoingEdgeName)
        {
            SourceTypeName = myVertexTypeName;
            SourceEdgeName = myOutgoingEdgeName;
            return this;
        }

    }
}

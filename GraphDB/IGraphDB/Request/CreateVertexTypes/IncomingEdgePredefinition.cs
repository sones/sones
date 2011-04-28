using System;

namespace sones.GraphDB.Request
{
    public class IncomingEdgePredefinition: AttributePredefinition
    {

        /// <summary>
        /// Creates a definition for an incoming edge.
        /// </summary>
        /// <param name="myEdgeName">The name of the IncomingEdge</param>
        public IncomingEdgePredefinition(String myEdgeName): base(myEdgeName)
        {
        }

        /// <summary>
        /// Sets the outgoing edge, this incoming edge is the backward version for.
        /// </summary>
        /// <param name="myVertexType">A vertex type pre-definition.</param>
        /// <param name="myOutgoingEdge">An outgoing IncomingEdge pre-definition.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(VertexTypePredefinition myVertexType, OutgoingEdgePredefinition myOutgoingEdge)
        {
            AttributeType = Combine(myVertexType.VertexTypeName, myOutgoingEdge.AttributeName);

            return this;
        }

        /// <summary>
        /// Sets the outgoing edge, this incoming edge is the backward version for.
        /// </summary>
        /// <param name="myVertexType">A vertex type pre-definition.</param>
        /// <param name="myOutgoingEdgeName">The name of the IncomingEdge on the vertex type.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(VertexTypePredefinition myVertexType, String myOutgoingEdgeName)
        {
            AttributeType = Combine(myVertexType.VertexTypeName, myOutgoingEdgeName);

            return this;            
        }

        /// <summary>
        /// Sets the outgoing edge, this incoming edge is the backward version for.
        /// </summary>
        /// <param name="VertexTypeName">The name of the vertex type that declares the outgoing IncomingEdge.</param>
        /// <param name="myOutgoingEdgeName">The name of the IncomingEdge on the vertex type.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(String myVertexTypeName, String myOutgoingEdgeName)
        {
            AttributeType = Combine(myVertexTypeName, myOutgoingEdgeName);

            return this;
        }

        public IncomingEdgePredefinition SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }

        public IncomingEdgePredefinition SetAttributeType(String myTypeName)
        {
            AttributeType = myTypeName;

            return this;
        }

        private string Combine(string myTargetType, string myTargetEdgeName)
        {
            return String.Join(".", myTargetType, myTargetEdgeName);
        }


    }
}

using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition for an outgoing edge.
    /// </summary>
    public sealed class OutgoingEdgePredefinition
    {

        #region Constant

        /// <summary>
        /// The name of the predefined edge type that represents a single edge with no attributes.
        /// </summary>
        public const string SingleEdge = "Edge";

        /// <summary>
        /// The name of the predefined edge type that represents multiple edges with no attributes.
        /// </summary>
        public const string HyperEdge = "Set";

        /// <summary>
        /// The name of the predefined edge type that represents a multiple edges with an attribute Weight of type double.
        /// </summary>
        public const string WeightedEdge = "Weighted";
        
        #endregion

        #region Data

        /// <summary>
        /// The name of the edge
        /// </summary>
        public String EdgeName { get; private set; }

        /// <summary>
        /// The edge type of this edge definition
        /// </summary>
        public String EdgeType { get; private set; }

        /// <summary>
        /// The vertex type the edge will direct to
        /// </summary>
        public String TargetVertexType { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a definition for an outgoing edge
        /// </summary>
        /// <param name="myEdgeName">The name of the edge</param>
        public OutgoingEdgePredefinition(String myEdgeName)
        {
            EdgeName = myEdgeName;
            EdgeType = SingleEdge;
        }

        /// <summary>
        /// Sets the edge type of this edge definition.
        /// </summary>
        /// <param name="myEdgeType">
        /// The name of the edge type. 
        /// </param>
        /// <seealso cref="SetAsSingleEdge"/>
        /// <seealso cref="SetAsHyperEdge"/>
        /// <seealso cref="SetAsWeightedEdge"/>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public OutgoingEdgePredefinition SetEdgeType(String myEdgeType)
        {
            EdgeType = myEdgeType;
            return this;
        }

        /// <summary>
        /// Sets the edge type of this edge definition to 'Edge'.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public OutgoingEdgePredefinition SetAsSingleEdge()
        {
            EdgeType = SingleEdge;
            return this;
        }

        /// <summary>
        /// Sets the edge type of this edge definition to 'Set'.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public OutgoingEdgePredefinition SetAsHyperEdge()
        {
            EdgeType = HyperEdge;
            return this;
        }

        /// <summary>
        /// Sets the edge type of this edge definition to 'Weighted'.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public OutgoingEdgePredefinition SetAsWeightedEdge()
        {
            EdgeType = WeightedEdge;
            return this;
        }

        /// <summary>
        /// Sets the target vertex type.
        /// </summary>
        /// <param name="myTargetVertexTypeName">The name of the vertex type, this edge type will point to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public OutgoingEdgePredefinition SetTargetVertexType(String myTargetVertexTypeName)
        {
            TargetVertexType = myTargetVertexTypeName;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myTargetVertexType"></param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public OutgoingEdgePredefinition SetTargetVertexType(VertexTypePredefinition myTargetVertexType)
        {
            TargetVertexType = myTargetVertexType.VertexTypeName;
            return this;
        }


        #endregion
    }
}
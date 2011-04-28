using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown, if an incoming edge is set to an nonexisting outgoing edge.
    /// </summary>
    public class OutgoingEdgeNotFoundException: AGraphDBVertexAttributeException
    {
        /// <summary>
        /// The predefinition, that contains the incoming edge.
        /// </summary>
        public VertexTypePredefinition Predefinition { get; private set; }

        /// <summary>
        /// The incoming edge that causes the exception.
        /// </summary>
        public IncomingEdgePredefinition IncomingEdge { get; private set; }

        /// <summary>
        /// Creates an instance of OutgoingEdgeNotFoundException.
        /// </summary>
        /// <param name="myPredefinition">
        /// The predefinition, that contains the incoming edge.
        /// </param>
        /// <param name="myIncomingEdge">
        /// The incoming edge that causes the exception.
        /// </param>
        public OutgoingEdgeNotFoundException(VertexTypePredefinition myPredefinition, IncomingEdgePredefinition myIncomingEdge)
        {
            Predefinition = myPredefinition;
            IncomingEdge = myIncomingEdge;
            _msg = string.Format("Vertextype {0} defines an incoming edge on a nonexisting outgoing edge ({1}).", myPredefinition.VertexTypeName, myIncomingEdge.AttributeType);
        }
    }
}

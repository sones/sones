using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    public class OutgoingEdgeNotFoundException: AGraphDBVertexAttributeException
    {
        private VertexTypePredefinition Predefinition;
        private IncomingEdgePredefinition IncomingEdge;

        public OutgoingEdgeNotFoundException(VertexTypePredefinition myPredefinition, IncomingEdgePredefinition myIncomingEdge)
        {
            this.Predefinition = myPredefinition;
            this.IncomingEdge = myIncomingEdge;
            _msg = string.Format("Vertextype {0} defines an incoming edge on a nonexisting outgoing edge ({1}.{2}).", myPredefinition.VertexTypeName, myIncomingEdge.SourceTypeName, myIncomingEdge.SourceEdgeName);
        }
    }
}

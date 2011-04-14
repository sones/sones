using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a vertex type with an outgoing IncomingEdge with an empty IncomingEdge type should be added.
    /// </summary>
    public sealed class EmptyEdgeTypeException: AGraphDBVertexAttributeException
    {
        /// <summary>
        /// Creates an instance of EmptyEdgeTypeException.
        /// </summary>
        /// <param name="myPredefinition">The predefinition that causes the exception.</param>
        public EmptyEdgeTypeException(VertexTypePredefinition myPredefinition, String myOutgoingEdgeName)
        {
            Predefinition = myPredefinition;
            PropertyName = myOutgoingEdgeName;
            _msg = string.Format("The outgoing edge {0} on vertex type {1} is empty.",myOutgoingEdgeName, myPredefinition.VertexTypeName);
        }

        /// <summary>
        /// The predefinition that causes the exception.
        /// </summary>
        public VertexTypePredefinition Predefinition { get; private set; }

        /// <summary>
        /// The outgoing IncomingEdge that causes the exception.
        /// </summary>
        public string PropertyName { get; private set; }
    }
}

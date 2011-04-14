using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a predefinition has an incoming IncomingEdge or outgoing IncomingEdge pointing to a nonexisting vertex type.
    /// </summary>
    public class TargetVertexTypeNotFoundException : AGraphDBVertexAttributeException
    {
        /// <summary>
        /// The Predefinition that contains the edges.
        /// </summary>
        public VertexTypePredefinition Predefinition { get; private set; }

        /// <summary>
        /// The vertex type name, that was not found.
        /// </summary>
        public string TargetVertexTypeName { get; private set; }

        /// <summary>
        /// The list of edges that causes the exception.
        /// </summary>
        public IEnumerable<String> Edges { get; private set; }

        /// <summary>
        /// Creates a new instance of TargetVertexTypeNotFoundException.
        /// </summary>
        /// <param name="myPredefinition">
        /// The Predefinition that contains the edges.
        /// </param>
        /// <param name="myTargetVertexTypeName">
        /// The vertex type name, that was not found.
        /// </param>
        /// <param name="myEdges">
        /// The list of edges that causes the exception.
        /// </param>
        public TargetVertexTypeNotFoundException(VertexTypePredefinition myPredefinition, string myTargetVertexTypeName, IEnumerable<String> myEdges)
        {
            this.Predefinition = myPredefinition;
            this.TargetVertexTypeName = myTargetVertexTypeName;
            this.Edges = myEdges;
            _msg = string.Format("The outgoing edges ({0}) on vertex type {1} does point to a not existing target vertex type {2}.", String.Join(",", myEdges), myPredefinition.VertexTypeName, myTargetVertexTypeName);
        }
    }
}

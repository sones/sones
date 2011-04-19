using System;
using System.Collections.Generic;
using System.IO;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphQL.Result
{
    /// <summary>
    /// The vertex view interface
    /// </summary>
    public interface IVertexView : IGraphElementView
    {
        #region Edges

        /// <summary>
        /// Is there a specified IncomingEdge?
        /// </summary>
        /// <param name="myEdgePropertyName">The property name of the interesting IncomingEdge</param>
        /// <returns>True if there is a specified IncomingEdge, otherwise false</returns>
        Boolean HasEdge(String myEdgePropertyName);

        /// <summary>
        /// Returns all edges
        /// </summary>
        /// <returns>An IEnumerable of all edges</returns>
        IEnumerable<Tuple<String, IEdgeView>> GetAllEdges();

        /// <summary>
        /// Returns all hyper edges
        /// </summary>
        /// <returns>An IEnumerable of propertyName/hyper IncomingEdge KVP</returns>
        IEnumerable<Tuple<String, IHyperEdgeView>> GetAllHyperEdges();

        /// <summary>
        /// Returns all single edges
        /// </summary>
        /// <returns>An IEnumerable of all single edges</returns>
        IEnumerable<Tuple<String, ISingleEdgeView>> GetAllSingleEdges();

        /// <summary>
        /// Returns a specified edge
        /// </summary>
        /// <param name="myEdgePropertyName">The property name of the specified edge</param>
        /// <returns>An IEdgeView</returns>
        IEdgeView GetEdge(String myEdgePropertyName);

        /// <summary>
        /// Returns a specified hyper edge
        /// </summary>
        /// <param name="myEdgePropertyName">The property name of the specified edge</param>
        /// <returns>A hyper edge view</returns>
        IHyperEdgeView GetHyperEdge(String myEdgePropertyName);

        /// <summary>
        /// Get a specified single edge
        /// </summary>
        /// <param name="myEdgePropertyName">The property name of the specified edge</param>
        /// <returns>A single edge view</returns>
        ISingleEdgeView GetSingleEdge(String myEdgePropertyName);

        /// <summary>
        /// Returns all target vertices of an edge.
        /// </summary>
        /// <param name="myEdgePropertyName">The name of the edge property.</param>
        /// <returns>All target vertices.</returns>
        IEnumerable<IVertexView> GetAllNeighbours(String myEdgePropertyName);
        

        #endregion

        #region Binary data

        /// <summary>
        /// Returns a specified binary property
        /// </summary>
        /// <param name="myPropertyName">The property name of the specified binary</param>
        /// <returns>A stream</returns>
        Stream GetBinaryProperty(String myPropertyName);

        /// <summary>
        /// Returns all binary properties
        /// </summary>
        /// <returns>An IEnumerable of PropertyName/stream KVP</returns>
        IEnumerable<Tuple<String, Stream>> GetAllBinaryProperties();

        #endregion
    }
}

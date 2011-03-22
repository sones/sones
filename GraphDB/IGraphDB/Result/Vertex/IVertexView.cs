using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;
using System.IO;

namespace sones.GraphDB.Result
{
    /// <summary>
    /// The vertex view interface
    /// </summary>
    public interface IVertexView : IGraphElementView
    {
        #region ID / Edition / Revision

        /// <summary>
        /// The name of the vertex type
        /// </summary>
        String VertexTypeName { get; }

        /// <summary>
        /// The id of the vertex
        /// </summary>
        Int64 VertexID { get; }

        /// <summary>
        /// Returns the revision id of this vertex
        /// </summary>
        VertexRevisionID VertexRevisionID { get; }

        /// <summary>
        /// Returns the name of the edition of this vertex
        /// </summary>
        String EditionName { get; }

        #endregion

        #region Edges

        /// <summary>
        /// Is there a specified edge?
        /// </summary>
        /// <param name="myEdgePropertyName">The property name of the interesting edge</param>
        /// <returns>True if there is a specified edge, otherwise false</returns>
        Boolean HasEdge(String myEdgePropertyName);

        /// <summary>
        /// Returns all edges
        /// </summary>
        /// <returns>An IEnumerable of all edges</returns>
        IEnumerable<Tuple<String, IEdgeView>> GetAllEdges();

        /// <summary>
        /// Returns all hyper edges
        /// </summary>
        /// <returns>An IEnumerable of propertyName/hyper edge KVP</returns>
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
        IEdgeView GetOutgoingEdge(String myEdgePropertyName);

        /// <summary>
        /// Returns a specified hyper edge
        /// </summary>
        /// <param name="myEdgePropertyName">The property name of the specified edge</param>
        /// <returns>A hyper edge view</returns>
        IHyperEdgeView GetOutgoingHyperEdge(String myEdgePropertyName);

        /// <summary>
        /// Get a specified single edge
        /// </summary>
        /// <param name="myEdgePropertyName">The property name of the specified edge</param>
        /// <returns>A single edge</returns>
        ISingleEdgeView GetOutgoingSingleEdge(String myEdgePropertyName);

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

using System;
using System.IO;
using System.Collections.Generic;


namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for insterting a new vertex.
    /// </summary>
    public sealed class RequestInsertVertex : IRequest
    {
        #region data

        /// <summary>
        /// The name of the vertex type that is going to be inserted.
        /// </summary>
        public readonly String VertexTypeName;

        /// <summary>
        /// The comment for the new vertex.
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// The edition of this vertex.
        /// </summary>
        public string Edition { get; private set; }

        //TODO: make dictionaries readonly
        /// <summary>
        /// The well defined properties of a vertex.
        /// </summary>
        public IDictionary<String, IComparable> StructuredProperties { get { return _structured; } }
        private Dictionary<string, IComparable> _structured;

        /// <summary>
        /// The unstructured part of a vertex.
        /// </summary>
        public IDictionary<String, Object> UnstructuredProperties { get { return _unstructured; } }
        private Dictionary<string, object> _unstructured;

        /// <summary>
        /// The binaries of a vertex.
        /// </summary>
        public IDictionary<String, Stream> BinaryProperties { get { return _binaries; } }
        private Dictionary<string, Stream> _binaries;

        /// <summary>
        /// The outgoing edges of a vertex.
        /// </summary>
        public IEnumerable<EdgeDefinition> OutgoingEdges { get { return _edges; } }
        private HashSet<EdgeDefinition> _edges;

        #endregion

        #region Fluent interface

        /// <summary>
        /// Sets the comment for this vertex.
        /// </summary>
        /// <param name="myComment">The comment for this vertex.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestInsertVertex SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }

        /// <summary>
        /// Sets the edition for this vertex.
        /// </summary>
        /// <param name="myEdition">The edtion for this vertex.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestInsertVertex SetEdition(String myEdition)
        {
            Edition = myEdition;

            return this;
        }

        /// <summary>
        /// Adds a new structured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestInsertVertex AddStructuredProperty(String myPropertyName, IComparable myProperty)
        {
            _structured = _structured ?? new Dictionary<String, IComparable>();
            _structured.Add(myPropertyName, myProperty);

            return this;
        }

        /// <summary>
        /// Adds a new unstructured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestInsertVertex AddUnstructuredProperty(String myPropertyName, Object myProperty)
        {
            _unstructured = _unstructured ?? new Dictionary<String, Object>();
            _unstructured.Add(myPropertyName, myProperty);

            return this;
        }

        /// <summary>
        /// Adds a new binary property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myStream">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestInsertVertex AddBinaryProperty(String myPropertyName, Stream myStream)
        {
            _binaries = _binaries ?? new Dictionary<String, Stream>();
            _binaries.Add(myPropertyName, myStream);

            return this;
        }

        /// <summary>
        /// Adds a new edge to the vertex defintion
        /// </summary>
        /// <param name="myEdgeName">The name of the edge to be inserted</param>
        /// <param name="myEdgeDefinition">The definition of the edge</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestInsertVertex AddEdge(EdgeDefinition myEdgeDefinition)
        {
            _edges = _edges ?? new HashSet<EdgeDefinition>();
            _edges.Add(myEdgeDefinition);

            return this;
        }

        /// <summary>
        /// Adds new edges to the vertex defintion.
        /// </summary>
        /// <param name="myEdgeName">The name of the edge to be inserted.</param>
        /// <param name="myEdgeDefinitions">The definitions of the edge.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestInsertVertex AddEdges(String myEdgeName, IEnumerable<EdgeDefinition> myEdgeDefinitions)
        {
            _edges = _edges ?? new HashSet<EdgeDefinition>();
            _edges.UnionWith(myEdgeDefinitions);

            return this;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that inserts a vertex
        /// </summary>
        /// <param name="VertexTypeName">The name of the vertex type.</param>
        public RequestInsertVertex(String myVertexTypeName)
        {
            VertexTypeName = myVertexTypeName;
        }

        #endregion

        #region IRequest Members

        GraphDBAccessMode IRequest.AccessMode
        {
            get { return GraphDBAccessMode.ReadWrite; }
        }

        #endregion
    }
}
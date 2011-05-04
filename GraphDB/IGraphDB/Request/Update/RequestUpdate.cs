using System;
using System.Collections.Generic;
using System.IO;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// Request to update a vertex / vertex type
    /// </summary>
    public sealed class RequestUpdate : IRequest
    {

        #region data

        /// <summary>
        /// A GetVertices request to get the vertices to be updated
        /// </summary>
        public readonly RequestGetVertices GetVerticesRequest;

        /// <summary>
        /// The comment for the updated vertex.
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// The edition of updated vertex.
        /// </summary>
        public string Edition { get; private set; }

        /// <summary>
        /// The UUID of updated vertex.
        /// </summary>
        public long? VertexUUID { get; private set; }

        /// <summary>
        /// The well defined properties of updated vertex.
        /// </summary>
        public IDictionary<String, IComparable> UpdateStructuredProperties { get { return _toBeUpdatedStructured; } }
        private Dictionary<string, IComparable> _toBeUpdatedStructured;

        /// <summary>
        /// The unstructured part of updated vertex.
        /// </summary>
        public IDictionary<String, Object> UpdateUnstructuredProperties { get { return _toBeUpdatedUnstructured; } }
        private Dictionary<string, object> _toBeUpdatedUnstructured;

        /// <summary>
        /// The binaries of updated vertex.
        /// </summary>
        public IDictionary<String, Stream> UpdateBinaryProperties { get { return _toBeUpdatedBinaries; } }
        private Dictionary<string, Stream> _toBeUpdatedBinaries;

        /// <summary>
        /// The outgoing edges of updated vertex.
        /// </summary>
        public IEnumerable<EdgePredefinition> UpdateOutgoingEdges { get { return _toBeUpdatedEdges; } }
        private HashSet<EdgePredefinition> _toBeUpdatedEdges;

        /// <summary>
        /// The unknwon properties of updated.
        /// </summary>
        public IDictionary<string, object> UpdateUnknownProperties { get { return _toBeUpdatedUnknown; } }
        private IDictionary<string, object> _toBeUpdatedUnknown;

        /// <summary>
        /// The well defined properties of updated vertex.
        /// </summary>
        public IDictionary<String, IComparable> RemoveValuedStructuredProperties { get { return _toBeRemovedValuedStructured; } }
        private Dictionary<string, IComparable> _toBeRemovedValuedStructured;

        /// <summary>
        /// The well defined properties which should be removed of updated vertex.
        /// </summary>
        public List<String> RemoveStructuredProperties { get { return _toBeRemovedStructured; } }
        private List<string> _toBeRemovedStructured;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new update request to get the vertices which should be updated
        /// </summary>
        /// <param name="myGetVerticesRequest">A request to get specific vertices.</param>
        public RequestUpdate(RequestGetVertices myGetVerticesRequest)
        {
            GetVerticesRequest = myGetVerticesRequest;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.TypeChange; }
        }

        #endregion

        #region fluent interface

        /// <summary>
        /// Sets the comment for updated vertex.
        /// </summary>
        /// <param name="myComment">The comment for updated vertex.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }

        /// <summary>
        /// Sets the edition for updated vertex.
        /// </summary>
        /// <param name="myEdition">The edtion for updated vertex.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate SetEdition(String myEdition)
        {
            Edition = myEdition;

            return this;
        }


        /// <summary>
        /// Sets the UUID of the updated vertex. If this is not done, an ID is creted by the system.
        /// </summary>
        /// <param name="myID">The ID of the updated vertex.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate SetUUID(long myID)
        {
            VertexUUID = myID;

            return this;

        }

        /// <summary>
        /// Adds a new structured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate UpdateStructuredProperty(String myPropertyName, IComparable myProperty)
        {
            _toBeUpdatedStructured = _toBeUpdatedStructured ?? new Dictionary<String, IComparable>();
            _toBeUpdatedStructured.Add(myPropertyName, myProperty);

            return this;
        }

        /// <summary>
        /// Adds a new unstructured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate UpdateUnstructuredProperty(String myPropertyName, Object myProperty)
        {
            _toBeUpdatedUnstructured = _toBeUpdatedUnstructured ?? new Dictionary<String, Object>();
            _toBeUpdatedUnstructured.Add(myPropertyName, myProperty);

            return this;
        }

        /// <summary>
        /// Adds a new unstructured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate UpdateUnknownProperty(String myPropertyName, Object myProperty)
        {
            _toBeUpdatedUnknown = _toBeUpdatedUnknown ?? new Dictionary<String, Object>();
            _toBeUpdatedUnknown.Add(myPropertyName, myProperty);

            return this;
        }
        /// <summary>
        /// Adds a new binary property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myStream">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate UpdateBinaryProperty(String myPropertyName, Stream myStream)
        {
            _toBeUpdatedBinaries = _toBeUpdatedBinaries ?? new Dictionary<String, Stream>();
            _toBeUpdatedBinaries.Add(myPropertyName, myStream);

            return this;
        }

        /// <summary>
        /// Adds a new edge to the vertex defintion
        /// </summary>
        /// <param name="myEdgeName">The name of the edge to be inserted</param>
        /// <param name="myEdgeDefinition">The definition of the edge</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate UpdateEdge(EdgePredefinition myEdgeDefinition)
        {
            _toBeUpdatedEdges = _toBeUpdatedEdges ?? new HashSet<EdgePredefinition>();
            _toBeUpdatedEdges.Add(myEdgeDefinition);

            return this;
        }

        /// <summary>
        /// Adds new edges to the vertex defintion.
        /// </summary>
        /// <param name="myEdgeName">The name of the edge to be inserted.</param>
        /// <param name="myEdgeDefinitions">The definitions of the edge.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate UpdateEdges(String myEdgeName, IEnumerable<EdgePredefinition> myEdgeDefinitions)
        {
            _toBeUpdatedEdges = _toBeUpdatedEdges ?? new HashSet<EdgePredefinition>();
            _toBeUpdatedEdges.UnionWith(myEdgeDefinitions);

            return this;
        }

        /// <summary>
        /// Adds a new valued structured property to remove
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate RemoveValuedStructuredProperty(String myPropertyName, IComparable myProperty)
        {
            _toBeRemovedValuedStructured = _toBeRemovedValuedStructured ?? new Dictionary<String, IComparable>();
            _toBeRemovedValuedStructured.Add(myPropertyName, myProperty);

            return this;
        }

        /// <summary>
        /// Adds a new structured property to remove
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate RemoveValuedStructuredProperty(String myPropertyName)
        {
            _toBeRemovedStructured = _toBeRemovedStructured ?? new List<String>(); 
            _toBeRemovedStructured.Add(myPropertyName);

            return this;
        }
        

        #endregion
    }
}

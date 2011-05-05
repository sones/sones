using System;
using System.Collections.Generic;
using System.IO;
using sones.GraphDB.Request.Insert;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// Request to update a vertex / vertex type
    /// </summary>
    public sealed class RequestUpdate : IRequest, IPropertyProvider
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
        public IDictionary<String, IEnumerable<IComparable>> RemoveValuedStructuredProperties { get { return _toBeRemovedValuedStructured; } }
        private Dictionary<string, IEnumerable<IComparable>> _toBeRemovedValuedStructured;

        /// <summary>
        /// The well defined properties which should be removed of updated vertex.
        /// </summary>
        public List<String> RemovedAttributes { get { return _toBeRemovedAttributes; } }
        private List<string> _toBeRemovedAttributes;

        /// <summary>
        /// The outgoing edges which should be removed from updated vertex.
        /// </summary>
        public IEnumerable<String> RemoveOutgoingEdges { get { return _toBeRemovedEdges; } }
        private HashSet<String> _toBeRemovedEdges;

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
        /// Removes a valued structured property.
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate RemoveFromCollection(String myPropertyName, IEnumerable<IComparable> myProperty)
        {
            _toBeRemovedValuedStructured = _toBeRemovedValuedStructured ?? new Dictionary<String, IEnumerable<IComparable>>();
            _toBeRemovedValuedStructured.Add(myPropertyName, myProperty);

            return this;
        }

        /// <summary>
        /// Removes a structured property.
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate RemoveAttribute(String myPropertyName)
        {
            _toBeRemovedAttributes = _toBeRemovedAttributes ?? new List<String>(); 
            _toBeRemovedAttributes.Add(myPropertyName);

            return this;
        }

        /// <summary>
        /// Removes a outgoing edge.
        /// </summary>
        /// <param name="myEdgeDefinition">The name of the edge.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate RemoveEdge(String myEdgeDefinition)
        {
            _toBeRemovedEdges = _toBeRemovedEdges ?? new HashSet<String>();
            _toBeRemovedEdges.Add(myEdgeDefinition);

            return this;
        }
        

        #endregion

        #region IPropertyProvider Members

        IDictionary<string, IComparable> IPropertyProvider.StructuredProperties
        {
            get { return UpdateStructuredProperties; }
        }

        IDictionary<string, object> IPropertyProvider.UnstructuredProperties
        {
            get { return UpdateUnstructuredProperties; }
        }

        IDictionary<string, object> IPropertyProvider.UnknownProperties
        {
            get { return UpdateUnknownProperties; }
        }

        IPropertyProvider IPropertyProvider.AddStructuredProperty(string myPropertyName, IComparable myProperty)
        {
            return UpdateStructuredProperty(myPropertyName, myProperty);
        }

        IPropertyProvider IPropertyProvider.AddUnstructuredProperty(string myPropertyName, object myProperty)
        {
            return UpdateUnstructuredProperty(myPropertyName, myProperty);
        }

        IPropertyProvider IPropertyProvider.AddUnknownProperty(string myPropertyName, object myProperty)
        {
            return UpdateUnknownProperty(myPropertyName, myProperty);
        }

        #endregion

        #region IUnknownProvider Members

        void IUnknownProvider.ClearUnknown()
        {
            _toBeUpdatedUnknown = null;
        }

        #endregion
    }
}

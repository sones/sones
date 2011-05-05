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
        public string UpdatedComment { get; private set; }

        /// <summary>
        /// The edition of updated vertex.
        /// </summary>
        public string Edition { get; private set; }

        /// <summary>
        /// The well defined properties of updated vertex.
        /// </summary>
        public IDictionary<String, IComparable> AddedStructuredProperties { get; private set; }

        /// <summary>
        /// The unstructured part of updated vertex.
        /// </summary>
        public IDictionary<String, Object> AddedUnstructuredProperties { get; private set; }

        /// <summary>
        /// The binaries of updated vertex.
        /// </summary>
        public IDictionary<String, Stream> AddedBinaryProperties { get; private set; }

        /// <summary>
        /// The outgoing edges of updated vertex.
        /// </summary>
        public List<EdgePredefinition> AddedOutgoingEdges { get; private set; }

        /// <summary>
        /// The unknwon properties of updated.
        /// </summary>
        public IDictionary<string, object> AddedUnknownProperties { get; private set; }

        /// <summary>
        /// The well defined properties which should be removed of updated vertex.
        /// </summary>
        public List<String> RemovedAttributes { get; private set; }

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

        #region misc

        /// <summary>
        /// Sets the comment for updated vertex.
        /// </summary>
        /// <param name="myComment">The comment for updated vertex.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate SetComment(String myComment)
        {
            UpdatedComment = myComment;

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

        #endregion

        /// <summary>
        /// Adds a new structured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate AddStructuredProperty(String myPropertyName, IComparable myProperty)
        {
            AddedStructuredProperties = AddedStructuredProperties ?? new Dictionary<String, IComparable>();
            AddedStructuredProperties.Add(myPropertyName, myProperty);

            return this;
        }

        /// <summary>
        /// Adds a new unstructured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate AddUnstructuredProperty(String myPropertyName, Object myProperty)
        {
            AddedUnstructuredProperties = AddedUnstructuredProperties ?? new Dictionary<String, Object>();
            AddedUnstructuredProperties.Add(myPropertyName, myProperty);

            return this;
        }

        /// <summary>
        /// Adds a new unstructured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate AddUnknownProperty(String myPropertyName, Object myProperty)
        {
            AddedUnknownProperties = AddedUnknownProperties ?? new Dictionary<String, Object>();
            AddedUnknownProperties.Add(myPropertyName, myProperty);

            return this;
        }
        /// <summary>
        /// Adds a new binary property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myStream">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate AddBinaryProperty(String myPropertyName, Stream myStream)
        {
            AddedBinaryProperties = AddedBinaryProperties ?? new Dictionary<String, Stream>();
            AddedBinaryProperties.Add(myPropertyName, myStream);

            return this;
        }

        /// <summary>
        /// Adds a new edge to the vertex defintion
        /// </summary>
        /// <param name="myEdgeName">The name of the edge to be inserted</param>
        /// <param name="myEdgeDefinition">The definition of the edge</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate AddEdge(EdgePredefinition myEdgeDefinition)
        {
            AddedOutgoingEdges = AddedOutgoingEdges ?? new List<EdgePredefinition>();
            AddedOutgoingEdges.Add(myEdgeDefinition);

            return this;
        }

        /// <summary>
        /// Removes a structured property.
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate RemoveAttribute(String myPropertyName)
        {
            RemovedAttributes = RemovedAttributes ?? new List<String>();
            RemovedAttributes.Add(myPropertyName);

            return this;
        }

        #region collection handling

        #region add to collection

        /// <summary>
        /// Adds elements to a collection
        /// </summary>
        /// <param name="myAttributeName">The attribute that is going to be updated</param>
        /// <param name="myToBeAddedElements">The elements that should be added</param>
        /// <returns>The request itself</returns>
        public RequestUpdate AddElementsToCollection(String myAttributeName, IEnumerable<IComparable> myToBeAddedElements)
        {


            return this;
        }

        /// <summary>
        /// Adds elements To a collection
        /// </summary>
        /// <param name="myAttributeName">The attribute that is going to be updated</param>
        /// <param name="myToBeAddedElements">The elements that should be added</param>
        /// <returns>The request itself</returns>
        public RequestUpdate AddElementsToCollection(String myAttributeName, IEnumerable<EdgePredefinition> myToBeAddedElements)
        {


            return this;
        }

        #endregion

        #region remove from collection

        /// <summary>
        /// Removes elements from a collection
        /// </summary>
        /// <param name="myAttributeName">The attribute that is going to be updated</param>
        /// <param name="myToBeRemovedElements">The elements that should be removed</param>
        /// <returns>The request itself</returns>
        public RequestUpdate RemoveElementsFromCollection(String myAttributeName, IEnumerable<IComparable> myToBeRemovedElements)
        {


            return this;
        }

        /// <summary>
        /// Removes elements from a collection
        /// </summary>
        /// <param name="myAttributeName">The attribute that is going to be updated</param>
        /// <param name="myToBeRemovedElements">The elements that should be removed</param>
        /// <returns>The request itself</returns>
        public RequestUpdate RemoveElementsFromCollection(String myAttributeName, IEnumerable<EdgePredefinition> myToBeRemovedElements)
        {


            return this;
        }

        #endregion

        #endregion

        #endregion

        #region IPropertyProvider Members

        IDictionary<string, IComparable> IPropertyProvider.StructuredProperties
        {
            get { return AddedStructuredProperties; }
        }

        IDictionary<string, object> IPropertyProvider.UnstructuredProperties
        {
            get { return AddedUnstructuredProperties; }
        }

        IDictionary<string, object> IPropertyProvider.UnknownProperties
        {
            get { return AddedUnknownProperties; }
        }

        IPropertyProvider IPropertyProvider.AddStructuredProperty(string myPropertyName, IComparable myProperty)
        {
            return AddStructuredProperty(myPropertyName, myProperty);
        }

        IPropertyProvider IPropertyProvider.AddUnstructuredProperty(string myPropertyName, object myProperty)
        {
            return AddUnstructuredProperty(myPropertyName, myProperty);
        }

        IPropertyProvider IPropertyProvider.AddUnknownProperty(string myPropertyName, object myProperty)
        {
            return AddUnknownProperty(myPropertyName, myProperty);
        }

        #endregion

        #region IUnknownProvider Members

        void IUnknownProvider.ClearUnknown()
        {
            AddedUnknownProperties = null;
        }

        #endregion
    }
}

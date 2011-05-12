/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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

        public IDictionary<string, IEnumerable<IComparable>> AddedElementsToCollectionProperties { get; private set; }
        public IDictionary<string, IEnumerable<IComparable>> RemovedElementsFromCollectionProperties { get; private set; }
        public IDictionary<string, EdgePredefinition> AddedElementsToCollectionEdges { get; private set; }
        public IDictionary<string, EdgePredefinition> RemovedElementsFromCollectionEdges { get; private set; }

        /// <summary>
        /// The comment for the updated vertex.
        /// </summary>
        public string UpdatedComment { get; private set; }

        /// <summary>
        /// The edition of updated vertex.
        /// </summary>
        public string UpdatedEdition { get; private set; }

        /// <summary>
        /// The well defined properties of updated vertex.
        /// </summary>
        public IDictionary<String, IComparable> UpdatedStructuredProperties { get; private set; }

        /// <summary>
        /// The unstructured part of updated vertex.
        /// </summary>
        public IDictionary<String, Object> UpdatedUnstructuredProperties { get; private set; }

        /// <summary>
        /// The binaries of updated vertex.
        /// </summary>
        public IDictionary<String, Stream> UpdatedBinaryProperties { get; private set; }

        /// <summary>
        /// The outgoing edges of updated vertex.
        /// </summary>
        public List<EdgePredefinition> UpdateOutgoingEdges { get; private set; }

        /// <summary>
        /// The unknwon properties of updated.
        /// </summary>
        public IDictionary<string, object> UpdatedUnknownProperties { get; private set; }

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
        public RequestUpdate UpdateComment(String myComment)
        {
            UpdatedComment = myComment;

            return this;
        }

        /// <summary>
        /// Sets the edition for updated vertex.
        /// </summary>
        /// <param name="myEdition">The edtion for updated vertex.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate UpdateEdition(String myEdition)
        {
            UpdatedEdition = myEdition;

            return this;
        }

        #endregion

        /// <summary>
        /// Adds a new structured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestUpdate UpdateStructuredProperty(String myPropertyName, IComparable myProperty)
        {
            UpdatedStructuredProperties = UpdatedStructuredProperties ?? new Dictionary<String, IComparable>();
            UpdatedStructuredProperties.Add(myPropertyName, myProperty);

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
            UpdatedUnstructuredProperties = UpdatedUnstructuredProperties ?? new Dictionary<String, Object>();
            UpdatedUnstructuredProperties.Add(myPropertyName, myProperty);

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
            UpdatedUnknownProperties = UpdatedUnknownProperties ?? new Dictionary<String, Object>();
            UpdatedUnknownProperties.Add(myPropertyName, myProperty);

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
            UpdatedBinaryProperties = UpdatedBinaryProperties ?? new Dictionary<String, Stream>();
            UpdatedBinaryProperties.Add(myPropertyName, myStream);

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
            UpdateOutgoingEdges = UpdateOutgoingEdges ?? new List<EdgePredefinition>();
            UpdateOutgoingEdges.Add(myEdgeDefinition);

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
            AddedElementsToCollectionProperties = AddedElementsToCollectionProperties ?? new Dictionary<String, IEnumerable<IComparable>>();
            AddedElementsToCollectionProperties[myAttributeName] = myToBeAddedElements;

            return this;
        }

        /// <summary>
        /// Adds elements To a collection
        /// </summary>
        /// <param name="myAttributeName">The attribute that is going to be updated</param>
        /// <param name="myToBeAddedElements">The elements that should be added</param>
        /// <returns>The request itself</returns>
        public RequestUpdate AddElementsToCollection(String myAttributeName, EdgePredefinition myToBeAddedElements)
        {
            AddedElementsToCollectionEdges = AddedElementsToCollectionEdges ?? new Dictionary<String, EdgePredefinition>();
            AddedElementsToCollectionEdges[myAttributeName] = myToBeAddedElements;

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
            RemovedElementsFromCollectionProperties = RemovedElementsFromCollectionProperties ?? new Dictionary<String, IEnumerable<IComparable>>();
            RemovedElementsFromCollectionProperties[myAttributeName] = myToBeRemovedElements;

            return this;
        }

        /// <summary>
        /// Removes elements from a collection
        /// </summary>
        /// <param name="myAttributeName">The attribute that is going to be updated</param>
        /// <param name="myToBeRemovedElements">The elements that should be removed</param>
        /// <returns>The request itself</returns>
        public RequestUpdate RemoveElementsFromCollection(String myAttributeName, EdgePredefinition myToBeRemovedElements)
        {
            RemovedElementsFromCollectionEdges = RemovedElementsFromCollectionEdges ?? new Dictionary<String, EdgePredefinition>();
            RemovedElementsFromCollectionEdges[myAttributeName] = myToBeRemovedElements;

            return this;
        }

        #endregion

        #endregion

        #endregion

        #region IPropertyProvider Members

        IDictionary<string, IComparable> IPropertyProvider.StructuredProperties
        {
            get { return UpdatedStructuredProperties; }
        }

        IDictionary<string, object> IPropertyProvider.UnstructuredProperties
        {
            get { return UpdatedUnstructuredProperties; }
        }

        IDictionary<string, object> IPropertyProvider.UnknownProperties
        {
            get { return UpdatedUnknownProperties; }
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
            UpdatedUnknownProperties = null;
        }

        #endregion
    }
}

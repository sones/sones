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
using sones.GraphDB.TypeSystem;


namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for insterting a new vertex.
    /// </summary>
    public sealed class RequestInsertVertex : IRequest, IPropertyProvider
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

        /// <summary>
        /// The VertexID of the vertex.
        /// </summary>
        public long? VertexUUID { get; private set; }

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
        public IEnumerable<EdgePredefinition> OutgoingEdges { get { return _edges; } }
        private HashSet<EdgePredefinition> _edges;

        /// <summary>
        /// The unknwon properties.
        /// </summary>
        public IDictionary<string, object> UnknownProperties { get { return _unknown; } }
        private IDictionary<string, object> _unknown;

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
        /// Sets the VertexID of the vertex. If this is not done, an ID is creted by the system.
        /// </summary>
        /// <param name="myID">The ID of the vertex.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestInsertVertex SetUUID(long myID)
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
        /// Adds a new unstructured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestInsertVertex AddUnknownProperty(String myPropertyName, Object myProperty)
        {
            _unknown = _unknown ?? new Dictionary<String, Object>();
            _unknown.Add(myPropertyName, myProperty);

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
        public RequestInsertVertex AddEdge(EdgePredefinition myEdgeDefinition)
        {
            _edges = _edges ?? new HashSet<EdgePredefinition>();
            _edges.Add(myEdgeDefinition);

            return this;
        }

        /// <summary>
        /// Adds new edges to the vertex defintion.
        /// </summary>
        /// <param name="myEdgeName">The name of the edge to be inserted.</param>
        /// <param name="myEdgeDefinitions">The definitions of the edge.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestInsertVertex AddEdges(String myEdgeName, IEnumerable<EdgePredefinition> myEdgeDefinitions)
        {
            _edges = _edges ?? new HashSet<EdgePredefinition>();
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

            Comment = String.Empty;
        }

        #endregion

        #region IRequest Members

        GraphDBAccessMode IRequest.AccessMode
        {
            get { return GraphDBAccessMode.ReadWrite; }
        }

        #endregion


        #region IUnknownProvider Members

        #endregion

        #region IPropertyProvider Members

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
            _unknown = null;
        }

        #endregion
    }
}
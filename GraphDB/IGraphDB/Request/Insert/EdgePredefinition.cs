using System;
using sones.GraphDB.Expression;
using System.Collections.Generic;
using System.IO;
using sones.GraphDB.Request.Insert;
using System.Linq;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition of an edge.
    /// </summary>
    public sealed class EdgePredefinition: IPropertyProvider
    {
        #region data

        /// <summary>
        /// </summary>
        public String EdgeName { get; private set; }

        /// <summary>
        /// The comment for this edge definition.
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// The IDs of the vertices where to connect to.
        /// </summary>
        public IDictionary<String, ISet<long>> VertexIDsByVertexTypeName { get { return _vertexIDsByVertexTypeName; } }
        private Dictionary<String, ISet<long>> _vertexIDsByVertexTypeName;

        /// <summary>
        /// The IDs of the vertices where to connect to.
        /// </summary>
        public IDictionary<Int64, ISet<long>> VertexIDsByVertexTypeID { get { return _vertexIDsByVertexTypeID; } }
        private Dictionary<Int64, ISet<long>> _vertexIDsByVertexTypeID;

        /// <summary>
        /// The well defined properties of a vertex.
        /// </summary>
        public IDictionary<String, IComparable> StructuredProperties { get { return _structured; } }
        private Dictionary<String, IComparable> _structured;

        /// <summary>
        /// The unstructured part of a vertex.
        /// </summary>
        public IDictionary<String, Object> UnstructuredProperties { get { return _unstructured; } }
        private Dictionary<String, Object> _unstructured;

        /// <summary>
        /// The properties where the user does not know if it is structured or not.
        /// </summary>
        public IDictionary<String, Object> UnknownProperties { get { return _unknown; } }
        private Dictionary<String, Object> _unknown;

        /// <summary>
        /// The edges contained by this hyper edge.
        /// </summary>
        public IEnumerable<EdgePredefinition> ContainedEdges { get { return _edges; } }
        private List<EdgePredefinition> _edges;

        public int ContainedEdgeCount { get { return (_edges == null)? 0 :_edges.Count; } }
        #endregion

        #region constructor

        /// <summary>
        /// Creates a new instance of EdgeDefinition.
        /// </summary>
        /// <param name="myEdgeName">The name of the edge.</param>
        public EdgePredefinition(String myEdgeName)
        {
            EdgeName = myEdgeName;
        }

        /// <summary>
        /// Creates a new instance of EdgeDefinition.
        /// </summary>
        /// <remarks>Use this constructor, if a contained edge is to be created.</remarks>
        public EdgePredefinition() { }

        #endregion

        #region fluent

        /// <summary>
        /// Adds a new structured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgePredefinition AddStructuredProperty(String myPropertyName, IComparable myProperty)
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
        public EdgePredefinition AddUnstructuredProperty(String myPropertyName, Object myProperty)
        {
            _unstructured = _unstructured ?? new Dictionary<String, Object>();
            _unstructured.Add(myPropertyName, myProperty);

            return this;
        }

        /// <summary>
        /// Adds a new unknown property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgePredefinition AddUnknownProperty(String myPropertyName, Object myProperty)
        {
            _unknown = _unknown ?? new Dictionary<String, Object>();
            _unknown.Add(myPropertyName, myProperty);

            return this;
        }

        /// <summary>
        /// Adds an edge to this edge.
        /// </summary>
        /// <param name="myContainedEdge">The edges that will be contained by this hyper edge.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgePredefinition AddEdge(EdgePredefinition myContainedEdge)
        {
            _edges = _edges ?? new List<EdgePredefinition>();
            _edges.Add(myContainedEdge);

            return this;

        }

        /// <summary>
        /// Adds a vertex ID to this edge definition..
        /// </summary>
        /// <param name="myVertexID">The vertex ID where to connect to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgePredefinition AddVertexID(String myVertexType, long myVertexID)
        {
            var set = EnsureHashSet(myVertexType);
            set.Add(myVertexID);

            return this;
        }

        /// <summary>
        /// Adds a vertex ID to this edge definition..
        /// </summary>
        /// <param name="myVertexTypeID">The vertextype ID where to connect to.</param>
        /// <param name="myVertexID">The vertex ID where to connect to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgePredefinition AddVertexID(long myVertexTypeID, long myVertexID)
        {
            var set = EnsureHashSet(myVertexTypeID);
            set.Add(myVertexID);

            return this;
        }

        /// <summary>
        /// Adds verex IDs to this edge definition..
        /// </summary>
        /// <param name="myVertexIDs">The vertex IDs where to connect to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgePredefinition AddVertexID(String myVertexType, IEnumerable<long> myVertexIDs)
        {
            var set = EnsureHashSet(myVertexType);
            set.UnionWith(myVertexIDs);

            return this;
        }

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


        private ISet<long> EnsureHashSet(String myVertexType)
        {
            _vertexIDsByVertexTypeName = _vertexIDsByVertexTypeName ?? new Dictionary<String, ISet<long>>();
            if (!_vertexIDsByVertexTypeName.ContainsKey(myVertexType))
                _vertexIDsByVertexTypeName.Add(myVertexType, new HashSet<long>());

            return _vertexIDsByVertexTypeName[myVertexType];
        }

        private ISet<long> EnsureHashSet(long myVertexTypeID)
        {
            _vertexIDsByVertexTypeID = _vertexIDsByVertexTypeID ?? new Dictionary<long, ISet<long>>();
            if (!_vertexIDsByVertexTypeID.ContainsKey(myVertexTypeID))
                _vertexIDsByVertexTypeID.Add(myVertexTypeID, new HashSet<long>());

            return _vertexIDsByVertexTypeID[myVertexTypeID];
        }

    }
    
}
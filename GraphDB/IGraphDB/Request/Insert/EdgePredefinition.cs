using System;
using sones.GraphDB.Expression;
using System.Collections.Generic;
using System.IO;
using sones.GraphDB.Request.Insert;

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
        /// The expressions, that defines the vertices where to connect to.
        /// </summary>
        public IEnumerable<IExpression> Expressions { get { return _expressions; } }
        private HashSet<IExpression> _expressions;

        /// <summary>
        /// The IDs of the vertices where to connect to.
        /// </summary>
        public IEnumerable<long> VertexIDs { get { return _ids; } }
        private HashSet<long> _ids;


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
        /// Adds an expression to this edge definition.
        /// </summary>
        /// <param name="myExpression">The expression, that will be evaluated to get the vertices, where to connect to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgePredefinition AddExpression(IExpression myExpression)
        {
            _expressions = _expressions ?? new HashSet<IExpression>();
            _expressions.Add(myExpression);

            return this;
        }

        /// <summary>
        /// Adds expressions to this edge definition.
        /// </summary>
        /// <param name="myExpressions">The expressions, that will be evaluated to get the vertices, where to connect to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgePredefinition AddExpression(IEnumerable<IExpression> myExpressions)
        {
            _expressions = _expressions ?? new HashSet<IExpression>();
            _expressions.UnionWith(myExpressions);

            return this;
        }

        /// <summary>
        /// Adds a verex ID to this edge definition..
        /// </summary>
        /// <param name="myVertexID">The vertex ID where to connect to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgePredefinition AddVertexID(long myVertexID)
        {
            _ids = _ids ?? new HashSet<long>();
            _ids.Add(myVertexID);

            return this;
        }

        /// <summary>
        /// Adds verex IDs to this edge definition..
        /// </summary>
        /// <param name="myVertexIDs">The vertex IDs where to connect to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgePredefinition AddVertexID(IEnumerable<long> myVertexID)
        {
            _ids = _ids ?? new HashSet<long>();
            _ids.UnionWith(myVertexID);

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

    }
    
}
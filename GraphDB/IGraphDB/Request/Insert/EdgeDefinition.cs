using System;
using sones.GraphDB.Expression;
using System.Collections.Generic;
using System.IO;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition of an edge.
    /// </summary>
    public sealed class EdgeDefinition
    {
        #region data

        /// <summary>
        /// </summary>
        public String EdgeName { get; private set; }

        /// <summary>
        /// The expressions, that defines the vertices where to connect to.
        /// </summary>
        public IEnumerable<IExpression> Expression { get { return _expressions; } }
        private HashSet<IExpression> _expressions;

        /// <summary>
        /// The IDs of the vertices where to connect to.
        /// </summary>
        public IEnumerable<long> VertexIDs { get { return _ids; } }
        private HashSet<long> _ids;


        /// <summary>
        /// The well defined properties of a vertex.
        /// </summary>
        public IEnumerable<KeyValuePair<String, IComparable>> StructuredProperties { get { return _structured; } }
        private Dictionary<string, IComparable> _structured;

        /// <summary>
        /// The unstructured part of a vertex.
        /// </summary>
        public IEnumerable<KeyValuePair<String, Object>> UnstructuredProperties { get { return _unstructured; } }
        private Dictionary<string, object> _unstructured;

        /// <summary>
        /// The binaries of a vertex.
        /// </summary>
        public IEnumerable<KeyValuePair<String, Stream>> BinaryProperties { get { return _binaries; } }
        private Dictionary<string, Stream> _binaries;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new instance of EdgeDefinition.
        /// </summary>
        /// <param name="myEdgeName">The name of the edge.</param>
        public EdgeDefinition(String myEdgeName)
        {
            EdgeName = myEdgeName;
        }

        #endregion

        #region fluent

        /// <summary>
        /// Adds a new structured property
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myProperty">The value of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeDefinition AddStructuredProperty(String myPropertyName, IComparable myProperty)
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
        public EdgeDefinition AddUnstructuredProperty(String myPropertyName, Object myProperty)
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
        public EdgeDefinition AddBinaryProperty(String myPropertyName, Stream myStream)
        {
            _binaries = _binaries ?? new Dictionary<String, Stream>();
            _binaries.Add(myPropertyName, myStream);

            return this;
        }

        /// <summary>
        /// Adds an expression to this edge definition.
        /// </summary>
        /// <param name="myExpression">The expression, that will be evaluated to get the vertices, where to connect to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeDefinition AddExpression(IExpression myExpression)
        {
            var expressions = _expressions ?? new HashSet<IExpression>();
            expressions.Add(myExpression);

            return this;
        }

        /// <summary>
        /// Adds expressions to this edge definition.
        /// </summary>
        /// <param name="myExpressions">The expressions, that will be evaluated to get the vertices, where to connect to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeDefinition AddExpression(IEnumerable<IExpression> myExpressions)
        {
            var expressions = _expressions ?? new HashSet<IExpression>();
            expressions.UnionWith(myExpressions);

            return this;
        }

        /// <summary>
        /// Adds a verex ID to this edge definition..
        /// </summary>
        /// <param name="myVertexID">The vertex ID where to connect to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeDefinition AddVertexID(long myVertexID)
        {
            var ids = _ids ?? new HashSet<long>();
            ids.Add(myVertexID);

            return this;
        }

        /// <summary>
        /// Adds verex IDs to this edge definition..
        /// </summary>
        /// <param name="myVertexIDs">The vertex IDs where to connect to.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeDefinition AddVertexID(IEnumerable<long> myVertexID)
        {
            var ids = _ids ?? new HashSet<long>();
            ids.UnionWith(myVertexID);

            return this;
        }

        #endregion
    }
    
}
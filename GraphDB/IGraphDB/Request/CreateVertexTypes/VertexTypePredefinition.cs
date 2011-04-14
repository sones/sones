using System;
using System.Collections.Generic;
using sones.GraphDB.Request.CreateVertexTypes;
using System.Linq;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition for vertex types
    /// </summary>
    public sealed class VertexTypePredefinition
    {
        #region Data

        /// <summary>
        /// The name of the vertex type that is going to be created
        /// </summary>
        public readonly string VertexTypeName;
        private List<PropertyPredefinition> _properties;
        private List<OutgoingEdgePredefinition> _outgoingEdges;
        private List<IncomingEdgePredefinition> _incomingEdges;
        private List<UniquePredefinition> _uniques;
        private List<IndexPredefinition> _indices;

        /// <summary>
        /// The name of the vertex type this vertex types inherites from.
        /// </summary>
        public string SuperVertexTypeName { get; private set; }

        /// <summary>
        /// The properties of the vertex type.
        /// </summary>
        public IEnumerable<PropertyPredefinition> Properties
        {
            get
            {
                return (_properties == null) 
                    ? Enumerable.Empty<PropertyPredefinition>() : 
                    _properties.AsReadOnly();
            }
        }

        /// <summary>
        /// The outgoing edges of this vertex type.
        /// </summary>
        public IEnumerable<OutgoingEdgePredefinition> OutgoingEdges
        {
            get 
            {
                return (_outgoingEdges == null)
                    ? Enumerable.Empty<OutgoingEdgePredefinition>()
                    : _outgoingEdges.AsReadOnly(); 
            }
        }

        /// <summary>
        /// The outgoing edges of this vertex type.
        /// </summary>
        public IEnumerable<IncomingEdgePredefinition> IncomingEdges
        {
            get
            {
                return (_incomingEdges == null)
                    ? Enumerable.Empty<IncomingEdgePredefinition>()
                    : _incomingEdges.AsReadOnly();
            }
        }

        /// <summary>
        /// The unique definitions of this vertex type.
        /// </summary>
        public IEnumerable<UniquePredefinition> Uniques
        {
            get
            {
                return (_uniques == null)
                    ? Enumerable.Empty<UniquePredefinition>()
                    : _uniques.AsReadOnly();
            }
        }

        /// <summary>
        /// The index definitions of this vertex type.
        /// </summary>
        public IEnumerable<IndexPredefinition> Indices
        {
            get
            {
                return (_indices == null)
                    ? Enumerable.Empty<IndexPredefinition>()
                    : _indices.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets if the vertex type will be sealed.
        /// </summary>
        public bool IsSealed { get; private set; }

        /// <summary>
        /// Gets if the vertex type will be abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new vertex type definition.
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type.</param>
        public VertexTypePredefinition(String myVertexTypeName)
        {
            if (string.IsNullOrEmpty(myVertexTypeName))
            {
                throw new ArgumentOutOfRangeException("myVertexTypeName", myVertexTypeName);
            }

            VertexTypeName = myVertexTypeName;
            SuperVertexTypeName = "Vertex";
            IsSealed = false;
            IsAbstract = false;

        }

        #endregion

        #region fluent methods


        /// <summary>
        /// Sets the name of the vertex type this one inherits from
        /// </summary>
        /// <param name="mySuperVertexTypeName">The name of the super vertex type</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition SetSuperVertexTypeName(String mySuperVertexTypeName)
        {
            if (!string.IsNullOrEmpty(mySuperVertexTypeName))
            {
                SuperVertexTypeName = mySuperVertexTypeName;
            }

            return this;
        }

        /// <summary>
        /// Adds a property to the vertex type definition
        /// </summary>
        /// <param name="myPropertyDefinition">The property definition that is going to be added</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddProperty(PropertyPredefinition myPropertyDefinition)
        {
            if (myPropertyDefinition != null)
            {
                _properties = (_properties) ?? new List<PropertyPredefinition>();
                _properties.Add(myPropertyDefinition);
            }

            return this;
        }

        /// <summary>
        /// Adds an outgoing edge.
        /// </summary>
        /// <param name="myOutgoingEdgePredefinition">The definition of the outgoing IncomingEdge</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddOutgoingEdge(OutgoingEdgePredefinition myOutgoingEdgePredefinition)
        {
            if (myOutgoingEdgePredefinition != null)
            {
                _outgoingEdges = (_outgoingEdges) ?? new List<OutgoingEdgePredefinition>();
                _outgoingEdges.Add(myOutgoingEdgePredefinition);
            }

            return this;
        }

        /// <summary>
        /// Adds a unique definition.
        /// </summary>
        /// <param name="myUniqueDefinition">The unique definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddUnique(UniquePredefinition myUniqueDefinition)
        {
            if (myUniqueDefinition != null)
            {
                _uniques = (_uniques) ?? new List<UniquePredefinition>();
                _uniques.Add(myUniqueDefinition);
            }

            return this;
        }

        /// <summary>
        /// Adds an index definition.
        /// </summary>
        /// <param name="myIndexDefinition">The index definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddIndex(IndexPredefinition myIndexDefinition)
        {
            if (myIndexDefinition != null)
            {
                _indices = (_indices) ?? new List<IndexPredefinition>();
                _indices.Add(myIndexDefinition);
            }

            return this;
        }

        /// <summary>
        /// Marks the vertex type as sealed.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition MarkAsSealed()
        {
            IsSealed = true;
            return this;
        }

        /// <summary>
        /// Marks the vertex type as abstract.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition MarkAsAbstract()
        {
            IsAbstract = true;
            return this;
        }

        #endregion
    }
}
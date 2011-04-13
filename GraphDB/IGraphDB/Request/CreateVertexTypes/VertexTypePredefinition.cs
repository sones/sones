using System;
using System.Collections.Generic;
using sones.GraphDB.Request.CreateVertexTypes;

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
        private readonly List<PropertyPredefinition> _properties;
        private readonly List<OutgoingEdgePredefinition> _outgoingEdges;
        private readonly List<IncomingEdgePredefinition> _incomingEdges;

        /// <summary>
        /// The name of the vertex type this vertex types inherites from
        /// </summary>
        public string SuperVertexTypeName { get; private set; }

        /// <summary>
        /// The properties of the vertex type
        /// </summary>
        public IEnumerable<PropertyPredefinition> Properties
        {
            get { return _properties.AsReadOnly(); }
        }

        /// <summary>
        /// The outgoing edges of this vertex type
        /// </summary>
        public IEnumerable<OutgoingEdgePredefinition> OutgoingEdges
        {
            get { return _outgoingEdges.AsReadOnly(); }
        }

        /// <summary>
        /// Gets if the vertex type will be sealed.
        /// </summary>
        public bool IsSealed { get; private set; }

        /// <summary>
        /// Gets if the vertex type will be abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// The outgoing edges of this vertex type
        /// </summary>
        public IEnumerable<IncomingEdgePredefinition> IncomingEdges
        {
            get { return _incomingEdges.AsReadOnly(); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new vertex type definition
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type</param>
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

            _properties = new List<PropertyPredefinition>();
            _outgoingEdges = new List<OutgoingEdgePredefinition>();
            _incomingEdges = new List<IncomingEdgePredefinition>();
            
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
                _properties.Add(myPropertyDefinition);
            }

            return this;
        }

        /// <summary>
        /// Adds an outgoing edge
        /// </summary>
        /// <param name="myOutgoingEdgePredefinition">The definition of the outgoing edge</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddOutgoingEdge(OutgoingEdgePredefinition myOutgoingEdgePredefinition)
        {
            if (myOutgoingEdgePredefinition != null)
            {
                _outgoingEdges.Add(myOutgoingEdgePredefinition);
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
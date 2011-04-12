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
        private readonly List<PropertyDefinition> _properties;
        private readonly List<OutgoingEdgePredefinition> _outgoingEdges;
        private readonly List<IncomingEdgePredefinition> _incomingEdges;

        /// <summary>
        /// The name of the vertex type this vertex types inherites from
        /// </summary>
        public string SuperVertexTypeName { get; private set; }

        /// <summary>
        /// The properties of the vertex type
        /// </summary>
        public IEnumerable<PropertyDefinition> Properties
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

            _properties = new List<PropertyDefinition>();
            _outgoingEdges = new List<OutgoingEdgePredefinition>();
            _incomingEdges = new List<IncomingEdgePredefinition>();
            
        }

        #endregion

        #region fluent methods

        /// <summary>
        /// Sets the name of the vertex type this one inherits from
        /// </summary>
        /// <param name="mySuperVertexTypeName">The name of the super vertex type</param>
        /// <returns>A vertex type definition</returns>
        private VertexTypePredefinition SetSuperVertexTypeName(String mySuperVertexTypeName)
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
        /// <returns>A vertex type definition</returns>
        private VertexTypePredefinition AddProperty(PropertyDefinition myPropertyDefinition)
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
        /// <returns>A vertex type definition</returns>
        private VertexTypePredefinition AddOutgoingEdge(OutgoingEdgePredefinition myOutgoingEdgePredefinition)
        {
            if (myOutgoingEdgePredefinition != null)
            {
                _outgoingEdges.Add(myOutgoingEdgePredefinition);
            }

            return this;
        }

        #endregion
    }
}
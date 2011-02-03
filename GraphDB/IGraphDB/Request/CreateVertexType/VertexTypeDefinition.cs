using System;
using System.Collections.Generic;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition for vertex types
    /// </summary>
    public sealed class VertexTypeDefinition
    {
        #region Data

        /// <summary>
        /// The name of the vertex type that is going to be created
        /// </summary>
        public readonly string VertexTypeName = null;

        /// <summary>
        /// The name of the vertex type this vertex types inherites from
        /// </summary>
        public string SuperVertexTypeName { get; private set; }

        /// <summary>
        /// The properties of the vertex type
        /// </summary>
        public List<PropertyDefinition> Properties { get; private set; }

        /// <summary>
        /// The outgoing edges of this vertex type
        /// </summary>
        public List<OutgoingEdgeDefinition> OutgoingEdges { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new vertex type definition
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type</param>
        public VertexTypeDefinition(String myVertexTypeName)
        {
            if (myVertexTypeName == null || myVertexTypeName.Length == 0)
            {
                throw new ArgumentOutOfRangeException("Name of new vertex type", myVertexTypeName);
            }

            VertexTypeName = myVertexTypeName;

            Properties = new List<PropertyDefinition>();
            OutgoingEdges = new List<OutgoingEdgeDefinition>();
            SuperVertexTypeName = null;
        }

        #endregion

        #region fluent methods

        /// <summary>
        /// Sets the name of the vertex type this one inherits from
        /// </summary>
        /// <param name="mySuperVertexTypeName">The name of the super vertex type</param>
        /// <returns>A vertex type definition</returns>
        VertexTypeDefinition SetSuperVertexTypeName(String mySuperVertexTypeName)
        {
            if (mySuperVertexTypeName != null && mySuperVertexTypeName.Length > 0)
            {
                this.SuperVertexTypeName = mySuperVertexTypeName;
            }

            return this;
        }

        /// <summary>
        /// Adds a property to the vertex type definition
        /// </summary>
        /// <param name="myPropertyDefinition">The property definition that is going to be added</param>
        /// <returns>A vertex type definition</returns>
        VertexTypeDefinition AddProperty(PropertyDefinition myPropertyDefinition)
        {
            if (myPropertyDefinition != null)
            {
                this.Properties.Add(myPropertyDefinition);
            }

            return this;
        }

        /// <summary>
        /// Adds an outgoing edge
        /// </summary>
        /// <param name="myOutgoingEdgeDefinition">The definition of the outgoing edge</param>
        /// <returns>A vertex type definition</returns>
        VertexTypeDefinition AddOutgoingEdge(OutgoingEdgeDefinition myOutgoingEdgeDefinition)
        {
            if (myOutgoingEdgeDefinition != null)
            {
                this.OutgoingEdges.Add(myOutgoingEdgeDefinition);
            }

            return this;
        }

        #endregion
    }
}

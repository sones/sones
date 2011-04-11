using System.Collections.Generic;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for creating a new vertex type
    /// </summary>
    public sealed class RequestCreateVertexTypes : IRequest
    {
        #region data

        /// <summary>
        /// The definition of the vertex that is going to be created
        /// </summary>
        public readonly IEnumerable<VertexTypeDefinition> VertexTypeDefinitions;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that clears the Graphdb
        /// </summary>
        /// <param name="myVertexTypeDefinition">Describes the vertex that is going to be created</param>
        public RequestCreateVertexTypes(IEnumerable<VertexTypeDefinition> myVertexTypeDefinition)
        {
            VertexTypeDefinitions = myVertexTypeDefinition;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.TypeChange; }
        }

        #endregion
    }
}
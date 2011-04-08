using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// A vertexType/property combination
    /// </summary>
    public sealed class QueryPlanProperty : IQueryPlan
    {
        #region data

        /// <summary>
        /// The vertex type
        /// </summary>
        public readonly IVertexType VertexType;

        /// <summary>
        /// The interesting property
        /// </summary>
        public readonly IPropertyDefinition Property;

        #endregion

        #region constructor

        /// <summary>
        /// Create a new query plan property
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type</param>
        /// <param name="myProperty">The interesting property</param>
        public QueryPlanProperty(IVertexType myVertexType, IPropertyDefinition myProperty)
        {
            VertexType = myVertexType;
            Property = myProperty;
        }

        #endregion
    }
}
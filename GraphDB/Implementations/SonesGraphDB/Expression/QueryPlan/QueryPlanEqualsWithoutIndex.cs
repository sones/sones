using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using System;
using sones.Library.VertexStore;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// An equals operation without any index
    /// </summary>
    public sealed class QueryPlanEqualsWithoutIndex : IQueryPlan
    {
        #region data

        /// <summary>
        /// The interesting property
        /// </summary>
        private readonly QueryPlanProperty _property;

        /// <summary>
        /// The constant value
        /// </summary>
        private readonly QueryPlanConstant _constant;

        /// <summary>
        /// The vertex store that is needed to load the vertices
        /// </summary>
        private readonly IVertexStore _vertexStore;

        /// <summary>
        /// Determines whether it is anticipated that the request could take longer
        /// </summary>
        private readonly Boolean _isLongrunning;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes an equals operation without any index
        /// </summary>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        /// <param name="IsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public QueryPlanEqualsWithoutIndex(QueryPlanProperty myProperty, QueryPlanConstant myConstant, IVertexStore myVertexStore, Boolean IsLongrunning)
        {
            _property = myProperty;
            _constant = myConstant;
            _vertexStore = myVertexStore;
        }

        #endregion

        #region IQueryPlan Members

        public IEnumerable<IVertex> Execute()
        {
            var result = new List<IVertex>();

            foreach (var aVertex in _vertexStore.GetVerticesByTypeID(_property.VertexType.ID, _property.Edition, VertexRevisionFilter))
            {
                if (aVertex.HasProperty(_property.Property.AttributeID))
                {
                    if (aVertex.GetProperty<IComparable>(_property.Property.AttributeID).CompareTo(_constant.Constant) == 0)
                    {
                        result.Add(aVertex);                        
                    }
                }
            }

            return result;
        }

        #endregion

        private bool VertexRevisionFilter(VertexRevisionID myToBeCheckedID)
        {
            return _property.Timespan.IsWithinTimeStamp(myToBeCheckedID);
        }
    }
}
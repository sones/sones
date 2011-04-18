using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using System;
using sones.Library.VertexStore;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Manager.Index;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// An equals operation using indices
    /// </summary>
    public sealed class QueryPlanEqualsWithIndex : IQueryPlan
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

        /// <summary>
        /// The index manager is needed to get the property related indices
        /// </summary>
        private readonly IIndexManager _indexManager;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes an equals operation using indices
        /// </summary>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        /// <param name="myVertexStore">The vertex store that is needed to load the vertices</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public QueryPlanEqualsWithIndex(QueryPlanProperty myProperty, QueryPlanConstant myConstant, IVertexStore myVertexStore, Boolean myIsLongrunning, IIndexManager myIndexManager)
        {
            _property = myProperty;
            _constant = myConstant;
            _vertexStore = myVertexStore;
            _isLongrunning = myIsLongrunning;
            _indexManager = myIndexManager;
        }

        #endregion

        #region IQueryPlan Members

        public IEnumerable<IVertex> Execute()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private helper

        /// <summary>
        /// Checks the revision of a vertex
        /// </summary>
        /// <param name="myToBeCheckedID">The revision that needs to be checked</param>
        /// <returns>True or false</returns>
        private bool VertexRevisionFilter(Int64 myToBeCheckedID)
        {
            return _property.Timespan.IsWithinTimeStamp(myToBeCheckedID);
        }

        #endregion
    }
}
using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using System;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Manager.Index;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Plugins.Index.Interfaces;
using System.Linq;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// An OR operation sequentially executed
    /// </summary>
    public sealed class QueryPlanORSequentiell : IQueryPlan
    {
        #region data

        /// <summary>
        /// The left query plan
        /// </summary>
        private readonly IQueryPlan _left;
        
        /// <summary>
        /// The right query plan
        /// </summary>
        private readonly IQueryPlan _right;

        /// <summary>
        /// Determines whether it is anticipated that the request could take longer
        /// </summary>
        private readonly Boolean _isLongrunning;

        #endregion

        #region constructor

        public QueryPlanORSequentiell(IQueryPlan myLeft, IQueryPlan myRight, Boolean myIsLongrunning)
        {
            _left = myLeft;
            _right = myRight;
            _isLongrunning = myIsLongrunning;
        }

        #endregion

        #region IQueryPlan Members
        
        public IEnumerable<IVertex> Execute()
        {
            return _left.Execute().Union(_right.Execute());
        }

        #endregion
    }
}
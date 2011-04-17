using sones.GraphDB.TypeSystem;
using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using System;
using sones.GraphDB.ErrorHandling.QueryPlan;
using sones.GraphDB.Expression.Tree;

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

        /// <summary>
        /// The edition that should be processed
        /// </summary>
        public readonly String Edition;

        /// <summary>
        /// The timespan that should be processed
        /// </summary>
        public readonly TimeSpanDefinition Timespan;

        #endregion

        #region constructor

        /// <summary>
        /// Create a new query plan property
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type</param>
        /// <param name="myProperty">The interesting property</param>
        public QueryPlanProperty(IVertexType myVertexType, IPropertyDefinition myProperty, String myInterestingEdition, TimeSpanDefinition myInterestingTimeSpan)
        {
            VertexType = myVertexType;
            Property = myProperty;
            Edition = myInterestingEdition;
            Timespan = myInterestingTimeSpan;
        }

        #endregion

        #region IQueryPlan Members

        public IEnumerable<IVertex> Execute()
        {
            throw new InvalidQueryPlanExecutionException("It is not possible to execute a query plan property.");
        }

        #endregion
    }
}
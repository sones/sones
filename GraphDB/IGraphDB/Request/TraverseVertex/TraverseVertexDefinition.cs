using System;
using sones.GraphDB.Expression;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition / start node wich should be requested from the graphdb
    /// </summary>
    public sealed class TraverseVertexDefinition
    {
        #region Data

        /// <summary>
        /// The expression which should be evaluated
        /// </summary>
        public readonly IExpression                                             Expression;

        public readonly Boolean                                                 AvoidCircles;

        public readonly Func<IVertex, IVertexType, IEdge, IEdgeType, Boolean>   FollowThisEdge;

        public readonly Func<IVertex, IVertexType, Boolean>                     MatchEvaluator;

        public readonly Action<IVertex>                                         MatchAction;

        public readonly Func<TraversalState, Boolean>                           StopEvaluator;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new get vertices definition
        /// </summary>
        /// <param name="myExpression">The expression which should be evaluated</param>
        public TraverseVertexDefinition(IExpression                                             myExpression, 
                                        Boolean                                                 myAvoidCircles      = true,
                                        Func<IVertex, IVertexType , IEdge, IEdgeType , Boolean> myFollowThisEdge    = null,
                                        Func<IVertex, IVertexType, Boolean>                     myMatchEvaluator    = null,
									    Action<IVertex>                                         myMatchAction       = null,
									    Func<TraversalState, Boolean>                           myStopEvaluator     = null)
        {
            Expression = myExpression;

            AvoidCircles = myAvoidCircles;

            FollowThisEdge = myFollowThisEdge;

            MatchEvaluator = myMatchEvaluator;

            MatchAction = myMatchAction;

            StopEvaluator = myStopEvaluator;
        }

        #endregion
    }
}

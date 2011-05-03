using sones.GraphDB.Expression;
using System;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;
namespace sones.GraphDB.Request
{
    /// <summary>
    /// The traverse vertex request
    /// </summary>
    public sealed class RequestTraverseVertex : IRequest
    {
        #region data

        /// <summary>
        /// The expression which should be evaluated
        /// </summary>
        public readonly IExpression                                             Expression;

        public readonly String                                                  VertexTypeName;

        public readonly long                                                    VertexID;

        public readonly Boolean                                                 AvoidCircles;

        public readonly Func<IVertex, IVertexType, IEdge, IEdgeType, Boolean>   FollowThisEdge;

        public readonly Func<IVertex, IVertexType, Boolean>                     MatchEvaluator;

        public readonly Action<IVertex>                                         MatchAction;

        public readonly Func<TraversalState, Boolean>                           StopEvaluator;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that traverses verticies
        /// </summary>
        public RequestTraverseVertex(IExpression                                                myExpression,
                                        Boolean                                                 myAvoidCircles   = true,
                                        Func<IVertex, IVertexType, IEdge, IEdgeType, Boolean>   myFollowThisEdge = null,
                                        Func<IVertex, IVertexType, Boolean>                     myMatchEvaluator = null,
                                        Action<IVertex>                                         myMatchAction    = null,
                                        Func<TraversalState, Boolean>                           myStopEvaluator  = null)
        {
            Expression = myExpression;

            AvoidCircles = myAvoidCircles;

            FollowThisEdge = myFollowThisEdge;

            MatchEvaluator = myMatchEvaluator;

            MatchAction = myMatchAction;

            StopEvaluator = myStopEvaluator;
        }

        /// <summary>
        /// Creates a new request that traverses verticies
        /// </summary>
        public RequestTraverseVertex(String myVerexTypeName,
                                        long myVertexID,
                                        Boolean myAvoidCircles = true,
                                        Func<IVertex, IVertexType, IEdge, IEdgeType, Boolean> myFollowThisEdge = null,
                                        Func<IVertex, IVertexType, Boolean> myMatchEvaluator = null,
                                        Action<IVertex> myMatchAction = null,
                                        Func<TraversalState, Boolean> myStopEvaluator = null)
        {
            VertexTypeName = myVerexTypeName;

            VertexID = myVertexID;

            Expression = null;

            AvoidCircles = myAvoidCircles;

            FollowThisEdge = myFollowThisEdge;

            MatchEvaluator = myMatchEvaluator;

            MatchAction = myMatchAction;

            StopEvaluator = myStopEvaluator;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.ReadOnly; }
        }

        #endregion
    }
}

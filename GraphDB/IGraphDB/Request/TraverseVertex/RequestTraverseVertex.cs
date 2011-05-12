/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
        public readonly IExpression                                                         Expression;

        public readonly String                                                              VertexTypeName;

        public readonly long                                                                VertexID;

        public readonly Avoidance                                                           Avoid;

        public readonly Func<IVertex, IVertexType, IEdge, IAttributeDefinition, Boolean>    FollowThisEdge;

        public readonly Func<IVertex, IVertexType, Boolean>                                 MatchEvaluator;

        public readonly Action<IVertex>                                                     MatchAction;

        public readonly Func<TraversalState, Boolean>                                       StopEvaluator;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that traverses verticies
        /// </summary>
        public RequestTraverseVertex(IExpression                                                            myExpression,
                                        Avoidance                                                           myAvoid          = Avoidance.None,
                                        Func<IVertex, IVertexType, IEdge, IAttributeDefinition, Boolean>    myFollowThisEdge = null,
                                        Func<IVertex, IVertexType, Boolean>                                 myMatchEvaluator = null,
                                        Action<IVertex>                                                     myMatchAction    = null,
                                        Func<TraversalState, Boolean>                                       myStopEvaluator  = null)
        {
            Expression = myExpression;

            Avoid = myAvoid;

            FollowThisEdge = myFollowThisEdge;

            MatchEvaluator = myMatchEvaluator;

            MatchAction = myMatchAction;

            StopEvaluator = myStopEvaluator;
        }

        /// <summary>
        /// Creates a new request that traverses verticies
        /// </summary>
        public RequestTraverseVertex(String                                                                 myVerexTypeName,
                                        long                                                                myVertexID,
                                        Avoidance                                                           myAvoid             = Avoidance.None,
                                        Func<IVertex, IVertexType, IEdge, IAttributeDefinition, Boolean>    myFollowThisEdge    = null,
                                        Func<IVertex, IVertexType, Boolean>                                 myMatchEvaluator    = null,
                                        Action<IVertex>                                                     myMatchAction       = null,
                                        Func<TraversalState, Boolean>                                       myStopEvaluator     = null)
        {
            VertexTypeName = myVerexTypeName;

            VertexID = myVertexID;

            Expression = null;

            Avoid = myAvoid;

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

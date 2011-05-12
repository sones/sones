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

using System;
using sones.GraphDB.Expression;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition (start node and properties) wich should be requested from the graphdb
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

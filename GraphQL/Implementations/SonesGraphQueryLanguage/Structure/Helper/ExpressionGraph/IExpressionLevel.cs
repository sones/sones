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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    /// <summary>
    /// Interface for the expression level.
    /// </summary>
    public interface IExpressionLevel
    {
        #region properties

        Dictionary<LevelKey, IExpressionLevelEntry> ExpressionLevels { get; }

        #endregion

        /// <summary>
        /// Adds a single forward edge to a node and creates it.
        /// </summary>
        /// <param name="levelKey">The place where the new node is going to be stored.</param>
        /// <param name="myVertexID">The ObjectUUID that's going to be a new node.</param>
        /// <param name="forwardEdgeDirection">The forward direction for the forward edge.</param>
        /// <param name="destination">The destination for the forward edge.</param>
        /// <param name="NodeWeight">The weight of the node.</param>
        void AddForwardEdgeToNode(LevelKey levelKey, Int64 myVertexID, EdgeKey forwardEdgeDirection, Int64 destination, IComparable NodeWeight);

        /// <summary>
        /// Adds an expression node to a level corresponding to its LevelKey
        /// </summary>
        /// <param name="levelKey">The destination of the node.</param>
        /// <param name="expressionNode">The node that should be inserted.</param>
        void AddNode(LevelKey levelKey, IExpressionNode expressionNode);

        /// <summary>
        /// Adds an empty LevelKey
        /// </summary>
        /// <param name="levelKey">The LevelKey that should be inserted.</param>
        void AddEmptyLevelKey(LevelKey levelKey);

        /// <summary>
        /// Adds a BackwardEdge to a node
        /// </summary>
        /// <param name="myPath">The LevelKey of the node</param>
        /// <param name="myVertexID">The ObjectUUID that identifies the node</param>
        /// <param name="myBackwardDirection">The direction of the Edge</param>
        /// <param name="myBackwardDestinations">The destination of the edge including a EdgeWeight</param>
        void AddBackwardEdgesToNode(LevelKey myPath, Int64 myVertexID, EdgeKey myBackwardDirection, Dictionary<Int64, IComparable> myBackwardDestinations);

        /// <summary>
        /// Adds a node and a BackwardEdge
        /// </summary>
        /// <param name="myPath">The LevelKey of the node</param>
        /// <param name="myDBObject">The DBObject for the new node</param>
        /// <param name="myBackwardDirection">The backward direction of the edge</param>
        /// <param name="myBackwardDestination">The backward destination of the edge</param>
        /// <param name="myEdgeWeight">The weight of the backward edge</param>
        /// <param name="myNodeWeight">The weight of the node</param>
        void AddNodeAndBackwardEdge(LevelKey myPath, IVertex myDBObject, EdgeKey myBackwardDirection, Int64 myBackwardDestination, IComparable myEdgeWeight, IComparable myNodeWeight);

        /// <summary>
        /// Get all ExpressionNodes of this level
        /// </summary>
        /// <param name="myLevelKey">The Level</param>
        /// <returns>All ExpressionNodes</returns>
        IEnumerable<IExpressionNode> GetNodes(LevelKey myLevelKey);

        /// <summary>
        /// Returns the node of the level
        /// </summary>
        /// <param name="myLevelKey">The Level</param>
        /// <returns>The node content at this level</returns>
        Dictionary<Int64, IExpressionNode> GetNode(LevelKey myLevelKey);

        /// <summary>
        /// This method removes a Node from a Level.
        /// </summary>
        /// <param name="myLevelKey">The place where the node should be removed.</param>
        /// <param name="myVertexID">The VertexID that identifies the Node.</param>
        void RemoveNode(LevelKey myLevelKey, Int64 myVertexID);

        /// <summary>
        /// Returns True, if the Level has any Node for the <paramref name="myLevelKey"/>
        /// </summary>
        /// <param name="levelKey"></param>
        Boolean HasLevelKey(LevelKey myLevelKey);

    }
}

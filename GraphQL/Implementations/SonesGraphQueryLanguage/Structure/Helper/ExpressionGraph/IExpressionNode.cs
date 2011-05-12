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
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    public interface IExpressionNode
    {
        #region Properties

        /// <summary>
        /// The edges that point to a higher level
        /// </summary>
        Dictionary<EdgeKey, HashSet<IExpressionEdge>> ForwardEdges { get; }

        /// <summary>
        /// The edges that point to a lower level
        /// </summary>
        Dictionary<EdgeKey, HashSet<IExpressionEdge>> BackwardEdges { get; }

        /// <summary>
        /// The complex connections
        /// </summary>
        Dictionary<LevelKey, HashSet<Int64>> ComplexConnection { get; }

        #endregion

        #region IVertex

        /// <summary>
        /// Return the ObjectUUID of the node
        /// </summary>
        /// <returns>An ObjectUUID</returns>
        Int64 GetVertexID();

        /// <summary>
        /// Returns the IVertex of the node
        /// </summary>
        /// <returns>A IVertex</returns>
        IVertex GetIVertex(IGraphDB myGraphDB, Int64 myVertexTypeID, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        #endregion

        #region Edges

        #region Adding Edges

        /// <summary>
        /// Adds a forward edge to a node
        /// </summary>
        /// <param name="myForwardEdgeDirection">The direction for the forward edge</param>
        /// <param name="myForwardEdgeDestination">The destination for the forward edge</param>
        /// <param name="myEdgeWeight">The weight of the new edge</param>
        void AddForwardEdge(EdgeKey myForwardEdgeDirection, Int64 myForwardEdgeDestination, IComparable myEdgeWeight);

        /// <summary>
        /// Adds a couple of forward edges
        /// </summary>
        /// <param name="myForwardEdges">A couple of forward edges</param>
        void AddForwardEdges(IEnumerable<IExpressionEdge> myForwardEdges);

        /// <summary>
        /// Adds a couple of forward edges
        /// </summary>
        /// <param name="myForwardEdgeDirection">The direction for the forward edges</param>
        /// <param name="myRawForwardEdges">A dictionary of destination/weight</param>
        void AddForwardEdges(EdgeKey myForwardEdgeDirection, Dictionary<Int64, IComparable> myRawForwardEdges);

        /// <summary>
        /// Adds a couple of backward edges
        /// </summary>
        /// <param name="myBackwardEdgeDirection">The direction for the backward edges</param>
        /// <param name="validUUIDs">A dictionary of destination/weight</param>
        void AddBackwardEdges(EdgeKey myBackwardEdgeDirection, Dictionary<Int64, IComparable> myRawBackwardEdges);

        /// <summary>
        /// Adds a couple of backward edges
        /// </summary>
        /// <param name="myBackwardEdges">A couple of backward edges</param>
        void AddBackwardEdges(IEnumerable<IExpressionEdge> myBackwardEdges);

        /// <summary>
        /// Adds a backward edge to a node
        /// </summary>
        /// <param name="myBackwardEdgeDirection">The direction for the backward edge</param>
        /// <param name="myBackwardEdgeDestination">The destination for the backward edge</param>
        /// <param name="myEdgeWeight">The weight of the new edge</param>
        void AddBackwardEdge(EdgeKey myBackwardEdgeDirection, Int64 myBackwardEdgeDestination, IComparable myEdgeWeight);

        /// <summary>
        /// Adds a complex connection between two nodes that are distributed across different types
        /// </summary>
        /// <param name="myLevelKey">The connected level</param>
        /// <param name="myUUID">The ObjectUUID of the connected ExpressionNode</param>
        void AddComplexConnection(LevelKey myLevelKey, Int64 myUUID);

        #endregion

        #region removing edges

        /// <summary>
        /// Remove all backward edges corresponding to an EdgeKey
        /// </summary>
        /// <param name="myEdgeKey">A EdgeKey</param>
        void RemoveBackwardEdges(EdgeKey myEdgeKey);

        /// <summary>
        /// Remove all forward edges corresponding to an EdgeKey
        /// </summary>
        /// <param name="myEdgeKey">A EdgeKey</param>
        void RemoveForwardEdges(EdgeKey myEdgeKey);

        /// <summary>
        /// Remove a single forward Edge
        /// </summary>
        /// <param name="myEdgeKey">A EdgeKey</param>
        /// <param name="myObjectUUID">The destination of the edge</param>
        void RemoveForwardEdge(EdgeKey myEdgeKey, Int64 myObjectUUID);

        /// <summary>
        /// Remove a single backward Edge
        /// </summary>
        /// <param name="myEdgeKey">A EdgeKey</param>
        /// <param name="myObjectUUID">The destination of the edge</param>
        void RemoveBackwardEdge(EdgeKey myEdgeKey, Int64 myObjectUUID);

        /// <summary>
        /// Removes a single complex connection
        /// </summary>
        /// <param name="myLevelKey">A LevelKey</param>
        /// <param name="myUUID">The destination of the complex connection</param>
        void RemoveComplexConnection(LevelKey myLevelKey, Int64 myUUID);

        #endregion

        #endregion
    }
}

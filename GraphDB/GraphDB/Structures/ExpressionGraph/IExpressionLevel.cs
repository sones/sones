/* <id name="GraphDB – interface for all ExpressionLevel" />
 * <copyright file="IExpressionLevel.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>Interface for the expression level.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;


#endregion

namespace sones.GraphDB.Structures.ExpressionGraph
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
        /// <param name="aObjectUUID">The ObjectUUID that's going to be a new node.</param>
        /// <param name="forwardEdgeDirection">The forward direction for the forward edge.</param>
        /// <param name="destination">The destination for the forward edge.</param>
        /// <param name="NodeWeight">The weight of the node.</param>
        void AddForwardEdgeToNode(LevelKey levelKey, ObjectUUID aObjectUUID, EdgeKey forwardEdgeDirection, ObjectUUID destination, ADBBaseObject NodeWeight);

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
        /// <param name="myObjectUUID">The ObjectUUID that identifies the node</param>
        /// <param name="myBackwardDirection">The direction of the Edge</param>
        /// <param name="myBackwardDestinations">The destination of the edge including a EdgeWeight</param>
        void AddBackwardEdgesToNode(LevelKey myPath, ObjectUUID myObjectUUID, EdgeKey myBackwardDirection, Dictionary<ObjectUUID, ADBBaseObject> myBackwardDestinations);

        /// <summary>
        /// Adds a node and a BackwardEdge
        /// </summary>
        /// <param name="myPath">The LevelKey of the node</param>
        /// <param name="myDBObject">The DBObject for the new node</param>
        /// <param name="myBackwardDirection">The backward direction of the edge</param>
        /// <param name="myBackwardDestination">The backward destination of the edge</param>
        /// <param name="myEdgeWeight">The weight of the backward edge</param>
        /// <param name="myNodeWeight">The weight of the node</param>
        void AddNodeAndBackwardEdge(LevelKey myPath, DBObjectStream myDBObject, EdgeKey myBackwardDirection, ObjectUUID myBackwardDestination, ADBBaseObject myEdgeWeight, ADBBaseObject myNodeWeight);

        /// <summary>
        /// Adds a node and a BackwardEdge
        /// </summary>
        /// <param name="myPath">The LevelKey of the node</param>
        /// <param name="myDBObjectUUID">The ObjectUUID for the new node</param>
        /// <param name="myBackwardDirection">The backward direction of the edge</param>
        /// <param name="myBackwardDestination">The backward destination of the edge</param>
        /// <param name="myEdgeWeight">The weight of the backward edge</param>
        /// <param name="myNodeWeight">The weight of the node</param>
        void AddNodeAndBackwardEdge(LevelKey myPath, ObjectUUID myDBObjectUUID, EdgeKey myBackwardDirection, ObjectUUID myBackwardDestination, ADBBaseObject myEdgeWeight, ADBBaseObject myNodeWeight);

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
        Dictionary<ObjectUUID, IExpressionNode> GetNode(LevelKey myLevelKey);

        /// <summary>
        /// This method removes a Node from a Level.
        /// </summary>
        /// <param name="myLevelKey">The place where the node should be removed.</param>
        /// <param name="myObjectUUID">The UUID that identifies the Node.</param>
        void RemoveNode(LevelKey myLevelKey, ObjectUUID myObjectUUID);

        /// <summary>
        /// Returns True, if the Level has any Node for the <paramref name="myLevelKey"/>
        /// </summary>
        /// <param name="levelKey"></param>
        Boolean HasLevelKey(LevelKey myLevelKey);

    }
}

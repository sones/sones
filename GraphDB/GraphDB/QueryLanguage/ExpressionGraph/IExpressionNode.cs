/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/* <id name="PandoraDB – Node interface" />
 * <copyright file="IExpressionNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Interface for the expression node.</summary>
 */

#region Usings

using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.QueryLanguage.ExpressionGraph
{
    /// <summary>
    /// Interface for the expression node.
    /// </summary>

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
        Dictionary<LevelKey, HashSet<ObjectUUID>> ComplexConnection { get; }

        /// <summary>
        /// List of Warnings
        /// </summary>
        List<IWarning> Warnings { get; }

        #endregion

        #region DBObjectStream
        
        /// <summary>
        /// Return the ObjectUUID of the node
        /// </summary>
        /// <returns>An ObjectUUID</returns>
        ObjectUUID GetObjectUUID();
        
        /// <summary>
        /// Returns the DBObjectStream of the node
        /// </summary>
        /// <param name="myDBObjectCache">The DBObjectCache that is responsible for loading a DBObjectStream</param>
        /// <param name="myTypeUUID">The TypeUUID of the DBObject that is going to be loaded</param>
        /// <returns>A DBObjectStream</returns>
        DBObjectStream GetDBObjectStream(DBObjectCache myDBObjectCache, TypeUUID myTypeUUID);

        #endregion

        #region Edges

        #region Adding Edges

        /// <summary>
        /// Adds a forward edge to a node
        /// </summary>
        /// <param name="myForwardEdgeDirection">The direction for the forward edge</param>
        /// <param name="myForwardEdgeDestination">The destination for the forward edge</param>
        /// <param name="myEdgeWeight">The weight of the new edge</param>
        void AddForwardEdge(EdgeKey myForwardEdgeDirection, ObjectUUID myForwardEdgeDestination, ADBBaseObject myEdgeWeight);

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
        void AddForwardEdges(EdgeKey myForwardEdgeDirection, Dictionary<ObjectUUID, ADBBaseObject> myRawForwardEdges);

        /// <summary>
        /// Adds a couple of backward edges
        /// </summary>
        /// <param name="myBackwardEdgeDirection">The direction for the backward edges</param>
        /// <param name="validUUIDs">A dictionary of destination/weight</param>
        void AddBackwardEdges(EdgeKey myBackwardEdgeDirection, Dictionary<ObjectUUID, ADBBaseObject> myRawBackwardEdges);

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
        void AddBackwardEdge(EdgeKey myBackwardEdgeDirection, ObjectUUID myBackwardEdgeDestination, ADBBaseObject myEdgeWeight);

        /// <summary>
        /// Adds a complex connection between two nodes that are distributed across different types
        /// </summary>
        /// <param name="myLevelKey">The connected level</param>
        /// <param name="myUUID">The ObjectUUID of the connected ExpressionNode</param>
        void AddComplexConnection(LevelKey myLevelKey, ObjectUUID myUUID);

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
        void RemoveForwardEdge(EdgeKey myEdgeKey, ObjectUUID myObjectUUID);

        /// <summary>
        /// Remove a single backward Edge
        /// </summary>
        /// <param name="myEdgeKey">A EdgeKey</param>
        /// <param name="myObjectUUID">The destination of the edge</param>
        void RemoveBackwardEdge(EdgeKey myEdgeKey, ObjectUUID myObjectUUID);

        /// <summary>
        /// Removes a single complex connection
        /// </summary>
        /// <param name="myLevelKey">A LevelKey</param>
        /// <param name="myUUID">The destination of the complex connection</param>
        void RemoveComplexConnection(LevelKey myLevelKey, ObjectUUID myUUID);

        #endregion

        #endregion
    }
}

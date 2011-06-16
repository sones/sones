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
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.VertexStore.Definitions;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    /// <summary>
    /// Interface for the ExpressionGraph.
    /// </summary>
    public interface IExpressionGraph
    {
        /// <summary>
        /// This method substracts another expression graph.
        /// </summary>
        /// <param name="anotherGraph">The IExpressionGraph that is going to be substracted.</param>
        void BuildDifferenceWith(IExpressionGraph anotherGraph);

        /// <summary>
        /// This method intersects with another graph.
        /// </summary>
        /// <param name="anotherGraph">The IExpressionGraph that is going to be intersected.</param>
        void IntersectWith(IExpressionGraph anotherGraph);

        /// <summary>
        /// This method executes an union with another graph.
        /// </summary>
        /// <param name="anotherGraph">Another IExpressionGraph.</param>
        void UnionWith(IExpressionGraph anotherGraph);

        /// <summary>
        /// This method sets all the weights for a certain start and end level.
        /// </summary>
        /// <param name="StartLevel">The starting level.</param>
        /// <param name="EndLevel">The ending level.</param>
        void GatherEdgeWeight(LevelKey StartLevel, LevelKey EndLevel);

        /// <summary>
        /// This method returns an instance of the derivied IExpressionGraph.
        /// </summary>
        /// <param name="myTypeManager">The TypeManager of the database.</param>
        /// <param name="myDBObjectCache">The current query cache.</param>
        /// <returns>An IExpressionGraph instance.</returns>
        IExpressionGraph GetNewInstance(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        /// <summary>
        /// This method integrates a IVertex in the expression graph.
        /// </summary>
        /// <param name="myVertex">The Ivertex that is going to be integrated.</param>
        /// <param name="myLevelKey">The location in the graph where the IVertex should be integrated.</param>
        /// <param name="backwardResolution">The number of levels that should be resolved backward.</param>
        void AddNode(IVertex myVertex, LevelKey myLevelKey, int backwardResolution);

        /// <summary>
        /// This method integrates a IVertex in the expression graph.
        /// </summary>
        /// <param name="myVertex">The IVertex that is going to be integrated.</param>
        /// <param name="myLevelKey">The location in the graph where the IVertex should be integrated.</param>
        void AddNode(IVertex myVertex, LevelKey myLevelKey);

        /// <summary>
        /// Adds an empty level identified by <paramref name="myLevelKey"/> if the level does not exist in the graph.
        /// This is needed by the select to recognize either a level was never added or just not been added due to an expression
        /// </summary>
        /// <param name="myLevelKey"></param>
        Boolean AddEmptyLevel(LevelKey myLevelKey);

        /// <summary>
        /// Adds a bunch of nodes to a specific level
        /// </summary>
        /// <param name="myNodes">The nodes to be added</param>
        /// <param name="myPath">The level</param>
        void AddNodes(IEnumerable<IExpressionNode> myNodes, LevelKey myPath);

        /// <summary>
        /// Return a specific level
        /// </summary>
        /// <param name="myLevel">The level</param>
        /// <returns>An ExpressionLevel</returns>
        IExpressionLevel GetLevel(int myLevel);

        /// <summary>
        /// All levels of the graph. Do not use this for direct level manipulation like add, remove!
        /// </summary>
        Dictionary<int, IExpressionLevel> Levels { get; }

        /// <summary>
        /// Select all DBObjects of this level, starting with an optional DBObject and following his forward edges
        /// </summary>
        /// <param name="myLevelKey">The level</param>
        /// <param name="mySourceVertex">An optional source DBObject. If null, the full level will be returned</param>
        /// <returns>All DBObjects</returns>
        IEnumerable<IVertex> Select(LevelKey myLevelKey, IVertex mySourceVertex = null, Boolean generateLevel = true);

        /// <summary>
        /// This method returns every ObjectUUID corresponding to a LevelKey
        /// </summary>
        /// <param name="myLevelKey">The interesting LevelKey</param>
        /// <param name="generateLevel">Generate the level if its not existing</param>
        /// <returns>An Enumerable of vertex informations</returns>
        IEnumerable<VertexInformation> SelectVertexIDs(LevelKey myLevelKey, IVertex mySourceVertex = null, Boolean generateLevel = true);

        /// <summary>
        /// Check the Graph whether or not the it is relevant for <paramref name="myLevelKey"/> and the optional <paramref name="mySourceVertex"/>
        /// </summary>
        /// <param name="myLevelKey"></param>
        /// <param name="mySourceVertex"></param>
        /// <returns></returns>
        Boolean IsGraphRelevant(LevelKey myLevelKey, IVertex mySourceVertex);

        /// <summary>
        /// Checks if a LevelKey exists in the ExpressionGraph
        /// </summary>
        /// <param name="levelKey">The LevelKey that should be checked.</param>
        /// <returns>True or false.</returns>
        bool ContainsLevelKey(LevelKey levelKey);

        /// <summary>
        /// Add Nodes to the Graph and add a special relation between both. 
        /// </summary>
        /// <param name="firstVertex">The first DBObject.</param>
        /// <param name="firstLevelKey">The LevelKey for the first DBObject.</param>
        /// <param name="secondVertex">The second DBObject.</param>
        /// <param name="secondLevelKey">The second LevelKey.</param>
        /// <param name="dbObjectCache">The DBObjectCache.</param>
        /// <param name="backwardResolution">The number of levels that should be resolved backward.</param> 
        void AddNodesWithComplexRelation(IVertex firstVertex, LevelKey firstLevelKey, IVertex secondVertex, LevelKey secondLevelKey, int backwardResolution);

        /// <summary>
        /// Checks if the graph contains a LevelKey that is based on a given type 
        /// </summary>
        /// <param name="myType">The starting type</param>
        /// <returns></returns>
        bool ContainsRelevantLevelForType(IVertexType myType);
    }
}

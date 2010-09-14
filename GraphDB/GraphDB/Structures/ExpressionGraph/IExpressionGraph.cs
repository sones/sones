/* <id name="GraphDB – IExpressionGraph" />
 * <copyright file="ExpressionGraph.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>Interface for the ExpressionGraph.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Structures.ExpressionGraph
{
    /// <summary>
    /// Interface for the ExpressionGraph.
    /// </summary>

    public interface IExpressionGraph
    {
        /// <summary>
        /// Returns all warnings that occurred during evaluation.
        /// </summary>
        /// <returns>A list of warnings.</returns>
        List<IWarning> GetWarnings();

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
        IExpressionGraph GetNewInstance(DBContext dbContext);

        /// <summary>
        /// This method integrates a DBObjectStream in the expression graph.
        /// </summary>
        /// <param name="myDBObjectStream">The DBObjectStream that is going to be integrated.</param>
        /// <param name="myLevelKey">The location in the graph where the DBObjectStream should be integrated.</param>
        /// <param name="backwardResolution">The number of levels that should be resolved backward.</param>
        void AddNode(DBObjectStream myDBObjectStream, LevelKey myLevelKey, int backwardResolution);

        /// <summary>
        /// This method integrates a DBObjectStream in the expression graph.
        /// </summary>
        /// <param name="myDBObjectStream">The DBObjectStream that is going to be integrated.</param>
        /// <param name="myLevelKey">The location in the graph where the DBObjectStream should be integrated.</param>
        void AddNode(DBObjectStream myDBObjectStream, LevelKey myLevelKey);

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
        /// <param name="mySourceDBObject">An optional source DBObject. If null, the full level will be returned</param>
        /// <returns>All DBObjects</returns>
        IEnumerable<Exceptional<DBObjectStream>> Select(LevelKey myLevelKey, DBObjectStream mySourceDBObject = null, Boolean generateLevel = true);

        /// <summary>
        /// This method returns every ObjectUUID corresponding to a LevelKey
        /// </summary>
        /// <param name="myLevelKey">The interesting LevelKey</param>
        /// <param name="generateLevel">Generate the level if its not existing</param>
        /// <returns>An Enumerable of ObjectUUIDs</returns>
        IEnumerable<ObjectUUID> SelectUUIDs(LevelKey myLevelKey, DBObjectStream mySourceDBObject = null, Boolean generateLevel = true);

        /// <summary>
        /// Check the Graph whether or not the it is relevant for <paramref name="myLevelKey"/> and the optional <paramref name="mySourceDBObject"/>
        /// </summary>
        /// <param name="myLevelKey"></param>
        /// <param name="mySourceDBObject"></param>
        /// <returns></returns>
        Boolean IsGraphRelevant(LevelKey myLevelKey, DBObjectStream mySourceDBObject);

        /// <summary>
        /// Checks if a LevelKey exists in the ExpressionGraph
        /// </summary>
        /// <param name="levelKey">The LevelKey that should be checked.</param>
        /// <returns>True or false.</returns>
        bool ContainsLevelKey(LevelKey levelKey);

        /// <summary>
        /// Add Nodes to the Graph and add a special relation between both. 
        /// </summary>
        /// <param name="firstBObject">The first DBObject.</param>
        /// <param name="firstLevelKey">The LevelKey for the first DBObject.</param>
        /// <param name="secondDBObject">The second DBObject.</param>
        /// <param name="secondLevelKey">The second LevelKey.</param>
        /// <param name="dbObjectCache">The DBObjectCache.</param>
        /// <param name="backwardResolution">The number of levels that should be resolved backward.</param> 
        void AddNodesWithComplexRelation(Exceptional<DBObjectStream> firstBObject, LevelKey firstLevelKey, Exceptional<DBObjectStream> secondDBObject, LevelKey secondLevelKey, DBObjectCache dbObjectCache, int backwardResolution);

        /// <summary>
        /// Checks if the graph contains a LevelKey that is based on a given type 
        /// </summary>
        /// <param name="myType">The starting type</param>
        /// <returns></returns>
        bool ContainsRelevantLevelForType(GraphDBType myType);
    }
}

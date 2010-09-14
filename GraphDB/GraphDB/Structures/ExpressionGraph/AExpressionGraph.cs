/* <id name="GraphDB – abstract expression graph class" />
 * <copyright file="AExpressionGraph.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>Abstract class for all expression graphs</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.ObjectManagement;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphDB.Structures.ExpressionGraph
{
    /// <summary>
    /// Abstract class for all expression graphs
    /// </summary>

    public abstract class AExpressionGraph : IExpressionGraph
    {
        #region IExpressionGraph Members

        #region abstract

        public abstract List<IWarning> GetWarnings();

        public abstract void BuildDifferenceWith(IExpressionGraph anotherGraph);

        public abstract void IntersectWith(IExpressionGraph anotherGraph);

        public abstract void UnionWith(IExpressionGraph anotherGraph);

        public abstract void GatherEdgeWeight(LevelKey StartLevel, LevelKey EndLevel);

        public abstract IExpressionGraph GetNewInstance(DBContext dbContext);

        public abstract void AddNode(DBObjectStream myDBObjectStream, LevelKey myIDNode, int backwardResolution);
        
        public abstract void AddNode(DBObjectStream myDBObjectStream, LevelKey myIDNode);

        public abstract Boolean AddEmptyLevel(LevelKey myLevelKey);

        public abstract void AddNodes(IEnumerable<IExpressionNode> iEnumerable, LevelKey myPath);

        public abstract Dictionary<GraphPerformanceCriteria, Int16> GetPerformanceStatement();

        public abstract IExpressionLevel GetLevel(int myLevel);

        public abstract Dictionary<int, IExpressionLevel> Levels { get; }

        public abstract IEnumerable<Exceptional<DBObjectStream>> Select(LevelKey myLevelKey, DBObjectStream mySourceDBObject = null, Boolean generateLevel = true);

        public abstract IEnumerable<ObjectUUID> SelectUUIDs(LevelKey myLevelKey, DBObjectStream mySourceDBObject = null, Boolean generateLevel = true);

        public abstract Boolean IsGraphRelevant(LevelKey myLevelKey, DBObjectStream mySourceDBObject);
        
        public abstract void AddNodesWithComplexRelation(Exceptional<DBObjectStream> leftDBObject, LevelKey leftLevelKey, Exceptional<DBObjectStream> rightDBObject, LevelKey rightLevelKey, DBObjectCache dbObjectCache, int backwardResolution);

        public abstract bool ContainsRelevantLevelForType(GraphDBType myType);

        #endregion

        #region implemented

        public bool ContainsLevelKey(LevelKey levelKey)
        {
            if (this.Levels.ContainsKey(levelKey.Level))
            {
                if (this.Levels[levelKey.Level].ExpressionLevels.ContainsKey(levelKey))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion

        #region protected methods

        protected EdgeKey GetBackwardEdgeKey(LevelKey myPath, int desiredBELevel, DBContext dbContext)
        {
            return dbContext.DBObjectManager.GetBackwardEdgeKey(myPath.Edges, desiredBELevel);
        }

        #endregion

        #region properties

        protected Dictionary<GraphPerformanceCriteria, Int16> _performanceStatement = new Dictionary<GraphPerformanceCriteria, Int16>();

        #endregion
    }
}

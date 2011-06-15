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
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.Commons.VertexStore.Definitions;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    /// <summary>
    /// Abstract class for all expression graphs
    /// </summary>
    public abstract class AExpressionGraph : IExpressionGraph
    {
        #region IExpressionGraph Members

        #region abstract

        public abstract void BuildDifferenceWith(IExpressionGraph anotherGraph);

        public abstract void IntersectWith(IExpressionGraph anotherGraph);

        public abstract void UnionWith(IExpressionGraph anotherGraph);

        public abstract void GatherEdgeWeight(LevelKey StartLevel, LevelKey EndLevel);

        public abstract IExpressionGraph GetNewInstance(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        public abstract void AddNode(IVertex myVertex, LevelKey myIDNode, int backwardResolution);

        public abstract void AddNode(IVertex myVertex, LevelKey myIDNode);

        public abstract Boolean AddEmptyLevel(LevelKey myLevelKey);

        public abstract void AddNodes(IEnumerable<IExpressionNode> iEnumerable, LevelKey myPath);

        public abstract Dictionary<GraphPerformanceCriteria, Int16> GetPerformanceStatement();

        public abstract IExpressionLevel GetLevel(int myLevel);

        public abstract Dictionary<int, IExpressionLevel> Levels { get; }

        public abstract IEnumerable<IVertex> Select(LevelKey myLevelKey, IVertex mySourceVertex = null, Boolean generateLevel = true);

        public abstract IEnumerable<VertexInformation> SelectVertexIDs(LevelKey myLevelKey, IVertex mySourceVertex = null, Boolean generateLevel = true);

        public abstract Boolean IsGraphRelevant(LevelKey myLevelKey, IVertex mySourceVertex);

        public abstract void AddNodesWithComplexRelation(IVertex leftVertex, LevelKey leftLevelKey, IVertex rightVertex, LevelKey rightLevelKey, int backwardResolution);

        public abstract bool ContainsRelevantLevelForType(IVertexType myType);

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

        protected EdgeKey GetBackwardEdgeKey(LevelKey myPath, int desiredBELevel, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            #region data

            IVertexType tempType;
            IAttributeDefinition tempAttr;

            #endregion

            tempType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken, 
                myTransactionToken,
                new RequestGetVertexType(myPath.Edges[desiredBELevel].VertexTypeID), 
                (stats, vertexType) => vertexType);
            
            tempAttr = tempType.GetAttributeDefinition(myPath.Edges[desiredBELevel].AttributeID);

            if (tempAttr.Kind != AttributeType.IncomingEdge)
            {
                return new EdgeKey(tempType.ID, tempAttr.ID);
            }
            else
            {
                IIncomingEdgeDefinition incomingEdgeDefinition = tempAttr as IIncomingEdgeDefinition;

                return new EdgeKey(incomingEdgeDefinition.RelatedEdgeDefinition.SourceVertexType.ID, incomingEdgeDefinition.RelatedEdgeDefinition.ID);
            }
        }

        #endregion

        #region properties

        protected Dictionary<GraphPerformanceCriteria, Int16> _performanceStatement = new Dictionary<GraphPerformanceCriteria, Int16>();

        #endregion
    }
}

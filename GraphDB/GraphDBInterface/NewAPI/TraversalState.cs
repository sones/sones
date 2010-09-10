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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.NewAPI
{

    public class TraversalState
    {
        #region Properties

        /// <summary>
        /// The starting vertex of the traversal
        /// </summary>
        public DBVertex StartNode               { get; private set; }
        
        /// <summary>
        /// The current distance from the StartNode
        /// </summary>
        public UInt64   Depth                   { get; private set; }
        
        /// <summary>
        /// The number of visited vertices
        /// </summary>
        public UInt64   NumberOfVisitedVertices { get { return Convert.ToUInt64(VisitedVertices.Count); } }
        
        /// <summary>
        /// The number of visited edges
        /// </summary>
        public UInt64   NumberOfVisitedEdges    { get { return Convert.ToUInt64(VisitedVertices.Values.Sum(item => item.Count)); } }
       
        /// <summary>
        /// The number of found vertices/paths/...
        /// </summary>
        public UInt64   NumberOfFoundElements   { get; private set; }

        /// <summary>
        /// The datastructure that is responsible for counting the visited vertices and edges (avoidance of circles).
        /// 
        /// Key(ObjectUUID): The ID of the already visited DBVertex
        /// Value(HashSet<ObjectUUID>): A set of ObjectUUIDs which represent IDs of DBEdges via the Key has been reached
        /// </summary>
        public Dictionary<ObjectUUID, HashSet<ObjectUUID>>  VisitedVertices     { get; private set; }

        #endregion

        #region Constructor

        public TraversalState(DBVertex myStartNode)
        {
            StartNode = myStartNode;
            VisitedVertices = new Dictionary<ObjectUUID, HashSet<ObjectUUID>>();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Increase the number of FoundElements
        /// </summary>
        public void IncreaseNumberOfFoundElements()
        {
            NumberOfFoundElements++;
        }

        /// <summary>
        /// Adds a vertex via edge to the traversalstate
        /// </summary>
        /// <param name="aVertex">The vertex that has been traversed</param>
        /// <param name="viaEdge">The edge via the vertex has been traversed</param>
        public void AddVisitedVertexViaEdge(DBVertex aVertex, DBEdge viaEdge)
        {
            if (VisitedVertices.ContainsKey(aVertex.UUID))
            {
                if (viaEdge != null)
                {
                    VisitedVertices[aVertex.UUID].Add(viaEdge.UUID);
                }
            }
            else
            {
                VisitedVertices.Add(aVertex.UUID, new HashSet<ObjectUUID>());

                if (viaEdge != null)
                {
                    VisitedVertices[aVertex.UUID].Add(viaEdge.UUID);
                }
            }
        }

        /// <summary>
        /// Checks whether a vertex has been visited via a certain edge
        /// </summary>
        /// <param name="aEdge">The "via" edge</param>
        /// <returns></returns>
        public bool AlreadyVisitedVertexViaEdge(DBEdge aEdge)
        {
            return VisitedVertices.ContainsKey(aEdge.TargetVertex.UUID) && VisitedVertices[aEdge.TargetVertex.UUID].Contains(aEdge.UUID);
        }

        #endregion
    }

}

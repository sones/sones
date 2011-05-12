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
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Expression;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Request
{

    public enum Avoidance
    {
        None,
        avoidMultiVertexVisit,
        avoidMultiEdgeVisit
    }

    public sealed class TraversalState
    {
        #region Properties

        /// <summary>
        /// The starting vertices of the traversion
        /// </summary>
        public readonly IEnumerable<IVertex> StartNodes;

        /// <summary>
        /// The current distance from the StartNode
        /// Note! Currently not used Note!
        /// </summary>
        public readonly UInt64 Depth;

        /// <summary>
        /// The number of visited vertices
        /// </summary>
        public UInt64 NumberOfVisitedVertices { get { return Convert.ToUInt64(VisitedVertices.Count); } }

        /// <summary>
        /// The number of visited edges
        /// </summary>
        public UInt64 NumberOfVisitedEdges { get { return Convert.ToUInt64(VisitedVertices.Values.Sum(item => item.Count)); } }

        /// <summary>
        /// The number of found vertices/paths/...
        /// </summary>
        public UInt64 NumberOfFoundElements { get; private set; }

        /// <summary>
        /// The datastructure that is responsible for counting the visited vertices and the elements from which they come (vertices or edges) to avoide circles.
        /// 
        /// Key(ID): The ID of the already visited element
        /// Value(HashSet<ID>): A set of IDs which represent IDs of DBEdges or DBVertex via the Key has been reached
        /// </summary>
        public readonly Dictionary<long, HashSet<long>> VisitedVertices;

        /// <summary>
        /// 
        /// </summary>
        public readonly HashSet<long> Visited;

        #endregion

        #region Constructor

        public TraversalState(IEnumerable<IVertex> myStartNodes)
        {
            StartNodes = myStartNodes;
            
            VisitedVertices = new Dictionary<long, HashSet<long>>();
            
            Visited = new HashSet<long>();
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

        public void AddVisited(long myVisitID)
        {
            if (!Visited.Contains(myVisitID))
            {
                Visited.Add(myVisitID);
            }
        }

        /// <summary>
        /// Adds a element ID and the via element ID to the traversalstate
        /// </summary>
        /// <param name="myVisited">The element ID that has been traversed</param>
        /// <param name="myVia">The element ID via the myVisited has been traversed</param>
        public void AddVisited(long myVisited, long myVia)
        {
            if (VisitedVertices.ContainsKey(myVisited))
            {
                VisitedVertices[myVisited].Add(myVia);
            }
            else if (VisitedVertices.ContainsKey(myVia))
            {
                VisitedVertices[myVia].Add(myVisited);
            }
            else
            {
                VisitedVertices.Add(myVisited, new HashSet<long>());

                VisitedVertices[myVisited].Add(myVia);
            }
        }

        public bool AlreadyVisited(long myVisitID)
        {
            return Visited.Contains(myVisitID);
        }

        /// <summary>
        /// Checks whether a vertex has been visited via a certain edge
        /// </summary>
        /// <param name="myIVertex">The vertex to be checked</param>
        /// <param name="myViaVertex">The "via" vertex</param>
        /// <returns></returns>
        public bool AlreadyVisited(long myVisitID, long myViaID)
        {
            //if myIVertex is already visited via myViaVertex OR myViaVertex is already visited via myIVertex return true
            //else false
            return ((VisitedVertices.ContainsKey(myVisitID) && VisitedVertices[myVisitID].Contains(myViaID)) ||
                    (VisitedVertices.ContainsKey(myViaID) && VisitedVertices[myViaID].Contains(myVisitID)));
        }

        #endregion
    }
}

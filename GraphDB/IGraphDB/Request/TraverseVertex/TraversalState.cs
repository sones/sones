using System;
using System.Collections.Generic;
using System.Linq;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class holds statistical informations of traversion
    /// </summary>
    public sealed class TraversalState
    {
        #region Properties

        /// <summary>
        /// The starting vertex of the traversal
        /// </summary>
        public IEnumerable<IVertex> StartNodes { get; private set; }

        /// <summary>
        /// The current distance from the StartNode
        /// </summary>
        public UInt64 Depth { get; private set; }

        /// <summary>
        /// The number of visited vertices
        /// </summary>
        public UInt64 NumberOfVisitedVertices { get { return Convert.ToUInt64(VisitedVertices.Count); } }

        /// <summary>
        /// The number of visited edges
        /// </summary>
        //public UInt64 NumberOfVisitedEdges { get { return Convert.ToUInt64(VisitedVertices.Values.Sum(item => item.Count)); } }

        /// <summary>
        /// The number of found vertices/paths/...
        /// </summary>
        public UInt64 NumberOfFoundElements { get; private set; }

        /// <summary>
        /// The datastructure that is responsible for counting the visited vertices and edges (avoidance of circles).
        /// 
        /// Key(ObjectUUID): The ID of the already visited DBVertex
        /// Value(HashSet<ObjectUUID>): A set of ObjectUUIDs which represent IDs of DBEdges via the Key has been reached
        /// </summary>
        public Dictionary<long, HashSet<long>> VisitedVertices { get; private set; }

        public HashSet<long> Visited { get; private set; }

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

        /// <summary>
        /// Adds a vertex via vertex to the traversalstate
        /// </summary>
        /// <param name="aVertex">The vertex that has been traversed</param>
        /// <param name="viaVertex">The vertex via the aVertex has been traversed</param>
        public void AddVisitedVertexViaVertex(IVertex aVertex, IVertex viaVertex)
        {
            if (aVertex != null && viaVertex != null)
            {
                if (VisitedVertices.ContainsKey(aVertex.VertexID))
                {
                    VisitedVertices[aVertex.VertexID].Add(viaVertex.VertexID);
                }
                else if (VisitedVertices.ContainsKey(viaVertex.VertexID))
                {
                    VisitedVertices[viaVertex.VertexID].Add(aVertex.VertexID);
                }
                else
                {
                    VisitedVertices.Add(aVertex.VertexID, new HashSet<long>());

                    VisitedVertices[aVertex.VertexID].Add(viaVertex.VertexID);
                }
            }
        }

        /// <summary>
        /// Checks whether a vertex has been visited via a certain edge
        /// </summary>
        /// <param name="myIVertex">The vertex to be checked</param>
        /// <param name="myViaVertex">The "via" vertex</param>
        /// <returns></returns>
        public bool AlreadyVisitedVertexViaVertex(IVertex myIVertex, IVertex myViaVertex)
        {
            //if myIVertex is already visited via myViaVertex OR myViaVertex is already visited via myIVertex return true
            //else false
            return ((VisitedVertices.ContainsKey(myIVertex.VertexID) && VisitedVertices[myIVertex.VertexID].Contains(myViaVertex.VertexID)) ||
                    (VisitedVertices.ContainsKey(myViaVertex.VertexID) && VisitedVertices[myViaVertex.VertexID].Contains(myIVertex.VertexID)));
        }

        public void AddVisitedVertex(IVertex myIVertex)
        {
            if(!Visited.Contains(myIVertex.VertexID))
            {
                Visited.Add(myIVertex.VertexID);
            }
        }

        public bool AlreadyVisitedVertex(IVertex myIVertex)
        {
            return Visited.Contains(myIVertex.VertexID);
        }

        #endregion
    }
}

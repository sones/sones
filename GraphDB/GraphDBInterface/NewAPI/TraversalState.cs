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
        public Vertex StartNode               { get; private set; }
        
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

        public TraversalState(Vertex myStartNode)
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
        public void AddVisitedVertexViaEdge(Vertex aVertex, EdgeLabel viaEdge)
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
        public bool AlreadyVisitedVertexViaEdge(EdgeLabel aEdge)
        {
            return VisitedVertices.ContainsKey(aEdge.TargetVertex.UUID) && VisitedVertices[aEdge.TargetVertex.UUID].Contains(aEdge.UUID);
        }

        #endregion
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.NewAPI
{

    public class TraversalState
    {

        #region Properties

        /// <summary>
        /// The starting vertex of the traversal
        /// </summary>
        public IVertex StartNode               { get; private set; }
        
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

        /// <summary>
        /// Errors that occure(d) during traversal
        /// </summary>
        public List<IError> Errors       { get; private set; }
        
        /// <summary>
        /// Warnings that occure(d) during traversal
        /// </summary>
        public List<IWarning> Warnings   { get; private set; }

        #endregion

        #region Constructor

        public TraversalState(IVertex myStartNode)
        {
            StartNode = myStartNode;
            VisitedVertices = new Dictionary<ObjectUUID, HashSet<ObjectUUID>>();
            Errors = new List<IError>();
            Warnings = new List<IWarning>();
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
        public void AddVisitedVertexViaEdge(IVertex aVertex, IEdge viaEdge)
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
        public bool AlreadyVisitedVertexViaEdge(IEdge myIEdge)
        {
            return VisitedVertices.ContainsKey(myIEdge.TargetVertex.UUID) && VisitedVertices[myIEdge.TargetVertex.UUID].Contains(myIEdge.UUID);
        }

        /// <summary>
        /// Adds a new error
        /// </summary>
        /// <param name="myNewError">The error to be added</param>
        public void AddError(IError myNewError)
        {
            Errors.Add(myNewError);
        }

        /// <summary>
        /// Adds a new warning
        /// </summary>
        /// <param name="myNewWarning">The warning to be added</param>
        public void AddWarning(IWarning myNewWarning)
        {
            Warnings.Add(myNewWarning);
        }

        #endregion
    }

}

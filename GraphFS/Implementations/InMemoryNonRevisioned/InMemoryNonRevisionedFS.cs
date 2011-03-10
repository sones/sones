using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sones.GraphFS.Definitions;
using sones.GraphFS.Element;
using sones.GraphFS.Element.Edge;
using sones.GraphFS.Element.Vertex;
using sones.GraphFS.ErrorHandling;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphFS
{
    /// <summary>
    /// The in-memory-store is a non persisitent vertex store without handling any revisions
    /// </summary>
    public sealed class InMemoryNonRevisionedFS : IGraphFS
    {
        /// <summary>
        /// The concurrent datastructure where all the vertices are stored
        /// 
        /// TypeID ( VertexID, IVertex)
        /// </summary>
        private ConcurrentDictionary<Int64, ConcurrentDictionary<Int64, InMemoryVertex>> _vertexStore;

        #region Constructor

        /// <summary>
        /// Creats a new in memory filesystem
        /// </summary>
        public InMemoryNonRevisionedFS()
        {
            Init();
        }

        #endregion

        #region IGraphFS Members

        public bool IsPersistent
        {
            get { return false; }
        }

        public bool HasRevisions
        {
            get { return false; }
        }

        public bool HasEditions
        {
            get { return false; }
        }

        public string GetFileSystemDescription()
        {
            return String.Format("A simple in memory filesystem without any revision or edition handling.");
        }

        public ulong GetNumberOfBytes()
        {
            return 0;
        }

        public ulong GetNumberOfFreeBytes()
        {
            return 0;
        }

        public ulong GrowFileSystem(ulong myNumberOfBytesToAdd)
        {
            return 0;
        }

        public ulong ShrinkFileSystem(ulong myNumberOfBytesToRemove)
        {
            return 0;
        }

        public void WipeFileSystem()
        {
            Init();
        }

        public IEnumerable<IVertex> CloneFileSystem(DateTime myTimeStamp)
        {
            return _vertexStore.Values.
                Select
                (aType => aType.Values.AsParallel().
                              Where
                              (aVertex
                               => aVertex.VertexRevisionID.Timestamp > myTimeStamp)).
                Aggregate((enumerableA, enumerableB) => enumerableA.Union(enumerableB));
        }

        public void ReplicateFileSystem(IEnumerable<IVertex> myReplicationStream)
        {
            var tempVertexStore = new ConcurrentDictionary<long, ConcurrentDictionary<long, InMemoryVertex>>();

            var counter = Int32.MinValue;

            Parallel.ForEach(myReplicationStream, aVertex =>
                                                      {
                                                          if (!tempVertexStore.ContainsKey(aVertex.TypeID))
                                                          {
                                                              tempVertexStore.TryAdd(aVertex.TypeID,
                                                                                     new ConcurrentDictionary<long, InMemoryVertex>());
                                                          }

                                                          tempVertexStore[aVertex.TypeID].TryAdd(aVertex.VertexID,TransferToInMemoryVertex(aVertex));

                                                          Interlocked.Increment(ref counter);
                                                      });

            _vertexStore = tempVertexStore;
        }

        public bool VertexExists(long myVertexID, long myVertexTypeID, string myEdition = null,
                                 VertexRevisionID myVertexRevisionID = null)
        {
            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                return vertices.ContainsKey(myVertexID);
            }

            return false;
        }

        public IVertex GetVertex(long myVertexID, long myVertexTypeID, string myEdition = null,
                                 VertexRevisionID myVertexRevisionID = null)
        {
            return GetVertexPrivate(myVertexID, myVertexTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs = null,
                                                        Func<string, bool> myEditionsFilterFunc = null,
                                                        Func<VertexRevisionID, bool> myInterestingRevisionIDFilterFunc =
                                                            null)
        {
            return myInterestingVertexIDs != null
                       ? GetVerticesByTypeID(myTypeID, myInterestingVertexIDs)
                       : GetVerticesByTypeID(myTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs = null,
                                                        IEnumerable<string> myInterestingEditionNames = null,
                                                        IEnumerable<VertexRevisionID> myInterestingRevisionIDs = null)
        {
            return myInterestingVertexIDs != null
                       ? GetVerticesByTypeID(myTypeID, myInterestingVertexIDs)
                       : GetVerticesByTypeID(myTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs)
        {
            var interestingVertices = new HashSet<long>(myInterestingVertexIDs);

            return GetVerticesByTypeID(myTypeID).Where(aVertex => interestingVertices.Contains(aVertex.VertexID));
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myVertexTypeID)
        {
            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                foreach (var aVertex in vertices)
                {
                    yield return aVertex.Value;
                }
            }

            yield break;
        }


        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID,
                                                        IEnumerable<VertexRevisionID> myInterestingRevisions)
        {
            return GetVerticesByTypeID(myTypeID);
        }

        public IEnumerable<string> GetVertexEditions(long myVertexID, long myVertexTypeID)
        {
            var vertex = GetVertex(myVertexID, myVertexTypeID);

            if (vertex == null)
            {
                throw new VertexDoesNotExistException(myVertexTypeID, myVertexID);
            }

            List<String> result = new List<string> {vertex.EditionName};

            return result;
        }

        public IEnumerable<VertexRevisionID> GetVertexRevisionIDs(long myVertexID, long myVertexTypeID,
                                                                  IEnumerable<string> myInterestingEditions = null)
        {
            var vertex = GetVertex(myVertexID, myVertexTypeID);

            if (vertex == null)
            {
                throw new VertexDoesNotExistException(myVertexTypeID, myVertexID);
            }

            var result = new List<VertexRevisionID>();

            if (myInterestingEditions != null)
            {
                if (myInterestingEditions.Contains(vertex.EditionName))
                {
                    result.Add(vertex.VertexRevisionID);
                }
            }
            else
            {
                result.Add(vertex.VertexRevisionID);
            }


            return result;
        }

        public bool RemoveVertexRevision(long myVertexID, long myVertexTypeID, string myInterestingEdition,
                                         VertexRevisionID myToBeRemovedRevisionID)
        {
            var vertex = GetVertexPrivate(myVertexID, myVertexTypeID);

            if (vertex != null)
            {
                if ((vertex.EditionName == myInterestingEdition) && (vertex.VertexRevisionID == myToBeRemovedRevisionID))
                {
                    ConcurrentDictionary<Int64, InMemoryVertex> vertices;

                    if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
                    {
                        return vertices.TryRemove(vertex.VertexID, out vertex);
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool RemoveVertexEdition(long myVertexID, long myVertexTypeID, string myToBeRemovedEdition)
        {
            var vertex = GetVertexPrivate(myVertexID, myVertexTypeID);

            if (vertex != null)
            {
                if (vertex.EditionName == myToBeRemovedEdition)
                {
                    ConcurrentDictionary<Int64, InMemoryVertex> vertices;

                    if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
                    {
                        return vertices.TryRemove(vertex.VertexID, out vertex);
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool RemoveVertex(long myVertexID, long myVertexTypeID)
        {
            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                InMemoryVertex vertex;
                return vertices.TryRemove(myVertexID, out vertex);
            }

            return false;
        }

        public void AddVertex(VertexAddDefinition myVertexDefinition,
                               VertexRevisionID myVertexRevisionID = null)
        {
            //check for vertex type
            if (!_vertexStore.ContainsKey(myVertexDefinition.GraphElementInformation.TypeID))
            {
                _vertexStore.TryAdd(myVertexDefinition.GraphElementInformation.TypeID,
                                    new ConcurrentDictionary<long, InMemoryVertex>());
            }

            #region create new vertex

            var vertexRevisionID = new VertexRevisionID(myVertexDefinition.GraphElementInformation.ModificationDate);
            var graphElementInformation = new InMemoryGraphElementInformation(myVertexDefinition.GraphElementInformation);

            #region process edges

            Dictionary<Int64, IEdge> edges = null;

            if (myVertexDefinition.OutgoingSingleEdges != null || myVertexDefinition.OutgoingHyperEdges != null)
            {
                edges = new Dictionary<long, IEdge>();
                SingleEdge singleEdge;

                #region single edges

                if (myVertexDefinition.OutgoingSingleEdges != null)
                {
                    foreach (var aSingleEdgeDefinition in myVertexDefinition.OutgoingSingleEdges)
                    {
                        //create the new Edge
                        singleEdge = new SingleEdge(aSingleEdgeDefinition.Value, GetVertexPrivate);
                        UpdateIncomingEdgesOnTargetVertex(aSingleEdgeDefinition.Value.TargetVertexInformation,
                                                          myVertexDefinition.GraphElementInformation.TypeID,
                                                          aSingleEdgeDefinition.Key,
                                                          singleEdge);

                        edges.Add(aSingleEdgeDefinition.Key, singleEdge);
                    }
                }

                #endregion

                #region hyper edges

                if (myVertexDefinition.OutgoingHyperEdges != null)
                {
                    HashSet<SingleEdge> containedSingleEdges;

                    foreach (var aHyperEdgeDefinition in myVertexDefinition.OutgoingHyperEdges)
                    {
                        containedSingleEdges = new HashSet<SingleEdge>();

                        foreach (var aSingleEdgeDefinition in aHyperEdgeDefinition.Value.ContainedSingleEdges)
                        {
                            singleEdge = new SingleEdge(aSingleEdgeDefinition, GetVertexPrivate);

                            UpdateIncomingEdgesOnTargetVertex(aSingleEdgeDefinition.TargetVertexInformation,
                                                              myVertexDefinition.GraphElementInformation.TypeID,
                                                              aHyperEdgeDefinition.Key,
                                                              singleEdge);

                            containedSingleEdges.Add(singleEdge);
                        }

                        //create the new edge
                        edges.Add(aHyperEdgeDefinition.Key, new HyperEdge(containedSingleEdges, aHyperEdgeDefinition.Value.GraphElementInformation, aHyperEdgeDefinition.Value.SourceVertex, GetVertexPrivate));

                    }
                }

                #endregion
            }

            #endregion

            var vertex = new InMemoryVertex(myVertexDefinition.VertexID, vertexRevisionID, myVertexDefinition.Edition, myVertexDefinition.BinaryProperties, edges, graphElementInformation);

            #endregion

            //store the new vertex
            if (!_vertexStore[myVertexDefinition.GraphElementInformation.TypeID].TryAdd(vertex.VertexID, vertex))
            {
                throw new VertexAlreadyExistException(myVertexDefinition.GraphElementInformation.TypeID,
                                                      myVertexDefinition.VertexID);
            }
        }

        public void UpdateVertex(long myToBeUpdatedVertexID, long myCorrespondingVertexTypeID,
                                 VertexUpdateDefinition myVertexUpdate, string myToBeUpdatedEditions = null,
                                 VertexRevisionID myToBeUpdatedRevisionIDs = null, bool myCreateNewRevision = false)
        {
            var toBeUpdatedVertex = GetVertexPrivate(myToBeUpdatedVertexID, myCorrespondingVertexTypeID);

            if (toBeUpdatedVertex == null)
            {
                throw new VertexDoesNotExistException(myCorrespondingVertexTypeID, myToBeUpdatedVertexID);
            }
            
            _vertexStore[myCorrespondingVertexTypeID][myToBeUpdatedVertexID] =
                UpdateVertex_private(toBeUpdatedVertex, myVertexUpdate);
        }

        #endregion

        #region private helper

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myTargetVertexInformation"></param>
        /// <param name="myIncomingVertexTypeID"></param>
        /// <param name="myIncomingEdgeID"></param>
        /// <param name="mySingleEdge"></param>
        private void UpdateIncomingEdgesOnTargetVertex(VertexInformation myTargetVertexInformation, Int64 myIncomingVertexTypeID, Int64 myIncomingEdgeID, SingleEdge mySingleEdge)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes the fs
        /// </summary>
        private void Init()
        {
            _vertexStore = new ConcurrentDictionary<long, ConcurrentDictionary<long, InMemoryVertex>>();
        }

        /// <summary>
        /// Updates an InMemoryVertex
        /// </summary>
        /// <param name="toBeUpdatedVertex">The vertex that should be updated</param>
        /// <param name="myVertexUpdate">The definition of the vertex update</param>
        /// <returns>The updated vertex</returns>
        private InMemoryVertex UpdateVertex_private(InMemoryVertex toBeUpdatedVertex,
                                                    VertexUpdateDefinition myVertexUpdate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new InMemoryVertex from an IVertex
        /// </summary>
        /// <param name="aVertex">The IVertex that is going to be transfered</param>
        /// <returns>An InMemoryVertex implementation of an IVertex</returns>
        private InMemoryVertex TransferToInMemoryVertex(IVertex aVertex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a vertex from the vertex store
        /// </summary>
        /// <param name="myVertexID">The interesting vertex id</param>
        /// <param name="myVertexTypeID">The interesting vertex type id</param>
        /// <returns>An InMemoryVertex, or null if there is no such vertex</returns>
        private InMemoryVertex GetVertexPrivate(long myVertexID, long myVertexTypeID)
        {
            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                InMemoryVertex vertex;

                if (vertices.TryGetValue(myVertexID, out vertex))
                {
                    return vertex;
                }
            }

            return null;
        }

        #endregion
    }
}
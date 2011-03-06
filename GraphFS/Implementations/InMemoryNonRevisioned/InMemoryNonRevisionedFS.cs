using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sones.GraphFS.Definitions;
using sones.GraphFS.Element;
using sones.GraphFS.Element.Vertex;
using sones.GraphFS.ErrorHandling;
using sones.PropertyHyperGraph;

namespace sones.GraphFS
{
    /// <summary>
    /// The in-memory-store is a non persisitent vertex store without handling any revisions
    /// </summary>
    public sealed class InMemoryNonRevisionedFS : IGraphFS
    {
        private Int64 _currentID;

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
            _vertexStore = new ConcurrentDictionary<long, ConcurrentDictionary<long, InMemoryVertex>>();
            _currentID = Int64.MinValue;
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
            return String.Format("A simple in memory fi^lesystem without any revision or edition handling.");
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
            _vertexStore = new ConcurrentDictionary<long, ConcurrentDictionary<long, InMemoryVertex>>();
        }

        public IEnumerable<IVertex> CloneFileSystem(Int64 myTimeStamp = 0L)
        {
            return _vertexStore.Values.
                Select
                (aType => aType.Values.AsParallel().
                              Where
                              (aVertex
                               => aVertex.VertexRevisionID.Timestamp > myTimeStamp)).
                Aggregate((EnumerableA, EnumerableB) => EnumerableA.Union(EnumerableB));
        }

        public void ReplicateFileSystem(IEnumerable<IVertex> myReplicationStream)
        {
            var tempVertexStore = new ConcurrentDictionary<long, ConcurrentDictionary<long, InMemoryVertex>>();

            Parallel.ForEach(myReplicationStream, aVertex =>
                                                      {
                                                          if (!tempVertexStore.ContainsKey(aVertex.TypeID))
                                                          {
                                                              tempVertexStore.TryAdd(aVertex.TypeID,
                                                                                     new ConcurrentDictionary
                                                                                         <long, InMemoryVertex>());
                                                          }

                                                          tempVertexStore[aVertex.TypeID].TryAdd(aVertex.VertexID,
                                                                                                 TransferToInMemoryVertex
                                                                                                     (aVertex));
                                                      });

            _vertexStore = tempVertexStore;
        }

        public bool VertexExists(long myVertexID, long myVertexTypeID, string myEdition = null,
                                 VertexRevisionID myVertexRevisionID = null)
        {
            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                InMemoryVertex vertex;

                if (vertices.TryGetValue(myVertexID, out vertex))
                {
                    //we found the vertex and return ... at this point we do not care about revisions or editions, because this filesystem 
                    //does not implement those structures
                    return true;
                }
            }

            return false;
        }

        public IVertex GetVertex(long myVertexID, long myVertexTypeID, string myEdition = null,
                                 VertexRevisionID myVertexRevisionID = null)
        {
            return GetVertex_private(myVertexID, myVertexTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs = null,
                                                        Func<string, bool> myEditionsFilterFunc = null,
                                                        Func<VertexRevisionID, bool> myInterestingRevisionIDFilterFunc =
                                                            null)
        {
            return myInterestingVertexIDs != null ? GetVerticesByTypeID(myTypeID, myInterestingVertexIDs) : GetVerticesByTypeID(myTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs = null,
                                                        IEnumerable<string> myInterestingEditionNames = null,
                                                        IEnumerable<VertexRevisionID> myInterestingRevisionIDs = null)
        {
            return myInterestingVertexIDs != null ? GetVerticesByTypeID(myTypeID, myInterestingVertexIDs) : GetVerticesByTypeID(myTypeID);
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
            var result = new List<String>();

            var vertex = GetVertex(myVertexID, myVertexTypeID);

            if (vertex != null)
            {
                result.Add(vertex.EditionName);
            }

            return result;
        }

        public IEnumerable<VertexRevisionID> GetVertexRevisionIDs(long myVertexID, long myVertexTypeID,
                                                                  IEnumerable<string> myInterestingEditions = null)
        {
            var result = new List<VertexRevisionID>();

            var vertex = GetVertex(myVertexID, myVertexTypeID);

            if (vertex != null)
            {
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
            }

            return result;
        }

        public bool RemoveVertexRevision(long myVertexID, long myVertexTypeID, string myInterestingEdition,
                                         VertexRevisionID myToBeRemovedRevisionID)
        {
            var vertex = GetVertex_private(myVertexID, myVertexTypeID);

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
            var vertex = GetVertex_private(myVertexID, myVertexTypeID);

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

        public Int64 AddVertex(VertexAddDefinition myVertexDefinition, string myEdition = null,
                               VertexRevisionID myVertexRevisionID = null)
        {
            if (!_vertexStore.ContainsKey(myVertexDefinition.GraphElementInformation.TypeID))
            {
                _vertexStore.TryAdd(myVertexDefinition.GraphElementInformation.TypeID,
                                    new ConcurrentDictionary<long, InMemoryVertex>());
            }

            var vertex = TransferToInMemoryVertex(myVertexDefinition);

            _vertexStore[myVertexDefinition.GraphElementInformation.TypeID].TryAdd(vertex.VertexID, vertex);

            return vertex.VertexID;
        }

        public void UpdateVertex(long myToBeUpdatedVertexID, long myCorrespondingVertexTypeID,
                                 VertexUpdateDefinition myVertexUpdate, string myToBeUpdatedEditions = null,
                                 VertexRevisionID myToBeUpdatedRevisionIDs = null, bool myCreateNewRevision = false)
        {
            var toBeUpdatedVertex = GetVertex_private(myToBeUpdatedVertexID, myCorrespondingVertexTypeID);

            if (toBeUpdatedVertex == null)
            {
                throw new VertexDoesNotExistException(myCorrespondingVertexTypeID, myToBeUpdatedVertexID);
            }
            else
            {
                _vertexStore[myCorrespondingVertexTypeID][myToBeUpdatedVertexID] =
                    UpdateVertex_private(toBeUpdatedVertex, myVertexUpdate);
            }
        }

        #endregion

        #region private helper

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
        /// Creates a new InMemoryVertex from a VertexDefinition
        /// 
        /// It also Generates the VertexID
        /// </summary>
        /// <param name="myVertexDefinition">The definition of the vertex that is going to be creates</param>
        /// <returns>The resulting InMemoryVertex</returns>
        private InMemoryVertex TransferToInMemoryVertex(VertexAddDefinition myVertexDefinition)
        {
            throw new NotImplementedException();

            return new InMemoryVertex(GetNextVertexID(), new VertexRevisionID(DateTime.UtcNow), myVertexDefinition);
        }

        /// <summary>
        /// Gets the nexct vertex id
        /// </summary>
        /// <returns>A vertexID</returns>
        private long GetNextVertexID()
        {
            return Interlocked.Increment(ref _currentID);
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
        private InMemoryVertex GetVertex_private(long myVertexID, long myVertexTypeID)
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
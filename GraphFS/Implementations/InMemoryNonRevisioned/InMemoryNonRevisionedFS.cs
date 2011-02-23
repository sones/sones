using System;
using System.Linq;
using System.Collections.Generic;
using sones.GraphFS;
using sones.PropertyHyperGraph;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using sones.GraphFS.Element;
using sones.GraphDB.ErrorHandling;

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
        private ConcurrentDictionary<UInt64, ConcurrentDictionary<UInt64,InMemoryVertex>> _vertexStore;

        #region Constructor

        /// <summary>
        /// Creats a new in memory filesystem
        /// </summary>
        public InMemoryNonRevisionedFS()
        {
            _vertexStore = new ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, InMemoryVertex>>();
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
            _vertexStore = new ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, InMemoryVertex>>();
        }

        public IEnumerable<IVertex> CloneFileSystem(ulong myTimeStamp = 0UL)
        {
            return _vertexStore.Values.
                Select 
                (aType => aType.Values.AsParallel().
                    Where 
                    (aVertex 
                        => aVertex.VertexRevisionID.Timestamp > myTimeStamp)).
                        Aggregate((EnumerableA, EnumerableB) =>
                            {
                                return EnumerableA.Union(EnumerableB);
                            });
        }

        public void ReplicateFileSystem(IEnumerable<IVertex> myReplicationStream)
        {
            var tempVertexStore = new ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, InMemoryVertex>>();

            Parallel.ForEach(myReplicationStream, aVertex =>
                {
                    if (!tempVertexStore.ContainsKey(aVertex.TypeID))
                    {
                        tempVertexStore.TryAdd(aVertex.TypeID, new ConcurrentDictionary<ulong, InMemoryVertex>());
                    }

                    tempVertexStore[aVertex.TypeID].TryAdd(aVertex.VertexID, TransferToInMemoryVertex(aVertex));
                });

            _vertexStore = tempVertexStore;
        }

        public bool VertexExists(ulong myVertexID, ulong myVertexTypeID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            ConcurrentDictionary<UInt64, InMemoryVertex> vertices = null;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                InMemoryVertex vertex = null;

                if (vertices.TryGetValue(myVertexID, out vertex))
                {
                    //we found the vertex and return ... at this point we do not care about revisions or editions, because this filesystem 
                    //does not implement those structures
                    return true;
                }
            }

            return false;
        }

        public IVertex GetVertex(ulong myVertexID, ulong myVertexTypeID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            return GetVertex_private(myVertexID, myVertexTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<ulong> myInterestingVertexIDs = null, Func<string, bool> myEditionsFilterFunc = null, Func<VertexRevisionID, bool> myInterestingRevisionIDFilterFunc = null)
        {
            if (myInterestingVertexIDs != null)
            {
                return GetVerticesByTypeID(myTypeID, myInterestingVertexIDs);
            }
            else
            {
                return GetVerticesByTypeID(myTypeID);
            }
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<ulong> myInterestingVertexIDs = null, IEnumerable<string> myInterestingEditionNames = null, IEnumerable<VertexRevisionID> myInterestingRevisionIDs = null)
        {
            if (myInterestingVertexIDs != null)
            {
                return GetVerticesByTypeID(myTypeID, myInterestingVertexIDs);
            }
            else
            {
                return GetVerticesByTypeID(myTypeID);
            }
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<ulong> myInterestingVertexIDs)
        {
            var interestingVertices = new HashSet<ulong>(myInterestingVertexIDs);

            return GetVerticesByTypeID(myTypeID).Where(aVertex => interestingVertices.Contains(aVertex.VertexID));
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myVertexTypeID)
        {
            ConcurrentDictionary<UInt64, InMemoryVertex> vertices = null;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                foreach (var aVertex in vertices)
                {
                    yield return aVertex.Value;
                }
            }

            yield break;
        }


        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<VertexRevisionID> myInterestingRevisions)
        {
            return GetVerticesByTypeID(myTypeID);
        }

        public IEnumerable<string> GetVertexEditions(ulong myVertexID, ulong myVertexTypeID)
        {
            var result = new List<String>();

            var vertex = GetVertex(myVertexID, myVertexTypeID);

            if (vertex != null)
            {
                result.Add(vertex.EditionName);
            }

            return result;
        }

        public IEnumerable<VertexRevisionID> GetVertexRevisionIDs(ulong myVertexID, ulong myVertexTypeID, IEnumerable<string> myInterestingEditions = null)
        {
            var result = new List<VertexRevisionID>();            

            var vertex = GetVertex(myVertexID, myVertexTypeID);

            if (vertex != null)
            {
                if (myInterestingEditions.Contains(vertex.EditionName))
                {
                    result.Add(vertex.VertexRevisionID);
                }
            }

            return result;
        }

        public bool RemoveVertexRevision(ulong myVertexID, ulong myVertexTypeID, string myInterestingEdition, VertexRevisionID myToBeRemovedRevisionID)
        {
            InMemoryVertex vertex = GetVertex_private(myVertexID, myVertexTypeID);

            if (vertex != null)
            {
                if ((vertex.EditionName == myInterestingEdition) && (vertex.VertexRevisionID == myToBeRemovedRevisionID))
                {
                    ConcurrentDictionary<UInt64, InMemoryVertex> vertices = null;

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

        public bool RemoveVertexEdition(ulong myVertexID, ulong myVertexTypeID, string myToBeRemovedEdition)
        {
            var vertex = GetVertex_private(myVertexID, myVertexTypeID);

            if (vertex != null)
            {
                if (vertex.EditionName == myToBeRemovedEdition)
                {
                    ConcurrentDictionary<UInt64, InMemoryVertex> vertices = null;

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

        public bool RemoveVertex(ulong myVertexID, ulong myVertexTypeID)
        {
            ConcurrentDictionary<UInt64, InMemoryVertex> vertices = null;
            InMemoryVertex vertex = null;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                return vertices.TryRemove(myVertexID, out vertex);
            }

            return false;
        }

        public UInt64 AddVertex(VertexDefinition myVertexDefinition, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            if (!_vertexStore.ContainsKey(myVertexDefinition.TypeID))
            {
                _vertexStore.TryAdd(myVertexDefinition.TypeID, new ConcurrentDictionary<ulong, InMemoryVertex>());
            }

            var vertex = TransferToInMemoryVertex(myVertexDefinition);

            _vertexStore[myVertexDefinition.TypeID].TryAdd(vertex.VertexID, vertex);
            
            return vertex.VertexID;
        }

        public void UpdateVertex(ulong myToBeUpdatedVertexID, ulong myCorrespondingVertexTypeID, VertexUpdateDefinition myVertexUpdate, string myToBeUpdatedEditions = null, VertexRevisionID myToBeUpdatedRevisionIDs = null, bool myCreateNewRevision = false)
        {
            var toBeUpdatedVertex = GetVertex_private(myToBeUpdatedVertexID, myCorrespondingVertexTypeID);

            if (toBeUpdatedVertex == null)
            {
                throw new VertexDoesNotExistException(myCorrespondingVertexTypeID, myToBeUpdatedVertexID);
            }
            else
            {
                _vertexStore[myCorrespondingVertexTypeID][myToBeUpdatedVertexID] = UpdateVertex_private(toBeUpdatedVertex, myVertexUpdate);
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
        private InMemoryVertex UpdateVertex_private(InMemoryVertex toBeUpdatedVertex, VertexUpdateDefinition myVertexUpdate)
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
        private InMemoryVertex TransferToInMemoryVertex(VertexDefinition myVertexDefinition)
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
        private InMemoryVertex GetVertex_private(ulong myVertexID, ulong myVertexTypeID)
        {
            ConcurrentDictionary<UInt64, InMemoryVertex> vertices = null;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                InMemoryVertex vertex = null;

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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using sones.GraphFS.Element.Edge;
using sones.GraphFS.Element.Vertex;
using sones.GraphFS.ErrorHandling;
using sones.Library.PropertyHyperGraph;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;
using sones.Library.Commons.VertexStore;
using sones.Library.Commons.VertexStore.Definitions;

namespace sones.GraphFS
{
    /// <summary>
    /// The in-memory-store is a non persisitent vertex store without handling any revisions
    /// </summary>
    public sealed class InMemoryNonRevisionedFS : IGraphFS
    {
        #region data

        /// <summary>
        /// The concurrent datastructure where all the vertices are stored
        /// 
        /// TypeID ( VertexID, IVertex)
        /// </summary>
        private ConcurrentDictionary<Int64, ConcurrentDictionary<Int64, InMemoryVertex>> _vertexStore;

        #endregion

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

        public bool IsTransactional
        {
            get { return false; }
        }

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
            var timeStamp = myTimeStamp.ToBinary();

            return _vertexStore.Values.
                Select
                (aType => aType.Values.AsParallel().
                              Where
                              (aVertex
                               => aVertex.VertexRevisionID > timeStamp)).
                Aggregate((enumerableA, enumerableB) => enumerableA.Union(enumerableB));
        }

        public void ReplicateFileSystem(IEnumerable<IVertex> myReplicationStream, Boolean myAppend = false)
        {
            var tempVertexStore = new ConcurrentDictionary<long, ConcurrentDictionary<long, InMemoryVertex>>();

            Parallel.ForEach(myReplicationStream, aVertex =>
                                                      {
                                                          if (!tempVertexStore.ContainsKey(aVertex.VertexTypeID))
                                                          {
                                                              tempVertexStore.TryAdd(aVertex.VertexTypeID,
                                                                                     new ConcurrentDictionary<long, InMemoryVertex>());
                                                          }

                                                          tempVertexStore[aVertex.VertexTypeID].TryAdd(aVertex.VertexID,TransferToInMemoryVertex(aVertex));
                                                      });

            _vertexStore = tempVertexStore;
        }

        public bool VertexExists(long myVertexID, long myVertexTypeID, string myEdition = null,
                                 Int64 myVertexRevisionID = 0L)
        {
            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                return vertices.ContainsKey(myVertexID) && !vertices[myVertexID].IsBulkVertex;
            }

            return false;
        }

        public IVertex GetVertex(long myVertexID, long myVertexTypeID, string myEdition = null,
                                 Int64 myVertexRevisionID = 0L)
        {
            var vertex = GetVertexPrivate(myVertexID, myVertexTypeID);

            if (vertex != null && !vertex.IsBulkVertex)
            {
                return vertex;
            }

            return null;
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs = null,
                                                        IEnumerable<string> myInterestingEditionNames = null,
                                                        IEnumerable<Int64> myInterestingRevisionIDs = null)
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

        public IEnumerable<IVertex> GetVerticesByTypeID(
            long myTypeID, 
            IEnumerable<long> myInterestingVertexIDs = null, 
            VertexStoreFilter.EditionFilter myEditionsFilterFunc = null, 
            VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc = null)
        {
            return myInterestingVertexIDs != null
                       ? GetVerticesByTypeID(myTypeID, myInterestingVertexIDs)
                       : GetVerticesByTypeID(myTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(
            long myTypeID, 
            string myEdition = null, 
            VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc = null)
        {
            return GetVerticesByTypeID(myTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myVertexTypeID)
        {
            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                foreach (var aVertex in vertices)
                {
                    if (!aVertex.Value.IsBulkVertex)
                    {
                        yield return aVertex.Value;                        
                    }
                }
            }

            yield break;
        }


        public IEnumerable<IVertex> GetVerticesByTypeIDAndRevisions(long myTypeID,
                                                        IEnumerable<Int64> myInterestingRevisions)
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

            var result = new List<string> {vertex.EditionName};

            return result;
        }

        public IEnumerable<Int64> GetVertexRevisionIDs(long myVertexID, long myVertexTypeID,
                                                                  IEnumerable<string> myInterestingEditions = null)
        {
            var vertex = GetVertex(myVertexID, myVertexTypeID);

            if (vertex == null)
            {
                throw new VertexDoesNotExistException(myVertexTypeID, myVertexID);
            }

            var result = new List<Int64>();

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
                                         Int64 myToBeRemovedRevisionID)
        {
            var vertex = GetVertexPrivate(myVertexID, myVertexTypeID);

            if (vertex != null && !vertex.IsBulkVertex)
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

            if (vertex != null && !vertex.IsBulkVertex)
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
            //Todo: remove references on this vertex

            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                InMemoryVertex vertex;
                return vertices.TryRemove(myVertexID, out vertex);
            }

            return false;
        }

        public IVertex AddVertex(VertexAddDefinition myVertexDefinition,
                              Int64 myVertexRevisionID = 0L,
                              Boolean myCreateIncomingEdges = true)
        {
            #region create vertex type entry

            //check for vertex type
            if (!_vertexStore.ContainsKey(myVertexDefinition.VertexTypeID))
            {
                _vertexStore.TryAdd(myVertexDefinition.VertexTypeID,
                                    new ConcurrentDictionary<long, InMemoryVertex>());
            }

            #endregion

            #region create new vertex

            var vertexRevisionID = 0L;

            Dictionary<Int64, Stream> binaryProperties;
            if (myVertexDefinition.BinaryProperties == null)
            {
                binaryProperties = null;
            }
            else
            {
                binaryProperties = myVertexDefinition.BinaryProperties.ToDictionary(key => key.PropertyID,
                                                                                    value => value.Stream);
            }

            Boolean addEdges = myVertexDefinition.OutgoingSingleEdges != null || myVertexDefinition.OutgoingHyperEdges != null;
            Dictionary<Int64, IEdge> edges = null;
            if (addEdges)
            {
                edges = new Dictionary<long, IEdge>();
            }

            InMemoryVertex toBeAddedVertex = new InMemoryVertex(
                myVertexDefinition.VertexID, 
                myVertexDefinition.VertexTypeID, 
                vertexRevisionID, 
                myVertexDefinition.Edition, 
                binaryProperties, 
                edges, 
                myVertexDefinition.Comment, 
                myVertexDefinition.CreationDate, 
                myVertexDefinition.ModificationDate, 
                myVertexDefinition.StructuredProperties, 
                myVertexDefinition.UnstructuredProperties);

            #endregion

            #region process edges

            if (addEdges)
            {

                SingleEdge singleEdge;
                InMemoryVertex targetVertex;

                #region single edges

                //create the single edges

                if (myVertexDefinition.OutgoingSingleEdges != null)
                {
                    foreach (var aSingleEdgeDefinition in myVertexDefinition.OutgoingSingleEdges)
                    {
                        targetVertex =
                            GetOrCreateTargetVertex(aSingleEdgeDefinition.TargetVertexInformation.VertexTypeID,
                                                    aSingleEdgeDefinition.TargetVertexInformation.VertexID);

                        //create the new Edge
                        singleEdge = new SingleEdge(aSingleEdgeDefinition.PropertyID, toBeAddedVertex, targetVertex,
                                                    aSingleEdgeDefinition.Comment, aSingleEdgeDefinition.CreationDate,
                                                    aSingleEdgeDefinition.ModificationDate,
                                                    aSingleEdgeDefinition.StructuredProperties,
                                                    aSingleEdgeDefinition.UnstructuredProperties);

                        CreateOrUpdateIncomingEdgesOnVertex(
                            targetVertex,
                            myVertexDefinition.VertexTypeID,
                            aSingleEdgeDefinition.PropertyID,
                            toBeAddedVertex);

                        edges.Add(aSingleEdgeDefinition.PropertyID, singleEdge);
                    }
                }

                #endregion

                #region hyper edges

                if (myVertexDefinition.OutgoingHyperEdges != null)
                {
                    foreach (var aHyperEdgeDefinition in myVertexDefinition.OutgoingHyperEdges)
                    {
                        List<SingleEdge> containedSingleEdges = new List<SingleEdge>();

                        foreach (var aSingleEdgeDefinition in aHyperEdgeDefinition.ContainedSingleEdges)
                        {
                            targetVertex =
                                GetOrCreateTargetVertex(aSingleEdgeDefinition.TargetVertexInformation.VertexTypeID,
                                                        aSingleEdgeDefinition.TargetVertexInformation.VertexID);

                            singleEdge = new SingleEdge(aSingleEdgeDefinition.PropertyID, toBeAddedVertex, targetVertex,
                                                        aSingleEdgeDefinition.Comment,
                                                        aSingleEdgeDefinition.CreationDate,
                                                        aSingleEdgeDefinition.ModificationDate,
                                                        aSingleEdgeDefinition.StructuredProperties,
                                                        aSingleEdgeDefinition.UnstructuredProperties);

                            CreateOrUpdateIncomingEdgesOnVertex(
                                targetVertex,
                                myVertexDefinition.VertexTypeID,
                                aSingleEdgeDefinition.PropertyID,
                                toBeAddedVertex);

                            containedSingleEdges.Add(singleEdge);
                        }

                        //create the new edge
                        edges.Add(
                            aHyperEdgeDefinition.PropertyID,
                            new HyperEdge(
                                containedSingleEdges,
                                aHyperEdgeDefinition.EdgeTypeID,
                                toBeAddedVertex,
                                aHyperEdgeDefinition.Comment,
                                aHyperEdgeDefinition.CreationDate,
                                aHyperEdgeDefinition.ModificationDate,
                                aHyperEdgeDefinition.StructuredProperties,
                                aHyperEdgeDefinition.UnstructuredProperties));

                    }
                }

                #endregion

            }

            #endregion

            #region store the new vertex

            _vertexStore[myVertexDefinition.VertexTypeID].
                AddOrUpdate(toBeAddedVertex.VertexID,
                            toBeAddedVertex,
                            (id, oldVertex) =>
                            {
                                if (!oldVertex.IsBulkVertex)
                                {
                                    throw new VertexAlreadyExistException(myVertexDefinition.VertexTypeID, myVertexDefinition.VertexID);
                                }
                                
                                toBeAddedVertex.IncomingEdges = oldVertex.IncomingEdges;

                                return toBeAddedVertex;
                            });

            #endregion

            return toBeAddedVertex;
        }

        public IVertex UpdateVertex(long myToBeUpdatedVertexID, long myCorrespondingVertexTypeID,
                                 VertexUpdateDefinition myVertexUpdate, string myToBeUpdatedEditions = null,
                                 Int64 myToBeUpdatedRevisionIDs = 0L, bool myCreateNewRevision = false)
        {
            var toBeUpdatedVertex = GetVertexPrivate(myToBeUpdatedVertexID, myCorrespondingVertexTypeID);

            if (toBeUpdatedVertex == null || toBeUpdatedVertex.IsBulkVertex)
            {
                throw new VertexDoesNotExistException(myCorrespondingVertexTypeID, myToBeUpdatedVertexID);
            }
            
            _vertexStore[myCorrespondingVertexTypeID][myToBeUpdatedVertexID] =
                UpdateVertex_private(toBeUpdatedVertex, myVertexUpdate);

            return null;
        }

        #endregion

        #region IPluginable Members

        public String PluginName
        {
            get { return "InMemoryNonRevisionedFS"; }
        }

        public Dictionary<String, Type> SetableParameters
        {
            get { return new Dictionary<string, Type>(); }
        }

        public IPluginable InitializePlugin(Dictionary<String, Object> myParameters)
        {
            return new InMemoryNonRevisionedFS();
        }

        #endregion

        #region private helper

        /// <summary>
        /// Gets or creates the target vertex of an edge
        /// </summary>
        /// <param name="myTargetVertexTypeID">The target vertex type id</param>
        /// <param name="myTargetVertexID">The target vertex id</param>
        /// <returns>The target vertex</returns>
        private InMemoryVertex GetOrCreateTargetVertex(Int64 myTargetVertexTypeID, Int64 myTargetVertexID)
        {
            if (!_vertexStore.ContainsKey(myTargetVertexTypeID))
            {
                _vertexStore.TryAdd(myTargetVertexTypeID,
                                    new ConcurrentDictionary<long, InMemoryVertex>());
            }

            InMemoryVertex targetVertex;

            if (_vertexStore[myTargetVertexTypeID].TryGetValue(myTargetVertexID, out targetVertex))
            {
                return targetVertex;
            }

            targetVertex = _vertexStore[myTargetVertexTypeID].GetOrAdd(myTargetVertexID, InMemoryVertex.CreateNewBulkVertex(myTargetVertexID, myTargetVertexTypeID));

            return targetVertex;
        }

        /// <summary>
        /// Creates or updates incoming edges on vertices
        /// </summary>
        /// <param name="myTargetVertex">The vertex that should be updated</param>
        /// <param name="myIncomingVertexTypeID">The id of the incoming vertex type</param>
        /// <param name="myIncomingEdgeID">The id of the incoming edge property</param>
        /// <param name="myIncomingVertex">The incoming single edge</param>
        private void CreateOrUpdateIncomingEdgesOnVertex(InMemoryVertex myTargetVertex, Int64 myIncomingVertexTypeID, Int64 myIncomingEdgeID, InMemoryVertex myIncomingVertex)
        {
            lock (myTargetVertex)
            {
                if (myTargetVertex.IncomingEdges == null)
                {
                    myTargetVertex.IncomingEdges = new Dictionary<long, Dictionary<long, IncomingEdgeCollection>>();

                    var payload = new IncomingEdgeCollection( myIncomingVertex );

                    var innerDict = new Dictionary<Int64, IncomingEdgeCollection> { { myIncomingEdgeID, payload } };

                    myTargetVertex.IncomingEdges.Add(myIncomingVertexTypeID, innerDict);
                }
                else
                {
                    if (myTargetVertex.IncomingEdges.ContainsKey(myIncomingVertexTypeID))
                    {
                        if (myTargetVertex.IncomingEdges[myIncomingVertexTypeID].ContainsKey(myIncomingEdgeID))
                        {
                            myTargetVertex.IncomingEdges[myIncomingVertexTypeID][myIncomingEdgeID].AddVertex(myIncomingVertex);
                        }
                        else
                        {
                            myTargetVertex.IncomingEdges[myIncomingVertexTypeID][myIncomingEdgeID] = new IncomingEdgeCollection( myIncomingVertex );
                        }
                    }
                    else
                    {
                        var payload = new IncomingEdgeCollection(myIncomingVertex);

                        var innerDict = new Dictionary<Int64, IncomingEdgeCollection> { { myIncomingEdgeID, payload } };

                        myTargetVertex.IncomingEdges.Add(myIncomingVertexTypeID, innerDict);
                    }
                }
            }
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
            return InMemoryVertex.CopyFromIVertex(aVertex);
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
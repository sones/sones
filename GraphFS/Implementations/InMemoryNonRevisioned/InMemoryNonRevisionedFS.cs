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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using sones.GraphFS.Element.Edge;
using sones.GraphFS.Element.Vertex;
using sones.GraphFS.ErrorHandling;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;
using sones.Library.Commons.VertexStore;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.Library.Commons.VertexStore.Definitions.Update;


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

        #region InMemoryNonRevisionedFS Members

        public IEnumerable<IVertex> GetAllVertices()
        {
            var result = _vertexStore.Select(_ => _.Value).SelectMany(_ => _.Values);
            return result;
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

                                                          tempVertexStore[aVertex.VertexTypeID].TryAdd(aVertex.VertexID, TransferToInMemoryVertex(aVertex));
                                                      });

            _vertexStore = tempVertexStore;
        }

        public bool VertexExists(SecurityToken mySecurityToken, Int64 myTransactionID,
                                 long myVertexID, long myVertexTypeID, string myEdition = null,
                                 Int64 myVertexRevisionID = 0L)
        {
            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                return vertices.ContainsKey(myVertexID) && !vertices[myVertexID].IsBulkVertex;
            }

            return false;
        }

        public long GetHighestVertexID(SecurityToken mySecurityToken, Int64 myTransactionID, long myVertexTypeID)
        {
            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                return vertices.Max(_ => _.Key);
            }

            return Int64.MinValue;
        }

        public ulong GetVertexCount(SecurityToken mySecurityToken, Int64 myTransactionID, long myVertexTypeID)
        {
            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                return Convert.ToUInt64(vertices.Count());
            }

            return 0UL;
        }

        public IVertex GetVertex(SecurityToken mySecurityToken, Int64 myTransactionID,
                                 long myVertexID, long myVertexTypeID, string myEdition = null,
                                 Int64 myVertexRevisionID = 0L)
        {
            return GetVertex_private(mySecurityToken, myTransactionID, myVertexID, myVertexTypeID);
        }

        public IVertex GetVertex(
            SecurityToken mySecurityToken, Int64 myTransactionID,
            long myVertexID, long myVertexTypeID,
            VertexStoreFilter.EditionFilter myEditionsFilterFunc = null,
            VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc = null)
        {
            return GetVertex_private(mySecurityToken, myTransactionID, myVertexID, myVertexTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(SecurityToken mySecurityToken, Int64 myTransactionID,
                                                        long myTypeID, IEnumerable<long> myInterestingVertexIDs,
                                                        IEnumerable<string> myInterestingEditionNames,
                                                        IEnumerable<Int64> myInterestingRevisionID)
        {
            return myInterestingVertexIDs != null
                       ? GetVerticesByTypeID(mySecurityToken, myTransactionID, myTypeID, myInterestingVertexIDs)
                       : GetVerticesByTypeID(mySecurityToken, myTransactionID, myTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(SecurityToken mySecurityToken, Int64 myTransactionID,
                                                        long myTypeID, IEnumerable<long> myInterestingVertexIDs)
        {
            var interestingVertices = new HashSet<long>(myInterestingVertexIDs);

            return GetVerticesByTypeID(mySecurityToken, myTransactionID, myTypeID).Where(aVertex => interestingVertices.Contains(aVertex.VertexID));
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(
            SecurityToken mySecurityToken, Int64 myTransactionID,
            long myTypeID,
            IEnumerable<long> myInterestingVertexIDs,
            VertexStoreFilter.EditionFilter myEditionsFilterFunc,
            VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc)
        {
            return myInterestingVertexIDs != null
                       ? GetVerticesByTypeID(mySecurityToken, myTransactionID, myTypeID, myInterestingVertexIDs)
                       : GetVerticesByTypeID(mySecurityToken, myTransactionID, myTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(
            SecurityToken mySecurityToken, Int64 myTransactionID,
            long myTypeID,
            string myEdition,
            VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc)
        {
            return GetVerticesByTypeID(mySecurityToken, myTransactionID, myTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(SecurityToken mySecurityToken, Int64 myTransactionID,
                                                        long myVertexTypeID)
        {
            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                foreach (var aVertex in vertices)
                {
                    if (!aVertex.Value.IsBulkVertex && aVertex.Value.VertexTypeID == myVertexTypeID)
                    {
                        yield return aVertex.Value;
                    }
                }
            }

            yield break;
        }


        public IEnumerable<IVertex> GetVerticesByTypeIDAndRevisions(
            SecurityToken mySecurityToken, Int64 myTransactionID,
            long myTypeID,
            IEnumerable<Int64> myInterestingRevisions)
        {
            return GetVerticesByTypeID(mySecurityToken, myTransactionID, myTypeID);
        }

        public IEnumerable<string> GetVertexEditions(
            SecurityToken mySecurityToken, Int64 myTransactionID,
            long myVertexID, long myVertexTypeID)
        {
            var vertex = GetVertex_private(mySecurityToken, myTransactionID, myVertexID, myVertexTypeID);

            if (vertex == null)
            {
                throw new VertexDoesNotExistException(myVertexTypeID, myVertexID);
            }

            var result = new List<string> { vertex.EditionName };

            return result;
        }

        public IEnumerable<Int64> GetVertexRevisionIDs(
            SecurityToken mySecurityToken, Int64 myTransactionID,
            long myVertexID, long myVertexTypeID,
            IEnumerable<string> myInterestingEditions = null)
        {
            var vertex = GetVertex_private(mySecurityToken, myTransactionID, myVertexID, myVertexTypeID);

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

        public bool RemoveVertexRevision(
            SecurityToken mySecurityToken, Int64 myTransactionID,
            long myVertexID, long myVertexTypeID, string myInterestingEdition,
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

        public bool RemoveVertexEdition(
            SecurityToken mySecurityToken, Int64 myTransactionID,
            long myVertexID, long myVertexTypeID, string myToBeRemovedEdition)
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

        public bool RemoveVertex(
            SecurityToken mySecurityToken, Int64 myTransactionID,
            long myVertexID, long myVertexTypeID)
        {

            ConcurrentDictionary<Int64, InMemoryVertex> vertices;

            if (_vertexStore.TryGetValue(myVertexTypeID, out vertices))
            {
                InMemoryVertex vertex;

                if (vertices.TryGetValue(myVertexID, out vertex))
                {
                    lock (vertex)
                    {
                        #region incoming edges

                        foreach (var incommingVertices in vertex.GetAllIncomingVertices())
                        {
                            foreach (InMemoryVertex sVertex in incommingVertices.Item3)
                            {
                                lock (sVertex)
                                {
                                    var edge = sVertex.OutgoingEdges[incommingVertices.Item2];

                                    if (edge is IHyperEdge)
                                    {
                                        var hyperEdge = edge as HyperEdge;

                                        lock (hyperEdge.ContainedSingleEdges)
                                        {
                                            hyperEdge.ContainedSingleEdges.RemoveWhere(item => item.TargetVertex.VertexID == myVertexID &&
                                                 item.TargetVertex.VertexTypeID == myVertexTypeID);
                                        }
                                    }
                                    else
                                    {
                                        sVertex.OutgoingEdges.Remove(incommingVertices.Item2);
                                    }
                                }
                            }
                        }

                        #endregion

                        #region outgoing edges

                        foreach (var aOutgoingEdge in vertex.GetAllOutgoingEdges())
                        {

                            foreach (InMemoryVertex aTargetVertex in aOutgoingEdge.Item2.GetTargetVertices())
                            {
                                lock (aTargetVertex)
                                {
                                    aTargetVertex.IncomingEdges[vertex.VertexTypeID][aOutgoingEdge.Item1].RemoveVertex(vertex);

                                    if (aTargetVertex.IncomingEdges[vertex.VertexTypeID][aOutgoingEdge.Item1].Count() == 0)
                                    {
                                        aTargetVertex.IncomingEdges[vertex.VertexTypeID].Remove(aOutgoingEdge.Item1);
                                    }

                                    if (aTargetVertex.IncomingEdges[vertex.VertexTypeID].Count() == 0)
                                    {
                                        aTargetVertex.IncomingEdges.Remove(vertex.VertexTypeID);
                                    }
                                }
                            }
                        }

                        #endregion

                    }

                    return vertices.TryRemove(myVertexID, out vertex);
                }
            }

            return false;
        }

        public void RemoveVertices(SecurityToken mySecurityToken, Int64 myTransactionID, long myVertexTypeID, IEnumerable<long> myToBeDeltedVertices = null)
        {
            HashSet<Int64> toBeDeletedVertices = (myToBeDeltedVertices != null) ? new HashSet<Int64>(myToBeDeltedVertices) : new HashSet<Int64>(GetVerticesByTypeID(mySecurityToken, myTransactionID, myVertexTypeID).Select(aVertex => aVertex.VertexID));

            foreach (var aToBeDeletedVertex in toBeDeletedVertices)
            {
                RemoveVertex(mySecurityToken, myTransactionID, aToBeDeletedVertex, myVertexTypeID);
            }

            ConcurrentDictionary<Int64, InMemoryVertex> outValue;

            if (_vertexStore.TryGetValue(myVertexTypeID, out outValue))
            {
                if (outValue.Count == 0)
                {
                    _vertexStore.TryRemove(myVertexTypeID, out outValue);
                }
            }
        }

        public IVertex AddVertex(
            SecurityToken mySecurityToken, Int64 myTransactionID,
            VertexAddDefinition myVertexDefinition,
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

            #endregion

            #region store the new vertex

            InMemoryVertex createdVertex = null;

            _vertexStore[myVertexDefinition.VertexTypeID].
                AddOrUpdate(myVertexDefinition.VertexID,
                (anotherLong) =>
                {

                    Dictionary<long, IEdge> newEdge = null;
                    if (addEdges)
                    {
                        newEdge = new Dictionary<long, IEdge>();
                    }

                    InMemoryVertex toBeAddedVertex = new InMemoryVertex(
                    myVertexDefinition.VertexID,
                    myVertexDefinition.VertexTypeID,
                    vertexRevisionID,
                    myVertexDefinition.Edition,
                    binaryProperties,
                    newEdge,
                    myVertexDefinition.Comment,
                    myVertexDefinition.CreationDate,
                    myVertexDefinition.ModificationDate,
                    myVertexDefinition.StructuredProperties,
                    myVertexDefinition.UnstructuredProperties);

                    if (addEdges)
                    {
                        AddEdgesToVertex(myVertexDefinition, toBeAddedVertex, newEdge);
                    }

                    createdVertex = toBeAddedVertex;

                    return toBeAddedVertex;
                }, 
                (id, oldVertex) =>
                {
                    if (!oldVertex.IsBulkVertex)
                    {
                        throw new VertexAlreadyExistException(myVertexDefinition.VertexTypeID, myVertexDefinition.VertexID);
                    }

                    Dictionary<long, IEdge> oldEdge = null;
                    if (addEdges)
                    {
                        oldEdge = new Dictionary<long, IEdge>();
                    }

                    oldVertex.Activate(
                        binaryProperties,
                        oldEdge,
                        myVertexDefinition.Comment,
                        myVertexDefinition.CreationDate,
                        myVertexDefinition.ModificationDate,
                        myVertexDefinition.StructuredProperties,
                        myVertexDefinition.UnstructuredProperties);

                    if (addEdges)
                    {
                        AddEdgesToVertex(myVertexDefinition, oldVertex, oldEdge);
                    }

                    createdVertex = oldVertex;

                    return oldVertex;
                });

            #endregion

            return createdVertex;
        }

        public IVertex UpdateVertex(
            SecurityToken mySecurityToken, Int64 myTransactionID,
            long myToBeUpdatedVertexID, long myCorrespondingVertexTypeID,
            VertexUpdateDefinition myVertexUpdate, Boolean myCreateIncomingEdges = true,
            string myToBeUpdatedEditions = null, Int64 myToBeUpdatedRevisionIDs = 0L, bool myCreateNewRevision = false)
        {
            var toBeUpdatedVertex = GetVertexPrivate(myToBeUpdatedVertexID, myCorrespondingVertexTypeID);

            if (toBeUpdatedVertex == null || toBeUpdatedVertex.IsBulkVertex)
            {
                throw new VertexDoesNotExistException(myCorrespondingVertexTypeID, myToBeUpdatedVertexID);
            }

            var updatedVertex = UpdateVertex_private(toBeUpdatedVertex, myVertexUpdate);

            ConcurrentDictionary<Int64, InMemoryVertex> outValue;

            if (_vertexStore.TryGetValue(myCorrespondingVertexTypeID, out outValue))
            {
                outValue.AddOrUpdate(myToBeUpdatedVertexID, updatedVertex, (key, oldValue) =>
                {
                    lock (oldValue)
                    {
                        oldValue = updatedVertex;
                        return oldValue;
                    }
                });
            }

            return updatedVertex;
        }

        public void Shutdown(SecurityToken mySecurityToken)
        {
            //do nothing here
        }

        #endregion

        #region IPluginable Members

        public String PluginName
        {
            get { return "sones.inmemorynonrevisionedfs"; }
        }

        public String PluginShortName
        {
            get { return "inmemnonrevfs"; }
        }

        public String PluginDescription
        {
            get { return "The in-memory-store is a non persisitent vertex store without handling any revisions."; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<String, Object> myParameters)
        {
            return new InMemoryNonRevisionedFS();
        }

        public void Dispose()
        { }

        #endregion

        #region private helper

        private void AddEdgesToVertex(VertexAddDefinition myVertexDefinition, 
                                        InMemoryVertex myVertex, 
                                        Dictionary<Int64, IEdge> myEdges)
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
                    singleEdge = new SingleEdge(aSingleEdgeDefinition.EdgeTypeID, 
                                                myVertex, 
                                                targetVertex,
                                                aSingleEdgeDefinition.Comment, 
                                                aSingleEdgeDefinition.CreationDate,
                                                aSingleEdgeDefinition.ModificationDate,
                                                aSingleEdgeDefinition.StructuredProperties,
                                                aSingleEdgeDefinition.UnstructuredProperties);

                    CreateOrUpdateIncomingEdgesOnVertex(
                        targetVertex,
                        myVertexDefinition.VertexTypeID,
                        aSingleEdgeDefinition.PropertyID,
                        myVertex);

                    myEdges.Add(aSingleEdgeDefinition.PropertyID, singleEdge);
                }
            }

            #endregion

            #region hyper edges

            if (myVertexDefinition.OutgoingHyperEdges != null)
            {
                foreach (var aHyperEdgeDefinition in myVertexDefinition.OutgoingHyperEdges)
                {
                    var containedSingleEdges = new HashSet<SingleEdge>();

                    foreach (var aSingleEdgeDefinition in aHyperEdgeDefinition.ContainedSingleEdges)
                    {
                        targetVertex =
                            GetOrCreateTargetVertex(aSingleEdgeDefinition.TargetVertexInformation.VertexTypeID,
                                                    aSingleEdgeDefinition.TargetVertexInformation.VertexID);

                        singleEdge = new SingleEdge(aSingleEdgeDefinition.EdgeTypeID, 
                                                    myVertex, 
                                                    targetVertex,
                                                    aSingleEdgeDefinition.Comment,
                                                    aSingleEdgeDefinition.CreationDate,
                                                    aSingleEdgeDefinition.ModificationDate,
                                                    aSingleEdgeDefinition.StructuredProperties,
                                                    aSingleEdgeDefinition.UnstructuredProperties);

                        CreateOrUpdateIncomingEdgesOnVertex(
                            targetVertex,
                            myVertexDefinition.VertexTypeID,
                            aHyperEdgeDefinition.PropertyID,
                            myVertex);

                        containedSingleEdges.Add(singleEdge);
                    }

                    //create the new edge
                    myEdges.Add(
                        aHyperEdgeDefinition.PropertyID,
                        new HyperEdge(
                            containedSingleEdges,
                            aHyperEdgeDefinition.EdgeTypeID,
                            myVertex,
                            aHyperEdgeDefinition.Comment,
                            aHyperEdgeDefinition.CreationDate,
                            aHyperEdgeDefinition.ModificationDate,
                            aHyperEdgeDefinition.StructuredProperties,
                            aHyperEdgeDefinition.UnstructuredProperties));

                }
            }

            #endregion

        }

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

                    var payload = new IncomingEdgeCollection(myIncomingVertex);

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
                            myTargetVertex.IncomingEdges[myIncomingVertexTypeID][myIncomingEdgeID] = new IncomingEdgeCollection(myIncomingVertex);
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
        /// Removes the incomming edge from a target vertex.
        /// </summary>
        /// <param name="myTargetVertex">The target vertex.</param>
        /// <param name="myIncommingVertexTypeID">The target vertex type id.</param>
        /// <param name="myIncommingEdgePropID">The edge property id.</param>
        /// <param name="myIncommingVertex">The vertex which is to be updated.</param>
        private void RemoveIncommingEdgeFromTargetVertex(InMemoryVertex myTargetVertex, Int64 myIncommingVertexTypeID, Int64 myIncommingEdgePropID, IVertex myIncommingVertex)
        {
            if (myTargetVertex.IncomingEdges != null)
            {
                lock (myTargetVertex.IncomingEdges)
                {
                    Dictionary<Int64, IncomingEdgeCollection> iEdgeCollection = null;

                    if (myTargetVertex.IncomingEdges.TryGetValue(myIncommingVertexTypeID, out iEdgeCollection))
                    {
                        IncomingEdgeCollection edgeCollection = null;

                        if (iEdgeCollection.TryGetValue(myIncommingEdgePropID, out edgeCollection))
                        {
                            edgeCollection.RemoveVertex((InMemoryVertex)myIncommingVertex);

                            if (edgeCollection.Count() == 0)
                            {
                                iEdgeCollection.Remove(myIncommingEdgePropID);
                            }
                        }

                        if (iEdgeCollection.Count == 0)
                        {
                            myTargetVertex.IncomingEdges.Remove(myIncommingVertexTypeID);
                        }
                    }
                }
            }
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
            #region udpate comment

            if (myVertexUpdate.CommentUpdate != null)
            {
                toBeUpdatedVertex.UpdateComment(myVertexUpdate.CommentUpdate);
            }

            #endregion

            #region update binary properties

            if (myVertexUpdate.UpdatedBinaryProperties != null)
            {
                toBeUpdatedVertex.UpdateBinaryProperties(myVertexUpdate.UpdatedBinaryProperties.Updated,
                                                         myVertexUpdate.UpdatedBinaryProperties.Deleted);
            }

            #endregion

            #region udpate single edges

            if (myVertexUpdate.UpdatedSingleEdges != null)
            {
                if (toBeUpdatedVertex.OutgoingEdges == null)
                {
                    lock (toBeUpdatedVertex)
                    {
                        toBeUpdatedVertex.OutgoingEdges = new Dictionary<long, IEdge>();
                    }
                }

                lock (toBeUpdatedVertex.OutgoingEdges)
                {

                    #region delete edges

                    if (myVertexUpdate.UpdatedSingleEdges.Deleted != null)
                    {
                        foreach (var item in myVertexUpdate.UpdatedSingleEdges.Deleted)
                        {
                            IEdge edge = null;

                            if (toBeUpdatedVertex.OutgoingEdges.TryGetValue(item, out edge))
                            {
                                if (edge is SingleEdge)
                                {
                                    var targetVertex = edge.GetTargetVertices().First();

                                    RemoveIncommingEdgeFromTargetVertex((InMemoryVertex)targetVertex, targetVertex.VertexTypeID, item, toBeUpdatedVertex);

                                    toBeUpdatedVertex.OutgoingEdges.Remove(item);
                                }
                            }
                        }
                    }

                    #endregion

                    #region update edges

                    if (myVertexUpdate.UpdatedSingleEdges.Updated != null)
                    {
                        foreach (var item in myVertexUpdate.UpdatedSingleEdges.Updated)
                        {
                            IEdge edge = null;
                            var targetVertex = GetOrCreateTargetVertex(item.Value.TargetVertex.VertexTypeID, item.Value.TargetVertex.VertexID);

                            if (toBeUpdatedVertex.OutgoingEdges.TryGetValue(item.Key, out edge))
                            {
                                if (edge is SingleEdge)
                                {
                                    var singleEdge = (SingleEdge)edge;

                                    if (edge.Comment != null)
                                    {
                                        singleEdge.UpdateComment(item.Value.CommentUpdate);
                                    }

                                    if (item.Value.EdgeTypeID != null)
                                    {
                                        singleEdge.UpdateEdgeType(item.Value.EdgeTypeID);
                                    }

                                    if (item.Value.UpdatedStructuredProperties != null)
                                    {
                                        singleEdge.UpdateStructuredProperties(
                                        item.Value.UpdatedStructuredProperties.Updated,
                                        item.Value.UpdatedStructuredProperties.Deleted);
                                    }

                                    if (item.Value.UpdatedUnstructuredProperties != null)
                                    {
                                        singleEdge.UpdateUnStructuredProperties(
                                        item.Value.UpdatedUnstructuredProperties.Updated,
                                        item.Value.UpdatedUnstructuredProperties.Deleted);
                                    }

                                    if (item.Value.SourceVertex != null)
                                    {
                                        lock (singleEdge)
                                        {
                                            singleEdge.SourceVertex = toBeUpdatedVertex;
                                        }
                                    }

                                    if (item.Value.TargetVertex != null)
                                    {
                                        lock (singleEdge)
                                        {
                                            if (singleEdge.TargetVertex != null)
                                            {
                                                RemoveIncommingEdgeFromTargetVertex(singleEdge.TargetVertex, toBeUpdatedVertex.VertexTypeID, item.Key, toBeUpdatedVertex);
                                            }

                                            singleEdge.TargetVertex = targetVertex;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                edge = new SingleEdge(item.Value.EdgeTypeID,
                                                      toBeUpdatedVertex,
                                                      targetVertex,
                                                      item.Value.CommentUpdate, 0, 0,
                                                      item.Value.UpdatedStructuredProperties == null ? null : item.Value.UpdatedStructuredProperties.Updated, item.Value.UpdatedUnstructuredProperties == null ? null : item.Value.UpdatedUnstructuredProperties.Updated);

                                toBeUpdatedVertex.OutgoingEdges.Add(item.Key, edge);
                            }

                            CreateOrUpdateIncomingEdgesOnVertex(targetVertex, toBeUpdatedVertex.VertexTypeID, item.Key, toBeUpdatedVertex);
                        }
                    }

                    #endregion
                }
            }

            #endregion

            #region update hyper edges

            if (myVertexUpdate.UpdateHyperEdges != null)
            {
                if (toBeUpdatedVertex.OutgoingEdges == null)
                {
                    lock (toBeUpdatedVertex)
                    {
                        toBeUpdatedVertex.OutgoingEdges = new Dictionary<long, IEdge>();
                    }
                }

                lock (toBeUpdatedVertex.OutgoingEdges)
                {
                    #region delete edges


                    if (myVertexUpdate.UpdateHyperEdges.Deleted != null)
                    {
                        foreach (var item in myVertexUpdate.UpdateHyperEdges.Deleted)
                        {
                            IEdge edge = null;

                            if (toBeUpdatedVertex.OutgoingEdges.TryGetValue(item, out edge))
                            {
                                if (edge is HyperEdge)
                                {
                                    foreach (var targetVertex in edge.GetTargetVertices())
                                    {
                                        RemoveIncommingEdgeFromTargetVertex((InMemoryVertex)targetVertex, targetVertex.VertexTypeID, item, toBeUpdatedVertex);
                                    }

                                    toBeUpdatedVertex.OutgoingEdges.Remove(item);
                                }
                            }
                        }
                    }

                    #endregion

                    #region update edges

                    if (myVertexUpdate.UpdateHyperEdges.Updated != null)
                    {
                        foreach (var item in myVertexUpdate.UpdateHyperEdges.Updated)
                        {
                            IEdge edge = null;

                            if (toBeUpdatedVertex.OutgoingEdges.TryGetValue(item.Key, out edge))
                            {
                                if (edge is HyperEdge)
                                {
                                    var hyperEdge = (HyperEdge)edge;

                                    if (edge.Comment != null)
                                    {
                                        hyperEdge.UpdateComment(item.Value.CommentUpdate);
                                    }

                                    if (item.Value.EdgeTypeID != null)
                                    {
                                        hyperEdge.UpdateEdgeType(item.Value.EdgeTypeID);
                                    }

                                    if (item.Value.UpdatedUnstructuredProperties != null)
                                        hyperEdge.UpdateUnStructuredProperties(
                                            item.Value.UpdatedUnstructuredProperties.Updated,
                                            item.Value.UpdatedUnstructuredProperties.Deleted);

                                    if (item.Value.UpdatedStructuredProperties != null)
                                        hyperEdge.UpdateStructuredProperties(
                                            item.Value.UpdatedStructuredProperties.Updated,
                                            item.Value.UpdatedStructuredProperties.Deleted);

                                    #region update the containing single edges

                                    lock (hyperEdge.ContainedSingleEdges)
                                    {
                                        if (item.Value.ToBeDeletedSingleEdges != null)
                                        {
                                            foreach (var singleEdge in item.Value.ToBeDeletedSingleEdges)
                                            {
                                                var targetVertex = GetOrCreateTargetVertex(singleEdge.TargetVertex.VertexTypeID, singleEdge.TargetVertex.VertexID);
                                                RemoveIncommingEdgeFromTargetVertex(targetVertex, toBeUpdatedVertex.VertexTypeID, item.Key, toBeUpdatedVertex);
                                                hyperEdge.ContainedSingleEdges.RemoveWhere(sEdge => (sEdge.SourceVertex.VertexTypeID == singleEdge.SourceVertex.VertexTypeID && sEdge.SourceVertex.VertexID == singleEdge.SourceVertex.VertexID) && (sEdge.TargetVertex.VertexID == singleEdge.TargetVertex.VertexID && sEdge.TargetVertex.VertexTypeID == singleEdge.TargetVertex.VertexTypeID));
                                            }
                                        }

                                        if (item.Value.ToBeUpdatedSingleEdges != null)
                                        {
                                            var newEdges = new List<SingleEdge>();

                                            foreach (var contEdge in item.Value.ToBeUpdatedSingleEdges)
                                            {
                                                var targetVertex =
                                                    GetOrCreateTargetVertex(contEdge.TargetVertex.VertexTypeID,
                                                                            contEdge.TargetVertex.VertexID);

                                                foreach (var singleEdgeItem in hyperEdge.ContainedSingleEdges)
                                                {
                                                    var correspondTarget =
                                                        GetOrCreateTargetVertex(contEdge.TargetVertex.VertexTypeID,
                                                                                contEdge.TargetVertex.VertexID);

                                                    var correspondSource =
                                                        GetOrCreateTargetVertex(contEdge.SourceVertex.VertexTypeID,
                                                                                contEdge.SourceVertex.VertexID);

                                                    if (correspondTarget == singleEdgeItem.TargetVertex &&
                                                        singleEdgeItem.SourceVertex == correspondSource)
                                                    {
                                                        if (contEdge.CommentUpdate != null)
                                                        {
                                                            singleEdgeItem.UpdateComment(contEdge.CommentUpdate);
                                                        }

                                                        if (contEdge.EdgeTypeID != null)
                                                        {
                                                            singleEdgeItem.UpdateEdgeType(contEdge.EdgeTypeID);
                                                        }

                                                        if (contEdge.UpdatedStructuredProperties != null)
                                                        {
                                                            singleEdgeItem.UpdateStructuredProperties(
                                                                contEdge.UpdatedStructuredProperties.Updated,
                                                                contEdge.UpdatedStructuredProperties.Deleted);
                                                        }

                                                        if (contEdge.UpdatedUnstructuredProperties != null)
                                                        {
                                                            singleEdgeItem.UpdateUnStructuredProperties(
                                                                contEdge.UpdatedUnstructuredProperties.Updated,
                                                                contEdge.UpdatedUnstructuredProperties.Deleted);
                                                        }

                                                        if (contEdge.TargetVertex != null)
                                                        {
                                                            lock (singleEdgeItem)
                                                            {
                                                                if (singleEdgeItem.TargetVertex != null)
                                                                {
                                                                    RemoveIncommingEdgeFromTargetVertex(singleEdgeItem.TargetVertex, toBeUpdatedVertex.VertexTypeID, item.Key, toBeUpdatedVertex);
                                                                }

                                                                singleEdgeItem.TargetVertex = targetVertex;
                                                            }
                                                        }

                                                        if (contEdge.SourceVertex != null)
                                                        {
                                                            lock (singleEdgeItem)
                                                            {
                                                                singleEdgeItem.SourceVertex =
                                                                    GetOrCreateTargetVertex(
                                                                        contEdge.SourceVertex.VertexTypeID,
                                                                        contEdge.SourceVertex.VertexID);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        newEdges.Add(new SingleEdge(contEdge.EdgeTypeID,
                                                                                    toBeUpdatedVertex,
                                                                                    GetOrCreateTargetVertex(
                                                                                        contEdge.TargetVertex.
                                                                                            VertexTypeID,
                                                                                        contEdge.TargetVertex.VertexID),
                                                                                    contEdge.CommentUpdate, 0, 0,
                                                                                    contEdge.UpdatedStructuredProperties ==
                                                                                    null
                                                                                        ? null
                                                                                        : contEdge.
                                                                                              UpdatedStructuredProperties
                                                                                              .
                                                                                              Updated,
                                                                                    contEdge.
                                                                                        UpdatedUnstructuredProperties ==
                                                                                    null
                                                                                        ? null
                                                                                        : contEdge.
                                                                                              UpdatedUnstructuredProperties
                                                                                              .
                                                                                              Updated));
                                                    }
                                                }

                                                CreateOrUpdateIncomingEdgesOnVertex(targetVertex,
                                                                                    toBeUpdatedVertex.VertexTypeID,
                                                                                    item.Key, toBeUpdatedVertex);
                                                hyperEdge.ContainedSingleEdges.UnionWith(newEdges);
                                                newEdges.Clear();
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }
                            else
                            {

                                var singleEdges = new HashSet<SingleEdge>();

                                if (item.Value.ToBeUpdatedSingleEdges != null)
                                {
                                    foreach (var singleItem in item.Value.ToBeUpdatedSingleEdges)
                                    {
                                        var targetVertex = GetOrCreateTargetVertex(singleItem.TargetVertex.VertexTypeID, singleItem.TargetVertex.VertexID);

                                        singleEdges.Add(new SingleEdge(singleItem.EdgeTypeID, toBeUpdatedVertex, targetVertex,
                                                                       singleItem.CommentUpdate == null ? null : singleItem.CommentUpdate, 0, 0,
                                                                       singleItem.UpdatedStructuredProperties == null
                                                                           ? null
                                                                           : singleItem.UpdatedStructuredProperties.
                                                                                 Updated,
                                                                       singleItem.UpdatedUnstructuredProperties == null
                                                                           ? null
                                                                           : singleItem.UpdatedUnstructuredProperties.
                                                                                 Updated));

                                        CreateOrUpdateIncomingEdgesOnVertex(targetVertex, toBeUpdatedVertex.VertexTypeID, item.Key, toBeUpdatedVertex);
                                    }

                                    toBeUpdatedVertex.OutgoingEdges.Add(item.Key,
                                                                        new HyperEdge(singleEdges,
                                                                                      item.Value.EdgeTypeID,
                                                                                      toBeUpdatedVertex,
                                                                                      item.Value.CommentUpdate == null ? null : item.Value.CommentUpdate,
                                                                                      0, 0,
                                                                                      item.Value.UpdatedStructuredProperties == null ? null : item.Value.UpdatedStructuredProperties.Updated,
                                                                                          item.Value.UpdatedUnstructuredProperties == null ? null : item.Value.UpdatedUnstructuredProperties.Updated));

                                }
                            }

                        }
                    }

                    #endregion
                }
            }

            #endregion

            #region update unstructured properties

            if (myVertexUpdate.UpdatedUnstructuredProperties != null)
            {
                toBeUpdatedVertex.UpdateUnstructuredProperties(myVertexUpdate.UpdatedUnstructuredProperties);
            }

            #endregion

            #region update structured properties

            if (myVertexUpdate.UpdatedStructuredProperties != null)
            {
                toBeUpdatedVertex.UpdateStructuredProperties(myVertexUpdate.UpdatedStructuredProperties);
            }

            #endregion

            return toBeUpdatedVertex;
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

        private IVertex GetVertex_private(SecurityToken mySecurityToken, Int64 myTransactionID, long myVertexID, long myVertexTypeID)
        {
            var vertex = GetVertexPrivate(myVertexID, myVertexTypeID);

            if (vertex != null && !vertex.IsBulkVertex)
            {
                return vertex;
            }

            return null;
        }

        #endregion
    }
}

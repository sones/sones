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
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;

namespace sones.Plugins.SonesGQL.Functions.ShortestPathAlgorithms.BreathFirstSearch
{
    public sealed class BFS
    {
        ///// <summary>
        ///// Sucht im Graphen nach Knoten "myEnd" ausgehend vom Knoten "myStart", bis zur max. Tiefe "myMaxDepth".
        ///// </summary>
        ///// <param name="myTypeAttribute">Kante über die gesucht werden soll</param>
        ///// <param name="myDBContext"></param>
        ///// <param name="myStart">Startknoten</param>
        ///// <param name="myEnd">gesuchter Knoten</param>
        ///// <param name="myMaxDepth">max. Tiefe</param>
        ///// <returns>true wenn gesuchter Knoten min. 1 mal gefunden, false sonst</returns>
        //public bool Find(IAttributeDefinition myTypeAttribute, IVertex myStart, IVertex myEnd, byte myMaxDepth)
        //{
        //    #region data
        //    //queue for BFS
        //    Queue<long> queue = new Queue<ObjectUUID>();

        //    //Dictionary to store visited TreeNodes
        //    HashSet<ObjectUUID> visitedNodes = new HashSet<ObjectUUID>();

        //    //current depth
        //    byte depth = 1;

        //    //first node in path tree, the start of the select
        //    ObjectUUID root = myStart.ObjectUUID;

        //    //target node, the target of the select
        //    ObjectUUID target = myEnd.ObjectUUID;

        //    //dummy node to check in which level the BFS is
        //    ObjectUUID dummy = null;

        //    //enqueue first node to start the BFS
        //    queue.Enqueue(root);
        //    queue.Enqueue(dummy);

        //    //add root to visitedNodes
        //    visitedNodes.Add(root);

        //    //holds the actual DBObject
        //    Exceptional<DBObjectStream> currentDBObject;
        //    #endregion data

        //    #region BFS

        //    #region validate root
        //    //check if root has edge
        //    var dbo = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), root);
        //    if (dbo.Failed())
        //    {
        //        throw new NotImplementedException();
        //    }

        //    if (!dbo.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
        //    {
        //        return false;
        //    }
        //    #endregion validate root

        //    //if there is more than one object in the queue and the actual depth is less than MaxDepth
        //    while ((queue.Count > 1) && (depth <= myMaxDepth))
        //    {
        //        //get the first Object of the queue
        //        ObjectUUID nodeOfQueue = queue.Dequeue();

        //        #region check if nodeOfQueue is a dummy
        //        //if nodeOfQueue is a dummy, this level is completely worked off
        //        if (nodeOfQueue == null)
        //        {
        //            depth++;

        //            queue.Enqueue(nodeOfQueue);

        //            continue;
        //        }
        //        #endregion check if current is a dummy

        //        //load DBObject
        //        currentDBObject = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), nodeOfQueue);
        //        if (currentDBObject.Failed())
        //        {
        //            throw new NotImplementedException();
        //        }

        //        if (currentDBObject.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
        //        {
        //            #region EdgeType is ASetOfReferencesEdgeType
        //            if (myTypeAttribute.EdgeType is ASetOfReferencesEdgeType)
        //            {
        //                //get all referenced ObjectUUIDs using the given Edge                                                
        //                var objectUUIDs = (currentDBObject.Value.GetAttribute(myTypeAttribute.UUID) as ASetOfReferencesEdgeType).GetAllReferenceIDs();
        //                ObjectUUID currentNode;

        //                foreach (ObjectUUID obj in objectUUIDs)
        //                {
        //                    #region obj is target
        //                    //if the child is the target
        //                    if (target.Equals(obj))
        //                    {
        //                        return true;
        //                    }
        //                    #endregion obj is target
        //                    #region never seen before
        //                    else if (!visitedNodes.Contains(obj))
        //                    {
        //                        //create new node and set nodeOfQueue as parent
        //                        currentNode = new ObjectUUID(obj.ToString());

        //                        //mark the node as visited
        //                        visitedNodes.Add(currentNode);

        //                        //put created node in queue
        //                        queue.Enqueue(currentNode);
        //                    }
        //                    #endregion never seen before
        //                }
        //            }
        //            #endregion EdgeType is ASetOfReferencesEdgeType
        //            #region EdgeType is ASingleReferenceEdgeType
        //            else if (myTypeAttribute.EdgeType is ASingleReferenceEdgeType)
        //            {
        //                //get all referenced ObjectUUIDs using the given Edge                                                
        //                var objectUUIDs = (currentDBObject.Value.GetAttribute(myTypeAttribute.UUID) as ASingleReferenceEdgeType).GetAllReferenceIDs();
        //                ObjectUUID objectUUID = objectUUIDs.First<ObjectUUID>();
        //                ObjectUUID currentNode;

        //                #region obj is target
        //                //if the child is the target
        //                if (target.Equals(objectUUID))
        //                {
        //                    return true;
        //                }
        //                #endregion obj is target
        //                #region never seen before
        //                else if (!visitedNodes.Contains(objectUUID))
        //                {
        //                    //create new node and set nodeOfQueue as parent
        //                    currentNode = new ObjectUUID(objectUUID.ToString());

        //                    //mark the node as visited
        //                    visitedNodes.Add(currentNode);

        //                    //put created node in queue
        //                    queue.Enqueue(currentNode);
        //                }
        //                #endregion never seen before
        //            }
        //            #endregion EdgeType is ASingleReferenceEdgeType
        //            else
        //            {
        //                throw new NotImplementedException();
        //            }
        //        }
        //    }

        //    #endregion BFS

        //    return false;
        //}

        ///// <summary>
        ///// Sucht im Graphen nach Knoten "myEnd" ausgehend von der Knotenmenge "myEdge", bis zur max. Tiefe "myMaxDepth".
        ///// </summary>
        ///// <param name="myTypeAttribute">Kante über die gesucht werden soll</param>
        ///// <param name="myDBContext"></param>
        ///// <param name="myStart">Startknoten</param>
        ///// <param name="myEnd">gesuchter Knoten</param>
        ///// <param name="myEdge">Menge an Knoten, ausgehend vom Startknoten welche mittels einer Funktion eingeschränkt wurde</param>
        ///// <param name="myMaxDepth">max. Tiefe</param>
        ///// <returns>true wenn gesuchter Knoten min. 1 mal gefunden, false sonst</returns>
        //public bool Find(IAttributeDefinition myTypeAttribute, IVertex myStart, IVertex myEnd, IEdge myEdge, byte myMaxDepth)
        //{
        //    #region data
        //    //queue for BFS
        //    Queue<ObjectUUID> queue = new Queue<ObjectUUID>();

        //    //Dictionary to store visited TreeNodes
        //    BigHashSet<ObjectUUID> visitedNodes = new BigHashSet<ObjectUUID>();

        //    //current depth
        //    byte depth = 2;

        //    //first node in path tree, the start of the select
        //    ObjectUUID root = myStart.ObjectUUID;

        //    //constrainted set of nodes, of start node
        //    HashSet<ObjectUUID> rootFriends = new HashSet<ObjectUUID>();

        //    //target node, the target of the select
        //    ObjectUUID target = myEnd.ObjectUUID;

        //    //dummy node to check in which level the BFS is
        //    ObjectUUID dummy = null;

        //    //add root to visitedNodes
        //    visitedNodes.Add(root);

        //    //holds the actual DBObject
        //    Exceptional<DBObjectStream> currentDBObject;
        //    #endregion data

        //    #region validate root
        //    //check if root has edge
        //    var dbo = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), root);
        //    if (dbo.Failed())
        //    {
        //        throw new NotImplementedException();
        //    }

        //    if (!dbo.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
        //    {
        //        return false;
        //    }
        //    #endregion validate root

        //    #region get friends of startElement and check if they are the target and valid
        //    //instead of inserting only the startObject, we are using the startObject and the friends of the startObject (which could be restricted)
        //    var firstUUIDs = myEdge.GetAllReferenceIDs();

        //    for (int i = 0; i < firstUUIDs.Count(); i++)
        //    {
        //        var element = firstUUIDs.ElementAt(i);

        //        if (element != null)
        //        {
        //            //create a new node and set root = parent
        //            var currentNode = element;

        //            #region check if the child is the target
        //            //start and target are conntected directly
        //            if (currentNode.Equals(myEnd.ObjectUUID))
        //            {
        //                //add node (which coud be the target) to startFriends (if start and target are directly connected, the target in the rootFriends list is needed)
        //                rootFriends.Add(currentNode);

        //                return true;
        //            }
        //            #endregion check if the child is the target

        //            //check if element has edge
        //            var dbobject = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), currentNode);
        //            if (dbobject.Failed())
        //            {
        //                continue;
        //            }

        //            if (!dbobject.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
        //            {
        //                continue;
        //            }

        //            //enqueue node to start from left side
        //            queue.Enqueue(currentNode);

        //            //add node to visitedNodes
        //            visitedNodes.Add(currentNode);

        //            //add node to startFriends
        //            rootFriends.Add(currentNode);
        //        }
        //    }
        //    #endregion get friends of startElement and check if they are the target and valid

        //    //enqueue dummy
        //    queue.Enqueue(dummy);

        //    #region BFS

        //    //if there is more than one object in the queue and the actual depth is less than MaxDepth
        //    while ((queue.Count > 1) && (depth <= myMaxDepth))
        //    {
        //        //get the first Object of the queue
        //        ObjectUUID nodeOfQueue = queue.Dequeue();

        //        #region check if nodeOfQueue is a dummy
        //        //if nodeOfQueue is a dummy, this level is completely worked off
        //        if (nodeOfQueue == null)
        //        {
        //            depth++;

        //            queue.Enqueue(nodeOfQueue);

        //            continue;
        //        }
        //        #endregion check if current is a dummy

        //        //load DBObject
        //        currentDBObject = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), nodeOfQueue);
        //        if (currentDBObject.Failed())
        //        {
        //            throw new NotImplementedException();
        //        }

        //        if (currentDBObject.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
        //        {
        //            #region EdgeType is ASetOfReferencesEdgeType
        //            if (myTypeAttribute.EdgeType is ASetOfReferencesEdgeType)
        //            {
        //                //get all referenced ObjectUUIDs using the given Edge                                                
        //                var objectUUIDs = (currentDBObject.Value.GetAttribute(myTypeAttribute.UUID) as ASetOfReferencesEdgeType).GetAllReferenceIDs();
        //                ObjectUUID currentNode;

        //                foreach (ObjectUUID obj in objectUUIDs)
        //                {
        //                    #region obj is target
        //                    //if the child is the target
        //                    if (target.Equals(obj))
        //                    {
        //                        return true;
        //                    }
        //                    #endregion obj is target
        //                    #region never seen before
        //                    else if (!visitedNodes.Contains(obj))
        //                    {
        //                        //create new node and set nodeOfQueue as parent
        //                        currentNode = new ObjectUUID(obj.ToString());

        //                        //mark the node as visited
        //                        visitedNodes.Add(currentNode);

        //                        //put created node in queue
        //                        queue.Enqueue(currentNode);
        //                    }
        //                    #endregion never seen before
        //                }
        //            }
        //            #endregion EdgeType is ASetOfReferencesEdgeType
        //            #region EdgeType is ASingleReferenceEdgeType
        //            else if (myTypeAttribute.EdgeType is ASingleReferenceEdgeType)
        //            {
        //                //get all referenced ObjectUUIDs using the given Edge                                                
        //                var objectUUIDs = (currentDBObject.Value.GetAttribute(myTypeAttribute.UUID) as ASingleReferenceEdgeType).GetAllReferenceIDs();
        //                ObjectUUID objectUUID = objectUUIDs.First<ObjectUUID>();
        //                ObjectUUID currentNode;

        //                #region obj is target
        //                //if the child is the target
        //                if (target.Equals(objectUUID))
        //                {
        //                    return true;
        //                }
        //                #endregion obj is target
        //                #region never seen before
        //                else if (!visitedNodes.Contains(objectUUID))
        //                {
        //                    //create new node and set nodeOfQueue as parent
        //                    currentNode = new ObjectUUID(objectUUID.ToString());

        //                    //mark the node as visited
        //                    visitedNodes.Add(currentNode);

        //                    //put created node in queue
        //                    queue.Enqueue(currentNode);
        //                }
        //                #endregion never seen before
        //            }
        //            #endregion EdgeType is ASingleReferenceEdgeType
        //            else
        //            {
        //                throw new NotImplementedException();
        //            }
        //        }
        //    }

        //    #endregion BFS

        //    return false;
        //}

        /// <summary>
        /// Searches shortest, all shortest or all paths starting from "myStart" to "myEnd".
        /// </summary>
        /// <param name="myTypeAttribute">The Attribute representing the edge to follow (p.e. "Friends")</param>
        /// <param name="myTypeManager">The TypeManager for the Node type</param>
        /// <param name="myDBObjectCache">The Object Cache for faster object lookup</param>
        /// <param name="myStart">The start node</param>
        /// <param name="myEnd">The end node</param>
        /// <param name="shortestOnly">true, if only shortest path shall be found</param>
        /// <param name="findAll">if true and shortestOnly is true, all shortest paths will be found. if true, and shortest only is false, all paths will be searched</param>
        /// <param name="myMaxDepth">The maximum depth to search</param>
        /// <param name="myMaxPathLength">The maximum path length which shall be analyzed</param>
        /// <returns>A HashSet which contains all found paths. Every path is represented by a List of ObjectUUIDs</returns>
        public HashSet<List<long>> Find(IAttributeDefinition myTypeAttribute, IVertex myStart, IVertex myEnd, bool shortestOnly, bool findAll, byte myMaxDepth, byte myMaxPathLength)
        {
            #region data

            //queue for BFS
            Queue<IVertex> queue = new Queue<IVertex>();

            //Dictionary to store visited TreeNodes
            Dictionary<long, Node> visitedNodes = new Dictionary<long, Node>();

            HashSet<long> visitedVertices = new HashSet<long>();

            //current depth
            byte depth = 0;

            //first node in path tree, the start of the select
            Node root = new Node(myStart.VertexID);

            //target node, the target of the select
            Node target = new Node(myEnd.VertexID);

            //dummy node to check in which level the BFS is
            IVertex dummy = null;

            //if the maxDepth is greater then maxPathLength, then set maxDepth to maxPathLength
            if (myMaxDepth > myMaxPathLength)
            {
                myMaxDepth = myMaxPathLength;
            }

            //enqueue first node to start the BFS
            queue.Enqueue(myStart);
            queue.Enqueue(dummy);

            //add root to visitedNodes
            visitedNodes.Add(root.Key, root);

            #endregion

            #region BFS

            //check if root node has edge and target has backwardedge
            if (!myStart.HasOutgoingEdge(myTypeAttribute.ID))
            {
                return null;
            }

            if (!myEnd.HasIncomingVertices(myEnd.VertexTypeID, myTypeAttribute.ID))
            {
                return null;
            }

            //if there is more than one object in the queue and the actual depth is less than MaxDepth
            while ((queue.Count > 0) && (depth <= myMaxDepth))
            {
                //get the first Object of the queue
                IVertex currentVertex = queue.Dequeue();

                //dummy
                if (currentVertex == null || visitedVertices.Contains(currentVertex.VertexID))
                {
                    continue;
                }

                visitedVertices.Add(currentVertex.VertexID);

                Node currentNode;

                if (visitedNodes.ContainsKey(currentVertex.VertexID))
                {
                    currentNode = visitedNodes[currentVertex.VertexID];
                }
                else
                {
                    currentNode = new Node(currentVertex.VertexID);
                }

                if (currentVertex.HasOutgoingEdge(myTypeAttribute.ID))
                {
                    var vertices = currentVertex.GetOutgoingEdge(myTypeAttribute.ID).GetTargetVertices();

                    Node nextNode;

                    foreach (var vertex in vertices)
                    {
                        //create a new node and set currentNode = parent, nextNode = child                            
                        nextNode = new Node(vertex.VertexID, currentNode);
                        currentNode.addChild(nextNode);

                        //if the child is the target
                        if (nextNode.Equals(target))
                        {
                            //node points on the target
                            target.Parents.Add(currentNode);

                            //if shortestOnly == true we are finished here
                            if (shortestOnly)
                            {
                                if (findAll)
                                {
                                    //continue searching the current depth if there are any other shortest paths
                                    myMaxDepth = Convert.ToByte(depth);
                                    myMaxPathLength = Convert.ToByte(depth + 2);
                                }
                                else
                                {
                                    //got the shortest, finished
                                    return new TargetAnalyzer(root, target, myMaxPathLength, shortestOnly, findAll).getPaths();
                                }
                            }
                        }
                        else
                        {
                            //been there before
                            if (visitedNodes.ContainsKey(nextNode.Key))
                            {
                                //node has more then one parent
                                visitedNodes[nextNode.Key].Parents.Add(currentNode);
                            }

                            //never seen before
                            //mark the node as visited
                            visitedNodes.Add(nextNode.Key, nextNode);
                            //and look what comes on the next level of depth
                            queue.Enqueue(vertex);
                        }
                    }
                }

                //if a new depth is reached
                if (queue.First() == null)
                {
                    //enqueue the dummy at the end of to mark the next depth
                    queue.Enqueue(queue.Dequeue());
                    //one step deeper in the dungen
                    depth++;
                }
            }

            #endregion

            //analyze paths
            return new TargetAnalyzer(root, target, myMaxPathLength, shortestOnly, findAll).getPaths();
        }
    }
}

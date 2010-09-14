/* <id name="GraphDB – BreadthFirstSearch" />
 * <copyright file="BreadthFirstSearch.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Martin Junghanns</developer>
 * <developer>Michael Woidak</developer>
 * <summary>
 * 
 * Breadth First Search is used to find paths between nodes in an unweighted graph.
 * 
 * algorithm (abstract):
 * 
 *      1. start at Node_A, put it into the queue
 *      2. dequeue the first Node and mark it as "visited"
 *          if the dequeued node is Node_B
 *              one of the shortest paths is found
 *          else
 *              get all successors of the dequeued node
 *              if one of the successors is Node_B
 *                  search is finished, one of the shortest paths is found
 *              else
 *                  enqueue the successors
 *      3. repeat step 2
 *      4. if the queue is empty or the maximum depth is reached, search is finished with no result
 *      
 * complexity:
 *  
 *  Worst case O(|V|+|E|), V - node count, E - edge count
 *  
 * enhancements:
 * 
 * This algorithm can be used to search for one of the shortest, all shortest or all paths.
 * 
 * In case of searching only for the shortest, it breaks up when the first match occurs.
 * If all shortest paths are searched and one match occurs, the search on current depth is 
 * finalized.
 * If all paths are requested, it searches until there are no successors or the maximum depth
 * is reached.
 * 
 * detail:
 * 
 * The Algorithm builds a so called "path-graph" which contains all paths beween two defined nodes.
 * The evaluation of that graph is done via an extra class, called "TargetAnalyzer" (you can find a 
 * documentation how evaluation works at the class).
 * Every Node is represented via an "Node-Object" which has an attribute which makes the node "unique"
 * (we use the DBObjectStream.UUID for that) and a HashSet of parent nodes. Every time we acquire the
 * successors of an dequeued node, we attach the dequeued node as parent to it's successors. 
 * When the dequeued node matches the target, the target's parents are updated. For evaluation we
 * start at the target and traverse all parent nodes until we hit the original start node. (this
 * is done by the class "TargetAnalyzer")
 *
 * </summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphAlgorithms.PathAlgorithm.BFSTreeStructure;
using sones.Lib.DataStructures.Big;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphDB;

#endregion usings

namespace sones.GraphAlgorithms.PathAlgorithm.BreadthFirstSearch
{
    public class BreadthFirstSearch
    {
        //Logger
        //private static Logger //_Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Sucht im Graphen nach Knoten "myEnd" ausgehend vom Knoten "myStart", bis zur max. Tiefe "myMaxDepth".
        /// </summary>
        /// <param name="myTypeAttribute">Kante über die gesucht werden soll</param>
        /// <param name="myDBContext"></param>
        /// <param name="myStart">Startknoten</param>
        /// <param name="myEnd">gesuchter Knoten</param>
        /// <param name="myMaxDepth">max. Tiefe</param>
        /// <returns>true wenn gesuchter Knoten min. 1 mal gefunden, false sonst</returns>
        public bool Find(TypeAttribute myTypeAttribute, DBContext myDBContext, DBObjectStream myStart, DBObjectStream myEnd, byte myMaxDepth)
        {
            #region data
            //queue for BFS
            Queue<ObjectUUID> queue = new Queue<ObjectUUID>();

            //Dictionary to store visited TreeNodes
            BigHashSet<ObjectUUID> visitedNodes = new BigHashSet<ObjectUUID>();

            //current depth
            byte depth = 1;

            //first node in path tree, the start of the select
            ObjectUUID root = myStart.ObjectUUID;

            //target node, the target of the select
            ObjectUUID target = myEnd.ObjectUUID;

            //dummy node to check in which level the BFS is
            ObjectUUID dummy = null;

            //enqueue first node to start the BFS
            queue.Enqueue(root);
            queue.Enqueue(dummy);

            //add root to visitedNodes
            visitedNodes.Add(root);

            //holds the actual DBObject
            Exceptional<DBObjectStream> currentDBObject;
            #endregion data

            #region BFS

            #region validate root
            //check if root has edge
            var dbo = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), root);
            if (dbo.Failed())
            {
                throw new NotImplementedException();
            }

            if (!dbo.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
            {
                return false;
            }
            #endregion validate root

            //if there is more than one object in the queue and the actual depth is less than MaxDepth
            while ((queue.Count > 1) && (depth <= myMaxDepth))
            {
                //get the first Object of the queue
                ObjectUUID nodeOfQueue = queue.Dequeue();

                #region check if nodeOfQueue is a dummy
                //if nodeOfQueue is a dummy, this level is completely worked off
                if (nodeOfQueue == null)
                {
                    depth++;

                    queue.Enqueue(nodeOfQueue);

                    continue;
                }
                #endregion check if current is a dummy

                //load DBObject
                currentDBObject = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), nodeOfQueue);
                if (currentDBObject.Failed())
                {
                    throw new NotImplementedException();
                }

                if (currentDBObject.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
                {
                    #region EdgeType is ASetOfReferencesEdgeType
                    if (myTypeAttribute.EdgeType is ASetOfReferencesEdgeType)
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var objectUUIDs = (currentDBObject.Value.GetAttribute(myTypeAttribute.UUID) as ASetOfReferencesEdgeType).GetAllReferenceIDs();
                        ObjectUUID currentNode;

                        foreach (ObjectUUID obj in objectUUIDs)
                        {
                            #region obj is target
                            //if the child is the target
                            if (target.Equals(obj))
                            {
                                return true;
                            }
                            #endregion obj is target
                            #region never seen before
                            else if (!visitedNodes.Contains(obj))
                            {
                                //create new node and set nodeOfQueue as parent
                                currentNode = new ObjectUUID(obj.ToString());

                                //mark the node as visited
                                visitedNodes.Add(currentNode);

                                //put created node in queue
                                queue.Enqueue(currentNode);
                            }
                            #endregion never seen before
                        }
                    }
                    #endregion EdgeType is ASetOfReferencesEdgeType
                    #region EdgeType is ASingleReferenceEdgeType
                    else if (myTypeAttribute.EdgeType is ASingleReferenceEdgeType)
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var objectUUIDs = (currentDBObject.Value.GetAttribute(myTypeAttribute.UUID) as ASingleReferenceEdgeType).GetAllReferenceIDs();
                        ObjectUUID objectUUID = objectUUIDs.First<ObjectUUID>();
                        ObjectUUID currentNode;

                        #region obj is target
                        //if the child is the target
                        if (target.Equals(objectUUID))
                        {
                            return true;
                        }
                        #endregion obj is target
                        #region never seen before
                        else if (!visitedNodes.Contains(objectUUID))
                        {
                            //create new node and set nodeOfQueue as parent
                            currentNode = new ObjectUUID(objectUUID.ToString());

                            //mark the node as visited
                            visitedNodes.Add(currentNode);

                            //put created node in queue
                            queue.Enqueue(currentNode);
                        }
                        #endregion never seen before
                    }
                    #endregion EdgeType is ASingleReferenceEdgeType
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            #endregion BFS

            return false;
        }

        /// <summary>
        /// Sucht im Graphen nach Knoten "myEnd" ausgehend von der Knotenmenge "myEdge", bis zur max. Tiefe "myMaxDepth".
        /// </summary>
        /// <param name="myTypeAttribute">Kante über die gesucht werden soll</param>
        /// <param name="myDBContext"></param>
        /// <param name="myStart">Startknoten</param>
        /// <param name="myEnd">gesuchter Knoten</param>
        /// <param name="myEdge">Menge an Knoten, ausgehend vom Startknoten welche mittels einer Funktion eingeschränkt wurde</param>
        /// <param name="myMaxDepth">max. Tiefe</param>
        /// <returns>true wenn gesuchter Knoten min. 1 mal gefunden, false sonst</returns>
        public bool Find(TypeAttribute myTypeAttribute, DBContext myDBContext, DBObjectStream myStart, DBObjectStream myEnd, IReferenceEdge myEdge, byte myMaxDepth)
        {
            #region data
            //queue for BFS
            Queue<ObjectUUID> queue = new Queue<ObjectUUID>();

            //Dictionary to store visited TreeNodes
            BigHashSet<ObjectUUID> visitedNodes = new BigHashSet<ObjectUUID>();

            //current depth
            byte depth = 2;

            //first node in path tree, the start of the select
            ObjectUUID root = myStart.ObjectUUID;

            //constrainted set of nodes, of start node
            HashSet<ObjectUUID> rootFriends = new HashSet<ObjectUUID>();

            //target node, the target of the select
            ObjectUUID target = myEnd.ObjectUUID;

            //dummy node to check in which level the BFS is
            ObjectUUID dummy = null;

            //add root to visitedNodes
            visitedNodes.Add(root);

            //holds the actual DBObject
            Exceptional<DBObjectStream> currentDBObject;
            #endregion data

            #region validate root
            //check if root has edge
            var dbo = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), root);
            if (dbo.Failed())
            {
                throw new NotImplementedException();
            }

            if (!dbo.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
            {
                return false;
            }
            #endregion validate root

            #region get friends of startElement and check if they are the target and valid
            //instead of inserting only the startObject, we are using the startObject and the friends of the startObject (which could be restricted)
            var firstUUIDs = myEdge.GetAllReferenceIDs();

            for (int i = 0; i < firstUUIDs.Count(); i++)
            {
                var element = firstUUIDs.ElementAt(i);

                if (element != null)
                {
                    //create a new node and set root = parent
                    var currentNode = element;

                    #region check if the child is the target
                    //start and target are conntected directly
                    if (currentNode.Equals(myEnd.ObjectUUID))
                    {
                        //add node (which coud be the target) to startFriends (if start and target are directly connected, the target in the rootFriends list is needed)
                        rootFriends.Add(currentNode);

                        return true;
                    }
                    #endregion check if the child is the target

                    //check if element has edge
                    var dbobject = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), currentNode);
                    if (dbobject.Failed())
                    {
                        continue;
                    }

                    if (!dbobject.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
                    {
                        continue;
                    }

                    //enqueue node to start from left side
                    queue.Enqueue(currentNode);

                    //add node to visitedNodes
                    visitedNodes.Add(currentNode);

                    //add node to startFriends
                    rootFriends.Add(currentNode);
                }
            }
            #endregion get friends of startElement and check if they are the target and valid

            //enqueue dummy
            queue.Enqueue(dummy);

            #region BFS

            //if there is more than one object in the queue and the actual depth is less than MaxDepth
            while ((queue.Count > 1) && (depth <= myMaxDepth))
            {
                //get the first Object of the queue
                ObjectUUID nodeOfQueue = queue.Dequeue();

                #region check if nodeOfQueue is a dummy
                //if nodeOfQueue is a dummy, this level is completely worked off
                if (nodeOfQueue == null)
                {
                    depth++;

                    queue.Enqueue(nodeOfQueue);

                    continue;
                }
                #endregion check if current is a dummy

                //load DBObject
                currentDBObject = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), nodeOfQueue);
                if (currentDBObject.Failed())
                {
                    throw new NotImplementedException();
                }

                if (currentDBObject.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
                {
                    #region EdgeType is ASetOfReferencesEdgeType
                    if (myTypeAttribute.EdgeType is ASetOfReferencesEdgeType)
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var objectUUIDs = (currentDBObject.Value.GetAttribute(myTypeAttribute.UUID) as ASetOfReferencesEdgeType).GetAllReferenceIDs();
                        ObjectUUID currentNode;

                        foreach (ObjectUUID obj in objectUUIDs)
                        {
                            #region obj is target
                            //if the child is the target
                            if (target.Equals(obj))
                            {
                                return true;
                            }
                            #endregion obj is target
                            #region never seen before
                            else if (!visitedNodes.Contains(obj))
                            {
                                //create new node and set nodeOfQueue as parent
                                currentNode = new ObjectUUID(obj.ToString());

                                //mark the node as visited
                                visitedNodes.Add(currentNode);

                                //put created node in queue
                                queue.Enqueue(currentNode);
                            }
                            #endregion never seen before
                        }
                    }
                    #endregion EdgeType is ASetOfReferencesEdgeType
                    #region EdgeType is ASingleReferenceEdgeType
                    else if (myTypeAttribute.EdgeType is ASingleReferenceEdgeType)
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var objectUUIDs = (currentDBObject.Value.GetAttribute(myTypeAttribute.UUID) as ASingleReferenceEdgeType).GetAllReferenceIDs();
                        ObjectUUID objectUUID = objectUUIDs.First<ObjectUUID>();
                        ObjectUUID currentNode;

                        #region obj is target
                        //if the child is the target
                        if (target.Equals(objectUUID))
                        {
                            return true;
                        }
                        #endregion obj is target
                        #region never seen before
                        else if (!visitedNodes.Contains(objectUUID))
                        {
                            //create new node and set nodeOfQueue as parent
                            currentNode = new ObjectUUID(objectUUID.ToString());

                            //mark the node as visited
                            visitedNodes.Add(currentNode);

                            //put created node in queue
                            queue.Enqueue(currentNode);
                        }
                        #endregion never seen before
                    }
                    #endregion EdgeType is ASingleReferenceEdgeType
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            #endregion BFS

            return false;
        }
        
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
        public HashSet<List<ObjectUUID>> Find(TypeAttribute myTypeAttribute, DBTypeManager myTypeManager, DBObjectCache myDBObjectCache, DBObjectStream myStart, DBObjectStream myEnd, bool shortestOnly, bool findAll, byte myMaxDepth, byte myMaxPathLength)
        {
            #region data

            //queue for BFS
            Queue<Node> queue = new Queue<Node>();

            //Dictionary to store visited TreeNodes
            BigDictionary<ObjectUUID, Node> visitedNodes = new BigDictionary<ObjectUUID, Node>();

            //current depth
            byte depth = 0;
            
            //first node in path tree, the start of the select
            Node root = new Node(myStart.ObjectUUID);
            
            //root changed in Dictionary because of BidirectionalBFS
            HashSet<ObjectUUID> rootFriends = new HashSet<ObjectUUID>();

            //target node, the target of the select
            Node target = new Node(myEnd.ObjectUUID);

            //holds the key of the backwardedge
            EdgeKey edgeKey = new EdgeKey(myTypeAttribute);
            
            //dummy node to check in which level the BFS is
            Node dummy = new Node();

            //if the maxDepth is greater then maxPathLength, then set maxDepth to maxPathLength
            if (myMaxDepth > myMaxPathLength)
            {
                myMaxDepth = myMaxPathLength;
            }

            //enqueue first node to start the BFS
            queue.Enqueue(root);
            queue.Enqueue(dummy);

            //add root to visitedNodes
            visitedNodes.Add(root.Key, root);

            //holds the actual DBObject
            Exceptional<DBObjectStream> currentDBObject;

            #endregion

            #region BFS

            //Log start
            ////_Logger.Info("Starting BFS..");

            //check if root node has edge and target has backwardedge
            var dbObject = myDBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myTypeManager), root.Key);
            if (dbObject.Failed())
            {
                throw new NotImplementedException();
            }

            if (!dbObject.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myTypeManager)))
            {
                ////_Logger.Info("Abort search! Start object has no edge!");
                //Console.WriteLine("No paths found!");
                return null;
            }

            var be = myDBObjectCache.LoadDBBackwardEdgeStream(myTypeAttribute.GetRelatedType(myTypeManager), target.Key);
            if (be.Failed())
            {
                throw new NotImplementedException();
            }

            if (!be.Value.ContainsBackwardEdge(edgeKey))
            {
                ////_Logger.Info("Abort search! End object has no backwardedge!");
                //Console.WriteLine("No paths found!");
                return null;
            }

            //if there is more than one object in the queue and the actual depth is less than MaxDepth
            while ((queue.Count > 0) && (depth <= myMaxDepth))
            {   
                //get the first Object of the queue
                Node current = queue.Dequeue();

                //dummy
                if (current.Key == null) 
                {
                    break;
                }

                //load DBObject
                currentDBObject = myDBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myTypeManager), current.Key);
                if (currentDBObject.Failed())
                {
                    throw new NotImplementedException();
                }

                if (currentDBObject.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myTypeManager)))
                {
                    if (myTypeAttribute.EdgeType is ASetOfReferencesEdgeType)
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var objectUUIDs = (currentDBObject.Value.GetAttribute(myTypeAttribute.UUID) as ASetOfReferencesEdgeType).GetAllReferenceIDs();
                        Node currentNode;
                        
                        foreach(ObjectUUID dbo in objectUUIDs)
                        {
                            //only for debug
                            //var currentNodeObject = myTypeManager.LoadDBObject(myTypeAttribute.RelatedGraphType, dbo);
                            
                            //create a new node and set current = parent, currentNode = child                            
                            currentNode = new Node(dbo, current);                           
                            
                            //if the child is the target
                            if (currentNode.Key.Equals(myEnd.ObjectUUID))
                            {   
                                //node points on the target
                                target.Parents.Add(current);
                                
                                //if shortestOnly == true we are finished here
                                if(shortestOnly)
                                {
                                    if (findAll)
                                    {
                                        //continue searching the current depth if there are any other shortest paths
                                        myMaxDepth = Convert.ToByte(depth);
                                        myMaxPathLength = Convert.ToByte(depth + 2);
                                    }
                                    else
                                    {
                                        ////_Logger.Info("found shortest path.");
                                        //got the shortest, finished
                                        return new TargetAnalyzer(root, rootFriends, target, myMaxPathLength, shortestOnly, findAll).getPaths();
                                    }
                                }
                            }
                            else
                            {
                                //been there before
                                if (visitedNodes.ContainsKey(currentNode.Key))
                                {
                                    //if currentNode.Key isn't root set parent
                                    if (!rootFriends.Contains(currentNode.Key))
                                    {
                                        //node has more then one parent
                                        visitedNodes[currentNode.Key].Parents.Add(current);
                                    }
                                    continue;
                                }

                                //never seen before
                                //mark the node as visited
                                visitedNodes.Add(currentNode.Key, currentNode);
                                //and look what comes on the next level of depth
                                queue.Enqueue(currentNode);

                                //some logging
                                if (queue.Count % 10000 == 0)
                                {
                                    ////_Logger.Info(queue.Count + " elements enqueued..");
                                }
                            }
                        }
                    }
                    else if (myTypeAttribute.EdgeType is ASingleReferenceEdgeType)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                //if a new depth is reached
                if (queue.First() == dummy)
                {                    
                    //enqueue the dummy at the end of to mark the next depth
                    queue.Enqueue(queue.Dequeue());
                    //one step deeper in the dungen
                    depth++;
                    ////_Logger.Info("going deeper in the dungeon.. current level: " + depth);
                }
            }

            #endregion

            ////_Logger.Info("finished building path-graph.. starting analyzer");
            //analyze paths
            return new TargetAnalyzer(root, rootFriends, target, myMaxPathLength, shortestOnly, findAll).getPaths();
        }        
    }
}

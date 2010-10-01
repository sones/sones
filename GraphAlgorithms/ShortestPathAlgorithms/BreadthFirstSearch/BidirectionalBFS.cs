/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/* <id name="GraphDB – BidirectionalBFS" />
 * <copyright file="BidirectionalBFS.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Martin Junghanns</developer>
 * <developer>Michael Woidak</developer>
 * <summary>
 * 
 * Bidrectional Breadth First Search is used to find paths between nodes in an unweighted graph.
 * As compared with the Unidirectional Breadth First Search it starts searching at both ends of
 * the paths, to reduce data amount.
 * 
 * algorithm (abstract):
 * 
 *      1. start at Node_A, put it into the queue_left
 *         start at Node_B, put it into the queue_right
 *      2. dequeue the first Node from queue_left and mark it as "visited"
 *          if the dequeued node is Node_B
 *              one of the shortest paths is found
 *          else
 *              get all successors of the dequeued node (forward edges)
 *              if one of the successors is Node_B
 *                  search is finished, one of the shortest paths is found
 *              else
 *                  enqueue the successors
 *      3. dequeue the first Node from queue_right and mark it as "visited"
 *          if the dequeued node is Node_A
 *              one of the shortest paths is found
 *          else
 *              get all predecessors of the dequeued node (backward edges)
 *              if one of the predecessors is Node_A
 *                  search is finished, one of the shortest paths is found
 *              else
 *                  enqueue the predecessors
 *      4. Make an intersect of visited nodes (right) and visited nodes (left)
 *          if the intersect is not empty
 *              found a path
 *      5. repeat with step 2
 *      6. if the queues are empty or the maximum depth is reached, search is finished with no result
 *      
 * complexity:
 *  
 * Worst case O(|V|+|E|), V - node count, E - edge count
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
 * The evaluation of that graph is done via an extra class, called "BidirectionalTargetAnalyzer" (you can find a 
 * documentation how evaluation works at the class).
 * Every Node is represented via an "Node-Object" which has an attribute which makes the node "unique"
 * (we use the DBObjectStream.UUID for that), a HashSet of children nodes and a HashSet of parent nodes. 
 * On the left side, every time we acquire the successors of an dequeued node, we attach the dequeued node 
 * as parent to it's successors. On the right side, every time we acquire the predecessor of an dequeued node,
 * we attach the dequeued node as children to it's successor.
 * 
 * For evaluation of the path-graph, have a look at the documentation in 
 * GraphAlgorithms.PathAlgorithm.BreadthFirstSearch.BidirectionalTargetAnalyzer
 * 
 * </summary>
 */

#region usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.DataStructures.Big;
using sones.Lib.ErrorHandling;

using sones.GraphAlgorithms.PathAlgorithm.BFSTreeStructure;
using sones.GraphFS.DataStructures;
using sones.GraphDB;

#endregion

namespace sones.GraphAlgorithms.PathAlgorithm.BreadthFirstSearch
{

    public class BidirectionalBFS
    {

        //Logger
        //private static Logger //_Logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Please look at the class documentation for detailed description how this algorithm works.
        /// </summary>
        /// <param name="myTypeAttribute">The Attribute representing the edge to follow (p.e. "Friends")</param>
        /// <param name="myDBContext">The DBContext holds the TypeManager and the ObjectCache</param>
        /// <param name="myStart">The start node</param>
        /// <param name="myEnd">The end node</param>
        /// <param name="shortestOnly">true, if only shortest path shall be found</param>
        /// <param name="findAll">if true and shortestOnly is true, all shortest paths will be found. if true, and shortest only is false, all paths will be searched</param>
        /// <param name="myMaxDepth">The maximum depth to search</param>
        /// <param name="myMaxPathLength">The maximum path length which shall be analyzed</param>
        /// <returns>A HashSet which contains all found paths. Every path is represented by a List of ObjectUUIDs</returns>m>
        public HashSet<List<ObjectUUID>> Find(TypeAttribute myTypeAttribute, DBContext myDBContext, DBObjectStream myStart, IReferenceEdge myEdge, DBObjectStream myEnd, bool shortestOnly, bool findAll, byte myMaxDepth, byte myMaxPathLength)
        {
            #region declarations
            //queue for BFS
            var queueLeft  = new Queue<Node>();
            var queueRight = new Queue<Node>();

            //Dictionary to store visited TreeNodes
            var visitedNodesLeft  = new BigDictionary<ObjectUUID, Node>();
            var visitedNodesRight = new BigDictionary<ObjectUUID, Node>();

            //set current depth left
            byte depthLeft = 2;
            //set current depth right
            byte depthRight = 1;
            
            //maximum depth
            byte maxDepthLeft = 0;
            byte maxDepthRight = 0;

            #region initialize maxDepths
            //if the maxDepth is greater then maxPathLength, then set maxDepth to maxPathLength
            if (myMaxDepth > myMaxPathLength)
            {
                myMaxDepth = myMaxPathLength;
            }

            //set depth for left side
            maxDepthLeft = Convert.ToByte(myMaxDepth / 2 + 1);

            //if myMaxDepth is 1 maxDepthRight keeps 0, just one side is searching
            if (myMaxDepth > 1)
            {
                //both sides have the same depth
                maxDepthRight = maxDepthLeft;
            }

            //if myMaxDepth is even, one side has to search in a greater depth
            if ((myMaxDepth % 2) == 0)
            {
                maxDepthRight = Convert.ToByte(maxDepthLeft - 1);
            }
            #endregion

            //shortest path length
            byte shortestPathLength = 0;

            //target node, the target of the select
            var target = new Node(myEnd.ObjectUUID);
            var root = new Node(myStart.ObjectUUID);
            HashSet<ObjectUUID> rootFriends = new HashSet<ObjectUUID>();

            //dummy node to check in which level the BFS is
            var dummyLeft = new Node();
            var dummyRight = new Node();

            #region get friends of startElement and check if they are the target and valid
            //instead of inserting only the startObject, we are using the startObject and the friends of the startObject (which could be restricted)
            var firstUUIDs = myEdge.GetAllReferenceIDs();

            foreach (var element in firstUUIDs)
            {
                if (element != null)
                {
                    //create a new node and set root = parent
                    var currentNodeLeft = new Node(element);

                    #region check if the child is the target
                    //start and target are conntected directly
                    if (currentNodeLeft.Key == myEnd.ObjectUUID)
                    {
                        //set depthRight to zero
                        depthRight = 0;

                        //add node (which coud be the target) to startFriends (if start and target are directly connected, the target in the rootFriends list is needed)
                        rootFriends.Add(currentNodeLeft.Key);

                        shortestPathLength = Convert.ToByte(depthLeft);

                        return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                    }
                    #endregion check if the child is the target

                    //check if element has edge
                    var dbo = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), currentNodeLeft.Key);
                    if (dbo.Failed())
                    {
                        continue;
                    }

                    if (!dbo.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
                    {
                        continue;
                    }

                    //enqueue node to start from left side
                    queueLeft.Enqueue(currentNodeLeft);

                    //add node to visitedNodes
                    visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);

                    //add node to startFriends
                    rootFriends.Add(currentNodeLeft.Key);
                }
            }

            #endregion get friends of startElement and check if they are the target and valid

            //elements of myEdge doesn't have edge
            if (visitedNodesLeft.Count == 0)
            {
                return null;
            }

            //check if target already found
            if (shortestPathLength != 0)
            {
                return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
            }

            //enqueue dummyLeft to analyze the depth of the left side
            queueLeft.Enqueue(dummyLeft);
                
            //holds the key of the backwardedge
            var edgeKey = new EdgeKey(myTypeAttribute);

            //holds the actual DBObject
            Exceptional<DBObjectStream> currentDBObjectLeft;
            Exceptional<BackwardEdgeStream> currentDBObjectRight;
            #endregion declarations

            #region BidirectionalBFS
            //check if the EdgeType is ASetReferenceEdgeType
            #region EdgeType is ASetReferenceEdgeType
            if (myEdge is ASetOfReferencesEdgeType)
            {
                #region initialize variables
                //enqueue target node to start from right side
                queueRight.Enqueue(target);

                //enqueue dummyRight to analyze the depth of the right side
                queueRight.Enqueue(dummyRight);

                //add root and target to visitedNodes
                visitedNodesRight.Add(target.Key, target);
                #endregion initialize variables

                #region check if target has backwardedge
                var be = myDBContext.DBObjectCache.LoadDBBackwardEdgeStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), target.Key);
                if (be.Failed())
                {
                    throw new NotImplementedException();
                }

                if (!be.Value.ContainsBackwardEdge(edgeKey))
                {
                    return null;
                }
                #endregion check if target has backwardedge

                //if there is more than one object in the queue and the actual depth is less than MaxDepth
                while (((queueLeft.Count > 0) && (queueRight.Count > 0)) && ((depthLeft <= maxDepthLeft) || (depthRight <= maxDepthRight)))
                {
                    #region both queues contain objects and both depths are not reached
                    if (((queueLeft.Count > 0) && (queueRight.Count > 0)) && ((depthLeft <= maxDepthLeft) && (depthRight <= maxDepthRight)))
                    {
                        //hold the actual element of the queues
                        Node currentLeft;
                        Node currentRight;

                        #region check if there is a dummyNode at the beginning of a queue
                        //first of both queues are dummies
                        if ((queueLeft.First<Node>() == dummyLeft) && (queueRight.First<Node>() == dummyRight))
                        {
                            //if maxDepth of a side is reached and there is a dummy, one level is totaly searched
                            if (depthLeft == maxDepthLeft && depthRight == maxDepthRight)
                            {
                                ////_Logger.Info("going deeper in the dungeon on the left side.. current level: " + depthLeft);
                                depthLeft++;
                                ////_Logger.Info("going deeper in the dungeon on the right side.. current level: " + depthRight);
                                depthRight++;

                                continue;
                            }
                            else if (depthLeft == maxDepthLeft)
                            {
                                ////_Logger.Info("going deeper in the dungeon on the left side.. current level: " + depthLeft);
                                depthLeft++;

                                continue;
                            }
                            else if (depthRight == maxDepthRight)
                            {
                                ////_Logger.Info("going deeper in the dungeon on the right side.. current level: " + depthRight);
                                depthRight++;

                                continue;
                            }

                            //dequeue dummies
                            queueLeft.Dequeue();
                            queueRight.Dequeue();

                            //increase depth's
                            ////_Logger.Info("going deeper in the dungeon on the left side.. current level: " + depthLeft);
                            depthLeft++;
                            ////_Logger.Info("going deeper in the dungeon on the right side.. current level: " + depthRight);
                            depthRight++;

                            //if both queues are empty -> break loop
                            if (queueLeft.Count == 0 && queueRight.Count == 0)
                            {
                                break;
                            }
                            //if left queue is empty enqueue right dummy and continue
                            else if (queueLeft.Count == 0)
                            {
                                queueRight.Enqueue(dummyRight);
                                continue;
                            }
                            //if right queue is empty enqueue left dummy and continue
                            else if (queueRight.Count == 0)
                            {
                                queueLeft.Enqueue(dummyLeft);
                                continue;
                            }
                            //enqueue both dummies
                            else
                            {
                                queueLeft.Enqueue(dummyLeft);
                                queueRight.Enqueue(dummyRight);
                            }
                        }
                        //first of left queue is a dummy
                        else if (queueLeft.First<Node>() == dummyLeft)
                        {
                            //if maxDepth of a side is reached and there is a dummy, one level is totaly searched
                            if (depthLeft == maxDepthLeft)
                            {
                                //_Logger.Info("going deeper in the dungeon on the left side.. current level: " + depthLeft);
                                depthLeft++;

                                continue;
                            }

                            //dequeue dummy
                            queueLeft.Dequeue();

                            //increase depth
                            //_Logger.Info("going deeper in the dungeon on the left side.. current level: " + depthLeft);
                            depthLeft++;

                            //if left queue is empty continue
                            if (queueLeft.Count == 0)
                            {
                                continue;
                            }
                            //enqueue dummy
                            else
                            {
                                queueLeft.Enqueue(dummyLeft);
                            }
                        }
                        //first of right queue is a dummy
                        else if (queueRight.First<Node>() == dummyRight)
                        {
                            //if maxDepth of a side is reached and there is a dummy, one level is totaly searched
                            if (depthRight == maxDepthRight)
                            {
                                //_Logger.Info("going deeper in the dungeon on the right side.. current level: " + depthRight);
                                depthRight++;

                                continue;
                            }

                            //dequeue dummy
                            queueRight.Dequeue();

                            //increase depth
                            //_Logger.Info("going deeper in the dungeon on the right side.. current level: " + depthRight);
                            depthRight++;

                            //if right queue is empty continue
                            if (queueRight.Count == 0)
                            {
                                continue;
                            }
                            //enqueue dummy
                            else
                            {
                                queueRight.Enqueue(dummyRight);
                            }
                        }
                        #endregion check if there is a dummyNode at the beginning of a queue

                        #region get first nodes of the queues
                        //get the first Object of the queue
                        currentLeft = queueLeft.Dequeue();
                        //get the first Object of the queue
                        currentRight = queueRight.Dequeue();
                        #endregion get first nodes of the queues

                        #region load DBObjects
                        //load DBObject
                        currentDBObjectLeft = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), currentLeft.Key);
                        if (currentDBObjectLeft.Failed())
                        {
                            throw new NotImplementedException();
                        }

                        //load DBObject
                        currentDBObjectRight = myDBContext.DBObjectCache.LoadDBBackwardEdgeStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), new ObjectUUID(currentRight.Key.ToString().TrimStart()));
                        if (currentDBObjectRight.Failed())
                        {
                            throw new NotImplementedException();
                        }
                        #endregion load DBObjects

                        #region the edge and the backwardedge are existing
                        if (currentDBObjectLeft.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager))
                            && currentDBObjectRight.Value.ContainsBackwardEdge(edgeKey))
                        {
                            //get all referenced ObjectUUIDs using the given Edge                                                
                            var objectUUIDsLeft = (currentDBObjectLeft.Value.GetAttribute(myTypeAttribute.UUID) as ASetOfReferencesEdgeType).GetAllReferenceIDs();
                            Node currentNodeLeft;

                            #region check left friends
                            foreach (var dboLeft in objectUUIDsLeft)
                            {
                                #region if the child is the target
                                if (dboLeft == myEnd.ObjectUUID)
                                {
                                    //set currentLeft as parent of target
                                    target.addParent(currentLeft);

                                    #region check if already visited
                                    if (visitedNodesLeft.ContainsKey(dboLeft))
                                    {
                                        //set currentLeft as parent
                                        visitedNodesLeft[dboLeft].addParent(currentLeft);

                                        //set currentNodeLeft as child
                                        currentLeft.addChild(visitedNodesLeft[dboLeft]);
                                    }
                                    else
                                    {
                                        //create a new node and set currentLeft = parent
                                        currentNodeLeft = new Node(dboLeft, currentLeft);

                                        //set currentNodeLeft as child of currentLeft
                                        currentLeft.addChild(currentNodeLeft);

                                        //never seen before
                                        //mark the node as visited
                                        visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);

                                        //and put node into the queue
                                        queueLeft.Enqueue(currentNodeLeft);
                                    }
                                    #endregion check if already visited

                                    #region check how much parents are searched
                                    if (shortestOnly && !findAll)
                                    {
                                        //_Logger.Info("found shortest path..starting analyzer");

                                        if ((depthLeft + depthRight + 1) > myMaxPathLength)
                                        {
                                            shortestPathLength = myMaxPathLength;
                                        }
                                        else
                                        {
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                        }

                                        return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    //if find all shortest paths
                                    else if (shortestOnly && findAll)
                                    {
                                        //set maxDepth to actual depth
                                        maxDepthLeft = depthLeft;
                                        maxDepthRight = depthRight;

                                        if ((depthLeft + depthRight) > myMaxPathLength)
                                        {
                                            shortestPathLength = myMaxPathLength;
                                        }
                                        else
                                        {
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight);
                                        }
                                    }
                                    #endregion check how much parents are searched
                                }
                                #endregion if the child is the target
                                #region already visited
                                else if (visitedNodesLeft.ContainsKey(dboLeft))
                                {
                                    //set currentLeft as parent
                                    visitedNodesLeft[dboLeft].addParent(currentLeft);

                                    //set currentNodeLeft as child
                                    currentLeft.addChild(visitedNodesLeft[dboLeft]);
                                }
                                #endregion already visited
                                #region set as visited
                                else
                                {
                                    //create a new node and set currentLeft = parent
                                    currentNodeLeft = new Node(dboLeft, currentLeft);

                                    //set currentNodeLeft as child of currentLeft
                                    currentLeft.addChild(currentNodeLeft);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);

                                    //and put node into the queue
                                    queueLeft.Enqueue(currentNodeLeft);
                                }
                                #endregion set as visited
                            }
                            #endregion check left friends

                            //get all referenced ObjectUUIDs using the given Edge                                                
                            var objectUUIDsRight = currentDBObjectRight.Value.GetBackwardEdgeUUIDs(edgeKey);
                            Node currentNodeRight;

                            #region check right friends
                            foreach (ObjectUUID dboRight in objectUUIDsRight)
                            {
                                #region if the child is the target
                                if (rootFriends.Contains(dboRight))
                                {
                                    #region check if already visited
                                    //mark node as visited
                                    if (visitedNodesRight.ContainsKey(dboRight))
                                    {
                                        //set found children
                                        visitedNodesRight[dboRight].addChild(currentRight);

                                        currentRight.addParent(visitedNodesRight[dboRight]);
                                    }
                                    else
                                    {
                                        //create a new node and set currentRight = child                            
                                        currentNodeRight = new Node(dboRight);
                                        currentNodeRight.addChild(currentRight);

                                        //set currentNodeRight as parent of current Right
                                        currentRight.addParent(currentNodeRight);

                                        //never seen before
                                        //mark the node as visited
                                        visitedNodesRight.Add(currentNodeRight.Key, currentNodeRight);

                                        //and look what comes on the next level of depth
                                        queueRight.Enqueue(currentNodeRight);
                                    }
                                    #endregion check if already visited

                                    #region check how much paths are searched
                                    if (shortestOnly && !findAll)
                                    {
                                        if ((depthLeft + depthRight + 1) > myMaxPathLength)
                                        {
                                            shortestPathLength = myMaxPathLength;
                                        }
                                        else
                                        {
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                        }

                                        return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    //if find all shortest paths
                                    else if (shortestOnly && findAll)
                                    {
                                        //set maxDepth to actual depth
                                        maxDepthLeft = depthLeft;
                                        maxDepthRight = depthRight;

                                        if ((depthLeft + depthRight) > myMaxPathLength)
                                        {
                                            shortestPathLength = myMaxPathLength;
                                        }
                                        else
                                        {
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight);
                                        }
                                    }
                                    #endregion check how much paths are searched
                                }
                                #endregion if the child is the target
                                #region already visited
                                else if (visitedNodesRight.ContainsKey(dboRight))
                                {
                                    //set found children
                                    visitedNodesRight[dboRight].addChild(currentRight);

                                    currentRight.addParent(visitedNodesRight[dboRight]);
                                }
                                #endregion already visited
                                #region set as visited
                                else
                                {
                                    //create a new node and set currentRight = child                            
                                    currentNodeRight = new Node(dboRight);
                                    currentNodeRight.addChild(currentRight);

                                    //set currentNodeRight as parent of current Right
                                    currentRight.addParent(currentNodeRight);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesRight.Add(currentNodeRight.Key, currentNodeRight);

                                    //and look what comes on the next level of depth
                                    queueRight.Enqueue(currentNodeRight);
                                }
                                #endregion set as visited
                            }
                            #endregion check right friends

                            #region build intersection of visitedNodesLeft and visitedNodesRight

                            //marks if intersection nodes are existing
                            bool foundIntersect = false;

                            foreach (var node in visitedNodesLeft)
                            {
                                if (visitedNodesRight[node.Key] != null)
                                {
                                    //set nodes children and parents
                                    node.Value.addChildren(visitedNodesRight[node.Key].Children);
                                    node.Value.addParents(visitedNodesRight[node.Key].Parents);

                                    //set nodes children and parents
                                    visitedNodesRight[node.Key].addChildren(node.Value.Children);
                                    visitedNodesRight[node.Key].addParents(node.Value.Parents);

                                    foundIntersect = true;
                                }
                            }
                            #endregion build intersection of visitedNodesLeft and visitedNodesRight

                            #region analyze intersection
                            //if intersection nodes existing
                            if (foundIntersect)
                            {
                                //only shortest path
                                if (shortestOnly && !findAll)
                                {
                                    //_Logger.Info("found shortest path..starting analyzer");

                                    if ((depthLeft + depthRight + 1) > myMaxPathLength)
                                    {
                                        shortestPathLength = myMaxPathLength;
                                    }
                                    else
                                    {
                                        shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                    }

                                    return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                }
                                //if find all shortest paths
                                else if (shortestOnly && findAll)
                                {
                                    //set maxDepth to actual depth
                                    maxDepthLeft = depthLeft;
                                    maxDepthRight = depthRight;

                                    if ((depthLeft + depthRight + 1) > myMaxPathLength)
                                    {
                                        shortestPathLength = myMaxPathLength;
                                    }
                                    else if (shortestPathLength == 0)
                                    {
                                        shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                    }

                                }
                            }
                            #endregion analyze intersection
                        }
                        #endregion the edge and the backwardedge are existing
                        #region only the edge exists
                        else if (currentDBObjectLeft.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
                        {
                            //get all referenced ObjectUUIDs using the given Edge                                                
                            var objectUUIDsLeft = (currentDBObjectLeft.Value.GetAttribute(myTypeAttribute.UUID) as ASetOfReferencesEdgeType).GetAllReferenceIDs();
                            Node currentNodeLeft;

                            #region check left friends
                            foreach (ObjectUUID dboLeft in objectUUIDsLeft)
                            {
                                #region if the child is the target
                                if (dboLeft == myEnd.ObjectUUID)
                                {
                                    target.addParent(currentLeft);

                                    #region check if already visited
                                    if (visitedNodesLeft.ContainsKey(dboLeft))
                                    {
                                        //set currentLeft as parent
                                        visitedNodesLeft[dboLeft].addParent(currentLeft);

                                        //set currentNodeLeft as child
                                        currentLeft.addChild(visitedNodesLeft[dboLeft]);
                                    }
                                    else
                                    {
                                        //create a new node and set currentLeft = parent
                                        currentNodeLeft = new Node(dboLeft, currentLeft);

                                        //set currentNodeLeft as child of currentLeft
                                        currentLeft.addChild(currentNodeLeft);

                                        //never seen before
                                        //mark the node as visited
                                        visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);

                                        //and put node into the queue
                                        queueLeft.Enqueue(currentNodeLeft);
                                    }
                                    #endregion check if already visited

                                    #region check how much paths are searched
                                    if (shortestOnly && !findAll)
                                    {
                                        //_Logger.Info("found shortest path..starting analyzer");

                                        if ((depthLeft + depthRight + 1) > myMaxPathLength)
                                        {
                                            shortestPathLength = myMaxPathLength;
                                        }
                                        else
                                        {
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                        }

                                        return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthLeft = depthLeft;

                                        shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                    }
                                    #endregion check if already visited
                                }
                                #endregion if the child is the target
                                #region already visited from right side
                                else if (visitedNodesRight.ContainsKey(dboLeft))
                                {
                                    //get node
                                    Node temp = visitedNodesRight[dboLeft];
                                    //add parent new
                                    temp.addParent(currentLeft);
                                    //add as child
                                    currentLeft.addChild(temp);

                                    visitedNodesRight.Remove(temp.Key);
                                    visitedNodesRight.Add(temp.Key, temp);

                                    if (visitedNodesLeft.Remove(temp.Key))
                                    {
                                        visitedNodesLeft.Add(temp.Key, temp);
                                    }

                                    if (shortestOnly && !findAll)
                                    {
                                        //_Logger.Info("found shortest path..starting analyzer");

                                        if ((depthLeft + depthRight + 1) > myMaxPathLength)
                                        {
                                            shortestPathLength = myMaxPathLength;
                                        }
                                        else
                                        {
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                        }

                                        return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthLeft = depthLeft;

                                        shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                    }
                                }
                                #endregion already visited from right side
                                #region already visited
                                else if (visitedNodesLeft.ContainsKey(dboLeft))
                                {
                                    //set found parents from left side as parents of matchNode
                                    //matchNode has now parents and children
                                    visitedNodesLeft[dboLeft].addParent(currentLeft);

                                    currentLeft.addChild(visitedNodesLeft[dboLeft]);
                                }
                                #endregion already visited
                                #region set as visited
                                else
                                {
                                    //create a new node and set currentLeft = parent
                                    currentNodeLeft = new Node(dboLeft, currentLeft);

                                    //set currentNodeLeft as child of currentLeft
                                    currentLeft.addChild(currentNodeLeft);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);

                                    //and look what comes on the next level of depth
                                    queueLeft.Enqueue(currentNodeLeft);
                                }
                                #endregion set as visited
                            }
                            #endregion check left friends
                        }
                        #endregion only the edge exists
                        #region only the backwardedge exists
                        else if (currentDBObjectRight.Value.ContainsBackwardEdge(edgeKey))
                        {
                            //get all referenced ObjectUUIDs using the given Edge                                                
                            var objectUUIDsRight = currentDBObjectRight.Value.GetBackwardEdgeUUIDs(edgeKey);
                            Node currentNodeRight;

                            #region check right friends
                            foreach (ObjectUUID dboRight in objectUUIDsRight)
                            {
                                #region if the child is the target
                                if (rootFriends.Contains(dboRight))
                                {
                                    #region check if already visited
                                    //mark node as visited
                                    if (visitedNodesRight.ContainsKey(dboRight))
                                    {
                                        //set found children
                                        visitedNodesRight[dboRight].addChild(currentRight);

                                        currentRight.addParent(visitedNodesRight[dboRight]);
                                    }
                                    else
                                    {
                                        //create a new node and set currentRight = child                            
                                        currentNodeRight = new Node(dboRight);
                                        currentNodeRight.addChild(currentRight);

                                        //set currentNodeRight as parent of current Right
                                        currentRight.addParent(currentNodeRight);

                                        //never seen before
                                        //mark the node as visited
                                        visitedNodesRight.Add(currentNodeRight.Key, currentNodeRight);

                                        //and look what comes on the next level of depth
                                        queueRight.Enqueue(currentNodeRight);
                                    }
                                    #endregion check if already visited

                                    #region check how much paths are searched
                                    if (shortestOnly && !findAll)
                                    {
                                        if ((depthLeft + depthRight + 1) > myMaxPathLength)
                                        {
                                            shortestPathLength = myMaxPathLength;
                                        }
                                        else
                                        {
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                        }

                                        return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthRight = depthRight;

                                        shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                    }
                                    #endregion check how much paths are searched
                                }
                                #endregion if the child is the target
                                #region already visited from left side
                                else if (visitedNodesLeft.ContainsKey(dboRight))
                                {
                                    //get node
                                    Node temp = visitedNodesLeft[dboRight];
                                    temp.addChild(currentRight);
                                    currentRight.addParent(temp);

                                    visitedNodesLeft.Remove(temp.Key);
                                    visitedNodesLeft.Add(temp.Key, temp);

                                    if (visitedNodesRight.Remove(temp.Key))
                                    {
                                        visitedNodesRight.Add(temp.Key, temp);
                                    }

                                    if (shortestOnly && !findAll)
                                    {
                                        //_Logger.Info("found shortest path..starting analyzer");

                                        if ((depthLeft + depthRight + 1) > myMaxPathLength)
                                        {
                                            shortestPathLength = myMaxPathLength;
                                        }
                                        else
                                        {
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                        }

                                        return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthRight = depthRight;

                                        shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                    }
                                }
                                #endregion already visited from left side
                                #region already visited
                                else if (visitedNodesRight.ContainsKey(dboRight))
                                {
                                    //set found children
                                    visitedNodesRight[dboRight].addChild(currentRight);

                                    currentRight.addParent(visitedNodesRight[dboRight]);
                                }
                                #endregion already visited
                                #region set as visited
                                else
                                {
                                    //create a new node and set currentRight = child                            
                                    currentNodeRight = new Node(dboRight);
                                    currentNodeRight.addChild(currentRight);

                                    //set currentNodeRight as parent of currentRight
                                    currentRight.addParent(currentNodeRight);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesRight.Add(currentNodeRight.Key, currentNodeRight);

                                    //and look what comes on the next level of depth
                                    queueRight.Enqueue(currentNodeRight);
                                }
                                #endregion set as visited
                            }
                            #endregion check right friends
                        }
                        #endregion only the backwardedge exists
                    }
                    #endregion  both queues contain objects and both depths are not reached

                    #region only left queue contain objects
                    else if ((queueLeft.Count > 0) && (depthLeft <= maxDepthLeft))
                    {
                        #region check if first element of queue is a dummy
                        //dummy
                        if (queueLeft.First<Node>().Key == null)
                        {
                            queueLeft.Dequeue();

                            //_Logger.Info("going deeper in the dungeon on the left side.. current level: " + depthLeft);
                            depthLeft++;

                            if (queueLeft.Count == 0)
                            {
                                continue;
                            }
                            else if (depthLeft > maxDepthLeft)
                            {
                                continue;
                            }
                        }
                        #endregion check if first element of queue is a dummy

                        //get the first Object of the queue
                        Node currentLeft = queueLeft.Dequeue();

                        //load DBObject
                        currentDBObjectLeft = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), currentLeft.Key);
                        if (currentDBObjectLeft.Failed())
                        {
                            throw new NotImplementedException();
                        }

                        if (currentDBObjectLeft.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
                        {
                            //get all referenced ObjectUUIDs using the given Edge                                                
                            var objectUUIDsLeft = (currentDBObjectLeft.Value.GetAttribute(myTypeAttribute.UUID) as ASetOfReferencesEdgeType).GetAllReferenceIDs();
                            Node currentNodeLeft;

                            #region check left friends
                            foreach (ObjectUUID dboLeft in objectUUIDsLeft)
                            {
                                #region if the child is the target
                                if (dboLeft == myEnd.ObjectUUID)
                                {
                                    target.addParent(currentLeft);

                                    #region check if already visited
                                    if (visitedNodesLeft.ContainsKey(dboLeft))
                                    {
                                        //set currentLeft as parent
                                        visitedNodesLeft[dboLeft].addParent(currentLeft);

                                        //set currentNodeLeft as child
                                        currentLeft.addChild(visitedNodesLeft[dboLeft]);
                                    }
                                    else
                                    {
                                        //create a new node and set currentLeft = parent
                                        currentNodeLeft = new Node(dboLeft, currentLeft);

                                        //set currentNodeLeft as child of currentLeft
                                        currentLeft.addChild(currentNodeLeft);

                                        //never seen before
                                        //mark the node as visited
                                        visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);

                                        //and put node into the queue
                                        queueLeft.Enqueue(currentNodeLeft);
                                    }
                                    #endregion check if already visited

                                    #region check how much paths are searched
                                    if (shortestOnly && !findAll)
                                    {
                                        //_Logger.Info("found shortest path..starting analyzer");

                                        if ((depthLeft + depthRight + 1) > myMaxPathLength)
                                        {
                                            shortestPathLength = myMaxPathLength;
                                        }
                                        else
                                        {
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                        }

                                        return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthLeft = depthLeft;

                                        shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                    }
                                    #endregion check how much paths are searched
                                }
                                #endregion if the child is the target
                                #region already visited from right side
                                else if (visitedNodesRight.ContainsKey(dboLeft))
                                {
                                    //get node
                                    Node temp = visitedNodesRight[dboLeft];
                                    //add parent new
                                    temp.addParent(currentLeft);
                                    //add as child
                                    currentLeft.addChild(temp);

                                    visitedNodesRight.Remove(temp.Key);
                                    visitedNodesRight.Add(temp.Key, temp);

                                    if (visitedNodesLeft.Remove(temp.Key))
                                    {
                                        visitedNodesLeft.Add(temp.Key, temp);
                                    }

                                    if (shortestOnly && !findAll)
                                    {
                                        //_Logger.Info("found shortest path..starting analyzer");

                                        if ((depthLeft + depthRight + 1) > myMaxPathLength)
                                        {
                                            shortestPathLength = myMaxPathLength;
                                        }
                                        else
                                        {
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                        }

                                        return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthLeft = depthLeft;

                                        if (shortestPathLength < (maxDepthLeft + maxDepthRight))
                                        {
                                            shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                        }
                                    }
                                }
                                #endregion already visited from right side
                                #region already visited
                                else if (visitedNodesLeft.ContainsKey(dboLeft))
                                {
                                    //set found parents from left side as parents of matchNode
                                    //matchNode has now parents and children
                                    visitedNodesLeft[dboLeft].addParent(currentLeft);

                                    currentLeft.addChild(visitedNodesLeft[dboLeft]);
                                }
                                #endregion already visited
                                #region set as visited
                                else
                                {
                                    //create a new node and set currentLeft = parent
                                    currentNodeLeft = new Node(dboLeft, currentLeft);

                                    currentLeft.addChild(currentNodeLeft);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);
                                    //and look what comes on the next level of depth
                                    queueLeft.Enqueue(currentNodeLeft);
                                }
                                #endregion set as visited
                            }
                            #endregion check left friends
                        }
                    }
                    #endregion only left queue contain objects

                    #region only right queue contain objects
                    else if ((queueRight.Count > 0) && (depthRight <= maxDepthRight))
                    {
                        //get the first Object of the queue
                        Node currentRight;

                        #region check if first element of the queue is a dummy
                        //dummy
                        if (queueRight.First<Node>().Key == null)
                        {
                            queueRight.Dequeue();

                            //_Logger.Info("going deeper in the dungeon on the right side.. current level: " + depthRight);
                            depthRight++;

                            if (queueRight.Count == 0)
                            {
                                continue;
                            }
                        }
                        #endregion check if first element of the queue is a dummy

                        currentRight = queueRight.Dequeue();

                        //load DBObject
                        currentDBObjectRight = myDBContext.DBObjectCache.LoadDBBackwardEdgeStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), currentRight.Key);
                        if (currentDBObjectRight.Failed())
                        {
                            throw new NotImplementedException();
                        }

                        if (currentDBObjectRight.Value.ContainsBackwardEdge(edgeKey))
                        {
                            //get all referenced ObjectUUIDs using the given Edge                                                
                            var objectUUIDsRight = currentDBObjectRight.Value.GetBackwardEdgeUUIDs(edgeKey);
                            Node currentNodeRight;

                            #region check right friends
                            foreach (ObjectUUID dboRight in objectUUIDsRight)
                            {
                                #region if the child is the target
                                if (rootFriends.Contains(dboRight))
                                {
                                    #region check if already visited
                                    //mark node as visited
                                    if (visitedNodesRight.ContainsKey(dboRight))
                                    {
                                        //set found children
                                        visitedNodesRight[dboRight].addChild(currentRight);

                                        currentRight.addParent(visitedNodesRight[dboRight]);
                                    }
                                    else
                                    {
                                        //create a new node and set currentRight = child                            
                                        currentNodeRight = new Node(dboRight);
                                        currentNodeRight.addChild(currentRight);

                                        //set currentNodeRight as parent of current Right
                                        currentRight.addParent(currentNodeRight);

                                        //never seen before
                                        //mark the node as visited
                                        visitedNodesRight.Add(currentNodeRight.Key, currentNodeRight);

                                        //and look what comes on the next level of depth
                                        queueRight.Enqueue(currentNodeRight);
                                    }
                                    #endregion check if already visited

                                    #region check how much paths are searched
                                    if (shortestOnly && !findAll)
                                    {
                                        //_Logger.Info("found shortest path..starting analyzer");
                                        shortestPathLength = Convert.ToByte(depthLeft + depthRight);
                                        
                                        return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthRight = depthRight;

                                        shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                    }
                                    #endregion check how much paths are searched
                                }
                                #endregion if the child is the target
                                #region already visited from left side
                                else if (visitedNodesLeft.ContainsKey(dboRight))
                                {
                                    //get node
                                    Node temp = visitedNodesLeft[dboRight];
                                    temp.addChild(currentRight);
                                    currentRight.addParent(temp);

                                    visitedNodesLeft.Remove(temp.Key);
                                    visitedNodesLeft.Add(temp.Key, temp);

                                    if (visitedNodesRight.Remove(temp.Key))
                                    {
                                        visitedNodesRight.Add(temp.Key, temp);
                                    }

                                    if (shortestOnly && !findAll)
                                    {
                                        //_Logger.Info("found shortest path..starting analyzer");

                                        if ((depthLeft + depthRight + 1) > myMaxPathLength)
                                        {
                                            shortestPathLength = myMaxPathLength;
                                        }
                                        else
                                        {
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                        }

                                        return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthRight = depthRight;

                                        shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                    }
                                }
                                #endregion already visited from left side
                                #region already visited
                                else if (visitedNodesRight.ContainsKey(dboRight))
                                {
                                    //set found children
                                    visitedNodesRight[dboRight].addChild(currentRight);

                                    currentRight.addParent(visitedNodesRight[dboRight]);
                                }
                                #endregion already visited
                                #region set as visited
                                else
                                {
                                    //create a new node and set currentRight = child                            
                                    currentNodeRight = new Node(dboRight);
                                    currentNodeRight.addChild(currentRight);

                                    currentRight.addParent(currentNodeRight);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesRight.Add(currentNodeRight.Key, currentNodeRight);
                                    //and look what comes on the next level of depth
                                    queueRight.Enqueue(currentNodeRight);
                                }
                                #endregion set as visited
                            }
                            #endregion check right friends
                        }
                    }
                    #endregion only right queue contain objects

                    #region abort loop
                    else
                    {
                        break;
                    }
                    #endregion abort loop
                }
            }
            #endregion EdgeType is ASetReferenceEdgeType
            //check if the EdgeType is ASingleReferenceEdgeType
            #region EdgeType is ASingleReferenceEdgeType
            else if (myEdge is ASingleReferenceEdgeType)
            {
                #region initialize variables
                //if the maxDepth is greater then maxPathLength, then set maxDepth to maxPathLength
                if (myMaxDepth > myMaxPathLength)
                {
                    myMaxDepth = myMaxPathLength;
                }

                //set depth for left side
                maxDepthLeft = myMaxDepth;
                #endregion initialize variables

                #region check if friends of root node have edge
                foreach (var node in visitedNodesLeft)
                {
                    var dbo = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), node.Key);
                    if (dbo.Failed())
                    {
                        if (visitedNodesLeft.Count != 0)
                        {
                            visitedNodesLeft.Remove(node.Key);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }

                    if (!dbo.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
                    {
                        if (visitedNodesLeft.Count != 0)
                        {
                            visitedNodesLeft.Remove(node.Key);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                #endregion check if friends of root node have edge

                //if there is more than one object in the queue and the actual depth is less than MaxDepth
                while ((queueLeft.Count > 0) && (depthLeft <= maxDepthLeft))
                {
                    #region check if first element of queue is a dummy
                    //dummy
                    if (queueLeft.First<Node>().Key == null)
                    {
                        queueLeft.Dequeue();

                        //_Logger.Info("going deeper in the dungeon on the left side.. current level: " + depthLeft);
                        depthLeft++;

                        if ((queueLeft.Count == 0) || (depthLeft > maxDepthLeft))
                        {
                            break;
                        }
                        else
                        {
                            queueLeft.Enqueue(dummyLeft);
                            continue;
                        }
                    }
                    #endregion check if first element of queue is a dummy

                    #region load DBObject
                    //get the first Object of the queue
                    var currentLeft = queueLeft.Dequeue();

                    //load DBObject
                    currentDBObjectLeft = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), currentLeft.Key);
                    if (currentDBObjectLeft.Failed())
                    {
                        throw new NotImplementedException();
                    }
                    #endregion load DBObject

                    #region check if currentDBObjectLeft has attribute
                    if (currentDBObjectLeft.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager)))
                    {
                        //get referenced ObjectUUID using the given Edge                                                
                        var objectUUIDsLeft = (currentDBObjectLeft.Value.GetAttribute(myTypeAttribute.UUID) as ASingleReferenceEdgeType).GetAllReferenceIDs();
                        Node currentNodeLeft;

                        #region check left friend
                        var dboLeft = objectUUIDsLeft.First();

                        //create a new node and set currentLeft = parent
                        currentNodeLeft = new Node(dboLeft, currentLeft);

                        #region if the child is the target
                        if (currentNodeLeft.Key == myEnd.ObjectUUID)
                        {
                            //set currentLeft as parent of target
                            target.addParent(currentLeft);

                            //shortestPathLength is actual depth of left side plus 1 (because the depth starts with zero, but the pathLength with 1)
                            shortestPathLength = Convert.ToByte(depthLeft + 1);
                            
                            return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
                        }
                        #endregion if the child is the target
                        #region already visited
                        else if (visitedNodesLeft.ContainsKey(currentNodeLeft.Key))
                        {
                            //set currentLeft as parent
                            visitedNodesLeft[currentNodeLeft.Key].addParent(currentLeft);

                            //set currentNodeLeft as child
                            currentLeft.addChild(visitedNodesLeft[currentNodeLeft.Key]);
                        }
                        #endregion already visited
                        #region set as visited
                        else
                        {
                            //set currentNodeLeft as child of currentLeft
                            currentLeft.addChild(currentNodeLeft);

                            //never seen before
                            //mark the node as visited
                            visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);

                            //and put node into the queue
                            queueLeft.Enqueue(currentNodeLeft);
                        }
                        #endregion set as visited

                        #endregion check left friend
                    }
                    #endregion check if currentDBObjectLeft has attribute
                    #region abort loop
                    else
                    {
                        break;
                    }
                    #endregion abort loop
                }
            }
            #endregion EdgeType is ASingleReferenceEdgeType
            else 
            {
                throw new NotImplementedException();
            }

            #region start TargetAnalyzer
            if (shortestOnly && findAll)
            {
                if (shortestPathLength > myMaxPathLength)
                {
                    shortestPathLength = myMaxPathLength;
                }

                return new TargetAnalyzer(root, rootFriends, target, shortestPathLength, shortestOnly, findAll).getPaths();
            }
            else
            {
                return new TargetAnalyzer(root, rootFriends, target, myMaxPathLength, shortestOnly, findAll).getPaths();
            }
            #endregion start TargetAnalyzer

            #endregion BidirectionalBFS
        }
    }
}

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

/* <id name="PandoraDB – BidirectionalBFS" />
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

using GraphAlgorithms.PathAlgorithm.BFSTreeStructure;
using sones.GraphFS.DataStructures;
using sones.GraphDB;

#endregion

namespace GraphAlgorithms.PathAlgorithm.BreadthFirstSearch
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
        public HashSet<List<ObjectUUID>> Find(TypeAttribute myTypeAttribute, DBContext myDBContext, DBObjectStream myStart, DBObjectStream myEnd, bool shortestOnly, bool findAll, byte myMaxDepth, byte myMaxPathLength)
        {

            #region declarations
            //queue for BFS
            var queueLeft  = new Queue<Node>();
            var queueRight = new Queue<Node>();

            //Dictionary to store visited TreeNodes
            var visitedNodesLeft  = new BigDictionary<ObjectUUID, Node>();
            var visitedNodesRight = new BigDictionary<ObjectUUID, Node>();

            //current depth
            byte depthLeft = 1;
            byte depthRight = 1;
            
            //maximum depth
            byte maxDepthLeft = 0;
            byte maxDepthRight = 0;

            //shortest path length
            byte shortestPathLength = 0;

            //first node in path tree, the start of the select
            var root = new Node(myStart.ObjectUUID);
            //target node, the target of the select
            var target = new Node(myEnd.ObjectUUID);

            //dummy node to check in which level the BFS is
            var dummyLeft = new Node();
            var dummyRight = new Node();

            //holds the key of the backwardedge
            var edgeKey = new EdgeKey(myTypeAttribute);

            //holds the actual DBObject
            Exceptional<DBObjectStream> currentDBObjectLeft;
            Exceptional<BackwardEdgeStream> currentDBObjectRight;
            #endregion

            #region BidirectionalBFS
            
            //check if the EdgeType is ASetReferenceEdgeType
            #region EdgeType is ASetReferenceEdgeType
            if (myTypeAttribute.EdgeType is ASetReferenceEdgeType)
            {
                #region initialize variables
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

                //enqueue root node to start from left side
                queueLeft.Enqueue(root);
                //enqueue target node to start from right side
                queueRight.Enqueue(target);

                //enqueue dummyLeft to analyze the depth of the left side
                queueLeft.Enqueue(dummyLeft);
                //enqueue dummyRight to analyze the depth of the right side
                queueRight.Enqueue(dummyRight);

                //add root and target to visitedNodes
                visitedNodesLeft.Add(root.Key, root);
                visitedNodesRight.Add(target.Key, target);
                #endregion

                #region check if root node has edge and target has backwardedge
                var dbo = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), root.Key);
                if (dbo.Failed)
                {
                    throw new NotImplementedException();
                }

                if (!dbo.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), null))
                {
                    ////_Logger.Info("Abort search! Start object has no edge!");
                    //Console.WriteLine("No paths found!");
                    return null;
                }

                var be = myDBContext.DBObjectCache.LoadDBBackwardEdgeStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), target.Key);
                if (be.Failed)
                {
                    throw new NotImplementedException();
                }

                if (!be.Value.ContainsBackwardEdge(edgeKey))
                {
                    ////_Logger.Info("Abort search! End object has no backwardedge!");
                    //Console.WriteLine("No paths found!");
                    return null;
                }
                #endregion

                //if there is more than one object in the queue and the actual depth is less than MaxDepth
                while (((queueLeft.Count > 0) && (queueRight.Count > 0)) && ((depthLeft <= maxDepthLeft) || (depthRight <= maxDepthRight)))
                {
                    #region both queues contain objects and both depths are not reached
                    #region logging

                    if (queueLeft.Count % 1000 == 0)
                    {
                        ////_Logger.Info(queueLeft.Count + " elements in the left queue..");
                    }

                    if (queueRight.Count % 1000 == 0)
                    {
                        ////_Logger.Info(queueRight.Count + " elements in the right queue..");
                    }

                    #endregion

                    if (((queueLeft.Count > 0) && (queueRight.Count > 0)) && ((depthLeft <= maxDepthLeft) && (depthRight <= maxDepthRight)))
                    {
                        //hold the first element of the queue
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
                        #endregion

                        #region get first nodes of the queues
                        //get the first Object of the queue
                        currentLeft = queueLeft.Dequeue();
                        //get the first Object of the queue
                        currentRight = queueRight.Dequeue();
                        #endregion

                        //load DBObject
                        currentDBObjectLeft = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), currentLeft.Key);
                        if (currentDBObjectLeft.Failed)
                        {
                            throw new NotImplementedException();
                        }

                        //load DBObject
                        currentDBObjectRight = myDBContext.DBObjectCache.LoadDBBackwardEdgeStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), new ObjectUUID(currentRight.Key.ToString().TrimStart()));
                        if (currentDBObjectRight.Failed)
                        {
                            throw new NotImplementedException();
                        }

                        #region the edge and the backwardedge are existing
                        if (currentDBObjectLeft.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), null)
                            && currentDBObjectRight.Value.ContainsBackwardEdge(edgeKey))
                        {
                            //get all referenced ObjectUUIDs using the given Edge                                                
                            var objectUUIDsLeft = (currentDBObjectLeft.Value.GetAttribute(myTypeAttribute.UUID) as ASetReferenceEdgeType).GetAllUUIDs();
                            Node currentNodeLeft;

                            #region check left friends
                            foreach (ObjectUUID dboLeft in objectUUIDsLeft)
                            {
                                //create a new node and set currentLeft = parent
                                currentNodeLeft = new Node(dboLeft, currentLeft);

                                #region if the child is the target
                                if (currentNodeLeft.Key == myEnd.ObjectUUID)
                                {
                                    //set currentLeft as parent of target
                                    target.addParent(currentLeft);

                                    #region found match node
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

                                        return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
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
                                    #endregion
                                }
                                #endregion
                                #region already visited
                                else if (visitedNodesLeft.ContainsKey(currentNodeLeft.Key))
                                {
                                    //set currentLeft as parent
                                    visitedNodesLeft[currentNodeLeft.Key].addParent(currentLeft);

                                    //set currentNodeLeft as child
                                    currentLeft.addChild(visitedNodesLeft[currentNodeLeft.Key]);
                                }
                                #endregion
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
                                #endregion
                            }
                            #endregion

                            //get all referenced ObjectUUIDs using the given Edge                                                
                            var objectUUIDsRight = currentDBObjectRight.Value.GetBackwardEdgeUUIDs(edgeKey);
                            Node currentNodeRight;

                            #region check right friends
                            foreach (ObjectUUID dboRight in objectUUIDsRight)
                            {
                                //create a new node and set current = parent, tmp = child                            
                                currentNodeRight = new Node(dboRight);
                                currentNodeRight.addChild(currentRight);

                                #region if the child is the target
                                if (currentNodeRight.Key == myStart.ObjectUUID)
                                {
                                    //set currentNodeRight as parent of current Right
                                    currentRight.addParent(currentNodeRight);

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

                                        return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
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
                                }
                                #endregion
                                #region already visited
                                else if (visitedNodesRight.ContainsKey(currentNodeRight.Key))
                                {
                                    //set found children
                                    visitedNodesRight[currentNodeRight.Key].addChild(currentRight);

                                    currentRight.addParent(visitedNodesRight[currentNodeRight.Key]);
                                }
                                #endregion
                                #region set as visited
                                else
                                {
                                    //set currentNodeRight as parent of current Right
                                    currentRight.addParent(currentNodeRight);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesRight.Add(currentNodeRight.Key, currentNodeRight);

                                    //and look what comes on the next level of depth
                                    queueRight.Enqueue(currentNodeRight);
                                }
                                #endregion
                            }
                            #endregion

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
                            #endregion

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

                                    return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
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
                                    else
                                    {
                                        shortestPathLength = Convert.ToByte(depthLeft + depthRight + 1);
                                    }

                                }
                            }
                            #endregion
                        }
                        #endregion
                        #region only the edge exists
                        else if (currentDBObjectLeft.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), null))
                        {
                            //get all referenced ObjectUUIDs using the given Edge                                                
                            var objectUUIDsLeft = (currentDBObjectLeft.Value.GetAttribute(myTypeAttribute.UUID) as ASetReferenceEdgeType).GetAllUUIDs();
                            Node currentNodeLeft;

                            #region check left friends
                            foreach (ObjectUUID dboLeft in objectUUIDsLeft)
                            {
                                //create a new node and set current = parent, tmp = child                            
                                currentNodeLeft = new Node(dboLeft, currentLeft);

                                #region if the child is the target
                                if (currentNodeLeft.Key == myEnd.ObjectUUID)
                                {
                                    target.addParent(currentLeft);

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

                                        return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthLeft = depthLeft;

                                        shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                    }
                                }
                                #endregion
                                #region already visited from right side
                                else if (visitedNodesRight.ContainsKey(currentNodeLeft.Key))
                                {
                                    //get node
                                    Node temp = visitedNodesRight[currentNodeLeft.Key];
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

                                        return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthLeft = depthLeft;

                                        shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                    }
                                }
                                #endregion
                                #region already visited
                                else if (visitedNodesLeft.ContainsKey(currentNodeLeft.Key))
                                {
                                    //set found parents from left side as parents of matchNode
                                    //matchNode has now parents and children
                                    visitedNodesLeft[currentNodeLeft.Key].addParent(currentLeft);

                                    currentLeft.addChild(visitedNodesLeft[currentNodeLeft.Key]);
                                }
                                #endregion
                                #region set as visited
                                else
                                {
                                    //set currentNodeLeft as child of currentLeft
                                    currentLeft.addChild(currentNodeLeft);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);

                                    //and look what comes on the next level of depth
                                    queueLeft.Enqueue(currentNodeLeft);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                        #region only the backwardedge exists
                        else if (currentDBObjectRight.Value.ContainsBackwardEdge(edgeKey))
                        {
                            //get all referenced ObjectUUIDs using the given Edge                                                
                            var objectUUIDsRight = currentDBObjectRight.Value.GetBackwardEdgeUUIDs(edgeKey);
                            Node currentNodeRight;

                            #region check right friends
                                foreach (ObjectUUID dboRight in objectUUIDsRight)
                                {
                                    //create a new node and set current = parent, tmp = child                            
                                    currentNodeRight = new Node(dboRight);
                                    currentNodeRight.addChild(currentRight);

                                    #region if the child is the target
                                    if (currentNodeRight.Key == myStart.ObjectUUID)
                                    {
                                        //set currentNodeRight as parent of currentRight
                                        currentRight.addParent(currentNodeRight);

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

                                            return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                        }
                                        else if (shortestOnly && findAll)
                                        {
                                            maxDepthRight = depthRight;

                                            shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                        }
                                    }
                                    #endregion
                                    #region already visited from left side
                                    else if (visitedNodesLeft.ContainsKey(currentNodeRight.Key))
                                    {
                                        //get node
                                        Node temp = visitedNodesLeft[currentNodeRight.Key];
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

                                            return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                        }
                                        else if (shortestOnly && findAll)
                                        {
                                            maxDepthRight = depthRight;

                                            shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                        }
                                    }
                                    #endregion
                                    #region already visited
                                    else if (visitedNodesRight.ContainsKey(currentNodeRight.Key))
                                    {
                                        //set found children
                                        visitedNodesRight[currentNodeRight.Key].addChild(currentRight);

                                        currentRight.addParent(visitedNodesRight[currentNodeRight.Key]);
                                    }
                                    #endregion
                                    #region set as visited
                                    else
                                    {
                                        //set currentNodeRight as parent of currentRight
                                        currentRight.addParent(currentNodeRight);

                                        //never seen before
                                        //mark the node as visited
                                        visitedNodesRight.Add(currentNodeRight.Key, currentNodeRight);

                                        //and look what comes on the next level of depth
                                        queueRight.Enqueue(currentNodeRight);
                                    }
                                    #endregion
                                }
                                #endregion
                        }
                        #endregion
                    }
                    #endregion

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
                        #endregion

                        //get the first Object of the queue
                        Node currentLeft = queueLeft.Dequeue();

                        //load DBObject
                        currentDBObjectLeft = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), currentLeft.Key);
                        if (currentDBObjectLeft.Failed)
                        {
                            throw new NotImplementedException();
                        }

                        if (currentDBObjectLeft.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), null))
                        {
                            //get all referenced ObjectUUIDs using the given Edge                                                
                            var objectUUIDsLeft = (currentDBObjectLeft.Value.GetAttribute(myTypeAttribute.UUID) as ASetReferenceEdgeType).GetAllUUIDs();
                            Node currentNodeLeft;

                            #region check left friends
                            foreach (ObjectUUID dboLeft in objectUUIDsLeft)
                            {
                                //create a new node and set current = parent, tmp = child                            
                                currentNodeLeft = new Node(dboLeft, currentLeft);

                                #region if the child is the target
                                if (currentNodeLeft.Key == myEnd.ObjectUUID)
                                {
                                    target.addParent(currentLeft);

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

                                        return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthLeft = depthLeft;

                                        shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                    }
                                }
                                #endregion
                                #region already visited from right side
                                else if (visitedNodesRight.ContainsKey(currentNodeLeft.Key))
                                {
                                    //get node
                                    Node temp = visitedNodesRight[currentNodeLeft.Key];
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

                                        return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                    }
                                    else if (shortestOnly && findAll)
                                    {
                                        maxDepthLeft = depthLeft;

                                        shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                    }
                                }
                                #endregion
                                #region already visited
                                else if (visitedNodesLeft.ContainsKey(currentNodeLeft.Key))
                                {
                                    //set found parents from left side as parents of matchNode
                                    //matchNode has now parents and children
                                    visitedNodesLeft[currentNodeLeft.Key].addParent(currentLeft);

                                    currentLeft.addChild(visitedNodesLeft[currentNodeLeft.Key]);
                                }
                                #endregion
                                #region set as visited
                                else
                                {
                                    currentLeft.addChild(currentNodeLeft);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);
                                    //and look what comes on the next level of depth
                                    queueLeft.Enqueue(currentNodeLeft);
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                    #endregion

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
                        #endregion

                        currentRight = queueRight.Dequeue();

                        //load DBObject
                        currentDBObjectRight = myDBContext.DBObjectCache.LoadDBBackwardEdgeStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), currentRight.Key);
                        if (currentDBObjectRight.Failed)
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
                                    //create a new node and set current = parent, tmp = child                            
                                    currentNodeRight = new Node(dboRight);
                                    currentNodeRight.addChild(currentRight);

                                    #region if the child is the target
                                    if (currentNodeRight.Key == myStart.ObjectUUID)
                                    {
                                        //set currentNodeRight as parent of currentRight
                                        currentRight.addParent(currentNodeRight);

                                        if (shortestOnly && !findAll)
                                        {
                                            //_Logger.Info("found shortest path..starting analyzer");
                                            shortestPathLength = Convert.ToByte(depthLeft + depthRight);
                                            return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                        }
                                        else if (shortestOnly && findAll)
                                        {
                                            maxDepthRight = depthRight;

                                            shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                        }
                                    }
                                    #endregion
                                    #region already visited from left side
                                    else if (visitedNodesLeft.ContainsKey(currentNodeRight.Key))
                                    {
                                        //get node
                                        Node temp = visitedNodesLeft[currentNodeRight.Key];
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

                                            return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                        }
                                        else if (shortestOnly && findAll)
                                        {
                                            maxDepthRight = depthRight;

                                            shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                        }
                                    }
                                    #endregion
                                    #region already visited
                                    else if (visitedNodesRight.ContainsKey(currentNodeRight.Key))
                                    {
                                        //set found children
                                        visitedNodesRight[currentNodeRight.Key].addChild(currentRight);

                                        currentRight.addParent(visitedNodesRight[currentNodeRight.Key]);
                                    }
                                    #endregion
                                    #region set as visited
                                    else
                                    {
                                        currentRight.addParent(currentNodeRight);

                                        //never seen before
                                        //mark the node as visited
                                        visitedNodesRight.Add(currentNodeRight.Key, currentNodeRight);
                                        //and look what comes on the next level of depth
                                        queueRight.Enqueue(currentNodeRight);
                                    }
                                    #endregion
                                }
                                #endregion
                        }
                    }
                    #endregion

                    #region abort loop
                    else
                    {
                        break;
                    }
                    #endregion
                }
            }
            #endregion

            //check if the EdgeType is ASingleReferenceEdgeType
            #region EdgeType is ASingleReferenceEdgeType
            else if (myTypeAttribute.EdgeType is ASingleReferenceEdgeType)
            {
                #region initialize variables
                //if the maxDepth is greater then maxPathLength, then set maxDepth to maxPathLength
                if (myMaxDepth > myMaxPathLength)
                {
                    myMaxDepth = myMaxPathLength;
                }

                //set depth for left side
                maxDepthLeft = myMaxDepth;

                //enqueue root node to start from left side
                queueLeft.Enqueue(root);
                
                //enqueue dummyLeft to analyze the depth of the left side
                queueLeft.Enqueue(dummyLeft);
                
                //add root and target to visitedNodes
                visitedNodesLeft.Add(root.Key, root);
                #endregion

                #region check if root node has edge
                var dbo = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), root.Key);
                if (dbo.Failed)
                {
                    throw new NotImplementedException();
                }

                if (!dbo.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), null))
                {
                    ////_Logger.Info("Abort search! Start object has no edge!");
                    //Console.WriteLine("No paths found!");
                    return null;
                }
                #endregion

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
                    #endregion

                    #region load DBObject
                    //get the first Object of the queue
                    var currentLeft = queueLeft.Dequeue();

                    //load DBObject
                    currentDBObjectLeft = myDBContext.DBObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), currentLeft.Key);
                    if (currentDBObjectLeft.Failed)
                    {
                        throw new NotImplementedException();
                    }

                    if (currentDBObjectLeft.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBContext.DBTypeManager), null))
                    {
                        //get referenced ObjectUUID using the given Edge                                                
                        var objectUUIDsLeft = (currentDBObjectLeft.Value.GetAttribute(myTypeAttribute.UUID) as ASingleReferenceEdgeType).GetAllUUIDs();
                        Node currentNodeLeft;

                        #region check left friend
                        var dboLeft = objectUUIDsLeft.First();

                        //create a new node and set currentLeft = parent
                        currentNodeLeft = new Node(dboLeft, currentLeft);

                        #region if the child is the target
                        if (currentNodeLeft.Key == myEnd.ObjectUUID)
                        {
                            #region found

                            //set currentLeft as parent of target
                            target.addParent(currentLeft);

                            //shortestPathLength is actual depth of left side plus 1 (because the depth starts with zero, but the pathLength with 1)
                            shortestPathLength = Convert.ToByte(depthLeft + 1);
                            
                            return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();

                            #endregion
                        }
                        #endregion
                        #region already visited
                        else if (visitedNodesLeft.ContainsKey(currentNodeLeft.Key))
                        {
                            //set currentLeft as parent
                            visitedNodesLeft[currentNodeLeft.Key].addParent(currentLeft);

                            //set currentNodeLeft as child
                            currentLeft.addChild(visitedNodesLeft[currentNodeLeft.Key]);
                        }
                        #endregion
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
                        #endregion

                        #endregion
                    }
                    #endregion
                    
                    #region abort loop
                    else
                    {
                        break;
                    }
                    #endregion
                }
            }
            #endregion

            else 
            {
                throw new NotImplementedException();
            }

            //_Logger.Info("finished building path-graph.. starting analyzer");
            if (shortestOnly && findAll)
            {
                if (shortestPathLength > myMaxPathLength)
                {
                    shortestPathLength = myMaxPathLength;
                }

                return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
            }
            else
            {
                return new TargetAnalyzer(root, target, myMaxPathLength, shortestOnly, findAll).getPaths();
            }
            #endregion
        }
    }
}

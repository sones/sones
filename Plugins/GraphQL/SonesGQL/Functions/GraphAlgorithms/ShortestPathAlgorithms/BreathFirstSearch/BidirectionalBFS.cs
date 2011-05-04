using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;

namespace ShortestPathAlgorithms.BreathFirstSearch
{
    public sealed class BidirectionalBFS
    {
        /// <summary>
        /// Please look at the class documentation for detailed description how this algorithm works.
        /// </summary>
        /// <param name="myTypeAttribute">The Attribute representing the edge to follow (p.e. "Friends")</param>
        /// <param name="myStart">The start node</param>
        /// <param name="myEnd">The end node</param>
        /// <param name="shortestOnly">true, if only shortest path shall be found</param>
        /// <param name="findAll">if true and shortestOnly is true, all shortest paths will be found. if true, and shortest only is false, all paths will be searched</param>
        /// <param name="myMaxDepth">The maximum depth to search</param>
        /// <param name="myMaxPathLength">The maximum path length which shall be analyzed</param>
        /// <returns>A HashSet which contains all found paths. Every path is represented by a List of ObjectUUIDs</returns>m>
        public HashSet<List<long>> Find(IAttributeDefinition myTypeAttribute, IVertex myStart, IVertex myEnd, bool shortestOnly, bool findAll, byte myMaxDepth, byte myMaxPathLength)
        {
            #region declarations

            //queue for BFS
            var queueLeft = new Queue<IVertex>();
            var queueRight = new Queue<IVertex>();

            //Dictionary to store visited TreeNodes
            var visitedNodesLeft = new Dictionary<long, Node>();
            var visitedNodesRight = new Dictionary<long, Node>();

            var visitedVerticesLeft = new HashSet<long>();
            var visitedVerticesRight = new HashSet<long>();

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
            var target = new Node(myEnd.VertexID);
            var root = new Node(myStart.VertexID);
            HashSet<long> rootFriends = new HashSet<long>();

            //dummy node to check in which level the BFS is
            IVertex dummyLeft = null;
            IVertex dummyRight = null;

            #endregion

            #region BidirectionalBFS
            //check if the EdgeType is ASetReferenceEdgeType
            
            #region initialize variables
            
            //enqueue start node to start from left side
            queueLeft.Enqueue(myStart);
            //enqueue dummyLeft to analyze the depth of the left side
            queueLeft.Enqueue(dummyLeft);

            //enqueue target node to start from right side
            queueRight.Enqueue(myEnd);
            //enqueue dummyRight to analyze the depth of the right side
            queueRight.Enqueue(dummyRight);

            visitedNodesLeft.Add(root.Key, root);
            //add root and target to visitedNodes
            visitedNodesRight.Add(target.Key, target);
            
            #endregion

            #region check if start has outgoing and target has incoming edge

            if (!myStart.HasOutgoingEdge(myTypeAttribute.ID))
            {
                return null;
            }
            if (!myEnd.HasIncomingVertices(myEnd.VertexTypeID, myTypeAttribute.ID))
            {
                return null;
            }

            #endregion

            //if there is more than one object in the queue and the actual depth is less than MaxDepth
            while (((queueLeft.Count > 0) && (queueRight.Count > 0)) && ((depthLeft <= maxDepthLeft) || (depthRight <= maxDepthRight)))
            {
                #region both queues contain objects and both depths are not reached
                if (((queueLeft.Count > 0) && (queueRight.Count > 0)) && ((depthLeft <= maxDepthLeft) && (depthRight <= maxDepthRight)))
                {
                    #region check if there is a dummyNode at the beginning of a queue
                    //first of left queue is a dummy
                    if (queueLeft.First<IVertex>() == null)
                    {
                        //if maxDepth of a side is reached and there is a dummy, one level is totaly searched
                        if (depthLeft == maxDepthLeft)
                        {
                            depthLeft++;

                            continue;
                        }

                        //dequeue dummy
                        queueLeft.Dequeue();

                        //increase depth
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
                    if (queueRight.First<IVertex>() == null)
                    {
                        //if maxDepth of a side is reached and there is a dummy, one level is totaly searched
                        if (depthRight == maxDepthRight)
                        {
                            depthRight++;

                            continue;
                        }

                        //dequeue dummy
                        queueRight.Dequeue();

                        //increase depth
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
                    //hold the actual element of the queues
                    Node currentNodeLeft;
                    Node currentNodeRight;

                    IVertex currentVertexLeft;
                    IVertex currentVertexRight;

                    //get the first Object of the queue
                    currentVertexLeft = queueLeft.Dequeue();

                    if (visitedVerticesLeft.Contains(currentVertexLeft.VertexID))
                    {
                        continue;
                    }

                    //get the first Object of the queue
                    currentVertexRight = queueRight.Dequeue();

                    if (visitedVerticesRight.Contains(currentVertexRight.VertexID))
                    {
                        //enqueue already dequeued vertex
                        queueLeft.Enqueue(currentVertexLeft);

                        continue;
                    }

                    visitedVerticesLeft.Add(currentVertexLeft.VertexID);
                    visitedVerticesRight.Add(currentVertexRight.VertexID);

                    if (visitedNodesLeft.ContainsKey(currentVertexLeft.VertexID))
                    {
                        currentNodeLeft = visitedNodesLeft[currentVertexLeft.VertexID];
                    }
                    else
                    {
                        currentNodeLeft = new Node(currentVertexLeft.VertexID);
                    }

                    if (visitedNodesRight.ContainsKey(currentVertexRight.VertexID))
                    {
                        currentNodeRight = visitedNodesRight[currentVertexRight.VertexID];
                    }
                    else
                    {
                        currentNodeRight = new Node(currentVertexRight.VertexID);
                    }
                    #endregion 
                    
                    #region the edge and the backwardedge are existing
                    if (currentVertexLeft.HasOutgoingEdge(myTypeAttribute.ID)
                        && currentVertexRight.HasIncomingVertices(currentVertexRight.VertexTypeID, myTypeAttribute.ID))
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var leftVertices = currentVertexLeft.GetOutgoingEdge(myTypeAttribute.ID).GetTargetVertices();
                        
                        #region check left friends
                        foreach (var nextLeftVertex in leftVertices)
                        {
                            Node nextLeftNode;

                            #region if the child is the target

                            if (nextLeftVertex.VertexID.Equals(target.Key))
                            {
                                //set currentLeft as parent of target
                                target.addParent(currentNodeLeft);
                                
                                #region check if already visited
                                if (visitedNodesLeft.ContainsKey(nextLeftVertex.VertexID))
                                {
                                    //set currentLeft as parent
                                    visitedNodesLeft[nextLeftVertex.VertexID].addParent(currentNodeLeft);

                                    //set currentNodeLeft as child
                                    currentNodeLeft.addChild(visitedNodesLeft[nextLeftVertex.VertexID]);
                                }
                                else
                                {
                                    //create a new node and set currentLeft = parent
                                    nextLeftNode = new Node(nextLeftVertex.VertexID, currentNodeLeft);

                                    //set currentNodeLeft as child of currentLeft
                                    currentNodeLeft.addChild(nextLeftNode);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesLeft.Add(nextLeftNode.Key, nextLeftNode);

                                    //and put node into the queue
                                    queueLeft.Enqueue(nextLeftVertex);
                                }

                                #endregion 

                                #region check how much parents are searched

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
                            else if (visitedNodesLeft.ContainsKey(nextLeftVertex.VertexID))
                            {
                                //set currentLeft as parent
                                visitedNodesLeft[nextLeftVertex.VertexID].addParent(currentNodeLeft);

                                //set currentNodeLeft as child
                                currentNodeLeft.addChild(visitedNodesLeft[nextLeftVertex.VertexID]);
                            }
                            #endregion already visited
                            #region set as visited
                            else
                            {
                                //create a new node and set currentLeft = parent
                                nextLeftNode = new Node(nextLeftVertex.VertexID, currentNodeLeft);

                                //set currentNodeLeft as child of currentLeft
                                currentNodeLeft.addChild(nextLeftNode);

                                //never seen before
                                //mark the node as visited
                                visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);

                                //and put node into the queue
                                queueLeft.Enqueue(nextLeftVertex);
                            }
                            #endregion set as visited
                        }
                        #endregion check left friends

                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var rightVertices = currentVertexRight.GetIncomingVertices(currentVertexRight.VertexTypeID, myTypeAttribute.ID);
                        
                        #region check right friends
                        foreach (var nextRightVertex in rightVertices)
                        {
                            Node nextRightNode;

                            #region if the child is the target
                            if (root.Key.Equals(nextRightVertex.VertexID))
                            {
                                #region check if already visited
                                //mark node as visited
                                if (visitedNodesRight.ContainsKey(nextRightVertex.VertexID))
                                {
                                    //set found children
                                    visitedNodesRight[nextRightVertex.VertexID].addChild(currentNodeRight);

                                    currentNodeRight.addParent(visitedNodesRight[nextRightVertex.VertexID]);
                                }
                                else
                                {
                                    //create a new node and set currentRight = child                            
                                    nextRightNode = new Node(nextRightVertex.VertexID);
                                    nextRightNode.addChild(currentNodeRight);

                                    //set currentNodeRight as parent of current Right
                                    currentNodeRight.addParent(nextRightNode);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesRight.Add(nextRightNode.Key, nextRightNode);

                                    //and look what comes on the next level of depth
                                    queueRight.Enqueue(nextRightVertex);
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

                                #endregion check how much paths are searched
                            }
                            #endregion if the child is the target
                            #region already visited
                            else if (visitedNodesRight.ContainsKey(nextRightVertex.VertexID))
                            {
                                //set found children
                                visitedNodesRight[nextRightVertex.VertexID].addChild(currentNodeRight);

                                currentNodeRight.addParent(visitedNodesRight[nextRightVertex.VertexID]);
                            }
                            #endregion already visited
                            #region set as visited
                            else
                            {
                                //create a new node and set currentRight = child                            
                                nextRightNode = new Node(nextRightVertex.VertexID);
                                nextRightNode.addChild(currentNodeRight);

                                //set currentNodeRight as parent of current Right
                                currentNodeRight.addParent(nextRightNode);

                                //never seen before
                                //mark the node as visited
                                visitedNodesRight.Add(nextRightNode.Key, nextRightNode);

                                //and look what comes on the next level of depth
                                queueRight.Enqueue(nextRightVertex);
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
                    else if (currentVertexLeft.HasOutgoingEdge(myTypeAttribute.ID))
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var leftVertices = currentVertexLeft.GetOutgoingEdge(myTypeAttribute.ID).GetTargetVertices();

                        #region check left friends
                        foreach (var nextLeftVertex in leftVertices)
                        {
                            Node nextLeftNode;

                            #region if the child is the target
                            if (nextLeftVertex.VertexID.Equals(target.Key))
                            {
                                //set currentLeft as parent of target
                                target.addParent(currentNodeLeft);

                                #region check if already visited
                                if (visitedNodesLeft.ContainsKey(nextLeftVertex.VertexID))
                                {
                                    //set currentLeft as parent
                                    visitedNodesLeft[nextLeftVertex.VertexID].addParent(currentNodeLeft);

                                    //set currentNodeLeft as child
                                    currentNodeLeft.addChild(visitedNodesLeft[nextLeftVertex.VertexID]);
                                }
                                else
                                {
                                    //create a new node and set currentLeft = parent
                                    nextLeftNode = new Node(nextLeftVertex.VertexID, currentNodeLeft);

                                    //set currentNodeLeft as child of currentLeft
                                    currentNodeLeft.addChild(nextLeftNode);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesLeft.Add(nextLeftNode.Key, nextLeftNode);

                                    //and put node into the queue
                                    queueLeft.Enqueue(nextLeftVertex);
                                }
                                #endregion

                                #region check how much parents are searched

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
                            #region already visited from right side
                            else if (visitedNodesRight.ContainsKey(nextLeftVertex.VertexID))
                            {
                                //get node
                                Node temp = visitedNodesRight[nextLeftVertex.VertexID];
                                //add parent new
                                temp.addParent(currentNodeLeft);
                                //add as child
                                currentNodeLeft.addChild(temp);

                                visitedNodesRight.Remove(temp.Key);
                                visitedNodesRight.Add(temp.Key, temp);

                                if (visitedNodesLeft.Remove(temp.Key))
                                {
                                    visitedNodesLeft.Add(temp.Key, temp);
                                }

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

                                    return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                }
                                else if (shortestOnly && findAll)
                                {
                                    maxDepthLeft = depthLeft;

                                    shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                }
                            }
                            #endregion already visited from right side
                            #region already visited
                            else if (visitedNodesLeft.ContainsKey(nextLeftVertex.VertexID))
                            {
                                //set currentLeft as parent
                                visitedNodesLeft[nextLeftVertex.VertexID].addParent(currentNodeLeft);

                                //set currentNodeLeft as child
                                currentNodeLeft.addChild(visitedNodesLeft[nextLeftVertex.VertexID]);
                            }
                            #endregion already visited
                            #region set as visited
                            else
                            {
                                //create a new node and set currentLeft = parent
                                nextLeftNode = new Node(nextLeftVertex.VertexID, currentNodeLeft);

                                //set currentNodeLeft as child of currentLeft
                                currentNodeLeft.addChild(nextLeftNode);

                                //never seen before
                                //mark the node as visited
                                visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);

                                //and put node into the queue
                                queueLeft.Enqueue(nextLeftVertex);
                            }
                            #endregion set as visited
                        }
                        #endregion check left friends
                    }
                    #endregion only the edge exists
                    #region only the backwardedge exists
                    else if (currentVertexRight.HasIncomingVertices(currentVertexRight.VertexTypeID, myTypeAttribute.ID))
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var rightVertices = currentVertexRight.GetIncomingVertices(currentVertexRight.VertexTypeID, myTypeAttribute.ID);

                        #region check right friends
                        foreach (var nextRightVertex in rightVertices)
                        {
                            Node nextRightNode;

                            #region if the child is the target
                            if (root.Key.Equals(nextRightVertex.VertexID))
                            {
                                #region check if already visited
                                //mark node as visited
                                if (visitedNodesRight.ContainsKey(nextRightVertex.VertexID))
                                {
                                    //set found children
                                    visitedNodesRight[nextRightVertex.VertexID].addChild(currentNodeRight);

                                    currentNodeRight.addParent(visitedNodesRight[nextRightVertex.VertexID]);
                                }
                                else
                                {
                                    //create a new node and set currentRight = child                            
                                    nextRightNode = new Node(nextRightVertex.VertexID);
                                    nextRightNode.addChild(currentNodeRight);

                                    //set currentNodeRight as parent of current Right
                                    currentNodeRight.addParent(nextRightNode);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesRight.Add(nextRightNode.Key, nextRightNode);

                                    //and look what comes on the next level of depth
                                    queueRight.Enqueue(nextRightVertex);
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

                                #endregion check how much paths are searched
                            }
                            #endregion if the child is the target
                            #region already visited from left side
                            else if (visitedNodesLeft.ContainsKey(nextRightVertex.VertexID))
                            {
                                //get node
                                Node temp = visitedNodesLeft[nextRightVertex.VertexID];
                                temp.addChild(currentNodeRight);
                                currentNodeRight.addParent(temp);

                                visitedNodesLeft.Remove(temp.Key);
                                visitedNodesLeft.Add(temp.Key, temp);

                                if (visitedNodesRight.Remove(temp.Key))
                                {
                                    visitedNodesRight.Add(temp.Key, temp);
                                }

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

                                    return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                }
                                else if (shortestOnly && findAll)
                                {
                                    maxDepthRight = depthRight;

                                    shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                }
                            }
                            #endregion already visited from left side
                            #region already visited
                            else if (visitedNodesRight.ContainsKey(nextRightVertex.VertexID))
                            {
                                //set found children
                                visitedNodesRight[nextRightVertex.VertexID].addChild(currentNodeRight);

                                currentNodeRight.addParent(visitedNodesRight[nextRightVertex.VertexID]);
                            }
                            #endregion already visited
                            #region set as visited
                            else
                            {
                                //create a new node and set currentRight = child                            
                                nextRightNode = new Node(nextRightVertex.VertexID);
                                nextRightNode.addChild(currentNodeRight);

                                //set currentNodeRight as parent of current Right
                                currentNodeRight.addParent(nextRightNode);

                                //never seen before
                                //mark the node as visited
                                visitedNodesRight.Add(nextRightNode.Key, nextRightNode);

                                //and look what comes on the next level of depth
                                queueRight.Enqueue(nextRightVertex);
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
                    if (queueLeft.First<IVertex>() == null)
                    {
                        queueLeft.Dequeue();

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

                    #region get first nodes of the queues
                    //hold the actual element of the queues
                    Node currentNodeLeft;

                    IVertex currentVertexLeft;

                    //get the first Object of the queue
                    currentVertexLeft = queueLeft.Dequeue();

                    if (visitedVerticesLeft.Contains(currentVertexLeft.VertexID))
                    {
                        continue;
                    }

                    visitedVerticesLeft.Add(currentVertexLeft.VertexID);

                    if (visitedNodesLeft.ContainsKey(currentVertexLeft.VertexID))
                    {
                        currentNodeLeft = visitedNodesLeft[currentVertexLeft.VertexID];
                    }
                    else
                    {
                        currentNodeLeft = new Node(currentVertexLeft.VertexID);
                    }
                    #endregion 

                    if (currentVertexLeft.HasOutgoingEdge(myTypeAttribute.ID))
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var leftVertices = currentVertexLeft.GetOutgoingEdge(myTypeAttribute.ID).GetTargetVertices();

                        #region check left friends
                        foreach (var nextLeftVertex in leftVertices)
                        {
                            Node nextLeftNode;

                            #region if the child is the target
                            if (nextLeftVertex.VertexID.Equals(target.Key))
                            {
                                //set currentLeft as parent of target
                                target.addParent(currentNodeLeft);

                                #region check if already visited
                                if (visitedNodesLeft.ContainsKey(nextLeftVertex.VertexID))
                                {
                                    //set currentLeft as parent
                                    visitedNodesLeft[nextLeftVertex.VertexID].addParent(currentNodeLeft);

                                    //set currentNodeLeft as child
                                    currentNodeLeft.addChild(visitedNodesLeft[nextLeftVertex.VertexID]);
                                }
                                else
                                {
                                    //create a new node and set currentLeft = parent
                                    nextLeftNode = new Node(nextLeftVertex.VertexID, currentNodeLeft);

                                    //set currentNodeLeft as child of currentLeft
                                    currentNodeLeft.addChild(nextLeftNode);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesLeft.Add(nextLeftNode.Key, nextLeftNode);

                                    //and put node into the queue
                                    queueLeft.Enqueue(nextLeftVertex);
                                }
                                #endregion

                                #region check how much parents are searched

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
                            #region already visited from right side
                            else if (visitedNodesRight.ContainsKey(nextLeftVertex.VertexID))
                            {
                                //get node
                                Node temp = visitedNodesRight[nextLeftVertex.VertexID];
                                //add parent new
                                temp.addParent(currentNodeLeft);
                                //add as child
                                currentNodeLeft.addChild(temp);

                                visitedNodesRight.Remove(temp.Key);
                                visitedNodesRight.Add(temp.Key, temp);

                                if (visitedNodesLeft.Remove(temp.Key))
                                {
                                    visitedNodesLeft.Add(temp.Key, temp);
                                }

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

                                    return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                }
                                else if (shortestOnly && findAll)
                                {
                                    maxDepthLeft = depthLeft;

                                    shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                }
                            }
                            #endregion already visited from right side
                            #region already visited
                            else if (visitedNodesLeft.ContainsKey(nextLeftVertex.VertexID))
                            {
                                //set currentLeft as parent
                                visitedNodesLeft[nextLeftVertex.VertexID].addParent(currentNodeLeft);

                                //set currentNodeLeft as child
                                currentNodeLeft.addChild(visitedNodesLeft[nextLeftVertex.VertexID]);
                            }
                            #endregion already visited
                            #region set as visited
                            else
                            {
                                //create a new node and set currentLeft = parent
                                nextLeftNode = new Node(nextLeftVertex.VertexID, currentNodeLeft);

                                //set currentNodeLeft as child of currentLeft
                                currentNodeLeft.addChild(nextLeftNode);

                                //never seen before
                                //mark the node as visited
                                visitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);

                                //and put node into the queue
                                queueLeft.Enqueue(nextLeftVertex);
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
                    #region check if first element of the queue is a dummy
                    //dummy
                    if (queueRight.First<IVertex>() == null)
                    {
                        queueRight.Dequeue();

                        depthRight++;

                        if (queueRight.Count == 0)
                        {
                            continue;
                        }
                    }
                    #endregion check if first element of the queue is a dummy

                    #region get first nodes of the queues
                    //hold the actual element of the queues
                    Node currentNodeRight;

                    IVertex currentVertexRight;

                    //get the first Object of the queue
                    currentVertexRight = queueRight.Dequeue();

                    if (visitedVerticesRight.Contains(currentVertexRight.VertexID))
                    {
                        continue;
                    }

                    visitedVerticesRight.Add(currentVertexRight.VertexID);

                    if (visitedNodesRight.ContainsKey(currentVertexRight.VertexID))
                    {
                        currentNodeRight = visitedNodesRight[currentVertexRight.VertexID];
                    }
                    else
                    {
                        currentNodeRight = new Node(currentVertexRight.VertexID);
                    }
                    #endregion

                    if (currentVertexRight.HasIncomingVertices(currentVertexRight.VertexTypeID, myTypeAttribute.ID))
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var rightVertices = currentVertexRight.GetIncomingVertices(currentVertexRight.VertexTypeID, myTypeAttribute.ID);

                        #region check right friends
                        foreach (var nextRightVertex in rightVertices)
                        {
                            Node nextRightNode;

                            #region if the child is the target
                            if (root.Key.Equals(nextRightVertex.VertexID))
                            {
                                #region check if already visited
                                //mark node as visited
                                if (visitedNodesRight.ContainsKey(nextRightVertex.VertexID))
                                {
                                    //set found children
                                    visitedNodesRight[nextRightVertex.VertexID].addChild(currentNodeRight);

                                    currentNodeRight.addParent(visitedNodesRight[nextRightVertex.VertexID]);
                                }
                                else
                                {
                                    //create a new node and set currentRight = child                            
                                    nextRightNode = new Node(nextRightVertex.VertexID);
                                    nextRightNode.addChild(currentNodeRight);

                                    //set currentNodeRight as parent of current Right
                                    currentNodeRight.addParent(nextRightNode);

                                    //never seen before
                                    //mark the node as visited
                                    visitedNodesRight.Add(nextRightNode.Key, nextRightNode);

                                    //and look what comes on the next level of depth
                                    queueRight.Enqueue(nextRightVertex);
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

                                #endregion check how much paths are searched
                            }
                            #endregion if the child is the target
                            #region already visited from left side
                            else if (visitedNodesLeft.ContainsKey(nextRightVertex.VertexID))
                            {
                                //get node
                                Node temp = visitedNodesLeft[nextRightVertex.VertexID];
                                temp.addChild(currentNodeRight);
                                currentNodeRight.addParent(temp);

                                visitedNodesLeft.Remove(temp.Key);
                                visitedNodesLeft.Add(temp.Key, temp);

                                if (visitedNodesRight.Remove(temp.Key))
                                {
                                    visitedNodesRight.Add(temp.Key, temp);
                                }

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

                                    return new TargetAnalyzer(root, target, shortestPathLength, shortestOnly, findAll).getPaths();
                                }
                                else if (shortestOnly && findAll)
                                {
                                    maxDepthRight = depthRight;

                                    shortestPathLength = Convert.ToByte(maxDepthLeft + maxDepthRight);
                                }
                            }
                            #endregion already visited from left side
                            #region already visited
                            else if (visitedNodesRight.ContainsKey(nextRightVertex.VertexID))
                            {
                                //set found children
                                visitedNodesRight[nextRightVertex.VertexID].addChild(currentNodeRight);

                                currentNodeRight.addParent(visitedNodesRight[nextRightVertex.VertexID]);
                            }
                            #endregion already visited
                            #region set as visited
                            else
                            {
                                //create a new node and set currentRight = child                            
                                nextRightNode = new Node(nextRightVertex.VertexID);
                                nextRightNode.addChild(currentNodeRight);

                                //set currentNodeRight as parent of current Right
                                currentNodeRight.addParent(nextRightNode);

                                //never seen before
                                //mark the node as visited
                                visitedNodesRight.Add(nextRightNode.Key, nextRightNode);

                                //and look what comes on the next level of depth
                                queueRight.Enqueue(nextRightVertex);
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

            //get result paths
            #region start TargetAnalyzer
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
            #endregion start TargetAnalyzer

            #endregion BidirectionalBFS
        }
    }
}

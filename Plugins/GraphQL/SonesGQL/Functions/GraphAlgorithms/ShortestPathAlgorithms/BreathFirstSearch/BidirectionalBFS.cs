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
    public sealed class BidirectionalBFS
    {
        #region data

        //queue for BidirectionalBFS
        Queue<IVertex> _QueueLeft;
        Queue<IVertex> _QueueRight;

        //Dictionary to store visited TreeNodes
        Dictionary<Tuple<long, long>, Node> _VisitedNodesLeft;
        Dictionary<Tuple<long, long>, Node> _VisitedNodesRight;

        HashSet<Tuple<long, long>> _VisitedVerticesLeft;
        HashSet<Tuple<long, long>> _VisitedVerticesRight;

        //current depth
        byte _DepthLeft;
        byte _DepthRight;

        //maximum depth
        byte _MaxDepthLeft;
        byte _MaxDepthRight;

        //shortest path length
        byte _ShortestPathLength;

        //the maximum path length
        byte _MaxPathLength;

        Node _Target;
        Node _Root;

        //dummy node to check in which level the BFS is
        IVertex _DummyLeft;
        IVertex _DummyRight;

        bool _ShortestOnly;
        bool _FindAll;

        //the edge wich is used to traverse the nodes
        IAttributeDefinition _AttributeDefinition;

        //is needed to search in an inheritance hierarchy
        //this list contains each type in an inheritance hirarchy, beginning on the reference type (of the select) up to the type on which the outgoing edge is defined
        List<IVertexType> _Types;

        #endregion

        #region constructor

        public BidirectionalBFS()
        {
            _QueueLeft = new Queue<IVertex>();
            _QueueRight = new Queue<IVertex>();

            _VisitedNodesLeft = new Dictionary<Tuple<long, long>, Node>();
            _VisitedNodesRight = new Dictionary<Tuple<long, long>, Node>();

            _VisitedVerticesLeft = new HashSet<Tuple<long, long>>();
            _VisitedVerticesRight = new HashSet<Tuple<long, long>>();

            _Types = new List<IVertexType>();
        }

        #endregion

        /// <summary>
        /// Please look at the class documentation for detailed description how this algorithm works.
        /// </summary>
        /// <param name="myTypeAttribute">The Attribute representing the edge to follow (p.e. "Friends")</param>
        /// <param name="myVertexType">The type of the start node.</param>
        /// <param name="myStart">The start node</param>
        /// <param name="myEnd">The end node</param>
        /// <param name="shortestOnly">true, if only shortest path shall be found</param>
        /// <param name="findAll">if true and shortestOnly is true, all shortest paths will be found. if true, and shortest only is false, all paths will be searched</param>
        /// <param name="myMaxDepth">The maximum depth to search</param>
        /// <param name="myMaxPathLength">The maximum path length which shall be analyzed</param>
        /// <returns>A HashSet which contains all found paths. Every path is represented by a List of ObjectUUIDs</returns>m>
        public HashSet<List<Tuple<long, long>>> Find(IAttributeDefinition myTypeAttribute,
                                                        IVertexType myVertexType,
                                                        IVertex myStart,
                                                        IVertex myEnd,
                                                        bool shortestOnly,
                                                        bool findAll,
                                                        byte myMaxDepth,
                                                        byte myMaxPathLength)
        {
            #region declarations

            //set current depth
            _DepthLeft = 1;
            _DepthRight = 1;

            //maximum depth
            _MaxDepthLeft = 0;
            _MaxDepthRight = 0;

            _MaxPathLength = myMaxPathLength;

            #region initialize maxDepths

            //if the maxDepth is greater then maxPathLength, then set maxDepth to maxPathLength
            if (myMaxDepth > _MaxPathLength)
                myMaxDepth = _MaxPathLength;
            else if (_MaxPathLength > myMaxDepth)
                _MaxPathLength = myMaxDepth;

            //set depth for left side
            _MaxDepthLeft = Convert.ToByte(myMaxDepth / 2 + 1);

            //if myMaxDepth is 1 _MaxDepthRight keeps 0, just one side is searching
            if (myMaxDepth > 1)
            {
                //both sides have the same depth
                _MaxDepthRight = _MaxDepthLeft;
            }

            //if myMaxDepth is even, one side has to search in a greater depth
            if ((myMaxDepth % 2) == 0)
            {
                _MaxDepthRight = Convert.ToByte(_MaxDepthLeft - 1);
            }

            #endregion

            //shortest path length
            _ShortestPathLength = 0;

            //_Target node, the _Target of the select
            _Target = new Node(myEnd.VertexTypeID, myEnd.VertexID);
            _Root = new Node(myStart.VertexTypeID, myStart.VertexID);

            //the attribute (edge) which is used for the search
            _AttributeDefinition = myTypeAttribute;

            //search the type on which the attribute is defined
            IVertexType current = myVertexType;
            List<IVertexType> tempList = new List<IVertexType>();
            tempList.Add(current);

            bool foundDefinedType = false;

            while (current.HasParentType && !foundDefinedType)
            {
                if (current.ParentVertexType.HasAttribute(_AttributeDefinition.Name))
                {
                    foundDefinedType = true;
                }

                current = current.ParentVertexType;
                tempList.Add(current);
            }

            if (foundDefinedType)
                _Types = tempList;
            else
                _Types.Add(myVertexType);

            //dummy node to check in which level the BFS is
            _DummyLeft = null;
            _DummyRight = null;

            _ShortestOnly = shortestOnly;
            _FindAll = findAll;

            #endregion

            #region BidirectionalBFS
            //check if the EdgeType is ASetReferenceEdgeType

            #region initialize variables

            //enqueue start node to start from left side
            _QueueLeft.Enqueue(myStart);
            //enqueue _DummyLeft to analyze the depth of the left side
            _QueueLeft.Enqueue(_DummyLeft);

            //enqueue _Target node to start from right side
            _QueueRight.Enqueue(myEnd);
            //enqueue _DummyRight to analyze the depth of the right side
            _QueueRight.Enqueue(_DummyRight);

            //add _Root and _Target to visitedNodes
            _VisitedNodesLeft.Add(_Root.Key, _Root);
            _VisitedNodesRight.Add(_Target.Key, _Target);

            #endregion

            #region check if start has outgoing and _Target has incoming edge

            //check that the start node has the outgoing edge and the target has incoming vertices
            if (!myStart.HasOutgoingEdge(_AttributeDefinition.ID) && !HasIncomingVertices(myEnd))
            {
                return null;
            }

            #endregion

            //if there is more than one object in the queue and the actual depth is less than MaxDepth
            while (((_QueueLeft.Count > 0) && (_QueueRight.Count > 0)) && ((_DepthLeft <= _MaxDepthLeft) || (_DepthRight <= _MaxDepthRight)))
            {
                #region both queues contain objects and both depths are not reached
                if (((_QueueLeft.Count > 0) && (_QueueRight.Count > 0)) && ((_DepthLeft <= _MaxDepthLeft) && (_DepthRight <= _MaxDepthRight)))
                {
                    #region check if a level is completely searched

                    if (LeftLevelCompleted())
                        continue;

                    if (RightLevelCompleted())
                        continue;

                    #endregion check if there is a dummyNode at the beginning of a queue

                    #region get first nodes of the queues

                    //hold the actual element of the queues
                    Node currentNodeLeft;
                    Node currentNodeRight;

                    IVertex currentVertexLeft;
                    IVertex currentVertexRight;

                    Tuple<long, long> currentLeft;
                    Tuple<long, long> currentRight;

                    //get the first Object of the queue
                    currentVertexLeft = _QueueLeft.Dequeue();
                    currentLeft = new Tuple<long, long>(currentVertexLeft.VertexTypeID, currentVertexLeft.VertexID);

                    if (_VisitedVerticesLeft.Contains(currentLeft))
                        continue;

                    //get the first Object of the queue
                    currentVertexRight = _QueueRight.Dequeue();
                    currentRight = new Tuple<long, long>(currentVertexRight.VertexTypeID, currentVertexRight.VertexID);

                    if (_VisitedVerticesRight.Contains(currentRight))
                    {
                        //enqueue already dequeued vertex
                        _QueueLeft.Enqueue(currentVertexLeft);

                        continue;
                    }

                    _VisitedVerticesLeft.Add(currentLeft);
                    _VisitedVerticesRight.Add(currentRight);

                    if (_VisitedNodesLeft.ContainsKey(currentLeft))
                        currentNodeLeft = _VisitedNodesLeft[currentLeft];
                    else
                        currentNodeLeft = new Node(currentLeft);

                    if (_VisitedNodesRight.ContainsKey(currentRight))
                        currentNodeRight = _VisitedNodesRight[currentRight];
                    else
                        currentNodeRight = new Node(currentRight);

                    #endregion

                    #region the edge and the backwardedge are existing
                    if (currentVertexLeft.HasOutgoingEdge(_AttributeDefinition.ID)
                        && HasIncomingVertices(currentVertexRight))
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var leftVertices = currentVertexLeft.GetOutgoingEdge(_AttributeDefinition.ID).GetTargetVertices();

                        #region check left friends
                        foreach (var nextLeftVertex in leftVertices)
                        {
                            Node nextLeftNode = null;
                            Tuple<long, long> nextLeft = new Tuple<long, long>(nextLeftVertex.VertexTypeID, nextLeftVertex.VertexID);

                            #region if the child is the _Target
                            if (nextLeft.Equals(_Target.Key))
                            {
                                if (TargetFoundCheckAbort(nextLeft, ref currentNodeLeft, ref nextLeftNode, nextLeftVertex))
                                    return new TargetAnalyzer(_Root, _Target, _ShortestPathLength, _ShortestOnly, _FindAll).GetPaths();
                            }
                            #endregion
                            #region already visited
                            else if (_VisitedNodesLeft.ContainsKey(nextLeft))
                            {
                                UpdateVisitedLeft(nextLeft, ref currentNodeLeft);
                            }
                            #endregion already visited
                            #region set as visited
                            else
                            {
                                SetAsVisitedLeft(nextLeft, ref currentNodeLeft, ref nextLeftNode, nextLeftVertex);
                            }
                            #endregion set as visited
                        }
                        #endregion check left friends

                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var rightVertices = GetIncomingVertices(currentVertexRight);

                        #region check right friends
                        foreach (var nextRightVertex in rightVertices)
                        {
                            Node nextRightNode = null;
                            Tuple<long, long> nextRight = new Tuple<long, long>(nextRightVertex.VertexTypeID, nextRightVertex.VertexID);

                            #region if the child is the _Target
                            if (_Root.Key.Equals(nextRight))
                            {
                                if (RootFoundCheckAbort(nextRight, ref currentNodeRight, ref nextRightNode, nextRightVertex))
                                    return new TargetAnalyzer(_Root, _Target, _ShortestPathLength, _ShortestOnly, _FindAll).GetPaths();
                            }
                            #endregion if the child is the _Target
                            #region already visited
                            else if (_VisitedNodesRight.ContainsKey(nextRight))
                            {
                                UpdateVisitedRight(nextRight, ref currentNodeRight);
                            }
                            #endregion already visited
                            #region set as visited
                            else
                            {
                                SetAsVisitedRight(nextRight, ref currentNodeRight, ref nextRightNode, nextRightVertex);
                            }
                            #endregion set as visited
                        }
                        #endregion check right friends

                        #region build intersection of _VisitedNodesLeft and _VisitedNodesRight
                        //marks if intersection nodes are existing
                        bool foundIntersect = false;

                        foreach (var node in _VisitedNodesLeft)
                        {
                            if (_VisitedNodesRight.ContainsKey(node.Key))
                            {
                                //set nodes children and parents
                                node.Value.addChildren(_VisitedNodesRight[node.Key].Children);
                                node.Value.addParents(_VisitedNodesRight[node.Key].Parents);

                                //set nodes children and parents
                                _VisitedNodesRight[node.Key].addChildren(node.Value.Children);
                                _VisitedNodesRight[node.Key].addParents(node.Value.Parents);

                                foundIntersect = true;
                            }
                        }
                        #endregion build intersection of _VisitedNodesLeft and _VisitedNodesRight

                        #region analyze intersection
                        //if intersection nodes existing
                        if (foundIntersect)
                        {
                            //only shortest path
                            if (_ShortestOnly && !_FindAll)
                            {
                                //_Logger.Info("found shortest path..starting analyzer");

                                if ((_DepthLeft + _DepthRight + 1) > _MaxPathLength)
                                {
                                    _ShortestPathLength = _MaxPathLength;
                                }
                                else
                                {
                                    _ShortestPathLength = Convert.ToByte(_DepthLeft + _DepthRight + 1);
                                }

                                return new TargetAnalyzer(_Root, _Target, _ShortestPathLength, _ShortestOnly, _FindAll).GetPaths();
                            }
                            //if find all shortest paths
                            else if (_ShortestOnly && _FindAll)
                            {
                                //set maxDepth to actual depth
                                _MaxDepthLeft = _DepthLeft;
                                _MaxDepthRight = _DepthRight;

                                if ((_DepthLeft + _DepthRight + 1) > _MaxPathLength)
                                {
                                    _ShortestPathLength = _MaxPathLength;
                                }
                                else if (_ShortestPathLength == 0)
                                {
                                    _ShortestPathLength = Convert.ToByte(_DepthLeft + _DepthRight + 1);
                                }

                            }
                        }
                        #endregion analyze intersection
                    }
                    #endregion the edge and the backwardedge are existing
                    #region only the edge exists
                    else if (currentVertexLeft.HasOutgoingEdge(_AttributeDefinition.ID))
                    {
                        var result = CheckNextVerticesOfLeftSide(ref currentVertexLeft, ref currentNodeLeft);

                        if (result != null)
                            return result;
                    }
                    #endregion only the edge exists
                    #region only the backwardedge exists
                    else if (HasIncomingVertices(currentVertexRight))
                    {
                        var result = CheckNextVerticesOfRightSide(ref currentVertexRight, ref currentNodeRight);

                        if (result != null)
                            return result;
                    }
                    #endregion only the backwardedge exists
                }
                #endregion  both queues contain objects and both depths are not reached
                #region only left queue contain objects
                else if ((_QueueLeft.Count > 0) && (_DepthLeft <= _MaxDepthLeft))
                {
                    #region check if first element of queue is a dummy

                    if (LeftLevelCompleted())
                        continue;

                    #endregion check if first element of queue is a dummy

                    #region get first nodes of the queues

                    //hold the actual element of the queues
                    Node currentNodeLeft;
                    IVertex currentVertexLeft;

                    //get the first Object of the queue
                    currentVertexLeft = _QueueLeft.Dequeue();
                    Tuple<long, long> currentLeft = new Tuple<long, long>(currentVertexLeft.VertexTypeID, currentVertexLeft.VertexID);

                    if (_VisitedVerticesLeft.Contains(currentLeft))
                        continue;

                    _VisitedVerticesLeft.Add(currentLeft);

                    if (_VisitedNodesLeft.ContainsKey(currentLeft))
                        currentNodeLeft = _VisitedNodesLeft[currentLeft];
                    else
                        currentNodeLeft = new Node(currentLeft);

                    #endregion

                    #region check next vertices

                    if (currentVertexLeft.HasOutgoingEdge(_AttributeDefinition.ID))
                    {
                        var result = CheckNextVerticesOfLeftSide(ref currentVertexLeft, ref currentNodeLeft);

                        if (result != null)
                            return result;
                    }

                    #endregion
                }
                #endregion only left queue contain objects
                #region only right queue contain objects
                else if ((_QueueRight.Count > 0) && (_DepthRight <= _MaxDepthRight))
                {
                    #region check if first element of the queue is a dummy

                    if (RightLevelCompleted())
                        continue;

                    #endregion check if first element of the queue is a dummy

                    #region get first nodes of the queues

                    //hold the actual element of the queues
                    Node currentNodeRight;
                    IVertex currentVertexRight;

                    //get the first Object of the queue
                    currentVertexRight = _QueueRight.Dequeue();
                    Tuple<long, long> currentRight = new Tuple<long, long>(currentVertexRight.VertexTypeID, currentVertexRight.VertexID);

                    if (_VisitedVerticesRight.Contains(currentRight))
                        continue;

                    _VisitedVerticesRight.Add(currentRight);

                    if (_VisitedNodesRight.ContainsKey(currentRight))
                        currentNodeRight = _VisitedNodesRight[currentRight];
                    else
                        currentNodeRight = new Node(currentRight);

                    #endregion

                    #region check next vertices

                    if (HasIncomingVertices(currentVertexRight))
                    {
                        var result = CheckNextVerticesOfRightSide(ref currentVertexRight, ref currentNodeRight);

                        if (result != null)
                            return result;
                    }

                    #endregion
                }
                #endregion only right queue contain objects
                #region abort loop

                else
                    break;

                #endregion abort loop
            }

            //get result paths
            #region start TargetAnalyzer
            if (_ShortestOnly && _FindAll)
            {
                if (_ShortestPathLength > _MaxPathLength)
                {
                    _ShortestPathLength = _MaxPathLength;
                }

                return new TargetAnalyzer(_Root, _Target, _ShortestPathLength, _ShortestOnly, _FindAll).GetPaths();
            }
            else
            {
                return new TargetAnalyzer(_Root, _Target, _MaxPathLength, _ShortestOnly, _FindAll).GetPaths();
            }
            #endregion start TargetAnalyzer

            #endregion BidirectionalBFS
        }

        #region private methods

        private bool LeftLevelCompleted()
        {
            //first of right queue is a dummy
            if (_QueueLeft.First<IVertex>() == null)
            {
                //if maxDepth of a side is reached and there is a dummy, one level is totaly searched
                if (_DepthLeft == _MaxDepthLeft)
                {
                    _DepthLeft++;

                    return true;
                }

                //dequeue dummy
                _QueueLeft.Dequeue();

                //increase depth
                _DepthLeft++;

                //if left queue is empty continue
                if (_QueueLeft.Count == 0)
                {
                    return true;
                }
                //enqueue dummy
                else
                {
                    _QueueLeft.Enqueue(_DummyLeft);
                }
            }

            return false;
        }

        private bool RightLevelCompleted()
        {
            //first of right queue is a dummy
            if (_QueueRight.First<IVertex>() == null)
            {
                //if maxDepth of a side is reached and there is a dummy, one level is totaly searched
                if (_DepthRight == _MaxDepthRight)
                {
                    _DepthRight++;

                    return true;
                }

                //dequeue dummy
                _QueueRight.Dequeue();

                //increase depth
                _DepthRight++;

                //if right queue is empty continue
                if (_QueueRight.Count == 0)
                {
                    return true;
                }
                //enqueue dummy
                else
                {
                    _QueueRight.Enqueue(_DummyRight);
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the search should be aborted.
        /// </summary>
        /// <param name="myTuple">the Tuple of the current Vertex.</param>
        /// <param name="myCurrentNode">The current Node.</param>
        /// <param name="myNextNode">The next Node.</param>
        /// <param name="myVertex">The actual Vertex.</param>
        /// <returns>True, if the target is found AND the BFS could be finished. False, if the BFS should be continued.</returns>
        private bool TargetFoundCheckAbort(Tuple<long, long> myTuple, ref Node myCurrentNode, ref Node myNextNode, IVertex myVertex)
        {
            if (myVertex.VertexID.Equals(_Target.Key))
            {
                //set currentLeft as parent of _Target
                _Target.addParent(myCurrentNode);

                #region check if already visited
                if (_VisitedNodesLeft.ContainsKey(myTuple))
                {
                    //set currentLeft as parent
                    _VisitedNodesLeft[myTuple].addParent(myCurrentNode);

                    //set currentNodeLeft as child
                    myCurrentNode.addChild(_VisitedNodesLeft[myTuple]);
                }
                else
                {
                    //create a new node and set currentLeft = parent
                    myNextNode = new Node(myTuple, myCurrentNode);

                    //set currentNodeLeft as child of currentLeft
                    myCurrentNode.addChild(myNextNode);

                    //never seen before
                    //mark the node as visited
                    _VisitedNodesLeft.Add(myNextNode.Key, myNextNode);

                    //and put node into the queue
                    _QueueLeft.Enqueue(myVertex);
                }

                #endregion

                #region check how much parents are searched

                if (_ShortestOnly && !_FindAll)
                {
                    if ((_DepthLeft + _DepthRight + 1) > _MaxPathLength)
                    {
                        _ShortestPathLength = _MaxPathLength;
                    }
                    else
                    {
                        _ShortestPathLength = Convert.ToByte(_DepthLeft + _DepthRight + 1);
                    }

                    return true;
                }
                //if find all shortest paths
                else if (_ShortestOnly && _FindAll)
                {
                    //set maxDepth to actual depth
                    _MaxDepthLeft = _DepthLeft;
                    _MaxDepthRight = _DepthRight;

                    if ((_DepthLeft + _DepthRight) > _MaxPathLength)
                    {
                        _ShortestPathLength = _MaxPathLength;
                    }
                    else
                    {
                        _ShortestPathLength = Convert.ToByte(_DepthLeft + _DepthRight);
                    }
                }

                #endregion
            }

            return false;
        }

        private bool RootFoundCheckAbort(Tuple<long, long> myTuple, ref Node myCurrentNode, ref Node myNextNode, IVertex myVertex)
        {
            #region check if already visited
            //mark node as visited
            if (_VisitedNodesRight.ContainsKey(myTuple))
            {
                //set found children
                _VisitedNodesRight[myTuple].addChild(myCurrentNode);

                myCurrentNode.addParent(_VisitedNodesRight[myTuple]);
            }
            else
            {
                //create a new node and set currentRight = child                            
                myNextNode = new Node(myTuple);
                myNextNode.addChild(myCurrentNode);

                //set currentNodeRight as parent of current Right
                myCurrentNode.addParent(myNextNode);

                //never seen before
                //mark the node as visited
                _VisitedNodesRight.Add(myNextNode.Key, myNextNode);

                //and look what comes on the next level of depth
                _QueueRight.Enqueue(myVertex);
            }

            #endregion check if already visited
            #region check how much paths are searched

            if (_ShortestOnly && !_FindAll)
            {
                if ((_DepthLeft + _DepthRight + 1) > _MaxPathLength)
                {
                    _ShortestPathLength = _MaxPathLength;
                }
                else
                {
                    _ShortestPathLength = Convert.ToByte(_DepthLeft + _DepthRight + 1);
                }

                return true;
            }
            //if find all shortest paths
            else if (_ShortestOnly && _FindAll)
            {
                //set maxDepth to actual depth
                _MaxDepthLeft = _DepthLeft;
                _MaxDepthRight = _DepthRight;

                if ((_DepthLeft + _DepthRight) > _MaxPathLength)
                {
                    _ShortestPathLength = _MaxPathLength;
                }
                else
                {
                    _ShortestPathLength = Convert.ToByte(_DepthLeft + _DepthRight);
                }
            }
            #endregion check how much paths are searched

            return false;
        }

        private void UpdateVisitedLeft(Tuple<long, long> myTuple, ref Node myNode)
        {
            //set currentLeft as parent
            _VisitedNodesLeft[myTuple].addParent(myNode);

            //set currentNodeLeft as child
            myNode.addChild(_VisitedNodesLeft[myTuple]);
        }

        private void SetAsVisitedLeft(Tuple<long, long> myTuple, ref Node myCurrentNode, ref Node myNextNode, IVertex myVertex)
        {
            //create a new node and set currentLeft = parent
            myNextNode = new Node(myTuple, myCurrentNode);

            //set currentNodeLeft as child of currentLeft
            myCurrentNode.addChild(myNextNode);

            //never seen before
            //mark the node as visited
            //_VisitedNodesLeft.Add(currentNodeLeft.Key, currentNodeLeft);
            _VisitedNodesLeft.Add(myNextNode.Key, myNextNode);

            //and put node into the queue
            _QueueLeft.Enqueue(myVertex);
        }

        private void UpdateVisitedRight(Tuple<long, long> myTuple, ref Node myNode)
        {
            //set found children
            _VisitedNodesRight[myTuple].addChild(myNode);

            myNode.addParent(_VisitedNodesRight[myTuple]);
        }

        private void SetAsVisitedRight(Tuple<long, long> myTuple, ref Node myCurrentNode, ref Node myNextNode, IVertex myVertex)
        {
            //create a new node and set currentRight = child                            
            myNextNode = new Node(myTuple);
            myNextNode.addChild(myCurrentNode);

            //set currentNodeRight as parent of current Right
            myCurrentNode.addParent(myNextNode);

            //never seen before
            //mark the node as visited
            _VisitedNodesRight.Add(myNextNode.Key, myNextNode);

            //and look what comes on the next level of depth
            _QueueRight.Enqueue(myVertex);
        }

        private bool VisitedByLeftSide(Tuple<long, long> myTuple, ref Node myCurrentNodeRight)
        {
            //get node
            Node temp = _VisitedNodesLeft[myTuple];
            temp.addChild(myCurrentNodeRight);
            myCurrentNodeRight.addParent(temp);

            _VisitedNodesLeft.Remove(temp.Key);
            _VisitedNodesLeft.Add(temp.Key, temp);

            if (_VisitedNodesRight.Remove(temp.Key))
            {
                _VisitedNodesRight.Add(temp.Key, temp);
            }

            if (_ShortestOnly && !_FindAll)
            {
                if ((_DepthLeft + _DepthRight + 1) > _MaxPathLength)
                {
                    _ShortestPathLength = _MaxPathLength;
                }
                else
                {
                    _ShortestPathLength = Convert.ToByte(_DepthLeft + _DepthRight + 1);
                }

                return true;
            }
            else if (_ShortestOnly && _FindAll)
            {
                _MaxDepthRight = _DepthRight;

                _ShortestPathLength = Convert.ToByte(_MaxDepthLeft + _MaxDepthRight);
            }

            return false;
        }

        private bool VisitedByRightSide(Tuple<long, long> myTuple, ref Node myCurrentNodeLeft)
        {
            //get node
            Node temp = _VisitedNodesRight[myTuple];
            //add parent new
            temp.addParent(myCurrentNodeLeft);
            //add as child
            myCurrentNodeLeft.addChild(temp);

            _VisitedNodesRight.Remove(temp.Key);
            _VisitedNodesRight.Add(temp.Key, temp);

            if (_VisitedNodesLeft.Remove(temp.Key))
            {
                _VisitedNodesLeft.Add(temp.Key, temp);
            }

            if (_ShortestOnly && !_FindAll)
            {
                if ((_DepthLeft + _DepthRight + 1) > _MaxPathLength)
                {
                    _ShortestPathLength = _MaxPathLength;
                }
                else
                {
                    _ShortestPathLength = Convert.ToByte(_DepthLeft + _DepthRight + 1);
                }

                return true;
            }
            else if (_ShortestOnly && _FindAll)
            {
                _MaxDepthLeft = _DepthLeft;

                _ShortestPathLength = Convert.ToByte(_MaxDepthLeft + _MaxDepthRight);
            }

            return false;
        }

        private HashSet<List<Tuple<long, long>>> CheckNextVerticesOfLeftSide(ref IVertex myCurrentVertexLeft, ref Node myCurrentNodeLeft)
        {
            //get all referenced ObjectUUIDs using the given Edge                                                
            var leftVertices = myCurrentVertexLeft.GetOutgoingEdge(_AttributeDefinition.ID).GetTargetVertices();

            #region check left friends
            foreach (var nextLeftVertex in leftVertices)
            {
                Node nextLeftNode = null;
                Tuple<long, long> nextLeft = new Tuple<long, long>(nextLeftVertex.VertexTypeID, nextLeftVertex.VertexID);

                #region if the child is the _Target
                if (nextLeft.Equals(_Target.Key))
                {
                    if (TargetFoundCheckAbort(nextLeft, ref myCurrentNodeLeft, ref nextLeftNode, nextLeftVertex))
                        return new TargetAnalyzer(_Root, _Target, _ShortestPathLength, _ShortestOnly, _FindAll).GetPaths();
                }
                #endregion
                #region already visited from right side
                else if (_VisitedNodesRight.ContainsKey(nextLeft))
                {
                    if (VisitedByRightSide(nextLeft, ref myCurrentNodeLeft))
                        return new TargetAnalyzer(_Root, _Target, _ShortestPathLength, _ShortestOnly, _FindAll).GetPaths();
                }
                #endregion already visited from right side
                #region already visited
                else if (_VisitedNodesLeft.ContainsKey(nextLeft))
                {
                    UpdateVisitedLeft(nextLeft, ref myCurrentNodeLeft);
                }
                #endregion already visited
                #region set as visited
                else
                {
                    SetAsVisitedLeft(nextLeft, ref myCurrentNodeLeft, ref nextLeftNode, nextLeftVertex);
                }
                #endregion set as visited
            }
            #endregion check left friends

            return null;
        }

        private HashSet<List<Tuple<long, long>>> CheckNextVerticesOfRightSide(ref IVertex myCurrentVertexRight, ref Node myCurrentNodeRight)
        {
            //get all referenced ObjectUUIDs using the given Edge                                                
            var rightVertices = GetIncomingVertices(myCurrentVertexRight);

            #region check right friends
            foreach (var nextRightVertex in rightVertices)
            {
                Node nextRightNode = null;
                Tuple<long, long> nextRight = new Tuple<long, long>(nextRightVertex.VertexTypeID, nextRightVertex.VertexID);

                #region if the child is the _Target
                if (_Root.Key.Equals(nextRight))
                {
                    if (RootFoundCheckAbort(nextRight, ref myCurrentNodeRight, ref nextRightNode, nextRightVertex))
                        return new TargetAnalyzer(_Root, _Target, _ShortestPathLength, _ShortestOnly, _FindAll).GetPaths();
                }
                #endregion if the child is the _Target
                #region already visited from right side
                else if (_VisitedNodesLeft.ContainsKey(nextRight))
                {
                    if (VisitedByLeftSide(nextRight, ref myCurrentNodeRight))
                        return new TargetAnalyzer(_Root, _Target, _ShortestPathLength, _ShortestOnly, _FindAll).GetPaths();
                }
                #endregion already visited from left side
                #region already visited
                else if (_VisitedNodesRight.ContainsKey(nextRight))
                {
                    UpdateVisitedRight(nextRight, ref myCurrentNodeRight);
                }
                #endregion already visited
                #region set as visited
                else
                {
                    SetAsVisitedRight(nextRight, ref myCurrentNodeRight, ref nextRightNode, nextRightVertex);
                }
                #endregion set as visited
            }
            #endregion check right friends

            return null;
        }

        private bool HasIncomingVertices(IVertex myVertex)
        {
            foreach (var type in _Types)
            {
                if (myVertex.HasIncomingVertices(type.ID, _AttributeDefinition.ID))
                    return true;
            }

            return false;
        }
        
        private IEnumerable<IVertex> GetIncomingVertices(IVertex myVertex)
        {
            List<IVertex> temp = new List<IVertex>();

            foreach (var type in _Types)
            {
                if (myVertex.HasIncomingVertices(type.ID, _AttributeDefinition.ID))
                    temp.AddRange(myVertex.GetIncomingVertices(type.ID, _AttributeDefinition.ID));
            }

            return temp;
        }

        #endregion
    }
}

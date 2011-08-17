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
        #region data

        List<IVertexType> _Types;

        #endregion

        #region constructor

        public BFS()
        {
            _Types = new List<IVertexType>();
        }

        #endregion

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
        public HashSet<List<Tuple<long, long>>> Find(IAttributeDefinition myTypeAttribute,
                                                        IVertexType myVertexType,
                                                        IVertex myStart,
                                                        IVertex myEnd,
                                                        bool shortestOnly,
                                                        bool findAll,
                                                        byte myMaxDepth,
                                                        byte myMaxPathLength)
        {
            #region data

            //queue for BFS
            Queue<IVertex> queue = new Queue<IVertex>();

            //Dictionary to store visited TreeNodes
            Dictionary<Tuple<long, long>, Node> visitedNodes = new Dictionary<Tuple<long, long>, Node>();

            HashSet<Tuple<long, long>> visitedVertices = new HashSet<Tuple<long, long>>();

            //current depth
            byte depth = 1;

            //first node in path tree, the start of the select
            Node root = new Node(myStart.VertexTypeID, myStart.VertexID);

            //target node, the target of the select
            Node target = new Node(myEnd.VertexTypeID, myEnd.VertexID);

            //dummy node to check in which level the BFS is
            IVertex dummy = null;

            //if the maxDepth is greater then maxPathLength, then set maxDepth to maxPathLength
            if (myMaxDepth > myMaxPathLength)
                myMaxDepth = myMaxPathLength;
            else if (myMaxPathLength > myMaxDepth)
                myMaxPathLength = myMaxDepth;

            //enqueue first node to start the BFS
            queue.Enqueue(myStart);
            queue.Enqueue(dummy);

            //add root to visitedNodes
            visitedNodes.Add(root.Key, root);

            //search the type on which the attribute is defined
            IVertexType currentType = myVertexType;
            List<IVertexType> tempList = new List<IVertexType>();
            tempList.Add(currentType);

            bool foundDefinedType = false;

            while (currentType.HasParentType && !foundDefinedType)
            {
                if (currentType.ParentVertexType.HasAttribute(myTypeAttribute.Name))
                {
                    foundDefinedType = true;
                }

                currentType = currentType.ParentVertexType;
                tempList.Add(currentType);
            }

            if (foundDefinedType)
                _Types = tempList;
            else
                _Types.Add(myVertexType);

            #endregion

            #region BFS

            //check that the start node has the outgoing edge and the target has incoming vertices
            if (!myStart.HasOutgoingEdge(myTypeAttribute.ID) && !HasIncomingVertices(myEnd, myTypeAttribute.ID))
            {
                return null;
            }

            //if there is more than one object in the queue and the actual depth is less than MaxDepth
            while ((queue.Count > 0) && (depth < myMaxDepth))
            {
                //get the first Object of the queue
                IVertex currentVertex = queue.Dequeue();

                //dummy
                if (currentVertex == null || visitedVertices.Contains(new Tuple<long, long>(currentVertex.VertexTypeID, currentVertex.VertexID)))
                {
                    continue;
                }

                Tuple<long, long> current = new Tuple<long, long>(currentVertex.VertexTypeID, currentVertex.VertexID);
                Node currentNode;

                visitedVertices.Add(current);

                if (visitedNodes.ContainsKey(current))
                {
                    currentNode = visitedNodes[current];
                }
                else
                {
                    currentNode = new Node(current);
                }

                if (currentVertex.HasOutgoingEdge(myTypeAttribute.ID))
                {
                    var vertices = currentVertex.GetOutgoingEdge(myTypeAttribute.ID).GetTargetVertices();

                    Node nextNode;
                    Tuple<long, long> next;

                    foreach (var vertex in vertices)
                    {
                        //create a new node and set currentNode = parent, nextNode = child
                        next = new Tuple<long, long>(vertex.VertexTypeID, vertex.VertexID);
                        nextNode = new Node(next, currentNode);
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
                                    if (Convert.ToByte(depth + 1) <= myMaxPathLength)
                                    {
                                        //continue searching the current depth if there are any other shortest paths
                                        myMaxDepth = Convert.ToByte(depth + 1);

                                        myMaxPathLength = Convert.ToByte(depth + 1);
                                    }
                                }
                                else
                                {
                                    //got the shortest, finished
                                    return new TargetAnalyzer(root, target, myMaxPathLength, shortestOnly, findAll).GetPaths();
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
                            else
                            {
                                //mark the node as visited
                                visitedNodes.Add(nextNode.Key, nextNode);
                            }

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
            return new TargetAnalyzer(root, target, myMaxPathLength, shortestOnly, findAll).GetPaths();
        }

        #region private member

        private bool HasIncomingVertices(IVertex myVertex, long myAttributeID)
        {
            foreach (var type in _Types)
            {
                if (myVertex.HasIncomingVertices(type.ID, myAttributeID))
                    return true;
            }

            return false;
        }

        #endregion
    }
}

/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

/* <id name="sones GraphDB – BreadthFirstSearch" />
 * <copyright file="BreadthFirstSearch.cs"
 *            company="sones GmbH">
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
//using sones.Lib.Frameworks.NLog;
using sones.GraphDB.Structures.EdgeTypes;
using GraphAlgorithms.PathAlgorithm.BFSTreeStructure;
using sones.Lib.DataStructures.Big;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

#endregion

namespace GraphAlgorithms.PathAlgorithm.BreadthFirstSearch
{
    public class BreadthFirstSearch
    {
        //Logger
        //private static Logger //_Logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Please look at the class documentation for detailed description how this algorithm works.
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
            if (dbObject.Failed)
            {
                throw new NotImplementedException();
            }

            if (!dbObject.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myTypeManager), null))
            {
                ////_Logger.Info("Abort search! Start object has no edge!");
                //Console.WriteLine("No paths found!");
                return null;
            }

            var be = myDBObjectCache.LoadDBBackwardEdgeStream(myTypeAttribute.GetRelatedType(myTypeManager), target.Key);
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
                if (currentDBObject.Failed)
                {
                    throw new NotImplementedException();
                }

                if (currentDBObject.Value.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myTypeManager), null))
                {
                    if (myTypeAttribute.EdgeType is ASetReferenceEdgeType)
                    {
                        //get all referenced ObjectUUIDs using the given Edge                                                
                        var objectUUIDs = (currentDBObject.Value.GetAttribute(myTypeAttribute.UUID) as ASetReferenceEdgeType).GetAllUUIDs();
                        Node tmp;
                        
                        foreach(ObjectUUID dbo in objectUUIDs)
                        {
                            //only for debug
                            //var tmpObject = myTypeManager.LoadDBObject(myTypeAttribute.RelatedPandoraType, dbo);
                            
                            //create a new node and set current = parent, tmp = child                            
                            tmp = new Node(dbo, current);                           
                            
                            //if the child is the target
                            if (tmp.Key == myEnd.ObjectUUID)
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
                                        return new TargetAnalyzer(root, target, myMaxPathLength, shortestOnly, findAll).getPaths();
                                    }
                                }
                            }
                            else
                            {
                                //been there before
                                if (visitedNodes.ContainsKey(tmp.Key))
                                {
                                    //if tmp.Key isn't root set parent
                                    if (tmp.Key != root.Key)
                                    {
                                        //node has more then one parent
                                        visitedNodes[tmp.Key].Parents.Add(current);
                                    }
                                    continue;
                                }

                                //never seen before
                                //mark the node as visited
                                visitedNodes.Add(tmp.Key, tmp);
                                //and look what comes on the next level of depth
                                queue.Enqueue(tmp);

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
            return new TargetAnalyzer(root, target, myMaxPathLength, shortestOnly, findAll).getPaths();
        }        
    }
}

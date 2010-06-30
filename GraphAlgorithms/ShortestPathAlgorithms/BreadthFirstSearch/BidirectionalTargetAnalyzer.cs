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

/* <id name="sones GraphDB – BidirectionalTargetAnalyzer" />
 * <copyright file="BidirectionalTargetAnalyzer.cs"
 *            company="sones GmbH">
 * </copyright>
 * <developer>Michael Woidak</developer>
 * <developer>Martin Junghanns</developer>
 * <summary>
 * This class evaluates the "path-graph" as the result of an BFS on a graph.
 * It can be compared with a Depth First Search on a graph where all edges guide
 * to the target.
 * 
 * There are two different algorithm to evaluate the result. 
 * 
 * I. algorithm (recursively) like GraphAlgorithms.PathAlgorithm.BreadthFirstSearch.TargetAnalyzer:
 * 
 * 1. Add start node to the temporary path list
 * 2. if last node matches the target node
 *      duplicate temporary list
 *      reverse duplicated list
 *      store the found path
 *      remove current node from the temporary list
 *      return
 *    else
 *      mark node as visited to avoid circles
 *      for each parent of the current node
 *          start with step 1 (recursive call)
 *  3. result is an HashSet which contains all paths between
 *     start and end
 * 
 * II. algorithm (evaluate starting from a match node)
 * 
 * 1. for all match nodes
 *      build a left part of the path using the parents of the match node
 *      build a right part of the path using the children of the match node
 * 2. merge the two parts to one path
 * 3. add it to the result
 * 
 * complexity:
 * 
 * the complexity is always O(|V| + |E|), V - node count, E - edge count
 * the mightiness of V and E are dramatically reduced as a result of the BFS.
 * 
 * NOTE:
 * 
 * currently only Algorithm I is used to evaluate a graph-path, because there was no
 * speedup using the bidirectional path evaluation.
 * 
 * </summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

using GraphAlgorithms.PathAlgorithm.BFSTreeStructure;

#endregion

namespace GraphAlgorithms.PathAlgorithm.BreadthFirstSearch
{
    class BidirectionalTargetAnalyzer
    {
        #region private members

        //list of paths which will be returned
        private HashSet<List<ObjectUUID>> _Paths;

        private Queue<List<ObjectUUID>> _PathsQueue;

        private HashSet<LinkedList<ObjectUUID>> _LeftPaths;
        private HashSet<LinkedList<ObjectUUID>> _RightPaths;
        
        //an actual path
        private LinkedList<ObjectUUID> _TempListLeft;
        private LinkedList<ObjectUUID> _TempListRight;

        //start is the target of the select
        private Node _Start;

        //end is the root or start of the select
        private Node _End;

        private byte _MaxPathLength;

        private bool _ShortestOnly = false;

        private bool _FindAll = false;

        //needed to abort recursion
        private bool _Done = false;

        #endregion

        #region constructors

        public BidirectionalTargetAnalyzer(Node myStart, Node myEnd)
        {
            _TempListLeft = new LinkedList<ObjectUUID>();
            _TempListRight = new LinkedList<ObjectUUID>();

            _LeftPaths = new HashSet<LinkedList<ObjectUUID>>();
            _RightPaths = new HashSet<LinkedList<ObjectUUID>>();

            _Paths = new HashSet<List<ObjectUUID>>();
            _PathsQueue = new Queue<List<ObjectUUID>>();
            
            _Start = myStart;
            _End = myEnd;
        }

        public BidirectionalTargetAnalyzer(Node myStart, Node myEnd, byte myMaxPathLength, bool myShortestOnly, bool myFindAll)
            : this(myStart, myEnd)
        {
            _ShortestOnly = myShortestOnly;
            _FindAll = myFindAll;

            _MaxPathLength = myMaxPathLength;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Evalulates a path-graph starting from the target using the parents to
        /// lookup for the start node.
        /// </summary>
        /// <returns>A HashSet containing all paths.</returns>
        public HashSet<List<ObjectUUID>> getPaths()
        {
            #region do lookup

            getPath(_End);

            #endregion

            #region evaluate the result paths

            //if (_ShortestOnly)
            //{
            //    foreach (var path in _PathsQueue)
            //    {
            //        if (_ShortestPathLength == 0 || path.Count < _ShortestPathLength)
            //        {
            //            _ShortestPathLength = Convert.ToByte(path.Count);
            //        }
            //    }

            //    while (_PathsQueue.Count > 0)
            //    {
            //        var list = _PathsQueue.Dequeue();

            //        if (list.Count <= _MaxPathLength)
            //        {
            //            if (_ShortestOnly && list.Count <= _ShortestPathLength)
            //            {
            //                _Paths.Add(list);
            //            }
            //        }
            //    }
            //}
            //else
            //{
                while (_PathsQueue.Count > 0)
                {
                    var list = _PathsQueue.Dequeue();

                    if (list.Count <= _MaxPathLength)
                    {
                        _Paths.Add(list);
                    }
                }
            //}

            #endregion

            #region cli output

            //no paths found
            if (_Paths.Count == 0)
            {
                Console.WriteLine("No paths found!");

                return null;
            }
            //if only shortest path is searched, return first path
            else if (!_FindAll)
            {
                HashSet<List<ObjectUUID>> temp = new HashSet<List<ObjectUUID>>();
                temp.Add(_Paths.First());
                return temp;
            }
            //all paths are searched
            else 
            {
                if (_Paths.Count == 1)
                {
                    Console.WriteLine(_Paths.Count + " path found.");
                }
                else
                {
                    Console.WriteLine(_Paths.Count + " paths found.");
                }

                return _Paths;
            }

            #endregion
        }

        #endregion public methods

        #region private methods

        private void getPath(Node myCurrent)
        {
            //add myCurrent to actual path
            _TempListLeft.AddLast(myCurrent.Key);
            //set flag to mark that myCurrent is in actual path
            myCurrent.AlreadyInPath = true;

            //abort recursion when myCurrent is the root node
            if (myCurrent.Key.Equals(_Start.Key))
            {
                //duplicate list
                var temp = new List<ObjectUUID>(_TempListLeft);

                if (!QueueContainsList(_PathsQueue, temp))
                {
                    //turn around the path (we're currently at the root)
                    temp.Reverse();

                    //add completed path to result list
                    _PathsQueue.Enqueue(temp);

                    //do some logging
                    PathViewer.LogPath(temp);

                    if (_ShortestOnly && !_FindAll)
                    {
                        //first path is analyzed
                        _Done = true; //we can stop evaluating next parents
                        return;
                    }
                }
            }

            foreach (Node parent in myCurrent.Parents)
            {
                //if parent node not already in actual path
                if (!parent.AlreadyInPath)
                {
                    //and MaxPathLength is not reached
                    if (_TempListLeft.Count < _MaxPathLength)
                    {
                        if (!_Done)
                        {
                            getPath(parent);
                        }
                    }
                }
            }

            //remove last node from actual path
            _TempListLeft.RemoveLast();
            //myCurrent isn't in actual path
            myCurrent.AlreadyInPath = false;

            return;
        }

        private static bool AreListsEqual(object obj1, object obj2)
        {
            bool areEqual = false;
            Type objectType = obj1.GetType();

            if (objectType == obj2.GetType())
            {
                if (objectType.FullName.Contains("Linked"))
                {
                    LinkedList<ObjectUUID> list1 = (LinkedList<ObjectUUID>)obj1;
                    LinkedList<ObjectUUID> list2 = (LinkedList<ObjectUUID>)obj2;

                    if (list1 == null && list2 == null)
                    {
                        areEqual = true;
                    }
                    else if ((list1 != null && list2 != null) && (list1.Count == list2.Count))
                    {
                        //lists are of equal size
                        areEqual = true;

                        //compare each element
                        for (int index = 0; index < list1.Count; index++)
                        {
                            if (!list1.ElementAt(index).Equals(list2.ElementAt(index)))
                            {
                                areEqual = false;
                                break;
                            }
                        }
                    }
                }
                else if (objectType.FullName.Contains("List"))
                {
                    List<ObjectUUID> list1 = (List<ObjectUUID>)obj1;
                    List<ObjectUUID> list2 = (List<ObjectUUID>)obj2;

                    if (list1 == null && list2 == null)
                    {
                        areEqual = true;
                    }
                    else if ((list1 != null && list2 != null) && (list1.Count == list2.Count))
                    {
                        //lists are of equal size
                        areEqual = true;

                        //compare each element
                        for (int index = 0; index < list1.Count; index++)
                        {
                            if (!list1[index].Equals(list2[index]))
                            {
                                areEqual = false;
                                break;
                            }
                        }
                    }
                }
            }

            return areEqual;
        }

        private static bool QueueContainsList(object myQueue, object myList)
        {
            bool contains = false;
            Type listType = myList.GetType();
            object list;

            if (listType.FullName.Contains("Linked"))
            {
                list = (LinkedList<ObjectUUID>)myList;
            }
            else
            {
                list = (List<ObjectUUID>)myList;
            }

            Queue<List<ObjectUUID>> queue = (Queue<List<ObjectUUID>>) myQueue;

            foreach (var queueList in queue)
            {
                if (AreListsEqual(queueList, list))
                {
                    contains = true;
                    break;
                }
            }
            
            return contains;
        }

        #endregion
    }
}

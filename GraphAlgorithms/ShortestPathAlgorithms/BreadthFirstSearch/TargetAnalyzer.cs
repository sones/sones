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

/* <id name="PandoraDB – TargetAnalyzer" />
 * <copyright file="TargetAnalyzer.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Martin Junghanns</developer>
 * <developer>Michael Woidak</developer>
 * <summary>
 * This class evaluates the "path-graph" as the result of an BFS on a graph.
 * It can be compared with a Depth First Search on a graph where all edges guide
 * to the target.
 * 
 * algorithm (recursively):
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
 * complexity:
 * 
 * the complexity is always O(|V| + |E|), V - node count, E - edge count
 * the mightiness of V and E are dramatically reduced based on the BFS.
 * 
 * </summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphAlgorithms.PathAlgorithm.BFSTreeStructure;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

#endregion

namespace GraphAlgorithms.PathAlgorithm.BreadthFirstSearch
{
    class TargetAnalyzer
    {

        #region private members

        //Logger
        //private static Logger //_Logger = LogManager.GetCurrentClassLogger();

        //list of paths which will be returned
        private HashSet<List<ObjectUUID>> _Paths;

        //an actual path
        private List<ObjectUUID> _TempList;

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

        public TargetAnalyzer(Node myStart, Node myEnd, byte myMaxPathLength)
        {
            _Paths = new HashSet<List<ObjectUUID>>();

            _TempList = new List<ObjectUUID>();

            _Start = myStart;

            _End = myEnd;

            _MaxPathLength = myMaxPathLength;
        }

        public TargetAnalyzer(Node myStart, Node myEnd, byte myMaxPathLength, bool myShortestOnly, bool myFindAll)
            : this(myStart, myEnd, myMaxPathLength)
        {
            _ShortestOnly = myShortestOnly;
            _FindAll = myFindAll;
        }

        #endregion

        #region public methods

        /// <summary>
        /// For a detailed documentation on how this evaluation works, have look at the class documentation.
        /// </summary>
        /// <returns>An HashSet which contains all paths between the Start- and the End-Node.</returns>
        public HashSet<List<ObjectUUID>> getPaths()
        {
            //MaxPathLength is not reached
            if (_TempList.Count < _MaxPathLength)
            {
                //analyze paths
                getPath(_End);
            }

            #region cli output

            //no paths found
            if (_Paths.Count == 0)
            {
                //Console.WriteLine("No paths found!");

                return null;
            }
            //if only shortest path is searched, return first path
            else if (!_FindAll)
            {
                HashSet<List<ObjectUUID>> temp = new HashSet<List<ObjectUUID>>();

                var shortestPath = _Paths.First();

                foreach (var path in _Paths)
                {
                    if (path.Count < shortestPath.Count)
                    {
                        shortestPath = path;
                    }
                }

                temp.Add(shortestPath);

                return temp;
            }
            //all paths are searched
            else
            {
                //if (_Paths.Count == 1)
                //{
                //    Console.WriteLine(_Paths.Count + " path found.");
                //}
                //else
                //{
                //    Console.WriteLine(_Paths.Count + " paths found.");
                //}

                return _Paths;
            }

            #endregion
        }

        #endregion public methods

        #region private methods

        private void getPath(Node myCurrent)
        {
            //add myCurrent to actual path
            _TempList.Add(myCurrent.Key);
            //set flag to mark that myCurrent is in actual path
            myCurrent.AlreadyInPath = true;

            //abort recursion when myCurrent is the root node
            if (myCurrent.Key.Equals(_Start.Key))
            {
                //duplicate list
                var temp = new List<ObjectUUID>(_TempList);
                
                //reverse because the path is calculated beginning at the target
                temp.Reverse();

                //add completed path to result list
                _Paths.Add(temp);

                //log the path
                PathViewer.LogPath(temp);

                //if (_ShortestOnly && !_FindAll)
                //{
                //    //first path is analyzed
                //    _Done = true; //we can stop evaluating next parents
                //    return;
                //}
            }

            foreach (Node parent in myCurrent.Parents)
            {
                //if parent node not already in actual path
                if(!parent.AlreadyInPath)
                {
                    //and MaxPathLength is not reached
                    if (_TempList.Count < _MaxPathLength)
                    {
                        if (!_Done)
                        {
                            getPath(parent);
                        }
                    }
                }
            }

            //remove last node from actual path
            _TempList.Remove(_TempList.Last<ObjectUUID>());
            //myCurrent isn't in actual path
            myCurrent.AlreadyInPath = false;

            return;
        }

        #endregion

    }
}

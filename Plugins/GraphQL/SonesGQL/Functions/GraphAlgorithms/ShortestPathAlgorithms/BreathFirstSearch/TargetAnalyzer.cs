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

namespace ShortestPathAlgorithms.BreathFirstSearch
{
    public sealed class TargetAnalyzer
    {
        #region private members

        //list of paths which will be returned
        private HashSet<List<long>> _Paths;

        //an actual path
        private List<long> _TempList;

        //end is the root or start of the select
        private Node _Start;

        //end is the root or start of the select
        private Node _End;

        private byte _MaxPathLength;

        private bool _ShortestOnly = true;

        private bool _FindAll = false;

        #endregion private members

        #region constructors

        public TargetAnalyzer(Node myStart, Node myEnd, byte myMaxPathLength)
        {
            _Paths = new HashSet<List<long>>();

            _TempList = new List<long>();

            _Start = myStart;

            _End = myEnd;

            if (myMaxPathLength != 0)
            {
                _MaxPathLength = Convert.ToByte(myMaxPathLength - 1);
            }
            else
            {
                _MaxPathLength = Convert.ToByte(myMaxPathLength);
            }
        }

        public TargetAnalyzer(Node myStart, Node myEnd, byte myMaxPathLength, bool myShortestOnly, bool myFindAll)
            : this(myStart, myEnd, myMaxPathLength)
        {
            _ShortestOnly = myShortestOnly;
            _FindAll = myFindAll;
        }

        #endregion constructors

        #region public methods

        /// <summary>
        /// For a detailed documentation on how this evaluation works, have look at the class documentation.
        /// </summary>
        /// <returns>An HashSet which contains all paths between the Start- and the End-Node.</returns>
        public HashSet<List<long>> getPaths()
        {
            //MaxPathLength is not reached
            if (_TempList.Count < _MaxPathLength)
            {
                //analyze paths
                getPath(_End);
            }

            #region create result

            //no paths found
            if (_Paths.Count == 0)
            {
                return null;
            }
            //if only shortest path is searched, return first path
            else if (!_FindAll)
            {
                HashSet<List<long>> temp = new HashSet<List<long>>();

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
                return _Paths;
            }

            #endregion
        }

        #endregion public methods

        #region private methods

        private void getPath(Node myCurrent)
        {
            if (!_TempList.Contains(myCurrent.Key))
            {
                //add myCurrent to actual path
                _TempList.Add(myCurrent.Key);

                //set flag to mark that myCurrent is in actual path
                myCurrent.AlreadyInPath = true;

                //abort recursion when myCurrent is the root node
                if (_Start.Key.Equals(myCurrent.Key))
                {
                    //duplicate list
                    var temp = new List<long>(_TempList);

                    //reverse because the path is calculated beginning at the target
                    temp.Reverse();

                    //add completed path to result list
                    _Paths.Add(temp);
                }

                foreach (Node parent in myCurrent.Parents)
                {
                    //if parent node not already in actual path
                    if (!parent.AlreadyInPath)
                    {
                        //and MaxPathLength is not reached
                        if (_TempList.Count < _MaxPathLength)
                        {
                            getPath(parent);
                        }
                    }
                }

                if (_TempList.Count != 0)
                {
                    //remove last node from actual path
                    _TempList.Remove(_TempList.Last<long>());
                    //myCurrent isn't in actual path
                    myCurrent.AlreadyInPath = false;
                }
            }
            return;
        }

        #endregion private methods
    }
}

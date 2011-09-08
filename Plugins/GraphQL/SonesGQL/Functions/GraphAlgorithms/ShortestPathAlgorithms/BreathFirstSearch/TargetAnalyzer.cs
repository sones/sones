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

namespace sones.Plugins.SonesGQL.Functions.ShortestPathAlgorithms.BreathFirstSearch
{
    public sealed class TargetAnalyzer
    {
        #region private members

        //list of paths which will be returned
        private HashSet<List<Tuple<long, long>>> _Paths;

        private HashSet<Tuple<long, long>> _Uninteresting;

        private HashSet<List<Tuple<long, long>>> _PathsLeft;
        private HashSet<List<Tuple<long, long>>> _PathsRight;

        //an actual path
        private List<Tuple<long, long>> _TempList;

        private List<Tuple<long, long>> _TempListLeft;
        private List<Tuple<long, long>> _TempListRight;

        //end is the root or start of the select
        private Node _Start;

        //end is the root or start of the select
        private Node _End;

        private int _MaxPathLength;
        private int _MaxPartLengthLeft;
        private int _MaxPartLengthRight;

        private bool _ShortestOnly = true;
        private bool _FindAll = false;

        private bool foundLeft;
        private bool foundRight;

        #endregion private members

        #region constructors

        public TargetAnalyzer(Node myStart, Node myEnd, UInt64 myMaxPathLength)
        {
            _Paths = new HashSet<List<Tuple<long, long>>>();

            _Uninteresting = new HashSet<Tuple<long, long>>();

            _PathsLeft = new HashSet<List<Tuple<long, long>>>();
            _PathsRight = new HashSet<List<Tuple<long, long>>>();

            _TempList = new List<Tuple<long, long>>();

            _TempListLeft = new List<Tuple<long, long>>();
            _TempListRight = new List<Tuple<long, long>>();

            _Start = myStart;

            _End = myEnd;

            _MaxPathLength = Convert.ToInt16(myMaxPathLength);
            _MaxPartLengthLeft = 0;
            _MaxPartLengthRight = 0;
        }

        public TargetAnalyzer(Node myStart, Node myEnd, UInt64 myMaxPathLength, bool myShortestOnly, bool myFindAll)
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
        public HashSet<List<Tuple<long, long>>> GetPaths()
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
            else if (_ShortestOnly && !_FindAll)
            {
                HashSet<List<Tuple<long, long>>> temp = new HashSet<List<Tuple<long, long>>>();

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

        public HashSet<List<Tuple<long, long>>> GetShortestPath(HashSet<Node> myIntersectNodes)
        {
            foundLeft = foundRight = false;

            //set maximum part lengths
            _MaxPartLengthLeft = _MaxPartLengthRight = (_MaxPathLength / 2);

            //if the max path length is odd add one
            if (_MaxPathLength % 2 != 0)
            {
                _MaxPartLengthLeft += 1;
                _MaxPartLengthRight += 1;
            }

            var enumerator = myIntersectNodes.GetEnumerator();

            while (enumerator.MoveNext() && _Paths.Count == 0)
            {
                //calculate parts from intersect node to end
                getShortestPathDownwards(enumerator.Current);

                ///recalculate the max part length...
                ///it could happen that the intersect node is not in the middle,
                ///so the max part length left could be less than calculated, 
                ///if this happens the right part length must be adapted
                _MaxPartLengthRight = _MaxPathLength - _MaxPartLengthLeft;

                //check if max path length is odd
                if (_MaxPathLength % 2 != 0)
                {
                    _MaxPartLengthRight += 1;
                }

                //calculate parts from intersect node to start
                getShortestPathUpwards(enumerator.Current);

                //calculate full path
                if (_PathsLeft.Count > 0 && _PathsRight.Count > 0)
                {
                    //forach part from left
                    foreach (var leftPath in _PathsLeft)
                    {
                        var first = leftPath.First();

                        //foreach part from right
                        foreach (var rightPath in _PathsRight)
                            //if there starts are the same (the intersect node)
                            if (rightPath.First().Equals(first) && 
                                ((rightPath.Count + leftPath.Count - 1) <= _MaxPathLength))
                            {
                                var temp = new List<Tuple<long, long>>(rightPath);
                                temp.Reverse();
                                leftPath.RemoveAt(0);
                                temp.InsertRange(temp.Count, leftPath);

                                _Paths.Add(temp);

                                return _Paths;
                            }
                    }
                }
            }

            return null;
        }

        #endregion public methods

        #region private methods

        private bool getPath(Node myCurrent)
        {
            bool currentIsInteresting = false;

            if (!_TempList.Contains(myCurrent.Key))
            {
                if (!_Uninteresting.Contains<Tuple<long, long>>(myCurrent.Key))
                {
                    //add myCurrent to actual path
                    _TempList.Add(myCurrent.Key);

                    //set flag to mark that myCurrent is in actual path
                    myCurrent.AlreadyInPath = true;

                    //abort recursion when myCurrent is the root node
                    if (_Start.Key.Equals(myCurrent.Key))
                    {
                        currentIsInteresting = true;

                        //duplicate list
                        var temp = new List<Tuple<long, long>>(_TempList);

                        //reverse because the path is calculated beginning at the target
                        temp.Reverse();

                        //add completed path to result list
                        _Paths.Add(temp);
                    }
                    //MaxPathLength is not reached
                    else if (_TempList.Count < _MaxPathLength)
                    {
                        //for all parent nodes which are not already in actual path
                        foreach (Node parent in myCurrent.Parents)
                        {
                            //if currentIsInteresting already true doen't set to false
                            if (currentIsInteresting)
                                getPath(parent);
                            else
                                currentIsInteresting = getPath(parent);
                        }
                    }
                    else
                        currentIsInteresting = true;

                    if (_TempList.Count != 0)
                    {
                        //remove last node from actual path
                        _TempList.Remove(_TempList.Last<Tuple<long, long>>());

                        //myCurrent isn't in actual path
                        myCurrent.AlreadyInPath = false;
                    }

                    if (!currentIsInteresting)
                        _Uninteresting.Add(myCurrent.Key);
                }
            }
            else
                currentIsInteresting = true;

            return currentIsInteresting;
        }

        private void getShortestPathDownwards(Node myCurrent)
        {
            if (!_TempListLeft.Contains(myCurrent.Key) && !foundLeft)
            {
                //add myCurrent to actual path
                _TempListLeft.Add(myCurrent.Key);

                //set flag to mark that myCurrent is in actual path
                myCurrent.AlreadyInPath = true;

                //abort recursion when myCurrent is the root node
                if (_End.Key.Equals(myCurrent.Key) && 
                    _TempListLeft.Count <= _MaxPartLengthLeft)
                {
                    var temp = new List<Tuple<long, long>>(_TempListLeft);

                    //actualize the max part length
                    _MaxPartLengthLeft = temp.Count;

                    _PathsLeft.Add(temp);

                    if(_ShortestOnly)
                        foundLeft = true;
                }
                //MaxPathLength is not reached
                else if (_TempListLeft.Count <= _MaxPartLengthLeft)
                    //for all parent nodes which are not already in actual path
                    foreach (Node child in myCurrent.Children.Where(_ => !_.AlreadyInPath))
                    {
                        getShortestPathDownwards(child);
                    }

                if (_TempListLeft.Count != 0)
                {
                    //remove last node from actual path
                    _TempListLeft.Remove(_TempListLeft.Last<Tuple<long, long>>());

                    //myCurrent isn't in actual path
                    myCurrent.AlreadyInPath = false;
                }
            }
            return;
        }

        private void getShortestPathUpwards(Node myCurrent)
        {
            if (!_TempListRight.Contains(myCurrent.Key) && !foundRight)
            {
                //add myCurrent to actual path
                _TempListRight.Add(myCurrent.Key);

                //set flag to mark that myCurrent is in actual path
                myCurrent.AlreadyInPath = true;

                //abort recursion when myCurrent is the root node
                if (_Start.Key.Equals(myCurrent.Key) &&
                    _TempListRight.Count <= _MaxPartLengthRight)
                {
                    //duplicate list
                    var temp = new List<Tuple<long, long>>(_TempListRight);

                    //actualize the max part length
                    _MaxPartLengthRight = temp.Count;

                    _PathsRight.Add(temp);

                    if (_ShortestOnly)
                        foundRight = true;
                }
                //MaxPathLength is not reached
                else if (_TempListRight.Count <= _MaxPartLengthRight)
                    //for all parent nodes which are not already in actual path
                    foreach (Node parent in myCurrent.Parents.Where(_ => !_.AlreadyInPath))
                    {
                        getShortestPathUpwards(parent);
                    }

                if (_TempListRight.Count != 0)
                {
                    //remove last node from actual path
                    _TempListRight.Remove(_TempListRight.Last<Tuple<long, long>>());

                    //myCurrent isn't in actual path
                    myCurrent.AlreadyInPath = false;
                }
            }
            return;
        }

        #endregion private methods
    }
}

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

/* <id name="PandoraDB – Level key" />
 * <copyright file="LevelKey.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using sones.GraphDB.ObjectManagement;
using System.Collections.Generic;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Exceptions;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.Lib;

#endregion

namespace sones.GraphDB.QueryLanguage.ExpressionGraph
{
    /// <summary>
    /// List&lt;EdgeKey&gt; wrapper to implementent CompareTO and GetHashcode
    /// </summary>
    public class LevelKey
    {

        #region Edges

        private List<EdgeKey> _edges;

        public List<EdgeKey> Edges
        {
            get { return _edges; }
        }

        #endregion

        private int _hashcode = int.MaxValue;

        #region Level

        private int _level;

        public int Level
        {
            get { return _level; }
        }

        #endregion

        #region Depth

        public Int32 Depth
        {
            get
            {
                if (_edges.IsNullOrEmpty())
                    return 0;
                else if (_edges.Count == 1 && _edges[0].AttrUUID == null)
                    return 0;
                else
                    return _edges.Count;
            }
        }

        #endregion

        public LevelKey()
            :this(new List<EdgeKey>() {}, 0)
        {
            //_level = 0;
            //_edges = new List<EdgeKey>() {};
        }

        public LevelKey(EdgeKey myEdgeKey)
            :this(new List<EdgeKey>() { myEdgeKey }, (myEdgeKey.AttrUUID == null) ? 0 : 1)
        {
            
        }

        /// <summary>
        /// Use with care!!! The correct level must be passed!!
        /// Create a new LevelKey from some edges.
        /// 
        /// </summary>
        /// <param name="myEdgeKey"></param>
        /// <param name="myLevel"></param>
        public LevelKey(IEnumerable<EdgeKey> myEdgeKey, int myLevel)
        {
            _edges = new List<EdgeKey>();
            _level = myLevel;

            _hashcode = CalcHashCode(myEdgeKey);

            _edges.AddRange(myEdgeKey);

        }

        private int CalcHashCode(IEnumerable<EdgeKey> myEdgeKey)
        {
            int myHashCode = 0;

            foreach (var aEdge in myEdgeKey)
            {
                myHashCode += (int)(aEdge.GetHashCode() >> 32);
            }

            return myHashCode;
        }

        public LevelKey(TypeUUID myTypeUUID)
            :this(new List<EdgeKey>() {new EdgeKey(myTypeUUID, null)}, 0)
        {
            
        }

        public LevelKey(GraphDBType myDBTypeStream)
            : this(myDBTypeStream.UUID)
        { }

        public override int GetHashCode()
        {
            return _hashcode;
        }

        #region override

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if(obj is LevelKey)
            {
                LevelKey p = (LevelKey)obj;
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(LevelKey p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            if (this._level != p.Level)
            {
                return false;
            }

            for (int i = 0; i < _edges.Count; i++)
            {
                if (this._edges[i] != p.Edges[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static Boolean operator ==(LevelKey a, LevelKey b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(LevelKey a, LevelKey b)
        {
            return !(a == b);
        }

        #endregion

        public override string ToString()
        {
            return String.Format("Level: {0} EdgeKey: {1}", _level, (_edges.IsNullOrEmpty()) ? 0 : _edges.Count);
        }

        #endregion

        #region Operators

        public static LevelKey operator +(LevelKey myLevelKey, EdgeKey myEdgeKey)
        {
            // an empty level
            if (myLevelKey.Edges == null)
                return new LevelKey(myEdgeKey);

            var edgeList = new List<EdgeKey>(myLevelKey.Edges);

            // if the first and only edge has a null attrUUID the new edge must have the same type!!
            if (edgeList.Count == 1 && edgeList[0].AttrUUID == null)
            {
                if (edgeList[0].TypeUUID != myEdgeKey.TypeUUID)
                {
                    throw new GraphDBException(new Error_InvalidLevelKeyOperation(myLevelKey, myEdgeKey, "+"));
                }
                else
                {
                    if (myEdgeKey.AttrUUID == null)
                    {
                        //so it must be lvl 0
                        return new LevelKey(new List<EdgeKey>() { myEdgeKey }, 0);
                    }
                    else
                    {
                        //so it must be lvl 1
                        return new LevelKey(new List<EdgeKey>() { myEdgeKey }, 1);
                    }
                }
            }
            else
            {
                if (myEdgeKey.AttrUUID != null)
                {
                    edgeList.Add(myEdgeKey);
                    return new LevelKey(edgeList, myLevelKey.Level + 1);
                }
                else
                {
                    return new LevelKey(edgeList, myLevelKey.Level);
                }
            }
        }

        public static LevelKey operator +(EdgeKey myKey, LevelKey myLevelKey)
        {
            return myLevelKey + myKey;
        }

        public static LevelKey operator +(LevelKey myLevelKey1, LevelKey myLevelKey2)
        {
            
            if ((myLevelKey1.Edges.IsNullOrEmpty()) && myLevelKey2.Edges.IsNullOrEmpty())
                return new LevelKey();
            else if (myLevelKey1.Edges.IsNullOrEmpty())
                return new LevelKey(myLevelKey2.Edges, myLevelKey2.Level);
            else if (myLevelKey2.Edges.IsNullOrEmpty())
                return new LevelKey(myLevelKey1.Edges, myLevelKey1.Level);

            if (myLevelKey1.Level == 0 && myLevelKey2.Level == 0)
            {
                #region both are level 0 (User/null)
                if (myLevelKey1.Edges[0].TypeUUID != myLevelKey2.Edges[0].TypeUUID) // if the types are different then something is really wrong
                    throw new GraphDBException(new Error_InvalidLevelKeyOperation(myLevelKey1, myLevelKey2, "+"));
                else
                    return new LevelKey(myLevelKey1.Edges, myLevelKey1.Level);
                #endregion
            }
            else if (myLevelKey1.Level == 0)
            {
                #region one of them is level 0 - so we can just skip this level: User/null + User/Friends == User/Friends
                if (myLevelKey1.Edges[0].TypeUUID != myLevelKey2.Edges[0].TypeUUID) // if the types are different then something is really wrong
                    throw new GraphDBException(new Error_InvalidLevelKeyOperation(myLevelKey1, myLevelKey2, "+"));
                else
                    return new LevelKey(myLevelKey2.Edges, myLevelKey2.Level); // just return the other level
                #endregion
            }
            else if (myLevelKey2.Level == 0)
            {
                #region one of them is level 0 - so we can just skip this level: User/null + User/Friends == User/Friends
                if (myLevelKey1.Edges[0].TypeUUID != myLevelKey2.Edges[0].TypeUUID) // if the types are different then something is really wrong
                    throw new GraphDBException(new Error_InvalidLevelKeyOperation(myLevelKey1, myLevelKey2, "+"));
                else
                    return new LevelKey(myLevelKey1.Edges, myLevelKey1.Level) ; // just return the other level
                #endregion
            }

            var edges = new List<EdgeKey>(myLevelKey1.Edges);
            edges.AddRange(myLevelKey2.Edges);

            return new LevelKey(edges, myLevelKey1.Level + myLevelKey2.Level);
        }

        public static LevelKey operator -(LevelKey myLevelKey, EdgeKey myEdgeKey)
        {
            var edgeList = new List<EdgeKey>(myLevelKey.Edges);
            //edgeList.Remove(myEdgeKey);

            if (edgeList[edgeList.Count - 1] != myEdgeKey)
                throw new GraphDBException(new Error_InvalidLevelKeyOperation(myLevelKey, myEdgeKey, "-"));

            return new LevelKey(edgeList.Take(edgeList.Count - 1), myLevelKey.Level - 1);
        }

        public static LevelKey operator -(LevelKey myKey, LevelKey myOtherLevelKey)
        {
            if (myKey.Level < myOtherLevelKey.Level)
                throw new ArgumentException("level of left (" + myKey.Level + ") operand is lower than right (" + myOtherLevelKey.Level + ") operand:", "myOtherLevelKey");

            if (!myKey.StartsWith(myOtherLevelKey, true))
                throw new ArgumentException("left operand level does not starts with right operand level");

            if (myOtherLevelKey.Level == 0)
                return myKey;

            if (myKey.Level == myOtherLevelKey.Level)
                return new LevelKey(myKey.Edges[0].TypeUUID);

            var edgeList = new List<EdgeKey>(myKey.Edges);

            return new LevelKey(myKey.Edges.Skip(myOtherLevelKey.Edges.Count), myKey.Level - myOtherLevelKey.Edges.Count);
        }

        #endregion

        public EdgeKey LastEdge
        {
            get
            {
                return _edges.Last();
            }
        }

        public LevelKey GetPredecessorLevel()
        {
            switch (_level)
            {
                case 0:

                    //maybe we need the LevelKey to be a class
                    return new LevelKey();

                case 1:

                    return new LevelKey(new List<EdgeKey>() { new EdgeKey(_edges[0].TypeUUID, null)}, 0);

                default:

                    return new LevelKey(_edges.Take(_edges.Count - 1), _level - 1);
            }
        }


        public bool StartsWith(LevelKey myLevel)
        {
            if (myLevel.Level > _level)
                throw new ArgumentException(myLevel + " is greater than " + _level);

            for (Int32 i = 0; i < myLevel.Level;i++ )
            {
                if (myLevel.Edges[i].TypeUUID != _edges[i].TypeUUID)
                    return false;
            }
            return true;
        }

        public bool StartsWith(LevelKey myLevel, Boolean IncludingAttrs)
        {
            if (!IncludingAttrs)
                return StartsWith(myLevel);

            if (myLevel.Level > _level)
                throw new ArgumentException(myLevel + " is greater than " + _level);

            for (Int32 i = 0; i < myLevel.Level; i++)
            {
                if (myLevel.Edges[i].TypeUUID != _edges[i].TypeUUID || myLevel.Edges[i].AttrUUID != _edges[i].AttrUUID)
                    return false;
            }
            return true;
        }
    }
}

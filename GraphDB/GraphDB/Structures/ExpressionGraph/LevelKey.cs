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

/* <id name="GraphDB – Level key" />
 * <copyright file="LevelKey.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.Lib;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Structures.ExpressionGraph
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

        private int _hashcode = 0;

        #region Level

        private int _level = 0;

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
        { }

        public LevelKey(DBTypeManager myTypeManager)
            : this(new List<EdgeKey>() { }, myTypeManager)
        {
            //_level = 0;
            //_edges = new List<EdgeKey>() {};
        }

        public LevelKey(EdgeKey myEdgeKey, DBTypeManager myTypeManager)
            : this(new List<EdgeKey>() { myEdgeKey }, myTypeManager)
        {
            
        }

        public LevelKey(IEnumerable<EdgeKey> myEdgeKey, DBTypeManager myTypeManager)
        {
            _edges = new List<EdgeKey>();

            foreach (var aEdgeKey in myEdgeKey)
            {
                if (aEdgeKey.AttrUUID != null && aEdgeKey.AttrUUID != UndefinedTypeAttribute.AttributeUUID)
                {
                    var attribute = aEdgeKey.GetTypeAndAttributeInformation(myTypeManager).Item2;

                    if (attribute.GetDBType(myTypeManager).IsUserDefined || attribute.IsBackwardEdge)
                    {
                        _edges.Add(aEdgeKey);
                        _level++;

                        AddHashCodeFromSingleEdge(ref _hashcode, aEdgeKey);
                    }
                    else
                    {
                        if (_level == 0)
                        {
                            var newEdgeKey = new EdgeKey(aEdgeKey.TypeUUID, null);
                            _edges.Add(newEdgeKey);

                            AddHashCodeFromSingleEdge(ref _hashcode, newEdgeKey);

                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (_level == 0)
                    {
                        _edges.Add(aEdgeKey);

                        AddHashCodeFromSingleEdge(ref _hashcode, aEdgeKey);

                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public LevelKey(TypeUUID myTypeUUID, DBTypeManager myTypeManager)
            : this(new List<EdgeKey>() { new EdgeKey(myTypeUUID, null) }, myTypeManager)
        {
            
        }

        public LevelKey(GraphDBType myDBTypeStream, DBTypeManager myTypeManager)
            : this(myDBTypeStream.UUID, myTypeManager)
        { }

        #region override

        public override int GetHashCode()
        {
            return _hashcode;
        }

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

        public LevelKey AddEdgeKey(EdgeKey myEdgeKey, DBTypeManager myTypeManager)
        {
            // an empty level
            if (this.Edges == null)
            {
                return new LevelKey(myEdgeKey, myTypeManager);
            }

            var edgeList = new List<EdgeKey>(this.Edges);

            // if the first and only edge has a null attrUUID the new edge must have the same type!!
            if (edgeList.Count == 1 && edgeList[0].AttrUUID == null)
            {
                if (edgeList[0].TypeUUID != myEdgeKey.TypeUUID)
                {
                    throw new GraphDBException(new Error_InvalidLevelKeyOperation(this, myEdgeKey, "+"));
                }
                else
                {
                    if (myEdgeKey.AttrUUID == null)
                    {
                        //so it must be lvl 0
                        return new LevelKey(new List<EdgeKey>() { myEdgeKey }, myTypeManager);
                    }
                    else
                    {
                        //so it must be lvl 1
                        return new LevelKey(new List<EdgeKey>() { myEdgeKey }, myTypeManager);
                    }
                }
            }
            else
            {
                if (myEdgeKey.AttrUUID != null)
                {
                    edgeList.Add(myEdgeKey);
                    return new LevelKey(edgeList, myTypeManager);
                }
                else
                {
                    return new LevelKey(edgeList, myTypeManager);
                }
            }
        }

        public LevelKey AddLevelKey(LevelKey myLevelKey2, DBTypeManager myTypeManager)
        {
            if ((this.Edges.IsNullOrEmpty()) && myLevelKey2.Edges.IsNullOrEmpty())
                return new LevelKey(myTypeManager);
            else if (this.Edges.IsNullOrEmpty())
                return new LevelKey(myLevelKey2.Edges, myTypeManager);
            else if (myLevelKey2.Edges.IsNullOrEmpty())
                return new LevelKey(this.Edges, myTypeManager);

            if (this.Level == 0 && myLevelKey2.Level == 0)
            {
                #region both are level 0 (User/null)
                if (this.Edges[0].TypeUUID != myLevelKey2.Edges[0].TypeUUID) // if the types are different then something is really wrong
                    throw new GraphDBException(new Error_InvalidLevelKeyOperation(this, myLevelKey2, "+"));
                else
                    return new LevelKey(this.Edges, myTypeManager);
                #endregion
            }
            else if (this.Level == 0)
            {
                #region one of them is level 0 - so we can just skip this level: User/null + User/Friends == User/Friends
                if (this.Edges[0].TypeUUID != myLevelKey2.Edges[0].TypeUUID) // if the types are different then something is really wrong
                    throw new GraphDBException(new Error_InvalidLevelKeyOperation(this, myLevelKey2, "+"));
                else
                    return new LevelKey(myLevelKey2.Edges, myTypeManager); // just return the other level
                #endregion
            }
            else if (myLevelKey2.Level == 0)
            {
                #region one of them is level 0 - so we can just skip this level: User/null + User/Friends == User/Friends
                if (this.Edges[0].TypeUUID != myLevelKey2.Edges[0].TypeUUID) // if the types are different then something is really wrong
                    throw new GraphDBException(new Error_InvalidLevelKeyOperation(this, myLevelKey2, "+"));
                else
                    return new LevelKey(this.Edges, myTypeManager); // just return the other level
                #endregion
            }

            var edges = new List<EdgeKey>(this.Edges);
            edges.AddRange(myLevelKey2.Edges);

            return new LevelKey(edges, myTypeManager);
        }

        public LevelKey RemoveEdgeKey(EdgeKey myEdgeKey, DBTypeManager myTypeManager)
        {
            var edgeList = new List<EdgeKey>(this.Edges);
            //edgeList.Remove(myEdgeKey);

            if (edgeList[edgeList.Count - 1] != myEdgeKey)
                throw new GraphDBException(new Error_InvalidLevelKeyOperation(this, myEdgeKey, "-"));

            return new LevelKey(edgeList.Take(edgeList.Count - 1), myTypeManager);
        }

        public LevelKey RemoveLevelKey(LevelKey myOtherLevelKey, DBTypeManager myTypeManager)
        {
            if (this.Level < myOtherLevelKey.Level)
                throw new ArgumentException("level of left (" + this.Level + ") operand is lower than right (" + myOtherLevelKey.Level + ") operand:", "myOtherLevelKey");

            if (!this.StartsWith(myOtherLevelKey, true))
                throw new ArgumentException("left operand level does not starts with right operand level");

            if (myOtherLevelKey.Level == 0)
                return this;

            if (this.Level == myOtherLevelKey.Level)
                return new LevelKey(this.Edges[0].TypeUUID, myTypeManager);

            var edgeList = new List<EdgeKey>(this.Edges);

            return new LevelKey(this.Edges.Skip(myOtherLevelKey.Edges.Count), myTypeManager);
        }

        #endregion

        public EdgeKey LastEdge
        {
            get
            {
                return _edges.Last();
            }
        }

        public LevelKey GetPredecessorLevel(DBTypeManager myTypeManager)
        {
            switch (_level)
            {
                case 0:

                    return this;

                case 1:

                    return new LevelKey(new List<EdgeKey>() { new EdgeKey(_edges[0].TypeUUID, null) }, myTypeManager);

                default:

                    return new LevelKey(_edges.Take(_edges.Count - 1), myTypeManager);
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

        #region private helper

        private int CalcHashCode(IEnumerable<EdgeKey> myEdgeKey)
        {
            int myHashCode = 0;

            foreach (var aEdge in myEdgeKey)
            {
                AddHashCodeFromSingleEdge(ref myHashCode, aEdge);
            }

            return myHashCode;
        }

        private void AddHashCodeFromSingleEdge(ref int myHashCode, EdgeKey aEdge)
        {
            myHashCode += (int)(aEdge.GetHashCode() >> 32);
        }

        #endregion
    }
}

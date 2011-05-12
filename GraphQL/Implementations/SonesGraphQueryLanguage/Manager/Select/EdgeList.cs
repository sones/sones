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
using System.Text;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;
using sones.GraphQL.GQL.ErrorHandling;
using sones.Library.LanguageExtensions;

namespace sones.GraphQL.GQL.Manager.Select
{
    /// <summary>
    /// This just hold a list of edges and provides some operations with them. This does not replace the LevelKey.
    /// </summary>
    public sealed class EdgeList
    {

        #region Fields

        Int32 _hashcode = 0;

        #endregion

        #region Properties

        public List<EdgeKey> Edges { get; private set; }

        public Int32 Level
        {
            get
            {
                if (Edges.Count == 1)
                {
                    if (!Edges[0].IsAttributeSet)
                    {
                        return 0;
                    }
                }
                return Edges.Count;
            }
        }

        public EdgeKey LastEdge
        {
            get
            {
                return Edges.Last();
            }
        }

        #endregion

        #region Ctors

        public EdgeList()
        {
            Edges = new List<EdgeKey>();
        }

        public EdgeList(List<EdgeKey> list)
        {
            Edges = list;
            _hashcode = CalcHashCode(Edges);
        }

        public EdgeList(EdgeKey myEdgeKey)
            : this(new List<EdgeKey>() { myEdgeKey })
        {
        }

        public EdgeList(IEnumerable<EdgeKey> iEnumerable)
            : this(new List<EdgeKey>(iEnumerable))
        { }

        public EdgeList(Int64 typeUUID)
            : this(new EdgeKey(typeUUID))
        { }

        #endregion

        #region GetPredecessorLevel

        public EdgeList GetPredecessorLevel()
        {
            switch (Edges.Count)
            {
                case 0:

                    return new EdgeList();

                case 1:

                    return new EdgeList(new List<EdgeKey>() { new EdgeKey(Edges[0].VertexTypeID) });

                default:

                    return new EdgeList(Edges.Take(Edges.Count - 1));
            }
        }

        #endregion

        #region Operators

        public static EdgeList operator +(EdgeList myEdgeList, EdgeKey myEdgeKey)
        {
            // an empty level
            if (myEdgeList.Edges == null)
                return new EdgeList(myEdgeKey);

            var edgeList = new List<EdgeKey>(myEdgeList.Edges);

            // if the first and only edge has a null attrUUID the new edge must have the same type!!
            if (edgeList.Count == 1 && !edgeList[0].IsAttributeSet)
            {
                if (edgeList[0].VertexTypeID != myEdgeKey.VertexTypeID)
                {
                    throw new InvalidEdgeListOperationException(myEdgeList, myEdgeKey, "+");
                }
                else
                {
                    if (!myEdgeKey.IsAttributeSet)
                    {
                        //so it must be lvl 0
                        return new EdgeList(new List<EdgeKey>() { myEdgeKey });
                    }
                    else
                    {
                        //so it must be lvl 1
                        return new EdgeList(new List<EdgeKey>() { myEdgeKey });
                    }
                }
            }
            else
            {
                if (myEdgeKey.IsAttributeSet)
                {
                    edgeList.Add(myEdgeKey);
                    return new EdgeList(edgeList);
                }
                else
                {
                    return new EdgeList(edgeList);
                }
            }
        }

        public static EdgeList operator +(EdgeKey myKey, EdgeList myEdgeList)
        {
            return myEdgeList + myKey;
        }

        public static EdgeList operator +(EdgeList myEdgeList1, EdgeList myEdgeList2)
        {

            if ((myEdgeList1.Edges.IsNullOrEmpty()) && myEdgeList2.Edges.IsNullOrEmpty())
                return new EdgeList();
            else if (myEdgeList1.Edges.IsNullOrEmpty())
                return new EdgeList(myEdgeList2.Edges);
            else if (myEdgeList2.Edges.IsNullOrEmpty())
                return new EdgeList(myEdgeList1.Edges);

            if (myEdgeList1.Level == 0 && myEdgeList2.Level == 0)
            {
                #region both are level 0 (User/null)
                if (myEdgeList1.Edges[0].VertexTypeID != myEdgeList2.Edges[0].VertexTypeID) // if the types are different then something is really wrong
                    throw new InvalidEdgeListOperationException(myEdgeList1, myEdgeList2, "+");
                else
                    return new EdgeList(myEdgeList1.Edges);
                #endregion
            }
            else if (myEdgeList1.Level == 0)
            {
                #region one of them is level 0 - so we can just skip this level: User/null + User/Friends == User/Friends
                if (myEdgeList1.Edges[0].VertexTypeID != myEdgeList2.Edges[0].VertexTypeID) // if the types are different then something is really wrong
                    throw new InvalidEdgeListOperationException(myEdgeList1, myEdgeList2, "+");
                else
                    return new EdgeList(myEdgeList2.Edges); // just return the other level
                #endregion
            }
            else if (myEdgeList2.Level == 0)
            {
                #region one of them is level 0 - so we can just skip this level: User/null + User/Friends == User/Friends
                if (myEdgeList1.Edges[0].VertexTypeID != myEdgeList2.Edges[0].VertexTypeID) // if the types are different then something is really wrong
                    throw new InvalidEdgeListOperationException(myEdgeList1, myEdgeList2, "+");
                else
                    return new EdgeList(myEdgeList1.Edges); // just return the other level
                #endregion
            }

            var edges = new List<EdgeKey>(myEdgeList1.Edges);
            edges.AddRange(myEdgeList2.Edges);

            return new EdgeList(edges);
        }

        public static EdgeList operator -(EdgeList myEdgeList, EdgeKey myEdgeKey)
        {
            var edgeList = new List<EdgeKey>(myEdgeList.Edges);
            //edgeList.Remove(myEdgeKey);

            if (edgeList[edgeList.Count - 1] != myEdgeKey)
                throw new InvalidEdgeListOperationException(myEdgeList, myEdgeKey, "-");

            return new EdgeList(edgeList.Take(edgeList.Count - 1));
        }

        public static EdgeList operator -(EdgeList myKey, EdgeList myOtherEdgeList)
        {
            if (myKey.Level < myOtherEdgeList.Level)
                throw new ArgumentException("level of left (" + myKey.Level + ") operand is lower than right (" + myOtherEdgeList.Level + ") operand:", "myOtherEdgeList");

            if (!myKey.StartsWith(myOtherEdgeList, true))
                throw new ArgumentException("left operand level does not starts with right operand level");

            if (myOtherEdgeList.Level == 0)
                return myKey;

            if (myKey.Level == myOtherEdgeList.Level)
                return new EdgeList(myKey.Edges[0].VertexTypeID);

            var edgeList = new List<EdgeKey>(myKey.Edges);

            return new EdgeList(myKey.Edges.Skip(myOtherEdgeList.Edges.Count));
        }

        #endregion

        #region StartsWith

        public bool StartsWith(EdgeList myLevel)
        {
            if (myLevel.Level > Level)
                throw new ArgumentException(myLevel + " is greater than " + Level);

            for (Int32 i = 0; i < myLevel.Level; i++)
            {
                if (myLevel.Edges[i].VertexTypeID != Edges[i].VertexTypeID)
                    return false;
            }
            return true;
        }

        public bool StartsWith(EdgeList myLevel, Boolean IncludingAttrs)
        {
            if (!IncludingAttrs)
                return StartsWith(myLevel);

            if (myLevel.Level > Level)
                throw new ArgumentException(myLevel + " is greater than " + Level);

            for (Int32 i = 0; i < myLevel.Level; i++)
            {
                if (myLevel.Edges[i].VertexTypeID != Edges[i].VertexTypeID || myLevel.Edges[i].AttributeID != Edges[i].AttributeID)
                    return false;
            }
            return true;
        }

        #endregion

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
            if (obj is EdgeList)
            {
                EdgeList p = (EdgeList)obj;
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(EdgeList p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            if (this.Level != p.Level)
            {
                return false;
            }

            for (int i = 0; i < Edges.Count; i++)
            {
                if (this.Edges[i] != p.Edges[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static Boolean operator ==(EdgeList a, EdgeList b)
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

        public static Boolean operator !=(EdgeList a, EdgeList b)
        {
            return !(a == b);
        }

        #endregion

        public override string ToString()
        {
            return String.Format("Level: {0} EdgeKey: {1}", Level, (Edges.IsNullOrEmpty()) ? 0 : Edges.Count);
        }

        #endregion

        private int CalcHashCode(IEnumerable<EdgeKey> myEdgeKey)
        {
            int myHashCode = 0;

            foreach (var aEdge in myEdgeKey)
            {
                myHashCode += (int)(aEdge.GetHashCode() >> 32);
            }

            return myHashCode;
        }

    }
}

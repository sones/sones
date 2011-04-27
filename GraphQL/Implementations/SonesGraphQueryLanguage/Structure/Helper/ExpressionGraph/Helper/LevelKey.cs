using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphQL.GQL.ErrorHandling;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    /// <summary>
    /// List&lt;EdgeKey&gt; wrapper to implementent CompareTO and GetHashcode
    /// </summary>
    public sealed class LevelKey
    {
        #region Edges

        public readonly List<EdgeKey> Edges;

        #endregion

        private int _hashcode = 0;

        #region Level

        public readonly int Level;

        #endregion

        #region Depth

        public Int32 Depth
        {
            get
            {
                if (Edges == null || Edges.Count == 0)
                    return 0;
                else if (Edges.Count == 1 && Edges[0].IsAttributeSet == false)
                    return 0;
                else
                    return Edges.Count;
            }
        }

        #endregion

        public LevelKey()
        {

        }

        public LevelKey(Int64 myVertexTypeID, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
            : this(new List<EdgeKey> { new EdgeKey(myVertexTypeID) }, myGraphDB, mySecurityToken, myTransactionToken)
        {

        }

        public LevelKey(IEnumerable<EdgeKey> myEdgeKey, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            Edges = new List<EdgeKey>();

            foreach (var aEdgeKey in myEdgeKey)
            {
                if (aEdgeKey.IsAttributeSet)
                {
                    var vertexType = myGraphDB.GetVertexType<IVertexType>
                        (mySecurityToken, 
                        myTransactionToken, 
                        new RequestGetVertexType(aEdgeKey.VertexTypeID), 
                        (stats, type) => type);

                    var attribute = vertexType.GetAttributeDefinition(aEdgeKey.AttributeID);

                    if (attribute.Kind != AttributeType.Property)
                    {
                        //so there is an edge
                        Edges.Add(aEdgeKey);
                        Level++;

                        AddHashCodeFromSingleEdge(ref _hashcode, aEdgeKey);
                    }
                    else
                    {
                        if (Level == 0)
                        {
                            var newEdgeKey = new EdgeKey(aEdgeKey.VertexTypeID);
                            Edges.Add(newEdgeKey);

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
                    if (Level == 0)
                    {
                        Edges.Add(aEdgeKey);

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
            if (obj is LevelKey)
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
            return String.Format("Level: {0} EdgeKey: {1}", Level, (Edges == null || Edges.Count == 0) ? 0 : Edges.Count);
        }

        #endregion

        #region Operators

        public LevelKey AddEdgeKey(EdgeKey myEdgeKey, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            // an empty level
            if (this.Edges == null)
            {
                return new LevelKey(new List<EdgeKey> { myEdgeKey }, myGraphDB, mySecurityToken, myTransactionToken);
            }

            var edgeList = new List<EdgeKey>(this.Edges);

            // if the first and only edge has a null attrUUID the new edge must have the same type!!
            if (edgeList.Count == 1 && !edgeList[0].IsAttributeSet)
            {
                if (edgeList[0].VertexTypeID != myEdgeKey.VertexTypeID)
                {
                    throw new InvalidLevelKeyOperationException(this, myEdgeKey, "+");
                }
                else
                {
                    return new LevelKey(new List<EdgeKey>() { myEdgeKey }, myGraphDB, mySecurityToken, myTransactionToken);
                }
            }
            else
            {
                if (myEdgeKey.IsAttributeSet)
                {
                    edgeList.Add(myEdgeKey);
                }

                return new LevelKey(edgeList, myGraphDB, mySecurityToken, myTransactionToken);
            }
        }

        public LevelKey AddLevelKey(LevelKey myLevelKey2, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if ((this.Edges == null || this.Edges.Count == 0) && (myLevelKey2.Edges == null || myLevelKey2.Edges.Count == 0))
                return new LevelKey();
            else if (this.Edges == null || this.Edges.Count == 0)
                return new LevelKey(myLevelKey2.Edges, myGraphDB, mySecurityToken, myTransactionToken);
            else if (myLevelKey2.Edges == null || myLevelKey2.Edges.Count == 0)
                return new LevelKey(this.Edges, myGraphDB, mySecurityToken, myTransactionToken);

            if (this.Level == 0 && myLevelKey2.Level == 0)
            {
                #region both are level 0 (User/null)
                if (this.Edges[0].VertexTypeID != myLevelKey2.Edges[0].VertexTypeID) // if the types are different then something is really wrong
                    throw new InvalidLevelKeyOperationException(this, myLevelKey2, "+");
                else
                    return new LevelKey(this.Edges, myGraphDB, mySecurityToken, myTransactionToken);
                #endregion
            }
            else if (this.Level == 0)
            {
                #region one of them is level 0 - so we can just skip this level: User/null + User/Friends == User/Friends
                if (this.Edges[0].VertexTypeID != myLevelKey2.Edges[0].VertexTypeID) // if the types are different then something is really wrong
                    throw new InvalidLevelKeyOperationException(this, myLevelKey2, "+");
                else
                    return new LevelKey(myLevelKey2.Edges, myGraphDB, mySecurityToken, myTransactionToken); // just return the other level
                #endregion
            }
            else if (myLevelKey2.Level == 0)
            {
                #region one of them is level 0 - so we can just skip this level: User/null + User/Friends == User/Friends
                if (this.Edges[0].VertexTypeID != myLevelKey2.Edges[0].VertexTypeID) // if the types are different then something is really wrong
                    throw new InvalidLevelKeyOperationException(this, myLevelKey2, "+");
                else
                    return new LevelKey(this.Edges, myGraphDB, mySecurityToken, myTransactionToken); // just return the other level
                #endregion
            }

            var edges = new List<EdgeKey>(this.Edges);
            edges.AddRange(myLevelKey2.Edges);

            return new LevelKey(edges, myGraphDB, mySecurityToken, myTransactionToken);
        }

        public LevelKey RemoveEdgeKey(EdgeKey myEdgeKey, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var edgeList = new List<EdgeKey>(this.Edges);
            //edgeList.Remove(myEdgeKey);

            if (edgeList[edgeList.Count - 1] != myEdgeKey)
                throw new InvalidLevelKeyOperationException(this, myEdgeKey, "-");

            return new LevelKey(edgeList.Take(edgeList.Count - 1), myGraphDB, mySecurityToken, myTransactionToken);
        }

        public LevelKey RemoveLevelKey(LevelKey myOtherLevelKey, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (this.Level < myOtherLevelKey.Level)
                throw new ArgumentException("level of left (" + this.Level + ") operand is lower than right (" + myOtherLevelKey.Level + ") operand:", "myOtherLevelKey");

            if (!this.StartsWith(myOtherLevelKey, true))
                throw new ArgumentException("left operand level does not starts with right operand level");

            if (myOtherLevelKey.Level == 0)
                return this;

            if (this.Level == myOtherLevelKey.Level)
                return new LevelKey(this.Edges[0].VertexTypeID, myGraphDB, mySecurityToken, myTransactionToken);

            var edgeList = new List<EdgeKey>(this.Edges);

            return new LevelKey(this.Edges.Skip(myOtherLevelKey.Edges.Count), myGraphDB, mySecurityToken, myTransactionToken);
        }

        #endregion

        public EdgeKey LastEdge
        {
            get
            {
                return Edges.Last();
            }
        }

        public LevelKey GetPredecessorLevel(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            switch (Level)
            {
                case 0:

                    return this;

                case 1:

                    return new LevelKey(new List<EdgeKey>() { new EdgeKey(Edges[0].VertexTypeID) }, myGraphDB, mySecurityToken, myTransactionToken);

                default:

                    return new LevelKey(Edges.Take(Edges.Count - 1), myGraphDB, mySecurityToken, myTransactionToken);
            }
        }


        public bool StartsWith(LevelKey myLevel)
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

        public bool StartsWith(LevelKey myLevel, Boolean IncludingAttrs)
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

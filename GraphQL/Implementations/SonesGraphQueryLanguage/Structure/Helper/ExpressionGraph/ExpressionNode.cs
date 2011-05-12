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
using sones.Library.PropertyHyperGraph;
using sones.GraphDB;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.Request;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    /// <summary>
    /// This class implements the nodes of the expression graph.
    /// </summary>
    public sealed class ExpressionNode : IExpressionNode
    {
        #region Properties

        /// <summary>
        /// A lock object
        /// </summary>
        private readonly Object _lockObj;
        
        /// <summary>
        /// The VertexID of the Node (equivalent to its DBObject)
        /// </summary>
        private Int64 _vertexID;

        /// <summary>
        /// The IVertex of the Node
        /// </summary>
        private IVertex _Object;

        /// <summary>
        /// The weight of the node
        /// </summary>
        private IComparable _NodeWeight;

        /// <summary>
        /// Set of weighted BackwardEdges
        /// </summary>
        private Dictionary<EdgeKey, HashSet<IExpressionEdge>> _BackwardEdges = new Dictionary<EdgeKey, HashSet<IExpressionEdge>>();

        public Dictionary<EdgeKey, HashSet<IExpressionEdge>> BackwardEdges
        {
            get
            {
                lock (_lockObj)
                {
                    return _BackwardEdges;
                }
            }

        }

        /// <summary>
        /// Set of weighted ForwardEdges
        /// </summary>
        private Dictionary<EdgeKey, HashSet<IExpressionEdge>> _ForwardEdges = new Dictionary<EdgeKey, HashSet<IExpressionEdge>>();

        public Dictionary<EdgeKey, HashSet<IExpressionEdge>> ForwardEdges
        {
            get
            {
                lock (_lockObj)
                {
                    return _ForwardEdges;
                }
            }
        }

        /// <summary>
        /// Set of weighted ForwardEdges
        /// </summary>
        private Dictionary<LevelKey, HashSet<Int64>> _ComplexConnection = new Dictionary<LevelKey, HashSet<Int64>>();

        public Dictionary<LevelKey, HashSet<Int64>> ComplexConnection
        {
            get
            {
                lock (_lockObj)
                {
                    return _ComplexConnection;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ObjectUUID">The ObjectUUID of the DBObject that is referenced by this node.</param>
        /// <param name="Weight">The Weight of this node.</param>
        public ExpressionNode(Int64 ObjectUUID, IComparable NodeWeight)
            : this(NodeWeight)
        {
            _vertexID = ObjectUUID;
            _Object = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="myObject">The IVertex that is referenced by this node.</param>
        /// <param name="Weight">The Weight of this node.</param>
        public ExpressionNode(IVertex myObject, IComparable NodeWeight)
            : this(NodeWeight)
        {
            _Object = myObject;
            _vertexID = myObject.VertexID;
        }

        private ExpressionNode(IComparable NodeWeight)
        {
            _NodeWeight = NodeWeight;
            _lockObj = new object();
        }

        #endregion

        #region public methods

        /// <summary>
        /// This method returns the VertexID of the IVertex that is referenced by this node.
        /// </summary>
        /// <returns>A VertexID</returns>
        public Int64 GetVertexID()
        {
            lock (_lockObj)
            {
                return _vertexID;
            }
        }

        /// <summary>
        /// This method returns the IVertex that is referenced by this node.
        /// </summary>
        /// <param name="myDBObjectCache">The actual query cache.</param>
        /// <param name="myTypeUUID">The TypeUUID of the DBObject.</param>
        /// <returns>A IVertex</returns>
        public IVertex GetIVertex(IGraphDB myGraphDB, Int64 myVertexTypeID, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            lock (_lockObj)
            {
                if (_Object != null)
                {
                    return _Object;
                }
                else
                {

                    _Object = myGraphDB.GetVertex<IVertex>(
                        mySecurityToken,
                        myTransactionToken,
                        new RequestGetVertex(myVertexTypeID, GetVertexID()), (stats, vertex) => vertex);

                    return _Object;
                }
            }
        }

        public void AddForwardEdges(EdgeKey forwardDestination, Dictionary<Int64, IComparable> validUUIDs)
        {
            lock (_lockObj)
            {
                if (_ForwardEdges.ContainsKey(forwardDestination))
                {

                    _ForwardEdges[forwardDestination].UnionWith(validUUIDs.Select(item => (IExpressionEdge)(new ExpressionEdge(item.Key, item.Value, forwardDestination))));

                }
                else
                {

                    _ForwardEdges.Add(forwardDestination, new HashSet<IExpressionEdge>(validUUIDs.Select(item => (IExpressionEdge)(new ExpressionEdge(item.Key, item.Value, forwardDestination)))));

                }
            }
        }

        public void AddForwardEdges(EdgeKey forwardDestination, IEnumerable<IExpressionEdge> validEdges)
        {
            lock (_lockObj)
            {
                if (_ForwardEdges.ContainsKey(forwardDestination))
                {

                    _ForwardEdges[forwardDestination].UnionWith(validEdges);

                }
                else
                {

                    _ForwardEdges.Add(forwardDestination, new HashSet<IExpressionEdge>(validEdges));

                }
            }
        }

        public void AddBackwardEdges(IEnumerable<IExpressionEdge> validEdges)
        {
            lock (_lockObj)
            {
                foreach (var aEdge in validEdges)
                {
                    if (_BackwardEdges.ContainsKey(aEdge.Direction))
                    {
                        //update
                        _BackwardEdges[aEdge.Direction].Add(aEdge);
                    }
                    else
                    {
                        //create
                        _BackwardEdges.Add(aEdge.Direction, new HashSet<IExpressionEdge>() { aEdge });
                    }
                }
            }
        }

        public void AddForwardEdges(IEnumerable<IExpressionEdge> validEdges)
        {
            lock (_lockObj)
            {
                foreach (var aEdge in validEdges)
                {
                    if (_ForwardEdges.ContainsKey(aEdge.Direction))
                    {
                        //update
                        _ForwardEdges[aEdge.Direction].Add(aEdge);
                    }
                    else
                    {
                        //create
                        _ForwardEdges.Add(aEdge.Direction, new HashSet<IExpressionEdge>() { aEdge });
                    }
                }

            }
        }

        public void AddBackwardEdges(EdgeKey backwardDestination, Dictionary<Int64, IComparable> validUUIDs)
        {
            lock (_lockObj)
            {
                if (_BackwardEdges.ContainsKey(backwardDestination))
                {

                    _BackwardEdges[backwardDestination].UnionWith(validUUIDs.Select(item => (IExpressionEdge)(new ExpressionEdge(item.Key, item.Value, backwardDestination))));

                }
                else
                {

                    _BackwardEdges.Add(backwardDestination, new HashSet<IExpressionEdge>(validUUIDs.Select(item => (IExpressionEdge)(new ExpressionEdge(item.Key, item.Value, backwardDestination)))));

                }
            }
        }

        public void AddForwardEdge(EdgeKey ForwardEdge, Int64 destination, IComparable weight)
        {
            lock (_lockObj)
            {
                if (_ForwardEdges.ContainsKey(ForwardEdge))
                {

                    _ForwardEdges[ForwardEdge].Add(new ExpressionEdge(destination, weight, ForwardEdge));

                }
                else
                {

                    _ForwardEdges.Add(ForwardEdge, new HashSet<IExpressionEdge>() { new ExpressionEdge(destination, weight, ForwardEdge) });

                }
            }
        }

        public void RemoveBackwardEdges(EdgeKey myEdgeKey)
        {
            lock (_lockObj)
            {
                _BackwardEdges.Remove(myEdgeKey);
            }
        }

        public void RemoveForwardEdges(EdgeKey myEdgeKey)
        {
            lock (_lockObj)
            {
                _ForwardEdges.Remove(myEdgeKey);
            }
        }

        public void RemoveForwardEdge(EdgeKey myEdgeKey, Int64 myObjectUUID)
        {
            lock (_lockObj)
            {
                if (!_ForwardEdges.ContainsKey(myEdgeKey))
                    return;

                _ForwardEdges[myEdgeKey].RemoveWhere(exprEdge => exprEdge.Destination == myObjectUUID);
            }
        }

        public void RemoveBackwardEdge(EdgeKey myEdgeKey, Int64 myObjectUUID)
        {
            lock (_lockObj)
            {
                var destEdges = from e in _BackwardEdges where e.Key.VertexTypeID == myEdgeKey.VertexTypeID select e.Key;
                foreach (var be in destEdges)
                {
                    _BackwardEdges[be].RemoveWhere(exprEdge => exprEdge.Destination == myObjectUUID);
                }
            }
        }

        public void AddBackwardEdge(EdgeKey backwardDestination, Int64 validUUIDs, IComparable edgeWeight)
        {
            lock (_lockObj)
            {
                var backwardEdges = new HashSet<IExpressionEdge>() { new ExpressionEdge(validUUIDs, edgeWeight, backwardDestination) };

                if (_BackwardEdges.ContainsKey(backwardDestination))
                {

                    _BackwardEdges[backwardDestination].UnionWith(backwardEdges);

                }
                else
                {

                    _BackwardEdges.Add(backwardDestination, backwardEdges);

                }
            }
        }

        public void AddComplexConnection(LevelKey myLevelKey, Int64 myUUID)
        {
            lock (_lockObj)
            {
                if (_ComplexConnection.ContainsKey(myLevelKey))
                {
                    _ComplexConnection[myLevelKey].Add(myUUID);
                }
                else
                {
                    _ComplexConnection.Add(myLevelKey, new HashSet<Int64>() { myUUID });
                }
            }
        }

        public void RemoveComplexConnection(LevelKey myLevelKey, Int64 myUUID)
        {
            lock (_lockObj)
            {
                if (_ComplexConnection.ContainsKey(myLevelKey))
                {
                    _ComplexConnection[myLevelKey].Remove(myUUID);
                }
            }
        }

        #endregion

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
            ExpressionNode p = obj as ExpressionNode;
            if ((System.Object)p == null)
            {
                return false;
            }

            return Equals(p);

        }

        public Boolean Equals(ExpressionNode p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return (this._vertexID == p.GetVertexID());
        }

        public static Boolean operator ==(ExpressionNode a, ExpressionNode b)
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

        public static Boolean operator !=(ExpressionNode a, ExpressionNode b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return _vertexID.GetHashCode();
        }

        #endregion

        public override string ToString()
        {
            return String.Format("{0}", _Object.ToString());
        }

        #endregion
    }
}

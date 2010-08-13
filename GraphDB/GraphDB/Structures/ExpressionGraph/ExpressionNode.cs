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


/* <id name="sones GraphDB – Node of ExpressionGraph" />
 * <copyright file="ExpressionNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This class implements the nodes of the expression graph.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Structures.ExpressionGraph
{

    /// <summary>
    /// This class implements the nodes of the expression graph.
    /// </summary>
    public class ExpressionNode : IExpressionNode
    {
        #region Properties
        /// <summary>
        /// The ObjectUUID of the Node (equivalent to its DBObject)
        /// </summary>
        private ObjectUUID _ObjectUUID;

        /// <summary>
        /// The DBObjectStream of the Node
        /// </summary>
        private DBObjectStream _Object;
        
        /// <summary>
        /// The weight of the node
        /// </summary>
        private ADBBaseObject _NodeWeight;
 
        /// <summary>
        /// Set of weighted BackwardEdges
        /// </summary>
        private Dictionary<EdgeKey, HashSet<IExpressionEdge>> _BackwardEdges = new Dictionary<EdgeKey, HashSet<IExpressionEdge>>();

        public Dictionary<EdgeKey, HashSet<IExpressionEdge>> BackwardEdges
        {
            get
            {
                lock (_BackwardEdges)
                {
                    return _BackwardEdges;
                }
            }

        }

        private List<IWarning> _Warnings = new List<IWarning>();

        public List<IWarning> Warnings
        {
            get
            {
                lock (_Warnings)
                {
                    return _Warnings;
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
                lock (_ForwardEdges)
                {
                    return _ForwardEdges;
                }
            }
        }

        /// <summary>
        /// Set of weighted ForwardEdges
        /// </summary>
        private Dictionary<LevelKey, HashSet<ObjectUUID>> _ComplexConnection = new Dictionary<LevelKey, HashSet<ObjectUUID>>();

        public Dictionary<LevelKey, HashSet<ObjectUUID>> ComplexConnection
        {
            get
            {
                lock (_ComplexConnection)
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
        public ExpressionNode(ObjectUUID ObjectUUID, ADBBaseObject NodeWeight)
            : this(NodeWeight)
        {
            _ObjectUUID = ObjectUUID;
            _Object = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="myObject">The DBObjectStream that is referenced by this node.</param>
        /// <param name="Weight">The Weight of this node.</param>
        public ExpressionNode(DBObjectStream myObject, ADBBaseObject NodeWeight)
            : this(NodeWeight)
        {
            _Object = myObject;
            _ObjectUUID = myObject.ObjectUUID;
        }

        private ExpressionNode(ADBBaseObject NodeWeight)
        {
            _NodeWeight = NodeWeight;
        }

        #endregion

        #region public methods

        /// <summary>
        /// This method returns the ObjectUUID of the DBOBjectStream that is referenced by this node.
        /// </summary>
        /// <returns>A ObjectUUID</returns>
        public ObjectUUID GetObjectUUID()
        {
            lock (_ObjectUUID)
            {
                return _ObjectUUID;
            }
        }

        /// <summary>
        /// This method returns the DBObjectStream that is referenced by this node.
        /// </summary>
        /// <param name="myDBObjectCache">The actual query cache.</param>
        /// <param name="myTypeUUID">The TypeUUID of the DBObject.</param>
        /// <returns>A DBObjectStream</returns>
        public DBObjectStream GetDBObjectStream(DBObjectCache myDBObjectCache, TypeUUID myTypeUUID)
        {
            lock (_ObjectUUID)
            {
                if (_Object != null)
                {
                    return _Object;
                }
                else
                {

                    var tempObject = myDBObjectCache.LoadDBObjectStream(myTypeUUID, GetObjectUUID());
                    if (tempObject.Failed())
                    {
                        return null;
                    }

                    _Object = tempObject.Value;


                    return _Object;
                }
            }
        }

        public void AddForwardEdges(EdgeKey forwardDestination, Dictionary<ObjectUUID, ADBBaseObject> validUUIDs)
        {
            lock (_ForwardEdges)
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
            lock (_ForwardEdges)
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
            lock (_BackwardEdges)
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
            lock (_ForwardEdges)
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

        public void AddBackwardEdges(EdgeKey backwardDestination, Dictionary<ObjectUUID, ADBBaseObject> validUUIDs)
        {
            lock (_BackwardEdges)
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

        public void AddForwardEdge(EdgeKey ForwardEdge, ObjectUUID destination, ADBBaseObject weight)
        {
            lock (_ForwardEdges)
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
            lock (_BackwardEdges)
            {
                _BackwardEdges.Remove(myEdgeKey);
            }
        }

        public void RemoveForwardEdges(EdgeKey myEdgeKey)
        {
            lock (_ForwardEdges)
            {
                _ForwardEdges.Remove(myEdgeKey);
            }
        }

        public void RemoveForwardEdge(EdgeKey myEdgeKey, ObjectUUID myObjectUUID)
        {
            lock (_ForwardEdges)
            {
                if (!_ForwardEdges.ContainsKey(myEdgeKey))
                    return;

                _ForwardEdges[myEdgeKey].RemoveWhere(exprEdge => exprEdge.Destination == myObjectUUID);
            }
        }

        public void RemoveBackwardEdge(EdgeKey myEdgeKey, ObjectUUID myObjectUUID)
        {
            lock (_BackwardEdges)
            {
                var destEdges = from e in _BackwardEdges where e.Key.TypeUUID == myEdgeKey.TypeUUID select e.Key;
                foreach (var be in destEdges)
                {
                    _BackwardEdges[be].RemoveWhere(exprEdge => exprEdge.Destination == myObjectUUID);
                }
            }
        }

        public void AddBackwardEdge(EdgeKey backwardDestination, ObjectUUID validUUIDs, ADBBaseObject edgeWeight)
        {
            lock (_BackwardEdges)
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

        public void AddComplexConnection(LevelKey myLevelKey, ObjectUUID myUUID)
        {
            lock (_ComplexConnection)
            {
                if (_ComplexConnection.ContainsKey(myLevelKey))
                {
                    _ComplexConnection[myLevelKey].Add(myUUID);
                }
                else
                {
                    _ComplexConnection.Add(myLevelKey, new HashSet<ObjectUUID>() { myUUID });
                }
            }
        }

        public void RemoveComplexConnection(LevelKey myLevelKey, ObjectUUID myUUID)
        {
            lock (_ComplexConnection)
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

            return (this._ObjectUUID == p.GetObjectUUID());
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
            return _ObjectUUID.GetHashCode();
        }

        #endregion

        public override string ToString()
        {
            return String.Format("{0}", _Object.ToString());
        }

        #endregion
    }
}

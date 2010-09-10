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

/* <id name="PandoraDB – Level of ExpressionGraph" />
 * <copyright file="ExpressionLevel.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This class implements the levels of the expression graph.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using sones.Lib;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.DataStructures;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.QueryLanguage.ExpressionGraph
{

    /// <summary>
    /// This class implements the levels of the expression graph.
    /// </summary>
    public class ExpressionLevel : IExpressionLevel
    {
        #region Properties

        /// <summary>
        /// The content of the level
        /// </summary>
        private Dictionary<LevelKey, IExpressionLevelEntry> _Content;

        public Dictionary<LevelKey, IExpressionLevelEntry> ExpressionLevels
        {
            get
            {
                lock (_Content)
                {
                    return _Content;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ExpressionLevel()
        {
            _Content = new Dictionary<LevelKey, IExpressionLevelEntry>();
        }

        #endregion

        #region IExpressionLevel Members

        public void AddNodeAndBackwardEdge(LevelKey myPath, ObjectUUID aDBObjectUUID, EdgeKey backwardDirection, ObjectUUID backwardDestination, ADBBaseObject EdgeWeight, ADBBaseObject NodeWeight)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myPath))
                {
                    //the level exists
                    if (_Content[myPath].Nodes.ContainsKey(aDBObjectUUID))
                    {


                        //Node exists
                        _Content[myPath].Nodes[aDBObjectUUID].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);


                    }
                    else
                    {

                        //Node does not exist
                        _Content[myPath].Nodes.Add(aDBObjectUUID, new ExpressionNode(aDBObjectUUID, NodeWeight));
                        _Content[myPath].Nodes[aDBObjectUUID].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);

                    }
                }
                else
                {
                    HashSet<IExpressionEdge> backwardEdges = new HashSet<IExpressionEdge>() { new ExpressionEdge(backwardDestination, EdgeWeight, backwardDirection) };

                    _Content.Add(myPath, new ExpressionLevelEntry(myPath));
                    _Content[myPath].Nodes.Add(aDBObjectUUID, new ExpressionNode(aDBObjectUUID, NodeWeight));
                    _Content[myPath].Nodes[aDBObjectUUID].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);

                }

            }
        }

        public void AddNodeAndBackwardEdge(LevelKey myPath, DBObjectStream aDBObjectStream, EdgeKey backwardDirection, ObjectUUID backwardDestination, ADBBaseObject EdgeWeight, ADBBaseObject NodeWeight)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myPath))
                {
                    //the level exists
                    if (_Content[myPath].Nodes.ContainsKey(aDBObjectStream.ObjectUUID))
                    {


                        //Node exists
                        _Content[myPath].Nodes[aDBObjectStream.ObjectUUID].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);


                    }
                    else
                    {

                        //Node does not exist
                        _Content[myPath].Nodes.Add(aDBObjectStream.ObjectUUID, new ExpressionNode(aDBObjectStream, NodeWeight));
                        _Content[myPath].Nodes[aDBObjectStream.ObjectUUID].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);

                    }
                }
                else
                {
                    HashSet<IExpressionEdge> backwardEdges = new HashSet<IExpressionEdge>() { new ExpressionEdge(backwardDestination, EdgeWeight, backwardDirection) };


                    _Content.Add(myPath, new ExpressionLevelEntry(myPath));
                    _Content[myPath].Nodes.Add(aDBObjectStream.ObjectUUID, new ExpressionNode(aDBObjectStream, NodeWeight));
                    _Content[myPath].Nodes[aDBObjectStream.ObjectUUID].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);

                }

            }
        }

        public void AddBackwardEdgesToNode(LevelKey myPath, ObjectUUID myObjectUUID, EdgeKey backwardDestination, Dictionary<ObjectUUID, ADBBaseObject> validUUIDs)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myPath))
                {
                    //the level exists
                    if (_Content[myPath].Nodes.ContainsKey(myObjectUUID))
                    {

                        //Node exists
                        _Content[myPath].Nodes[myObjectUUID].AddBackwardEdges(backwardDestination, validUUIDs);

                    }
                    else
                    {
                        //Node does not exist
                        throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), "The node does not exist in this LevelKey."));
                    }
                }
                else
                {
                    //LevelKey does not exist
                    throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), "The LevelKey does not exist in this ExpressionLevel."));
                }
            }
        }

        public void AddForwardEdgeToNode(LevelKey levelKey, ObjectUUID aObjectUUID, EdgeKey forwardDirection, ObjectUUID destination, ADBBaseObject edgeWeight)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(levelKey))
                {
                    //the level exists
                    if (_Content[levelKey].Nodes.ContainsKey(aObjectUUID))
                    {
                        //Node exists
                        _Content[levelKey].Nodes[aObjectUUID].AddForwardEdge(forwardDirection, destination, edgeWeight);
                    }
                    else
                    {
                        //Node does not exist
                        throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), "The node does not exist in this LevelKey."));
                    }
                }
                else
                {
                    //LevelKey does not exist
                    throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), "The LevelKey does not exist in this ExpressionLevel."));
                }
            }
        }

        public void AddNode(LevelKey levelKey, IExpressionNode expressionNode)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(levelKey))
                {
                    var tempObjectUUID = expressionNode.GetObjectUUID();

                    if (_Content[levelKey].Nodes.ContainsKey(tempObjectUUID))
                    {
                        //the node already exist, so update its edges
                        foreach (var aBackwardEdge in expressionNode.BackwardEdges)
                        {
                            _Content[levelKey].Nodes[tempObjectUUID].AddBackwardEdges(aBackwardEdge.Value);
                        }

                        foreach (var aForwardsEdge in expressionNode.ForwardEdges)
                        {
                            _Content[levelKey].Nodes[tempObjectUUID].AddForwardEdges(aForwardsEdge.Value);
                        }
                    }
                    else
                    {
                        _Content[levelKey].Nodes.Add(tempObjectUUID, expressionNode);
                    }
                }
                else
                {
                    _Content.Add(levelKey, new ExpressionLevelEntry(levelKey));
                    _Content[levelKey].Nodes.Add(expressionNode.GetObjectUUID(), expressionNode);
                }
            }
        }

        public void AddEmptyLevelKey(LevelKey levelKey)
        {
            lock (_Content)
            {
                if (!_Content.ContainsKey(levelKey))
                {
                    _Content.Add(levelKey, new ExpressionLevelEntry(levelKey));
                }
            }
        }

        public IEnumerable<IExpressionNode> GetNodes(LevelKey myLevelKey)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myLevelKey))
                {
                    return _Content[myLevelKey].Nodes.Values;
                }

                return null;
            }
        }

        public Dictionary<ObjectUUID, IExpressionNode> GetNode(LevelKey myLevelKey)
        {
            lock (_Content)
            {

                if (_Content.ContainsKey(myLevelKey))
                    return _Content[myLevelKey].Nodes;

                return null;
            }
        }

        public void RemoveNode(LevelKey myLevelKey, ObjectUUID myObjectUUID)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myLevelKey))
                {
                    _Content[myLevelKey].Nodes.Remove(myObjectUUID);
                }
            }
        }

        public Boolean HasLevelKey(LevelKey myLevelKey)
        {
            lock (_Content)
            {
                return _Content.ContainsKey(myLevelKey);
            }
        }

        #endregion

        #region override

        public override string ToString()
        {
            return String.Format("Level#: {0}", ExpressionLevels.Count);
        }

        #endregion
    }

}


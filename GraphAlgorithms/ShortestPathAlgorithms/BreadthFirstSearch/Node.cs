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

/* <id name="PandoraDB – BreadthFirstSearch" />
 * <copyright file="BreadthFirstSearch.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Martin Junghanns</developer>
 * <developer>Michael Woidak</developer>
 * <summary>
 * A Node represents a node in a graph. The Key is unique (ObjectUUID) and every
 * node know about its parents and childrens.
 * 
 * The Node class is used by BFS search and evaluation.
 * </summary>
 */

using System.Collections.Generic;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

namespace GraphAlgorithms.PathAlgorithm.BFSTreeStructure
{
    public class Node
    {
        #region private members

        //unique identifier
        private ObjectUUID _Key;

        //are used to create the paths recursive
        private HashSet<Node> _Parents;

        //are used to create the paths recursive
        private HashSet<Node> _Children;

        //is used to check if the node is already in path
        private bool _AlreadyInPath;

        #endregion

        #region constructors

        public Node()
        {
            _Key = null;
            _Parents = new HashSet<Node>();
            _Children = new HashSet<Node>();
            _AlreadyInPath = false;
        }

        public Node(ObjectUUID myObjectUUID) : this()
        {
            _Key = myObjectUUID;
        }

        public Node(ObjectUUID myObjectUUID, bool AlreadyInPath)
            : this(myObjectUUID)
        {
            _AlreadyInPath = AlreadyInPath;
        }

        public Node(ObjectUUID myObjectUUID, Node myParent) 
            : this(myObjectUUID)
        {
            _Parents.Add(myParent);
        }

        #endregion

        #region getter/setter

        public ObjectUUID Key
        {
            get { return this._Key; }
            set { this._Key = value; }
        }

        public HashSet<Node> Parents
        {
            get { return this._Parents; }
            set { this._Parents = value; }
        }

        public HashSet<Node> Children
        {
            get { return this._Children; }
            set { this._Children = value; }
        }

        public bool AlreadyInPath
        {
            get { return this._AlreadyInPath; }
            set { this._AlreadyInPath = value; }
        }

        #endregion

        #region public methods

        public bool addChild(Node myChild)
        {
            bool equal = false;

            foreach (var child in _Children)
            {
                if (child._Key.Equals(myChild.Key))
                {
                    equal = true;

                    child.addChildren(myChild.Children);
                    child.addParents(myChild.Parents);

                    break;
                }
            }

            if (!equal)
            {
                return _Children.Add(myChild);
            }

            return false;
        }

        public void addChildren(HashSet<Node> myChildren)
        {
            bool equal = false;

            foreach (var myChild in myChildren)
            {
                foreach (var child in _Children)
                {
                    if (myChild != null && child != null)
                    {
                        if (child._Key.Equals(myChild.Key))
                        {
                            equal = true;

                            break;
                        }
                    }
                }

                if (!equal)
                {
                    _Children.Add(myChild);
                    if (!myChild.Parents.Contains(this))
                    {
                        myChild.addParent(this);
                    }
                }
                else
                {
                    equal = false;
                }
            }
        }

        public bool addParent(Node myParent)
        {
            bool equal = false;

            foreach (var parent in _Parents)
            {
                if (parent._Key.Equals(myParent.Key))
                {
                    equal = true;

                    parent.addChildren(myParent.Children);
                    parent.addParents(myParent.Parents);

                   break;
                }
            }

            if (!equal)
            {
                return _Parents.Add(myParent);
            }

            return false;
        }

        public void addParents(HashSet<Node> myParents)
        {
            bool equal = false;

            foreach (var myParent in myParents)
            {
                foreach (var parent in _Parents)
                {
                    if (parent._Key.Equals(myParent.Key))
                    {
                        equal = true;
                       
                        break;
                    }
                }

                if (!equal)
                {
                    _Parents.Add(myParent);
                }
                else
                {
                    equal = false;
                }
            }
        }

        #endregion

        #region Overrides
                
        public override int GetHashCode()
        {
            return _Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Node)
            {
                return _Key.Equals((obj as Node)._Key);
            }
            else
            {
                return false;
            }
        }
        
        #endregion

    }
}

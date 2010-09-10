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

using System;
using System.Collections.Generic;
using System.Text;

namespace sones.Lib.BTree
{

    /// <summary>
    /// The Node class represents the base concept of a Node for a tree or graph.  It contains
    /// a data item of type T, and a list of neighbors.
    /// </summary>
    /// <typeparam name="T">The type of data contained in the Node.</typeparam>
    /// <remarks>None of the classes in the sones.Pandora.Storage.BTree namespace use the Node class directly;
    /// they all derive from this class, adding necessary functionality specific to each data structure.</remarks>

    

    public class Node<T>
    {


        #region Private Member Variables

        private T data;

        private NodeList<T> neighbors = null;

        #endregion


        #region Constructors

        public Node() { }
        
        public Node(T data) : this(data, null) { }
        
        public Node(T data, NodeList<T> neighbors)
        {
            this.data = data;
            this.neighbors = neighbors;
        }

        #endregion


        #region Properties

        /// <summary>
        /// The element stored in this tree
        /// </summary>
        public T Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        /// <summary>
        /// the neighbors of this tree node
        /// </summary>
        protected NodeList<T> Neighbors
        {
            get
            {
                return neighbors;
            }
            set
            {
                neighbors = value;
            }
        }

        #endregion


    }

}

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

namespace sones.Library.Commons.VertexStore.Definitions
{
    /// <summary>
    /// This struct represents the filesystem definition for an incoming edge
    /// </summary>
    public struct IncomingEdgeAddDefinition
    {
        #region data

        /// <summary>
        /// The property id of the edge that points to another vertex
        /// </summary>
        public readonly Int64 PropertyID;

        /// <summary>
        /// The ids of the incoming vertices
        /// </summary>
        public readonly IEnumerable<Int64> VertexIDs;

        /// <summary>
        /// The type id of the incoming vertex
        /// </summary>
        public readonly Int64 VertexTypeID;

        /// <summary>
        /// The incoming vertex edition name
        /// </summary>
        public readonly String VertexEditionName;

        /// <summary>
        /// The incoming vertex revision id
        /// </summary>
        public readonly Int64 VertexRevisionID;

        private int _hashCode;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new IncomingEdgeAddDefinition
        /// </summary>
        /// <param name="myVertexTypeID">The type id of the incoming vertex</param>
        /// <param name="myPropertyID">The property id of the edge that points to another vertex</param>
        /// <param name="myVertexIDs">The ids of the incoming vertices</param>
        /// <param name="myVertexRevisionID">The incoming vertex revision id</param>
        /// <param name="myVertexEditionName">The incoming vertex edition name</param>
        public IncomingEdgeAddDefinition(
            Int64 myVertexTypeID,
            Int64 myPropertyID,
            IEnumerable<Int64> myVertexIDs,
            Int64 myVertexRevisionID = 0L,
            String myVertexEditionName = ConstantsVertexStore.DefaultVertexEdition)
        {
            PropertyID = myPropertyID;
            VertexIDs = myVertexIDs;
            VertexTypeID = myVertexTypeID;
            VertexRevisionID = myVertexRevisionID;
            VertexEditionName = myVertexEditionName;
            _hashCode = 0;

            CalcHashCode();
        }

        #endregion

        #region private helper

        private void CalcHashCode()
        {
            if (VertexIDs != null && VertexIDs.Count() > 0)
            {
                foreach (var aIncomingVertex in VertexIDs)
                {
                    _hashCode = _hashCode ^ aIncomingVertex.GetHashCode();
                }
            }

            _hashCode = _hashCode ^ VertexTypeID.GetHashCode() ^ PropertyID.GetHashCode() ^ VertexEditionName.GetHashCode() ^ VertexRevisionID.GetHashCode();
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is IncomingEdgeAddDefinition)
            {
                return Equals((IncomingEdgeAddDefinition)obj);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(IncomingEdgeAddDefinition p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return
                (this.VertexTypeID == p.VertexTypeID) &&
                (this.PropertyID == p.PropertyID) &&
                (this.VertexEditionName == p.VertexEditionName) &&
                (this.VertexRevisionID == p.VertexRevisionID) &&
                (EqualVertexIDs(this.VertexIDs, p.VertexIDs));
        }

        private bool EqualVertexIDs(IEnumerable<long> myVertexIDsA, IEnumerable<long> myVertexIDsB)
        {
            if (myVertexIDsA == null && myVertexIDsB == null)
            {
                return true;
            }

            if (myVertexIDsA == null || myVertexIDsB == null)
            {
                return false;
            }

            if (myVertexIDsA.Count() != myVertexIDsB.Count())
            {
                return false;
            }

            foreach (var aVertexIDFromA in myVertexIDsA)
            {
                if (!myVertexIDsB.Contains(aVertexIDFromA))
                {
                    return false;
                }
            }

            return true;
        }

        public static Boolean operator ==(IncomingEdgeAddDefinition a, IncomingEdgeAddDefinition b)
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

        public static Boolean operator !=(IncomingEdgeAddDefinition a, IncomingEdgeAddDefinition b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        #endregion
    }
}
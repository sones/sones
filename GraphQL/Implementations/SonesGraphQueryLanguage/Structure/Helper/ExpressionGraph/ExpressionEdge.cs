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

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    /// <summary>
    /// This class implements the edges of the expression graph.
    /// </summary>
    public sealed class ExpressionEdge : IExpressionEdge
    {
        #region data

        /// <summary>
        /// The weight of the edge.
        /// </summary>
        public readonly IComparable Weight;

        /// <summary>
        /// The direction of the edge
        /// </summary>
        private readonly EdgeKey _direction;

        /// <summary>
        /// The vertexID where the edge points to.
        /// </summary>
        private readonly Int64 _destination;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="myDestination">The ExpressionNode that this edge is pointing to.</param>
        /// <param name="myWeight">The Weight of this edge.</param>
        /// <param name="myDirection">The direction (Type/Attribute) that this edge is pointing to.</param>
        public ExpressionEdge(Int64 myDestination, IComparable myWeight, EdgeKey myDirection)
        {
            _destination = myDestination;
            Weight = myWeight;
            _direction = myDirection;
        }

        #endregion

        #region IExpressionEdge Members

        public EdgeKey Direction
        {
            get { return _direction; }
        }

        public long Destination
        {
            get { return _destination; }
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
            ExpressionEdge p = obj as ExpressionEdge;
            if ((System.Object)p == null)
            {
                return false;
            }

            return Equals(p);

        }

        public Boolean Equals(ExpressionEdge p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return (this._destination == p.Destination) && (this._direction == p.Direction);
        }

        public static Boolean operator ==(ExpressionEdge a, ExpressionEdge b)
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

        public static Boolean operator !=(ExpressionEdge a, ExpressionEdge b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return _destination.GetHashCode() ^ _direction.GetHashCode();
        }

        #endregion
    }
}

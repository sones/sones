/* <id name="GraphDB – Edge of ExpressionGraph" />
 * <copyright file="ExpressionEdge.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements the edges of the expression graph.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphDB.TypeManagement.BasicTypes;


#endregion

namespace sones.GraphDB.Structures.ExpressionGraph
{

    /// <summary>
    /// This class implements the edges of the expression graph.
    /// </summary>
    public class ExpressionEdge: IExpressionEdge
    {
        #region Properties

        /// <summary>
        /// The DBObject where the edge points to.
        /// </summary>
        private ObjectUUID _Destination;

        public ObjectUUID Destination
        {
            get { return _Destination; }
        }

        /// <summary>
        /// The weight of the edge.
        /// </summary>
        private ADBBaseObject _Weight;

        public ADBBaseObject Weight
        {
            get { return _Weight; }
        }

        /// <summary>
        /// The direction of the edge
        /// </summary>
        private EdgeKey _Direction;

        public EdgeKey Direction
        {
            get { return _Direction; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Destination">The ExpressionNode that this edge is pointing to.</param>
        /// <param name="Weight">The Weight of this edge.</param>
        /// <param name="Direction">The direction (Type/Attribute) that this edge is pointing to.</param>
        public ExpressionEdge(ObjectUUID Destination, ADBBaseObject Weight, EdgeKey Direction)
        {
            _Destination = Destination;
            _Weight = Weight;
            _Direction = Direction;
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

            return (this._Destination == p.Destination) && (this._Direction == p.Direction);
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
            return _Destination.GetHashCode() ^ _Direction.GetHashCode();
        }

        #endregion
    }

}

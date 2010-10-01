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

/*
 * sones GraphDS API - EdgeLabel
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

#endregion

namespace sones.GraphDB.NewAPI
{

    /// <summary>
    /// The edge class for user-defined edge types.
    /// </summary>

    public class Edge : DBObject, IEdge
    {

        // Do not mix it up with the internal Edge.cs!
        // This is a user-defined edge!

        #region Properties

        #region SourceVertex

        [HideFromDatabase]
        public IVertex SourceVertex { get; private set; }

        #endregion

        #region TargetVertex

        [HideFromDatabase]
        public IVertex TargetVertex
        {
            get
            {
                return _TargetVertices.FirstOrDefault();
            }
        }

        #endregion

        #region TargetVertices

        private readonly IEnumerable<IVertex> _TargetVertices;

        [HideFromDatabase]
        public IEnumerable<IVertex> TargetVertices
        {
            get
            {
                return _TargetVertices;
            }
        }

        #endregion

        #region EdgeTypeName

        public String EdgeTypeName { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        #region Edge()

        public Edge()
        {
            SourceVertex    = null;
            _TargetVertices = null;
        }

        #endregion

        #region Edge(mySourceVertex, myTargetVertex, myAttributes = null)

        public Edge(IVertex mySourceVertex, IVertex myTargetVertex, IDictionary<String, Object> myAttributes = null)
        {

            SourceVertex    = null;
            _TargetVertices = new List<IVertex> { myTargetVertex };

            if (myAttributes != null && myAttributes.Any())
                AddAttribute(myAttributes);

        }

        #endregion

        #region Edge(mySourceVertex, myTargetVertices, myAttributes = null)

        public Edge(IVertex mySourceVertex, IEnumerable<IVertex> myTargetVertices, IDictionary<String, Object> myAttributes = null)
        {

            SourceVertex    = mySourceVertex;
            _TargetVertices = myTargetVertices;

            if (myAttributes != null && myAttributes.Any())
                AddAttribute(myAttributes);

        }

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (myDBEdge1, myDBEdge2)

        public static Boolean operator ==(Edge myDBEdge1, Edge myDBEdge2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(myDBEdge1, myDBEdge2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myDBEdge1 == null) || ((Object) myDBEdge2 == null))
                return false;

            return myDBEdge1.Equals(myDBEdge2);

        }

        #endregion

        #region Operator != (myDBEdge1, myDBEdge2)

        public static Boolean operator !=(Edge myDBEdge1, Edge myDBEdge2)
        {
            return !(myDBEdge1 == myDBEdge2);
        }

        #endregion

        #endregion

        #region IEquatable<IEdge> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject == null)
                return false;

            var _Object = myObject as IEdge;
            if (_Object == null)
                return (Equals(_Object));

            return false;

        }

        #endregion

        #region Equals(myIEdge)

        public Boolean Equals(IEdge myIEdge)
        {

            if ((Object) myIEdge == null)
            {
                return false;
            }

            return true;

            //TODO: Here it might be good to check all attributes of the UNIQUE constraint!
            //return (this.UUID == myDBEdge.UUID);

        }

        #endregion

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
        
        #region ToString()

        public override String ToString()
        {

            var _ReturnValue = new StringBuilder(_Attributes.Count + " Attributes: ");

            foreach (var _KeyValuePair in _Attributes)
                _ReturnValue.Append(_KeyValuePair.Key + " = '" + _KeyValuePair.Value + "', ");

            _ReturnValue.Length = _ReturnValue.Length - 2;

            return _ReturnValue.ToString();

        }

        #endregion


    }

}

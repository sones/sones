/*
 * sones GraphDS API - EdgeLabel
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Structures;

#endregion

namespace sones.GraphDB.NewAPI
{

    /// <summary>
    /// The DBEdge class for user-defined edge types
    /// </summary>

    public class EdgeLabel : DBObject, IEquatable<EdgeLabel>
    {

        // Do not mix it up with the internal Edge.cs!
        // This is a user-defined edge!

        #region Properties

        #region SourceVertex

        [HideFromDatabase]
        public Vertex SourceVertex { get; private set; }

        #endregion

        #region TargetVertex

        [HideFromDatabase]
        public Vertex TargetVertex
        {

            get
            {
                return _TargetVertices.FirstOrDefault();
            }
        }

        #endregion

        #region TargetVertices

        private readonly IEnumerable<Vertex> _TargetVertices;

        [HideFromDatabase]
        public IEnumerable<Vertex> TargetVertices
        {
            get
            {
                return _TargetVertices;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region DBEdgeNew()

        public EdgeLabel()
        {
            SourceVertex    = null;
            _TargetVertices = null;
        }

        #endregion

        #region DBEdgeNew(mySourceVertex, myTargetVertex, myAttributes = null)

        public EdgeLabel(Vertex mySourceVertex, Vertex myTargetVertex, IDictionary<String, Object> myAttributes = null)
        {

            SourceVertex    = null;
            _TargetVertices = new List<Vertex> { myTargetVertex };

            if (myAttributes != null && myAttributes.Any())
                AddAttribute(myAttributes);

        }

        #endregion

        #region DBEdgeNew(mySourceVertex, myTargetVertices, myAttributes = null)

        public EdgeLabel(Vertex mySourceVertex, IEnumerable<Vertex> myTargetVertices, IDictionary<String, Object> myAttributes = null)
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

        public static Boolean operator ==(EdgeLabel myDBEdge1, EdgeLabel myDBEdge2)
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

        public static Boolean operator !=(EdgeLabel myDBEdge1, EdgeLabel myDBEdge2)
        {
            return !(myDBEdge1 == myDBEdge2);
        }

        #endregion

        #endregion

        #region IEquatable<DBEdgeNew> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject == null)
                return false;

            var _Object = myObject as EdgeLabel;
            if (_Object == null)
                return (Equals(_Object));

            return false;

        }

        #endregion

        #region Equals(myDBObject)

        public Boolean Equals(EdgeLabel myDBEdge)
        {

            if ((object) myDBEdge == null)
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

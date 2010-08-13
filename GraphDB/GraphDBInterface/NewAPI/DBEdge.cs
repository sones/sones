/*
 * sones GraphDS API - DBEdge
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

    public class DBEdge : DBObject, IEquatable<DBEdge>
    {

        // ToDo: Needs clean'up!

        #region Properties

        #region SourceVertex

        [HideFromDatabase]
        public DBVertex SourceVertex { get; private set; }

        #endregion

        #region TargetVertex

        [HideFromDatabase]
        public DBVertex TargetVertex
        {

            get
            {

                if (TargetVertices.Any())
                    return TargetVertices.First();

                return null;

            }

            set
            {
                _TargetVertices.Clear();
                _TargetVertices.Add(value);
            }
        
        }

        #endregion

        #region TargetVertices

        private readonly HashSet<DBVertex> _TargetVertices;

        [HideFromDatabase]
        public IEnumerable<DBVertex> TargetVertices
        {

            get
            {
                return _TargetVertices;
            }

            set
            {
                _TargetVertices.Clear();
                foreach (var _Vertex in value)
                    _TargetVertices.Add(_Vertex);
            }

        }

        #endregion

        #endregion

        #region Constructors

        #region DBEdge()

        public DBEdge()
        {
            SourceVertex      = null;
            _TargetVertices    = null;
        }

        #endregion

        #region DBEdge(mySourceVertex, myTargetVertex)

        public DBEdge(DBVertex mySourceVertex, DBVertex myTargetVertex)
        {
            SourceVertex      = mySourceVertex;
            _TargetVertices    = new HashSet<DBVertex>() { myTargetVertex };
        }

        #endregion

        #region DBEdge(mySourceVertex, myTargetVertices)

        public DBEdge(DBVertex mySourceVertex, IEnumerable<DBVertex> myTargetVertices)
        {
            SourceVertex      = mySourceVertex;
            _TargetVertices    = new HashSet<DBVertex>(myTargetVertices);
        }

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (myDBEdge1, myDBEdge2)

        public static Boolean operator == (DBEdge myDBEdge1, DBEdge myDBEdge2)
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

        public static Boolean operator != (DBEdge myDBEdge1, DBEdge myDBEdge2)
        {
            return !(myDBEdge1 == myDBEdge2);
        }

        #endregion

        #endregion

        #region IEquatable<DBEdge> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject == null)
                return false;

            var _Object = myObject as DBEdge;
            if (_Object == null)
                return (Equals(_Object));

            return false;

        }

        #endregion

        #region Equals(myDBObject)

        public Boolean Equals(DBEdge myDBEdge)
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

    }

}

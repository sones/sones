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

/*
 * sones GraphDS API - DBObject
 * Achim Friedland, 2009 - 2010
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

namespace sones.GraphDS.API.CSharp.Reflection
{

    /// <summary>
    /// The DBObject class for user-defined graph data base types
    /// </summary>

    public abstract class DBObject : DynamicObject, IEquatable<DBObject>
    {

        #region Data

        /// <summary>
        /// This dictionary will hold undefined attributes 
        /// </summary>
        protected readonly Dictionary<String, Object> _UndefinedAttributes;

        #endregion

        #region Properties

        public GraphDSSharp GraphDSSharp { get; set; }

        #region CreateTypeQuery

        protected String _CreateTypeQuery;

        internal String CreateTypeQuery
        {
            get
            {
                if (_CreateTypeQuery == null) ReflectMyself();
                return _CreateTypeQuery;
            }

        }

        #endregion

        #region CreateIndicesQueries

        // Just used within tests!

        protected readonly List<String> _CreateIndicesQueries;

        [HideFromDatabase]
        public List<String> CreateIndicesQueries
        {
            get
            {
                if (_CreateIndicesQueries == null) ReflectMyself();
                return _CreateIndicesQueries;
            }

        }

        #endregion

        #region Omnipresent attributes

        public ObjectUUID   UUID        { get; set; }

        [HideFromDatabase]
        public String       TYPE        { get; protected set; }

        public String       Edition     { get; set; }

        [HideFromDatabase]
        public ObjectRevisionID   RevisionID  { get; set; }

        [HideFromDatabase]
        public virtual String Comment   { get; set; }

        #endregion

        #endregion

        #region Constructors

        #region DBObject()

        public DBObject()
        {
            UUID                    = null;
            Edition                 = null;
            RevisionID              = null;
            _UndefinedAttributes    = new Dictionary<String, Object>();
            _CreateIndicesQueries   = new List<String>();
        }

        #endregion

        #endregion


        public abstract String GetInsertValues(String mySeperator);
        public abstract void ReflectMyself();


        #region Members of DynamicObject

        // Used to access undefined attributes

        #region GetDynamicMemberNames()

        public override IEnumerable<String> GetDynamicMemberNames()
        {
            return _UndefinedAttributes.Keys;
        }

        #endregion

        #region TryGetMember(myBinder, out myObject)

        public override Boolean TryGetMember(GetMemberBinder myBinder, out Object myObject)
        {
            return _UndefinedAttributes.TryGetValue(myBinder.Name, out myObject);
        }

        #endregion

        #region TrySetMember(myBinder, myObject)

        public override Boolean TrySetMember(SetMemberBinder myBinder, Object myObject)
        {

            //if (_StructuredData.Contains(myBinder.Name))
            //    throw new ArgumentException("Invalid operation!");

            if (_UndefinedAttributes.ContainsKey(myBinder.Name))
                _UndefinedAttributes[myBinder.Name] = myObject;

            else
                _UndefinedAttributes.Add(myBinder.Name, myObject);

            return true;

        }

        #endregion

        #endregion



        #region Operator overloading

        #region Operator == (myDBObject1, myDBObject2)

        public static Boolean operator == (DBObject myDBObject1, DBObject myDBObject2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(myDBObject1, myDBObject2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myDBObject1 == null) || ((Object) myDBObject2 == null))
                return false;

            return myDBObject1.Equals(myDBObject2);

        }

        #endregion

        #region Operator != (myDBObject1, myDBObject2)

        public static Boolean operator != (DBObject myDBObject1, DBObject myDBObject2)
        {
            return !(myDBObject1 == myDBObject2);
        }

        #endregion

        #endregion

        #region IEquatable<DBObject> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject == null)
                return false;

            var _Object = myObject as DBObject;
            if (_Object == null)
                return (Equals(_Object));

            return false;

        }

        #endregion

        #region Equals(myDBObject)

        public Boolean Equals(DBObject myDBObject)
        {

            if ((object) myDBObject == null)
            {
                return false;
            }

            //TODO: Here it might be good to check all attributes of the UNIQUE constraint!
            return (this.UUID == myDBObject.UUID);

        }

        #endregion

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return UUID.GetHashCode();
        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            var UUIDString = (UUID == null) ? "<null>" : UUID.ToString();
            var RevisionIDString = (RevisionID == null) ? "<null>" : RevisionID.ToString();

            return this.GetType().Name + "(UUID = " + UUIDString + ", RevisionID = " + RevisionIDString + ")";

        }

        #endregion


    }

}

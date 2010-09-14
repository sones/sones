/*
 * sones GraphDS API - DBObject
 * (c) Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Structures;
using sones.Lib.DataStructures.UUID;
using System.Diagnostics;

#endregion

namespace sones.GraphDB.NewAPI
{

    /// <summary>
    /// The abstract DBObject class for all user-defined vertices and edges.
    /// </summary>
    public abstract class DBObject : DynamicObject, IEnumerable<KeyValuePair<String, Object>>, IEquatable<DBObject>
    {

        #region Properties

        // Must be IGraphDBSession cause of the SessionToken!
        // But first IGraphDB and IGraphDBSession has to be cleaned
        // from graphdb-internal datastructures!
        protected          IGraphDB     GraphDBInterface;
        protected          SessionToken SessionToken;

        protected readonly IDictionary<String, Object> _Attributes;

        #region Omnipresent attributes

        #region Attributes (OBSOLETE!!!)

        /// <summary>
        /// This dictionary will hold information about the DBObject
        /// </summary>
        [HideFromDatabase, Obsolete("Do not use this dictionary directly!")]
        public IDictionary<String, Object> ObsoleteAttributes
        {
            get
            {
                return _Attributes;
            }
        }

        #endregion

        #region UUID

        public ObjectUUID UUID
        {

            get
            {
                
                Object _Object = null;

                if (_Attributes.TryGetValue("UUID", out _Object))
                    return _Object as ObjectUUID;

                return null;

            }

            set
            {

                if (_Attributes.ContainsKey("UUID"))
                    _Attributes["UUID"] = value;

                else
                    _Attributes.Add("UUID", value);

            }

        }

        #endregion

        #region TYPE

        [HideFromDatabase]
        public String TYPE
        {

            get
            {
                
                Object _Object = null;

                if (_Attributes.TryGetValue("TYPE", out _Object))
                    return _Object as String;

                return null;

            }

            protected set
            {

                if (_Attributes.ContainsKey("TYPE"))
                    _Attributes["TYPE"] = value;

                else
                    _Attributes.Add("TYPE", value);

            }

        }

        #endregion

        #region EDITION

        [HideFromDatabase]
        public String EDITION
        {

            get
            {
                
                Object _Object = null;

                if (_Attributes.TryGetValue("EDITION", out _Object))
                    return _Object as String;

                return null;

            }

            set
            {

                if (_Attributes.ContainsKey("EDITION"))
                    _Attributes["EDITION"] = value;

                else
                    _Attributes.Add("EDITION", value);

            }

        }

        #endregion

        #region REVISIONID

        [HideFromDatabase]
        public ObjectRevisionID REVISIONID
        {

            get
            {
                
                Object _Object = null;

                if (_Attributes.TryGetValue("REVISION", out _Object))
                    return _Object as ObjectRevisionID;

                return null;

            }

            set
            {

                if (_Attributes.ContainsKey("REVISION"))
                    _Attributes["REVISION"] = value;

                else
                    _Attributes.Add("REVISION", value);

            }

        }

        #endregion

        [HideFromDatabase]
        public virtual String Comment { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        #region DBObject()

        public DBObject()
        {

            // StringComparison.OrdinalIgnoreCase is often used to compare file names,
            // path names, network paths, and any other string whose value does not change
            // based on the locale of the user's computer.
            _Attributes = new Dictionary<String, Object>(StringComparer.OrdinalIgnoreCase);

            //_CreateIndicesQueries   = new List<String>();

        }

        #endregion

        #endregion


        #region Attribute manipulation

        #region AddAttribute(myAttributeName, myObject)

        public void AddAttribute(String myAttributeName, Object myObject)
        {

            if (myAttributeName == "UUID" && !(myObject is ObjectUUID))
                Debug.WriteLine("Setting vertex property 'UUID' to an unwanted value of '{0}'", myObject);

            if (myAttributeName == "REVISION" && !(myObject is ObjectRevisionID))
                Debug.WriteLine("Setting vertex property 'REVISION' to an unwanted value of '{0}'", myObject);

            _Attributes.Add(myAttributeName, myObject);

        }

        #endregion

        #region AddAttribute(myKeyValuePair)

        public void AddAttribute(KeyValuePair<String, Object> myKeyValuePair)
        {
            AddAttribute(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion

        #region AddAttribute(myKeyValuePairs)

        public void AddAttribute(IEnumerable<KeyValuePair<String, Object>> myKeyValuePairs)
        {
            foreach (var _KeyValuePair in myKeyValuePairs)
                AddAttribute(_KeyValuePair.Key, _KeyValuePair.Value);
        }

        #endregion

        #region AddAttribute(myIDictionary)

        public void AddAttribute(IDictionary<String, Object> myIDictionary)
        {
            foreach (var _KeyValuePair in myIDictionary)
                AddAttribute(_KeyValuePair.Key, _KeyValuePair.Value);
        }

        #endregion


        #region SetAttribute(myAttributeName, myObject)

        public void SetAttribute(String myAttributeName, Object myObject)
        {
            _Attributes[myAttributeName] = myObject;
        }

        #endregion

        #region SetAttribute(myKeyValuePair)

        public void SetAttribute(KeyValuePair<String, Object> myKeyValuePair)
        {
            SetAttribute(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion

        #region SetAttribute(myKeyValuePairs)

        public void SetAttribute(IEnumerable<KeyValuePair<String, Object>> myKeyValuePairs)
        {
            foreach (var _KeyValuePair in myKeyValuePairs)
                SetAttribute(_KeyValuePair.Key, _KeyValuePair.Value);
        }

        #endregion

        #region SetAttribute(myIDictionary)

        public void SetAttribute(IDictionary<String, Object> myIDictionary)
        {
            foreach (var _KeyValuePair in myIDictionary)
                SetAttribute(_KeyValuePair.Key, _KeyValuePair.Value);
        }

        #endregion


        #region RemoveAttribute(myAttributeName)

        public Boolean RemoveAttribute(String myAttributeName)
        {
            return _Attributes.Remove(myAttributeName);
        }

        #endregion


        #region CompareAttributes(myDBObject1, myDBObject2)

        private Boolean CompareAttributes(DBObject myDBObject1, DBObject myDBObject2)
        {

            if (myDBObject1.Count() != myDBObject2.Count())
                return false;

            foreach (var _KeyValuePair in myDBObject1)
            {

                if (myDBObject2.Contains(_KeyValuePair))
                    continue;

                return false;

            }

            return true;

        }

        #endregion

        #endregion


        #region DynamicObject Members

        #region GetDynamicMemberNames()

        public override IEnumerable<String> GetDynamicMemberNames()
        {
            return _Attributes.Keys;
        }

        #endregion

        #region TrySetMember(myBinder, myObject)

        public override Boolean TrySetMember(SetMemberBinder myBinder, Object myObject)
        {

            if (_Attributes.ContainsKey(myBinder.Name))
                _Attributes[myBinder.Name] = myObject;

            else
                _Attributes.Add(myBinder.Name, myObject);

            return true;

        }

        #endregion

        #region TryGetMember(myBinder, out myObject)

        public override Boolean TryGetMember(GetMemberBinder myBinder, out Object myObject)
        {
            return _Attributes.TryGetValue(myBinder.Name, out myObject);
        }

        #endregion

        #region TryDeleteMember(myBinder)

        public override Boolean TryDeleteMember(DeleteMemberBinder myBinder)
        {
            return _Attributes.Remove(myBinder.Name);
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
        
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Attributes.GetEnumerator();
        }

        #endregion

        #region IEnumerable<KeyValuePair<String, Object>> Members

        public IEnumerator<KeyValuePair<String, Object>> GetEnumerator()
        {
            return _Attributes.GetEnumerator();
        }

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

            if (UUID == null)
                return _Attributes.GetHashCode();

            return UUID.GetHashCode();

        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            var UUIDString = (UUID == null) ? "<null>" : UUID.ToString();
            var RevisionIDString = (REVISIONID == null) ? "<null>" : REVISIONID.ToString();

            return this.GetType().Name + "(UUID = " + UUIDString + ", RevisionID = " + RevisionIDString + ")";

        }

        #endregion


    }

}

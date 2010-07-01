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

    public class DBObject : DynamicObject, IEquatable<DBObject>
    {

        #region Data

        /// <summary>
        /// This dictionary will hold undefined attributes 
        /// </summary>
        private Dictionary<String, Object> _UndefinedAttributes;

        #endregion

        #region Properties

        #region CreateTypeQuery

        private String _CreateTypeQuery;

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

        private List<String> _CreateIndicesQueries;

        internal List<String> CreateIndicesQueries
        {
            get
            {
                if (_CreateIndicesQueries == null) ReflectMyself();
                return _CreateIndicesQueries;
            }

        }

        #endregion

        #region Omnipresent attributes

        public ObjectUUID  UUID       { get; set; }
        public String      TYPE       { get; set; }
        public String      Edition    { get; set; }
        public RevisionID  RevisionID { get; set; }

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
        }

        #endregion

        #endregion


        #region GetInsertValues(mySeperator)

        public String GetInsertValues(String mySeperator)
        {

            var _StringBuilder = new StringBuilder();
            Object _PropertyValue = null;

            var _AllProperties = this.GetType().GetProperties();

            if (_AllProperties.Length > 0)
            {

                foreach (var _PropertyInfo in _AllProperties)
                {

                    if (_PropertyInfo.CanRead && _PropertyInfo.CanWrite)
                    {

                        _PropertyValue = _PropertyInfo.GetValue(this, null);

                        if (_PropertyValue != null)
                            _StringBuilder.Append(_PropertyInfo.Name).Append(" = '").Append(_PropertyInfo.GetValue(this, null)).Append("'").Append(mySeperator);

                    }

                }

                _StringBuilder.Length = _StringBuilder.Length - mySeperator.Length;

            }

            return _StringBuilder.ToString();

        }

        #endregion

        #region ReflectMyself()

        private void ReflectMyself()
        {

            #region Init StringBuilder

            var _Command = new StringBuilder();
            var _Attributes = new StringBuilder();
            var _BackwardEdges = new StringBuilder();

            _Command.Append("CREATE TYPE " + this.GetType().Name);
            _Command.Append(" EXTENDS " + this.GetType().BaseType.Name);

            #endregion

            #region Find Attributes and Backwardedges

            var _AllProperties = this.GetType().GetProperties();

            if (_AllProperties.Length > 0)
            {

                foreach (var _PropertyInfo in _AllProperties)
                {

                    if (_PropertyInfo.CanRead && _PropertyInfo.CanWrite)
                    {

                        #region Check found attribute

                        var _AddToDatabaseType = true;
                        var _IsBackwardEdge = "";
                        var _CreateAttributeIndex = "";

                        // Ignore inherited attributes
                        if (_PropertyInfo.DeclaringType.Name != this.GetType().Name)
                            _AddToDatabaseType = false;

                        // Check attribute attributes ;)
                        else
                        {

                            foreach (var _Property in _PropertyInfo.GetCustomAttributes(true))
                            {

                                #region Check "HideFromDatabase"-Attribute

                                if (_Property as HideFromDatabase != null)
                                {
                                    _AddToDatabaseType = false;
                                    break;
                                }

                                #endregion

                                #region Check "NoAutoCreation"-Attribute

                                if (_Property as NoAutoCreation != null)
                                {
                                    _AddToDatabaseType = false;
                                    break;
                                }

                                #endregion

                                #region Check "BackwardEdge"-Attribute

                                var _BackwardEdge = _Property as BackwardEdge;

                                if (_BackwardEdge != null)
                                {
                                    _IsBackwardEdge = _BackwardEdge.ReferencedAttributeName;
                                    break;
                                }

                                #endregion

                                #region Check "CreateIndex"-Attribute

                                var _CreateIndexProperty = _Property as Indexed;

                                if (_CreateIndexProperty != null)
                                {

                                    var _IndexName = _CreateIndexProperty.IndexName;

                                    if (_IndexName.Equals(""))
                                        _IndexName = "IDX_" + _PropertyInfo.Name;

                                    var _IndexOrder = _CreateIndexProperty.IndexOrder;
                                    if (!_IndexOrder.Equals(""))
                                        _IndexOrder = " " + _IndexOrder;

                                    var _IndexType = _CreateIndexProperty.IndexType;
                                    if (!_IndexType.Equals(""))
                                        _IndexType = " INDEXTYPE " + _IndexType;

                                    _CreateAttributeIndex = "CREATE INDEX " + _IndexName + " ON " + this.GetType().Name + " (" + _PropertyInfo.Name + _IndexOrder + ")" + _IndexType;

                                }

                                #endregion

                            }

                        }

                        #endregion

                        #region In case: Add attribute to database type

                        if (_AddToDatabaseType)
                        {

                            #region Add the type of the property

                            var _DatabaseAttributeType = _PropertyInfo.PropertyType.Name;

                            #endregion

                            //HACK: Refactor this! e.g. use a dictionary for C#->PandoraDB type mappings
                            if (_DatabaseAttributeType == "UInt64" || _DatabaseAttributeType == "Int64" || _DatabaseAttributeType == "UInt32" || _DatabaseAttributeType == "Int32" ||
                                _DatabaseAttributeType == "ulong" || _DatabaseAttributeType == "long" || _DatabaseAttributeType == "uint" || _DatabaseAttributeType == "int")
                                _DatabaseAttributeType = "Integer";

                            #region Handle generic types like "LIST<...>"

                            if (_IsBackwardEdge.Equals(""))
                            {

                                if (_PropertyInfo.PropertyType.IsGenericType && _DatabaseAttributeType.StartsWith("List"))
                                    _DatabaseAttributeType = "LIST<" + _PropertyInfo.PropertyType.GetGenericArguments()[0].Name + "> ";

                                if (_PropertyInfo.PropertyType.IsGenericType && _DatabaseAttributeType.StartsWith("Set"))
                                    _DatabaseAttributeType = "SET<" + _PropertyInfo.PropertyType.GetGenericArguments()[0].Name + "> ";

                                _Attributes.Append(_DatabaseAttributeType + " ");

                            }

                            #endregion

                            #region Add the name of the property

                            var _DatabaseAttributeName = _PropertyInfo.Name;

                            if (_IsBackwardEdge.Equals(""))
                                _Attributes.Append(_DatabaseAttributeName + ", ");

                            else
                                _BackwardEdges.Append(_PropertyInfo.PropertyType.GetGenericArguments()[0].Name + "." + _IsBackwardEdge + " " + _DatabaseAttributeName);

                            #endregion

                            #region Add Attribute Index

                            if (!_CreateAttributeIndex.Equals(""))
                            {

                                if (_CreateIndicesQueries == null)
                                    _CreateIndicesQueries = new List<String>();

                                _CreateIndicesQueries.Add(_CreateAttributeIndex);

                            }

                            #endregion

                        }

                        #endregion

                    }

                }

                if (_Attributes.Length > 0)
                    _Attributes.Remove(_Attributes.Length - 2, 2);

            }

            #endregion

            #region Add Attributes

            if (_Attributes.Length > 0)
            {
                _Command.Append(" ATTRIBUTES (");
                _Command.Append(_Attributes);
                _Command.Append(")");
            }

            #endregion

            #region Add Backwardedges

            if (_BackwardEdges.Length > 0)
            {
                _Command.Append(" BACKWARDEDGES (");
                _Command.Append(_BackwardEdges);
                _Command.Append(")");
            }

            #endregion

            _CreateTypeQuery = _Command.ToString();

        }

        #endregion

        #region GetEdge(myEdgeName)

        public IEnumerable<DBObject> GetEdge(String myEdgeName)
        {

            var prop = this.GetType().GetProperty(myEdgeName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                return null;
            
            var edgeProp = this.GetType().GetProperty(myEdgeName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(this, null) as IList;
            if (edgeProp == null)
                return null;
                //yield break;


            var retVal = new List<DBObject>();

            foreach (var edge in edgeProp)
            {
                retVal.Add(edge as DBObject);
                //yield return edge as DBObject;
            }

            return retVal;

        }

        #endregion


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

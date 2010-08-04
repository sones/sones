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

namespace sones.GraphDS.API.CSharp.Reflection
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


        #region GetInsertValues(mySeperator)

        public override String GetInsertValues(String mySeperator)
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
                        {

                            _StringBuilder.Append(_PropertyInfo.Name).Append(" = ");

                            #region List<...>

                            if (_PropertyInfo.PropertyType.IsGenericType &&
                                _PropertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "System.Collections.Generic.List`1")
                            {
                                
                                _StringBuilder.Append(" LISTOF(");

                                var _List = _PropertyInfo.GetValue(this, null) as IEnumerable<Object>;
                                if (_List != null)
                                {

                                    foreach (var _Object in _List)
                                        _StringBuilder.Append("'").Append(_Object).Append("'").Append(mySeperator);

                                    _StringBuilder.Length = _StringBuilder.Length - 2;

                                }

                                _StringBuilder.Append(") ");
                                
                            }

                            #endregion

                            #region DBObject

                            else if (_PropertyInfo.PropertyType.BaseType.FullName == "sones.GraphDS.API.CSharp.Reflection.DBObject")
                            // ToDo: Improve basetype lookup!
                            {

                                var _DBObject = _PropertyInfo.GetValue(this, null) as DBObject;
                                if (_DBObject != null)
                                {
                                    _StringBuilder.Append("REF(UUID = '").Append(_DBObject.UUID).Append("')");
                                }

                            }

                            #endregion

                            #region Set<DBObject, DBObject>

                            else if (_PropertyInfo.PropertyType.IsGenericType &&
                                _PropertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "sones.GraphDS.API.CSharp.Reflection.Set`2")
                            {

                                _StringBuilder.Append(" SETOF(");

                                var _Set = _PropertyInfo.GetValue(this, null) as Set<DBObject, DBEdge>;
                                if (_Set != null)
                                {

                                    foreach (var _DBObject in _Set)
                                        _StringBuilder.Append("UUID = '").Append(_DBObject.UUID).Append("'").Append(mySeperator);

                                    _StringBuilder.Length = _StringBuilder.Length - 2;

                                }

                                _StringBuilder.Append(") ");

                            }

                            #endregion

                            #region Set<DBObject>

                            else if (_PropertyInfo.PropertyType.IsGenericType &&
                                     _PropertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "sones.GraphDS.API.CSharp.Reflection.Set`1" &&
                                     _PropertyInfo.PropertyType.GetGenericArguments()[0].BaseType.FullName == "sones.GraphDS.API.CSharp.Reflection.DBObject")
                                // ToDo: Improve basetype lookup!
                            {

                                _StringBuilder.Append(" SETOF(");

                                var _Set = _PropertyInfo.GetValue(this, null) as Set<DBObject>;
                                if (_Set != null)
                                {

                                    foreach (var _DBObject in _Set)
                                        _StringBuilder.Append("UUID = '").Append(_DBObject.UUID).Append("'").Append(mySeperator);

                                    _StringBuilder.Length = _StringBuilder.Length - 2;

                                }

                                _StringBuilder.Append(") ");

                            }

                            #endregion

                            #region Set<...>

                            else if (_PropertyInfo.PropertyType.IsGenericType &&
                                _PropertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "sones.GraphDS.API.CSharp.Reflection.Set`1")
                            {

                                _StringBuilder.Append(" SETOF(");

                                var _Set = _PropertyInfo.GetValue(this, null) as IEnumerable<Object>;
                                if (_Set != null)
                                {

                                    foreach (var _Object in _Set)
                                        _StringBuilder.Append("'").Append(_Object).Append("'").Append(mySeperator);

                                    _StringBuilder.Length = _StringBuilder.Length - 2;

                                }

                                _StringBuilder.Append(") ");

                            }

                            #endregion

                            #region Single value

                            else
                                _StringBuilder.Append("'").Append(_PropertyInfo.GetValue(this, null)).Append("'");

                            #endregion

                            _StringBuilder.Append(mySeperator);

                        }

                    }

                }

                _StringBuilder.Length = _StringBuilder.Length - mySeperator.Length;

            }

            return _StringBuilder.ToString();

        }

        #endregion

        #region ReflectMyself()

        public override void ReflectMyself()
        {

            #region Init StringBuilder

            var _Command        = new StringBuilder();
            var _Attributes     = new StringBuilder();
            TYPE = this.GetType().Name;

            _Command.Append("CREATE EDGE " + TYPE);
            _Command.Append(" EXTENDS " + this.GetType().BaseType.Name);

            #endregion

            #region Find Attributes

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

                            _Attributes.Append(_PropertyInfo.Name + ", ");

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

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

using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Structures;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDS.API.CSharp.Reflection
{

    /// <summary>
    /// The DBObject class for user-defined graph data base types
    /// </summary>

    public class DBVertex : DBObject, IEquatable<DBVertex>
    {


        #region Constructor(s)

        #region DBVertex()

        public DBVertex()
        {
            UUID        = null;
            Edition     = null;
            RevisionID  = null;
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

                            else if (_PropertyInfo.PropertyType.BaseType.FullName == "sones.GraphDS.API.CSharp.Reflection.DBVertex")
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
                                     _PropertyInfo.PropertyType.GetGenericArguments()[0].BaseType.FullName == "sones.GraphDS.API.CSharp.Reflection.DBVertex")
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
            var _BackwardEdges  = new StringBuilder();
            _CreateIndicesQueries.Clear();
            TYPE                = this.GetType().Name;

            _Command.Append("CREATE VERTEX " + TYPE);
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

            #region Add Comment

            _Command.Append(" Comment = '" + Comment + "'");

            #endregion

            _CreateTypeQuery = _Command.ToString();

        }

        #endregion


        #region Graph Operations

        #region Link()

        public Exceptional Link(DBVertex myDBVertex)
        {

            if (myDBVertex == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Link()

        public Exceptional Link(params DBVertex[] myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Link()

        public Exceptional Link(IEnumerable<DBVertex> myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region Unlink()

        public Exceptional Unlink(DBVertex myDBVertex)
        {

            if (myDBVertex == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Unlink()

        public Exceptional Unlink(params DBVertex[] myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Unlink()

        public Exceptional Unlink(IEnumerable<DBVertex> myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region GetNeighbors(myDBVertexQualifier = null, myDepth = 0)

        public IEnumerable<DBVertex> GetNeighbors(Func<DBVertex, Boolean> myDBVertexQualifier = null, UInt64 myDepth = 0)
        {

            if (myDBVertexQualifier == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region GetNeighborCount(myDBVertexQualifier = null, myDepth = 0)

        public UInt64 GetNeighborCount(Func<DBVertex, Boolean> myDBVertexQualifier = null, UInt64 myDepth = 0)
        {

            if (myDBVertexQualifier == null)
                // count all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region GetEdge(myEdgeName)

        public IEnumerable<DBObject> GetEdge(String myEdgeName)
        {

            //ToDo: Rethink me!

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

        #region GetEdges(myDBEdgeQualifier = null, myDepth = 0)

        public IEnumerable<DBEdge> GetEdges(Func<DBEdge, Boolean> myDBEdgeQualifier = null, UInt64 myDepth = 0)
        {

            if (myDBEdgeQualifier == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region GetEdgeCount(myDBEdgeQualifier = null, myDepth = 0)

        public UInt64 GetEdgeCount(Func<DBEdge, Boolean> myDBEdgeQualifier = null, UInt64 myDepth = 0)
        {

            if (myDBEdgeQualifier == null)
                // count all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region Traverse(...)

        /// <summary>
        /// Starts a traversal and returns the found paths or an aggreagted result
        /// </summary>
        /// <typeparam name="T">The resulttype after applying the result transformation</typeparam>
        /// <param name="TraversalOperation">BreathFirst|DepthFirst</param>
        /// <param name="myFollowThisEdge">Follow this edge? Based on its TYPE or any other property/characteristic...</param>
        /// <param name="myFollowThisPath">Follow this path (== actual path + NEW edge + NEW dbobject? Based on edge/object UUID, TYPE or any other property/characteristic...</param>
        /// <param name="myMatchEvaluator">Mhm, this vertex/path looks interesting!</param>
        /// <param name="myMatchAction">Hey! I have found something interesting!</param>
        /// <param name="myStopEvaluator">Will stop the traversal on a condition</param>
        /// <param name="myWhenFinished">Finish this traversal by calling (a result transformation method and) an external method...</param>
        /// <returns></returns>
        public T Traverse<T>(TraversalOperation                     TraversalOperation  = TraversalOperation.BreathFirst,
                             Func<Path, DBEdge, Boolean>            myFollowThisEdge    = null,
                             Func<Path, DBEdge, DBVertex, Boolean>  myFollowThisPath    = null,
                             Func<Path, Boolean>                    myMatchEvaluator    = null,
                             Action<Path>                           myMatchAction       = null,
                             Func<TraversalState, Boolean>          myStopEvaluator     = null,
                             Func<IEnumerable<Path>, T>             myWhenFinished      = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (myDBObject1, myDBVertex2)

        public static Boolean operator == (DBVertex myDBVertex1, DBVertex myDBVertex2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(myDBVertex1, myDBVertex2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myDBVertex1 == null) || ((Object) myDBVertex2 == null))
                return false;

            return myDBVertex1.Equals(myDBVertex2);

        }

        #endregion

        #region Operator != (myDBVertex1, myDBVertex2)

        public static Boolean operator != (DBVertex myDBVertex1, DBVertex myDBVertex2)
        {
            return !(myDBVertex1 == myDBVertex2);
        }

        #endregion

        #endregion

        #region IEquatable<DBObject> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject == null)
                return false;

            var _Object = myObject as DBVertex;
            if (_Object == null)
                return (Equals(_Object));

            return false;

        }

        #endregion

        #region Equals(myDBVertex)

        public Boolean Equals(DBVertex myDBVertex)
        {

            if ((object) myDBVertex == null)
            {
                return false;
            }

            //TODO: Here it might be good to check all attributes of the UNIQUE constraint!
            return (this.UUID == myDBVertex.UUID);

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

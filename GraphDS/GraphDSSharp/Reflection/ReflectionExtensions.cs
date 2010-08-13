/*
 * sones GraphDS API - ReflectionExtensions
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.NewAPI;
using sones.GraphDB.Structures.Result;

using sones.Lib;

#endregion

namespace sones.GraphDS.API.CSharp.Reflection
{


    public static class ReflectionExtensions
    {

        #region ReflectMyself(this myDBVertex)

        public static Tuple<String, List<String>> ReflectMyself(this DBVertex myDBVertex)
        {

            String _CreateTypeQuery = "";
            List<String> _CreateIndicesQueries = new List<String>();

            #region Init StringBuilder

            var _Command = new StringBuilder();
            var _Attributes = new StringBuilder();
            var _BackwardEdges = new StringBuilder();

      //      myDBVertex.TYPE = myDBVertex.GetType().Name;

            _Command.Append("CREATE VERTEX " + myDBVertex.GetType().Name);
            _Command.Append(" EXTENDS " + myDBVertex.GetType().BaseType.Name);

            #endregion

            #region Find Attributes and Backwardedges

            var _AllProperties = myDBVertex.GetType().GetProperties();

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
                        if (_PropertyInfo.DeclaringType.Name != myDBVertex.GetType().Name)
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

                                    _CreateAttributeIndex = "CREATE INDEX " + _IndexName + " ON VERTEX " + myDBVertex.GetType().Name + " (" + _PropertyInfo.Name + _IndexOrder + ")" + _IndexType;

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

                            //HACK: Refactor this! e.g. use a dictionary for C#->GraphDB type mappings
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

            _Command.Append(" Comment = '" + myDBVertex.Comment + "'");

            #endregion

            _CreateTypeQuery = _Command.ToString();

            return new Tuple<String, List<String>>(_CreateTypeQuery, _CreateIndicesQueries);

        }

        #endregion

        #region ReflectMyself(this myDBEdge)

        public static Tuple<String, List<String>> ReflectMyself(this DBEdge myDBEdge)
        {

            String _CreateTypeQuery = "";
            List<String> _CreateIndicesQueries = new List<String>();

            #region Init StringBuilder

            var _Command = new StringBuilder();
            var _Attributes = new StringBuilder();
            var _BackwardEdges = new StringBuilder();

            //      myDBVertex.TYPE = myDBVertex.GetType().Name;

            _Command.Append("CREATE EDGE " + myDBEdge.GetType().Name);
            _Command.Append(" EXTENDS " + myDBEdge.GetType().BaseType.Name);

            #endregion

            #region Find Attributes and Backwardedges

            var _AllProperties = myDBEdge.GetType().GetProperties();

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
                        if (_PropertyInfo.DeclaringType.Name != myDBEdge.GetType().Name)
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

                                    _CreateAttributeIndex = "CREATE INDEX " + _IndexName + " ON EDGE " + myDBEdge.GetType().Name + " (" + _PropertyInfo.Name + _IndexOrder + ")" + _IndexType;

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

                            //HACK: Refactor this! e.g. use a dictionary for C#->GraphDB type mappings
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

            #region Add Comment

            _Command.Append(" Comment = '" + myDBEdge.Comment + "'");

            #endregion

            _CreateTypeQuery = _Command.ToString();

            return new Tuple<String, List<String>>(_CreateTypeQuery, _CreateIndicesQueries);

        }

        #endregion


        #region GetInsertValues(mySeperator)

        public static String GetInsertValues(this DBVertex myDBVertex, String mySeperator)
        {

            var _StringBuilder = new StringBuilder();
            Object _PropertyValue = null;

            var _AllProperties = myDBVertex.GetType().GetProperties();

            if (_AllProperties.Length > 0)
            {

                foreach (var _PropertyInfo in _AllProperties)
                {

                    if (_PropertyInfo.CanRead && _PropertyInfo.CanWrite)
                    {

                        _PropertyValue = _PropertyInfo.GetValue(myDBVertex, null);

                        if (_PropertyValue != null)
                        {

                            _StringBuilder.Append(_PropertyInfo.Name).Append(" = ");

                            #region List<...>

                            if (_PropertyInfo.PropertyType.IsGenericType &&
                                _PropertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "System.Collections.Generic.List`1")
                            {

                                _StringBuilder.Append(" LISTOF(");

                                var _List = _PropertyInfo.GetValue(myDBVertex, null) as IEnumerable<Object>;
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

                            else if (_PropertyInfo.PropertyType.BaseType != null &&
                                     _PropertyInfo.PropertyType.BaseType.FullName == "sones.GraphDB.NewAPI.DBVertex")
                            // ToDo: Improve basetype lookup!
                            {

                                var _DBObject = _PropertyInfo.GetValue(myDBVertex, null) as DBVertex;
                                if (_DBObject != null)
                                {
                                    _StringBuilder.Append("REF(UUID = '").Append(_DBObject.UUID).Append("')");
                                }

                            }

                            #endregion

                            #region Set<DBObject, DBObject>

                            else if (_PropertyInfo.PropertyType.IsGenericType &&
                                _PropertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "sones.GraphDB.NewAPI.Set`2")
                            {

                                _StringBuilder.Append(" SETOF(");

                                var _Set = _PropertyInfo.GetValue(myDBVertex, null) as Set<DBVertex, DBEdge>;
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
                                     _PropertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "sones.GraphDB.NewAPI.Set`1" &&
                                     _PropertyInfo.PropertyType.GetGenericArguments()[0].BaseType.FullName == "sones.GraphDB.NewAPI.DBVertex")
                            // ToDo: Improve basetype lookup!
                            {

                                _StringBuilder.Append(" SETOF(");

                                var _Set = _PropertyInfo.GetValue(myDBVertex, null) as Set<DBVertex>;
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
                                _PropertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "sones.GraphDB.NewAPI.Set`1")
                            {

                                _StringBuilder.Append(" SETOF(");

                                var _Set = _PropertyInfo.GetValue(myDBVertex, null) as IEnumerable<Object>;
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
                                _StringBuilder.Append("'").Append(_PropertyInfo.GetValue(myDBVertex, null)).Append("'");

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


        #region Create(Types/Vertices/Edges)

        #region CreateTypes (DBVertices and -Edges)

        #region CreateTypes(this myAGraphDSSharp, params myType)

        /// <summary>
        /// Creates all given types extending DBObject, thus DBVertex and DBEdge.
        /// May be used within the GraphDSSharp reflection interface to create a
        /// DBVertex or DBEdge based on an annotated class.
        /// </summary>
        /// <param name="myAction"></param>
        /// <param name="myTypes"></param>
        /// <returns></returns>
        public static QueryResult CreateTypes(this AGraphDSSharp myAGraphDSSharp, params Type[] myTypes)
        {
            // ToDo: Allow to create DBEdges!
            var a = myTypes.Select(type => Activator.CreateInstance(type) as DBVertex).Where(dbvertex => dbvertex != null).ToArray();
            return myAGraphDSSharp.CreateVertices(a);
        }

        #endregion

        #region CreateTypes(this myAGraphDSSharp, myAction, params myType)

        public static QueryResult CreateTypes(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, params Type[] myTypes)
        {

            var _QueryResult = myAGraphDSSharp.CreateTypes(myTypes);

            myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region CreateTypes(this myAGraphDSSharp, mySuccessAction, myFailureAction, params myType)

        public static QueryResult CreateTypes(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params Type[] myTypes)
        {

            var _QueryResult = myAGraphDSSharp.CreateTypes(myTypes);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region CreateTypes(this myAGraphDSSharp, mySuccessAction, myPartialSuccessAction, myFailureAction, params myType)

        public static QueryResult CreateTypes(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params Type[] myTypes)
        {

            var _QueryResult = myAGraphDSSharp.CreateTypes(myTypes);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #endregion


        #region CreateVertex/-Vertices

        #region CreateVertices(this myAGraphDSSharp, params myDBVertices)

        public static QueryResult CreateVertices(this AGraphDSSharp myAGraphDSSharp, params DBVertex[] myDBVertices)
        {

            var _CreateTypesQuery      = new StringBuilder("CREATE VERTICES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBVertex in myDBVertices)
            {

                var _Reflection = _DBVertex.ReflectMyself();
                _CreateTypesQuery.Append(_Reflection.Item1.Replace("CREATE VERTEX ", "") + ", ");

                // CreateIndicesQueries
                if (_Reflection.Item2 != null)
                    _CreateIndiciesQueries.AddRange(_Reflection.Item2);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = myAGraphDSSharp.Query(_CreateTypesQuery.ToString());

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = myAGraphDSSharp.Query(_CreateIndexQuery);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateVertices(this myAGraphDSSharp, myAction, params myDBVertices)

        public static QueryResult CreateVertices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, params DBVertex[] myDBVertices)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE VERTICES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBVertex in myDBVertices)
            {

                var _Reflection = _DBVertex.ReflectMyself();
                _CreateTypesQuery.Append(_Reflection.Item1.Replace("CREATE VERTEX ", "") + ", ");

                // CreateIndicesQueries
                if (_Reflection.Item2 != null)
                    _CreateIndiciesQueries.AddRange(_Reflection.Item2);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = myAGraphDSSharp.Query(_CreateTypesQuery.ToString());

            myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = myAGraphDSSharp.Query(_CreateIndexQuery);

                myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateVertices(this myAGraphDSSharp, mySuccessAction, myFailureAction, params myDBVertices)

        public static QueryResult CreateVertices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params DBVertex[] myDBVertices)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE VERTICES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBVertex in myDBVertices)
            {

                var _Reflection = _DBVertex.ReflectMyself();
                _CreateTypesQuery.Append(_Reflection.Item1.Replace("CREATE VERTEX ", "") + ", ");

                // CreateIndicesQueries
                if (_Reflection.Item2 != null)
                    _CreateIndiciesQueries.AddRange(_Reflection.Item2);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = myAGraphDSSharp.Query(_CreateTypesQuery.ToString());

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = myAGraphDSSharp.Query(_CreateIndexQuery);

                myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateVertices(this myAGraphDSSharp, mySuccessAction, myPartialSuccessAction, myFailureAction, params myDBVertices)

        public static QueryResult CreateVertices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params DBVertex[] myDBVertices)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE VERTICES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBVertex in myDBVertices)
            {

                var _Reflection = _DBVertex.ReflectMyself();
                _CreateTypesQuery.Append(_Reflection.Item1.Replace("CREATE VERTEX ", "") + ", ");

                // CreateIndicesQueries
                if (_Reflection.Item2 != null)
                    _CreateIndiciesQueries.AddRange(_Reflection.Item2);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = myAGraphDSSharp.Query(_CreateTypesQuery.ToString());

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = myAGraphDSSharp.Query(_CreateIndexQuery);

                myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #endregion


        #region CreateEdge/-Edges

        #region CreateEdges(this myAGraphDSSharp, params myDBEdges)

        public static QueryResult CreateEdges(this AGraphDSSharp myAGraphDSSharp, params DBEdge[] myDBEdges)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE EDGES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBEdge in myDBEdges)
            {

                var _Reflection = _DBEdge.ReflectMyself();
                _CreateTypesQuery.Append(_Reflection.Item1.Replace("CREATE EDGE ", "") + ", ");

                // CreateIndicesQueries
                if (_Reflection.Item2 != null)
                    _CreateIndiciesQueries.AddRange(_Reflection.Item2);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = myAGraphDSSharp.Query(_CreateTypesQuery.ToString());

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = myAGraphDSSharp.Query(_CreateIndexQuery);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateEdges(this myAGraphDSSharp, myAction, params myDBEdges)

        public static QueryResult CreateEdges(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, params DBEdge[] myDBEdges)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE EDGES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBEdge in myDBEdges)
            {

                var _Reflection = _DBEdge.ReflectMyself();
                _CreateTypesQuery.Append(_Reflection.Item1.Replace("CREATE EDGE ", "") + ", ");

                // CreateIndicesQueries
                if (_Reflection.Item2 != null)
                    _CreateIndiciesQueries.AddRange(_Reflection.Item2);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = myAGraphDSSharp.Query(_CreateTypesQuery.ToString());

            myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = myAGraphDSSharp.Query(_CreateIndexQuery);

                myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateEdges(this myAGraphDSSharp, mySuccessAction, myFailureAction, params myDBEdges)

        public static QueryResult CreateEdges(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params DBEdge[] myDBEdges)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE EDGES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBEdge in myDBEdges)
            {

                var _Reflection = _DBEdge.ReflectMyself();
                _CreateTypesQuery.Append(_Reflection.Item1.Replace("CREATE EDGE ", "") + ", ");

                // CreateIndicesQueries
                if (_Reflection.Item2 != null)
                    _CreateIndiciesQueries.AddRange(_Reflection.Item2);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = myAGraphDSSharp.Query(_CreateTypesQuery.ToString());

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = myAGraphDSSharp.Query(_CreateIndexQuery);

                myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateEdges(this myAGraphDSSharp, mySuccessAction, myPartialSuccessAction, myFailureAction, params myDBEdges)

        public static QueryResult CreateEdges(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params DBVertex[] myDBEdges)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE EDGES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBEdge in myDBEdges)
            {

                var _Reflection = _DBEdge.ReflectMyself();
                _CreateTypesQuery.Append(_Reflection.Item1.Replace("CREATE EDGE ", "") + ", ");

                // CreateIndicesQueries
                if (_Reflection.Item2 != null)
                    _CreateIndiciesQueries.AddRange(_Reflection.Item2);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = myAGraphDSSharp.Query(_CreateTypesQuery.ToString());

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = myAGraphDSSharp.Query(_CreateIndexQuery);

                myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #endregion
        
        #endregion

    }

}

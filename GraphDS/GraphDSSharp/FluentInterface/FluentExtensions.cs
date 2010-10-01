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
 * GraphDSSharp - FluentExtensions
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.NewAPI;


using sones.Lib;
using sones.GraphDB.Result;
using System.Diagnostics;
using sones.GraphFS.DataStructures;
using System.Reflection;

#endregion

namespace sones.GraphDS.API.CSharp.Fluent
{

    /// <summary>
    /// Extension classes for the GraphDSSharp API to provide fluent queries
    /// </summary>
    public static class FluentExtensions
    {


        #region CreateVertex/-Vertices

        #region CreateVertex(this myAGraphDSSharp, myVertexTypeName, myIsAbstract)

        public static CreateVertexQuery CreateVertex(this AGraphDSSharp myAGraphDSSharp, String myVertexTypeName, Boolean myIsAbstract = false)
        {
            return new CreateVertexQuery(myAGraphDSSharp, myVertexTypeName, myIsAbstract);
        }

        #endregion


        #region CreateVertices(this myAGraphDSSharp, params myCreateVerticesQuery)

        public static QueryResult CreateVertices(this AGraphDSSharp myAGraphDSSharp, params CreateVertexQuery[] myCreateVerticesQueries)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE VERTICES ");
            String tmp = null;

            foreach (var CreateTypeQuery in myCreateVerticesQueries)
            {

                if (CreateTypeQuery._IsAbstract)
                {
                    tmp = " ABSTRACT ";
                    tmp += CreateTypeQuery.GetGQLQuery().Replace("CREATE ABSTRACT VERTEX ", "") + ", ";
                    _CreateTypesQuery.Append(tmp);
                }
                else
                {
                    _CreateTypesQuery.Append(CreateTypeQuery.GetGQLQuery().Replace("CREATE VERTEX ", "") + ", ");
                }

            }

            _CreateTypesQuery.RemoveEnding(2);

            return myAGraphDSSharp.Query(_CreateTypesQuery.ToString());

        }

        #endregion

        #region CreateVertices(this myAGraphDSSharp, myAction, params myCreateVerticesQueries)

        public static QueryResult CreateVertices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, params CreateVertexQuery[] myCreateVerticesQueries)
        {

            var _QueryResult = myAGraphDSSharp.CreateVertices(myCreateVerticesQueries);

            myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region CreateVertices(this myAGraphDSSharp, mySuccessAction, myFailureAction, params myCreateVerticesQueries)

        public static QueryResult CreateVertices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params CreateVertexQuery[] myCreateVerticesQueries)
        {

            var _QueryResult = myAGraphDSSharp.CreateVertices(myCreateVerticesQueries);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region CreateVertices(this myAGraphDSSharp, mySuccessAction, myPartialSuccessAction, myFailureAction, params myCreateVerticesQueries)

        public static QueryResult CreateVertices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params CreateVertexQuery[] myCreateVerticesQueries)
        {

            var _QueryResult = myAGraphDSSharp.CreateVertices(myCreateVerticesQueries);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #endregion

        #region AlterVertex/-Vertices

        #region AlterVertex(this myAGraphDSSharp, myVertexTypeName)

        public static AlterVertexQuery AlterVertex(this AGraphDSSharp myAGraphDSSharp, String myVertexTypeName)
        {
            return new AlterVertexQuery(myAGraphDSSharp, myVertexTypeName);
        }

        #endregion

        #region AlterVertex(this myAGraphDSSharp, myCreateVertexQuery)

        public static AlterVertexQuery AlterVertex(this AGraphDSSharp myAGraphDSSharp, CreateVertexQuery myCreateVertexQuery)
        {
            return new AlterVertexQuery(myAGraphDSSharp, myCreateVertexQuery.Name);
        }

        #endregion


        #region AlterVertices(this myAGraphDSSharp, params myAlterVerticesQuery)

        public static QueryResult AlterVertices(this AGraphDSSharp myAGraphDSSharp, params AlterVertexQuery[] myAlterVerticesQueries)
        {

            QueryResult _QueryResult = null;

            foreach (var _AlterVertexQuery in myAlterVerticesQueries)
            {

                _QueryResult = _AlterVertexQuery.Execute();

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region AlterVertices(this myAGraphDSSharp, myAction, params myAlterVerticesQueries)

        public static QueryResult AlterVertices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, params AlterVertexQuery[] myAlterVerticesQueries)
        {

            QueryResult _QueryResult = null;

            foreach (var _AlterVertexQuery in myAlterVerticesQueries)
            {

                _QueryResult = _AlterVertexQuery.Execute();

                myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region AlterVertices(this myAGraphDSSharp, mySuccessAction, myFailureAction, params myAlterVerticesQueries)

        public static QueryResult AlterVertices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params AlterVertexQuery[] myAlterVerticesQueries)
        {

            QueryResult _QueryResult = null;

            foreach (var _AlterVertexQuery in myAlterVerticesQueries)
            {

                _QueryResult = _AlterVertexQuery.Execute();

                myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region AlterVertices(this myAGraphDSSharp, mySuccessAction, myPartialSuccessAction, myFailureAction, params myAlterVerticesQueries)

        public static QueryResult AlterVertices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params AlterVertexQuery[] myAlterVerticesQueries)
        {

            QueryResult _QueryResult = null;

            foreach (var _AlterVertexQuery in myAlterVerticesQueries)
            {

                _QueryResult = _AlterVertexQuery.Execute();

                myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #endregion


        #region CreateEdge/-Edges

        #region CreateVertex(this myAGraphDSSharp, myVertexTypeName, hyperEdge = false, abstractEdge = false)

        public static CreateEdgeQuery CreateEdge(this AGraphDSSharp myAGraphDSSharp, String myVertexTypeName, Boolean hyperEdge = false, Boolean abstractEdge = false)
        {
            return new CreateEdgeQuery(myAGraphDSSharp, myVertexTypeName, hyperEdge, abstractEdge);
        }

        #endregion


        #region CreateEdges(this myAGraphDSSharp, params myCreateEdgesQuery)

        public static QueryResult CreateEdges(this AGraphDSSharp myAGraphDSSharp, params CreateEdgeQuery[] myCreateEdgesQueries)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE Edges ");
            String tmp = null;

            foreach (var CreateTypeQuery in myCreateEdgesQueries)
            {

                if (CreateTypeQuery.IsAbstract)
                {
                    tmp = " ABSTRACT ";
                    tmp += CreateTypeQuery.GetGQLQuery().Replace("CREATE ABSTRACT EDGE ", "") + ", ";
                    _CreateTypesQuery.Append(tmp);
                }
                else
                {
                    _CreateTypesQuery.Append(CreateTypeQuery.GetGQLQuery().Replace("CREATE EDGE ", "") + ", ");
                }

            }

            _CreateTypesQuery.RemoveEnding(2);

            return myAGraphDSSharp.Query(_CreateTypesQuery.ToString());

        }

        #endregion

        #region CreateEdges(this myAGraphDSSharp, myAction, params myCreateEdgesQueries)

        public static QueryResult CreateEdges(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, params CreateEdgeQuery[] myCreateEdgesQueries)
        {

            var _QueryResult = myAGraphDSSharp.CreateEdges(myCreateEdgesQueries);

            myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region CreateEdges(this myAGraphDSSharp, mySuccessAction, myFailureAction, params myCreateEdgesQueries)

        public static QueryResult CreateEdges(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params CreateEdgeQuery[] myCreateEdgesQueries)
        {

            var _QueryResult = myAGraphDSSharp.CreateEdges(myCreateEdgesQueries);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region CreateEdges(this myAGraphDSSharp, mySuccessAction, myPartialSuccessAction, myFailureAction, params myCreateEdgesQueries)

        public static QueryResult CreateEdges(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params CreateEdgeQuery[] myCreateEdgesQueries)
        {

            var _QueryResult = myAGraphDSSharp.CreateEdges(myCreateEdgesQueries);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #endregion

        #region AlterEdge/-Edges
        #endregion


        #region CreateIndex/-Indices

        #region CreateIndex(this myAGraphDSSharp, myIndexName)

        /// <summary>
        /// Creates an index. If the name is null it will be later auto-generated!
        /// </summary>
        /// <param name="myIndexName">The name of the index</param>
        /// <returns>A CreateIndexQuery object</returns>
        public static CreateIndexQuery CreateIndex(this AGraphDSSharp myAGraphDSSharp, String myIndexName = null)
        {
            return new CreateIndexQuery(myAGraphDSSharp, myIndexName);
        }

        #endregion

        #region CreateIndex(this myAGraphDSSharp, myCreateVertexQuery)

        public static CreateIndexQuery CreateIndex(this AGraphDSSharp myAGraphDSSharp, CreateVertexQuery myCreateVertexQuery)
        {
            return new CreateIndexQuery(myAGraphDSSharp, myCreateVertexQuery.Name);
        }

        #endregion


        #region CreateIndices(this myAGraphDSSharp, params myCreateIndexQueries)

        public static QueryResult CreateIndices(this AGraphDSSharp myAGraphDSSharp, params CreateIndexQuery[] myCreateIndexQueries)
        {

            // myCreateVertexQuery.Name will never be null or its size zero!
            Debug.Assert(myCreateIndexQueries != null || myCreateIndexQueries.Length == 0);

            QueryResult _QueryResult = null;

            foreach (var _CreateIndexQuery in myCreateIndexQueries)
            {

                _QueryResult = myAGraphDSSharp.Query(_CreateIndexQuery.GetGQLQuery());

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateIndices(this myAGraphDSSharp, myAction, params myCreateIndexQueries)

        public static QueryResult CreateIndices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, params CreateIndexQuery[] myCreateIndexQueries)
        {

            // myCreateVertexQuery.Name will never be null or its size zero!
            Debug.Assert(myCreateIndexQueries != null || myCreateIndexQueries.Length == 0);

            QueryResult _QueryResult = null;

            foreach (var _CreateIndexQuery in myCreateIndexQueries)
            {

                _QueryResult = _CreateIndexQuery.Execute();

                myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateIndices(this myAGraphDSSharp, mySuccessAction, myFailureAction, params myCreateIndexQueries)

        public static QueryResult CreateIndices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params CreateIndexQuery[] myCreateIndexQueries)
        {

            // myCreateVertexQuery.Name will never be null or its size zero!
            Debug.Assert(myCreateIndexQueries != null || myCreateIndexQueries.Length == 0);

            QueryResult _QueryResult = null;

            foreach (var _CreateIndexQuery in myCreateIndexQueries)
            {

                _QueryResult = _CreateIndexQuery.Execute();

                myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateIndices(this myAGraphDSSharp, mySuccessAction, myPartialSuccessAction, myFailureAction, params myCreateIndexQueries)

        public static QueryResult CreateIndices(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params CreateIndexQuery[] myCreateIndexQueries)
        {

            // myCreateVertexQuery.Name will never be null or its size zero!
            Debug.Assert(myCreateIndexQueries != null || myCreateIndexQueries.Length == 0);

            QueryResult _QueryResult = null;

            foreach (var _CreateIndexQuery in myCreateIndexQueries)
            {

                _QueryResult = _CreateIndexQuery.Execute();

                myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #endregion


        #region Insert(this myAGraphDSSharp, myAction, myGraphDBType, myAnonymousClass)

        public static QueryResult Insert<T>(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, CreateVertexQuery myGraphDBType, T myAnonymousClass) where T : class
        {

            var _StringBuilder = new StringBuilder("INSERT INTO ").Append(myGraphDBType.Name).Append(" VALUES (");

            Object              _AttributeValue;
            String              _StringValue;
            IEnumerable<Object> _ListValue;
            Boolean             _HideFromDatabase = false;


            foreach (var _AnonymousProperty in myAnonymousClass.GetType().GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance))
            {

                #region Check HideFromDatabase attribute

                _HideFromDatabase = false;
                foreach (var _Attribute in _AnonymousProperty.GetCustomAttributes(true))
                {
                    if ((_Attribute as HideFromDatabase) != null)
                        _HideFromDatabase = true;
                }

                #endregion

                if (_HideFromDatabase == false)
                {

                    _AttributeValue = _AnonymousProperty.GetValue(myAnonymousClass, null);

                    if (_AttributeValue != null)
                    {

                        #region String value

                        _StringValue = _AttributeValue as String;

                        if (_StringValue != null)
                        {
                            _StringValue = "'" + _StringValue + "'";
                            _StringBuilder.Append(_AnonymousProperty.Name).Append(" = ").Append(_StringValue).Append(", ");
                            continue;
                        }

                        #endregion

                        #region List Attributes

                        _ListValue = _AttributeValue as IEnumerable<Object>;

                        if (_ListValue != null && _ListValue.Count() > 0)
                        {

                            _StringBuilder.Append(_AnonymousProperty.Name).Append(" = LISTOF(");

                            foreach (var _Item in _ListValue)
                                _StringBuilder.Append("'").Append(_Item.ToString()).Append("', ");

                            _StringBuilder.Length = _StringBuilder.Length - 2;

                            _StringBuilder.Append("), ");

                            continue;

                        }

                        #endregion

                        _StringBuilder.Append(_AnonymousProperty.Name).Append(" = ").Append(_AttributeValue).Append(", ");

                    }

                }

            }

            _StringBuilder.Length = _StringBuilder.Length - 2;

            _StringBuilder.Append(")");

            return myAGraphDSSharp.Query(_StringBuilder.ToString(), myAction);

        }

        #endregion


        #region Link

        #region Link(this myAGraphDSSharp, mySubject, myCreateEdgeQuery, params myObjects)

        public static QueryResult Link(this AGraphDSSharp myAGraphDSSharp, Vertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params Vertex[] myObjects)
        {
            return new QueryResult();
        }

        #endregion

        #region Link(this myAGraphDSSharp, myAction, mySubject, myCreateEdgeQuery, params myObjects)

        public static QueryResult Link(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, Vertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Link(mySubject, myCreateEdgeQuery, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region Link(this myAGraphDSSharp, mySuccessAction, myFailureAction, mySubject, myCreateEdgeQuery, params myObjects)

        public static QueryResult Link(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, Vertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Link(mySubject, myCreateEdgeQuery, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region Link(this myAGraphDSSharp, mySuccessAction, myPartialSuccessAction, myFailureAction, mySubject, myCreateEdgeQuery, params myObjects)

        public static QueryResult Link(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, Vertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Link(mySubject, myCreateEdgeQuery, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion


        #region Link(this myAGraphDSSharp, mySubject, myEdge, params myObjects)

        public static QueryResult Link(this AGraphDSSharp myAGraphDSSharp, Vertex mySubject, Edge myEdge, params Vertex[] myObjects)
        {
            return new QueryResult();
        }

        #endregion

        #region Link(this myAGraphDSSharp, myAction, mySubject, myEdge, params myObjects)

        public static QueryResult Link(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, Vertex mySubject, Edge myEdge, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Link(mySubject, myEdge, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region Link(this myAGraphDSSharp, mySuccessAction, myFailureAction, mySubject, myEdge, params myObjects)

        public static QueryResult Link(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, Vertex mySubject, Edge myEdge, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Link(mySubject, myEdge, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region Link(this myAGraphDSSharp, mySuccessAction, myPartialSuccessAction, myFailureAction, mySubject, myEdge, params myObjects)

        public static QueryResult Link(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, Vertex mySubject, Edge myEdge, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Link(mySubject, myEdge, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #endregion

        #region Unlink

        #region Unlink(this myAGraphDSSharp, mySubject, myCreateEdgeQuery, params myObjects)

        public static QueryResult Unlink(this AGraphDSSharp myAGraphDSSharp, Vertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params Vertex[] myObjects)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Unlink(this myAGraphDSSharp, myAction, mySubject, myCreateEdgeQuery, params myObjects)

        public static QueryResult Unlink(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, Vertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Unlink(mySubject, myCreateEdgeQuery, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region Unlink(this myAGraphDSSharp, mySuccessAction, myFailureAction, mySubject, myCreateEdgeQuery, params myObjects)

        public static QueryResult Unlink(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, Vertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Unlink(mySubject, myCreateEdgeQuery, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region Unlink(this myAGraphDSSharp, mySuccessAction, myPartialSuccessAction, myFailureAction, mySubject, myCreateEdgeQuery, params myObjects)

        public static QueryResult Unlink(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, Vertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Unlink(mySubject, myCreateEdgeQuery, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion


        #region Unlink(this myAGraphDSSharp, mySubject, myEdge, params myObjects)

        public static QueryResult Unlink(this AGraphDSSharp myAGraphDSSharp, Vertex mySubject, Edge myEdge, params Vertex[] myObjects)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Unlink(this myAGraphDSSharp, myAction, mySubject, myEdge, params myObjects)

        public static QueryResult Unlink(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> myAction, Vertex mySubject, Edge myEdge, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Unlink(mySubject, myEdge, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region Unlink(this myAGraphDSSharp, mySuccessAction, myFailureAction, mySubject, myEdge, params myObjects)

        public static QueryResult Unlink(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, Vertex mySubject, Edge myEdge, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Unlink(mySubject, myEdge, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region Unlink(this myAGraphDSSharp, mySuccessAction, myPartialSuccessAction, myFailureAction, mySubject, myEdge, params myObjects)

        public static QueryResult Unlink(this AGraphDSSharp myAGraphDSSharp, Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, Vertex mySubject, Edge myEdge, params Vertex[] myObjects)
        {

            var _QueryResult = myAGraphDSSharp.Unlink(mySubject, myEdge, myObjects);

            myAGraphDSSharp.QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #endregion


        #region Query(this myAGraphDSSharp)

        public static SelectQuery Query(this AGraphDSSharp myAGraphDSSharp)
        {
            return new SelectQuery(myAGraphDSSharp);
        }

        #endregion

    }

}

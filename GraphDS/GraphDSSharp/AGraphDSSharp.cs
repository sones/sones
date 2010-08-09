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
 * AGraphDSSharp
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using sones.GraphFS.Transactions;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Transactions;
using sones.GraphDB.Structures;
using sones.GraphDS.API.CSharp.Reflection;
using sones.Lib;
using sones.GraphDS.API.CSharp.Linq;
using sones.GraphFS.Events;

#endregion

namespace sones.GraphDS.API.CSharp
{

    public abstract class AGraphDSSharp
    {


        #region (protected) QueryResultAction(myQueryResult, myAction, mySuccessAction, myPartialSuccessAction, myFailureAction)

        protected void QueryResultAction(QueryResult myQueryResult, Action<QueryResult> myAction = null, Action<QueryResult> mySuccessAction = null, Action<QueryResult> myPartialSuccessAction = null, Action<QueryResult> myFailureAction = null)
        {

            if (mySuccessAction         != null && myQueryResult.Success)
            {
                mySuccessAction(myQueryResult);
                return;
            }

            if (myPartialSuccessAction  != null && myQueryResult.PartialSuccess)
            {
                myPartialSuccessAction(myQueryResult);
                return;
            }

            if (myFailureAction         != null && myQueryResult.Failed)
            {
                myFailureAction(myQueryResult);
                return;
            }

            if (myAction                != null)
            {
                myAction(myQueryResult);
                return;
            }

        }

        #endregion


        #region Query(...)

        #region Query()

        public SelectQuery Query()
        {
            return new SelectQuery(this);
        }

        #endregion

        #region Query(myQuery, myAction = null, mySuccessAction = null, myPartialSuccessAction = null, myFailureAction = null)

        /// <summary>
        /// This will execute a usual query on the current GraphDBSharp implementation.
        /// Be aware that under some circumstances (e.g. REST) you will not get a valid result!
        /// </summary>
        /// <param name="myQuery"></param>
        /// <returns></returns>
        public abstract QueryResult Query(String myQuery, Action<QueryResult> myAction = null, Action<QueryResult> mySuccessAction = null, Action<QueryResult> myPartialSuccessAction = null, Action<QueryResult> myFailureAction = null);

        #endregion

        #region Query<T>(myQuery, myAction = null, mySuccessAction = null, myPartialSuccessAction = null, myFailureAction = null)

        public IEnumerable<T> Query<T>(String myQuery, Action<QueryResult> myAction = null, Action<QueryResult> mySuccessAction = null, Action<QueryResult> myPartialSuccessAction = null, Action<QueryResult> myFailureAction = null)
            where T : DBObject, new()
        {
            return new SelectToObjectGraph(Query(myQuery, myAction, mySuccessAction, myPartialSuccessAction, myFailureAction)).ToVertexType<T>();
        }

        #endregion


        #region QuerySelect(myQuery)

        [Obsolete]
        public abstract SelectToObjectGraph QuerySelect(String myQuery);

        #endregion

        #endregion



        #region CreateTypes (DBVertices and DBEdges)

        #region CreateTypes(params myType)

        /// <summary>
        /// Creates all given types extending DBObject, thus DBVertex and DBEdge.
        /// May be used within the GraphDSSharp reflection interface to create a
        /// DBVertex or DBEdge based on an annotated class.
        /// </summary>
        /// <param name="myAction"></param>
        /// <param name="myTypes"></param>
        /// <returns></returns>
        public QueryResult CreateTypes(params Type[] myTypes)
        {
            // ToDo: Allow to create DBEdges!
            var a = myTypes.Select(type => Activator.CreateInstance(type) as DBVertex).Where(dbvertex => dbvertex != null).ToArray();
            return CreateVertices(a);
        }

        #endregion

        #region CreateTypes(myAction, params myType)

        public QueryResult CreateTypes(Action<QueryResult> myAction, params Type[] myTypes)
        {

            var _QueryResult = CreateTypes(myTypes);

            QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region CreateTypes(mySuccessAction, myFailureAction, params myType)

        public QueryResult CreateTypes(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params Type[] myTypes)
        {

            var _QueryResult = CreateTypes(myTypes);

            QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region CreateTypes(mySuccessAction, myPartialSuccessAction, myFailureAction, params myType)

        public QueryResult CreateTypes(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params Type[] myTypes)
        {

            var _QueryResult = CreateTypes(myTypes);

            QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #endregion


        #region CreateVertex/-Vertices

        #region CreateVertex(myVertexTypeName, myIsAbstract)

        public CreateVertexQuery CreateVertex(String myVertexTypeName, Boolean myIsAbstract = false)
        {
            return new CreateVertexQuery(this, myVertexTypeName, myIsAbstract);
        }

        #endregion


        #region CreateVertices(params myDBVertices)

        public QueryResult CreateVertices(params DBVertex[] myDBVertices)
        {

            var _CreateTypesQuery       = new StringBuilder("CREATE VERTICES ");
            var _CreateIndiciesQueries  = new List<String>();

            foreach (var _DBVertex in myDBVertices)
            {

                _CreateTypesQuery.Append(_DBVertex.CreateTypeQuery.Replace("CREATE VERTEX ", "") + ", ");

                if (_DBVertex.CreateIndicesQueries != null)
                    _CreateIndiciesQueries.AddRange(_DBVertex.CreateIndicesQueries);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = Query(_CreateTypesQuery.ToString());

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = Query(_CreateIndexQuery);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateVertices(myAction, params myDBVertices)

        public QueryResult CreateVertices(Action<QueryResult> myAction, params DBVertex[] myDBVertices)
        {

            var _CreateTypesQuery       = new StringBuilder("CREATE VERTICES ");
            var _CreateIndiciesQueries  = new List<String>();

            foreach (var _DBVertex in myDBVertices)
            {

                _CreateTypesQuery.Append(_DBVertex.CreateTypeQuery.Replace("CREATE VERTEX ", "") + ", ");

                if (_DBVertex.CreateIndicesQueries != null)
                    _CreateIndiciesQueries.AddRange(_DBVertex.CreateIndicesQueries);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = Query(_CreateTypesQuery.ToString());

            QueryResultAction(_QueryResult, myAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = Query(_CreateIndexQuery);

                QueryResultAction(_QueryResult, myAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateVertices(mySuccessAction, myFailureAction, params myDBVertices)

        public QueryResult CreateVertices(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params DBVertex[] myDBVertices)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE VERTICES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBVertex in myDBVertices)
            {

                _CreateTypesQuery.Append(_DBVertex.CreateTypeQuery.Replace("CREATE VERTEX ", "") + ", ");

                if (_DBVertex.CreateIndicesQueries != null)
                    _CreateIndiciesQueries.AddRange(_DBVertex.CreateIndicesQueries);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = Query(_CreateTypesQuery.ToString());

            QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = Query(_CreateIndexQuery);

                QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateVertices(mySuccessAction, myPartialSuccessAction, myFailureAction, params myDBVertices)

        public QueryResult CreateVertices(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params DBVertex[] myDBVertices)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE VERTICES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBVertex in myDBVertices)
            {

                _CreateTypesQuery.Append(_DBVertex.CreateTypeQuery.Replace("CREATE VERTEX ", "") + ", ");

                if (_DBVertex.CreateIndicesQueries != null)
                    _CreateIndiciesQueries.AddRange(_DBVertex.CreateIndicesQueries);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = Query(_CreateTypesQuery.ToString());

            QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = Query(_CreateIndexQuery);

                QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion


        #region CreateVertices(params myCreateVerticesQuery)

        public QueryResult CreateVertices(params CreateVertexQuery[] myCreateVerticesQueries)
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

            return Query(_CreateTypesQuery.ToString());

        }

        #endregion

        #region CreateVertices(myAction, params myCreateVerticesQueries)

        public QueryResult CreateVertices(Action<QueryResult> myAction, params CreateVertexQuery[] myCreateVerticesQueries)
        {

            var _QueryResult = CreateVertices(myCreateVerticesQueries);

            QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region CreateVertices(mySuccessAction, myFailureAction, params myCreateVerticesQueries)

        public QueryResult CreateVertices(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params CreateVertexQuery[] myCreateVerticesQueries)
        {

            var _QueryResult = CreateVertices(myCreateVerticesQueries);

            QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region CreateVertices(mySuccessAction, myPartialSuccessAction, myFailureAction, params myCreateVerticesQueries)

        public QueryResult CreateVertices(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params CreateVertexQuery[] myCreateVerticesQueries)
        {

            var _QueryResult = CreateVertices(myCreateVerticesQueries);

            QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #endregion

        #region AlterVertex/-Vertices

        #region AlterVertex(myVertexTypeName)

        public AlterVertexQuery AlterVertex(String myVertexTypeName)
        {
            return new AlterVertexQuery(this, myVertexTypeName);
        }

        #endregion

        #region AlterVertex(myCreateVertexQuery)

        public AlterVertexQuery AlterVertex(CreateVertexQuery myCreateVertexQuery)
        {
            return new AlterVertexQuery(this, myCreateVertexQuery.Name);
        }

        #endregion


        #region AlterVertices(params myAlterVerticesQuery)

        public QueryResult AlterVertices(params AlterVertexQuery[] myAlterVerticesQueries)
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

        #region AlterVertices(myAction, params myAlterVerticesQueries)

        public QueryResult AlterVertices(Action<QueryResult> myAction, params AlterVertexQuery[] myAlterVerticesQueries)
        {

            QueryResult _QueryResult = null;

            foreach (var _AlterVertexQuery in myAlterVerticesQueries)
            {

                _QueryResult = _AlterVertexQuery.Execute();

                QueryResultAction(_QueryResult, myAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region AlterVertices(mySuccessAction, myFailureAction, params myAlterVerticesQueries)

        public QueryResult AlterVertices(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params AlterVertexQuery[] myAlterVerticesQueries)
        {

            QueryResult _QueryResult = null;

            foreach (var _AlterVertexQuery in myAlterVerticesQueries)
            {

                _QueryResult = _AlterVertexQuery.Execute();

                QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region AlterVertices(mySuccessAction, myPartialSuccessAction, myFailureAction, params myAlterVerticesQueries)

        public QueryResult AlterVertices(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params AlterVertexQuery[] myAlterVerticesQueries)
        {

            QueryResult _QueryResult = null;

            foreach (var _AlterVertexQuery in myAlterVerticesQueries)
            {

                _QueryResult = _AlterVertexQuery.Execute();

                QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #endregion


        #region CreateEdge/-Edges

        #region CreateVertex(myVertexTypeName, hyperEdge = false, abstractEdge = false)

        public CreateEdgeQuery CreateEdge(String myVertexTypeName, Boolean hyperEdge = false, Boolean abstractEdge = false)
        {
            return new CreateEdgeQuery(this, myVertexTypeName, hyperEdge, abstractEdge);
        }

        #endregion


        #region CreateEdges(params myDBEdges)

        public QueryResult CreateEdges(params DBEdge[] myDBEdges)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE EDGES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBEdge in myDBEdges)
            {

                _CreateTypesQuery.Append(_DBEdge.CreateTypeQuery.Replace("CREATE EDGE ", "") + ", ");

                if (_DBEdge.CreateIndicesQueries != null)
                    _CreateIndiciesQueries.AddRange(_DBEdge.CreateIndicesQueries);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = Query(_CreateTypesQuery.ToString());

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = Query(_CreateIndexQuery);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateEdges(myAction, params myDBEdges)

        public QueryResult CreateEdges(Action<QueryResult> myAction, params DBEdge[] myDBEdges)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE EDGES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBVertex in myDBEdges)
            {

                _CreateTypesQuery.Append(_DBVertex.CreateTypeQuery.Replace("CREATE EDGE ", "") + ", ");

                if (_DBVertex.CreateIndicesQueries != null)
                    _CreateIndiciesQueries.AddRange(_DBVertex.CreateIndicesQueries);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = Query(_CreateTypesQuery.ToString());

            QueryResultAction(_QueryResult, myAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = Query(_CreateIndexQuery);

                QueryResultAction(_QueryResult, myAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateEdges(mySuccessAction, myFailureAction, params myDBEdges)

        public QueryResult CreateEdges(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params DBEdge[] myDBEdges)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE EDGES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBVertex in myDBEdges)
            {

                _CreateTypesQuery.Append(_DBVertex.CreateTypeQuery.Replace("CREATE EDGE ", "") + ", ");

                if (_DBVertex.CreateIndicesQueries != null)
                    _CreateIndiciesQueries.AddRange(_DBVertex.CreateIndicesQueries);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = Query(_CreateTypesQuery.ToString());

            QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = Query(_CreateIndexQuery);

                QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateEdges(mySuccessAction, myPartialSuccessAction, myFailureAction, params myDBEdges)

        public QueryResult CreateEdges(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params DBVertex[] myDBEdges)
        {

            var _CreateTypesQuery = new StringBuilder("CREATE EDGES ");
            var _CreateIndiciesQueries = new List<String>();

            foreach (var _DBVertex in myDBEdges)
            {

                _CreateTypesQuery.Append(_DBVertex.CreateTypeQuery.Replace("CREATE EDGE ", "") + ", ");

                if (_DBVertex.CreateIndicesQueries != null)
                    _CreateIndiciesQueries.AddRange(_DBVertex.CreateIndicesQueries);

            }

            _CreateTypesQuery.RemoveEnding(2);

            var _QueryResult = Query(_CreateTypesQuery.ToString());

            QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            if (_QueryResult.Failed)
                return _QueryResult;

            foreach (var _CreateIndexQuery in _CreateIndiciesQueries)
            {

                _QueryResult = Query(_CreateIndexQuery);

                QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion


        #region CreateEdges(params myCreateEdgesQuery)

        public QueryResult CreateEdges(params CreateEdgeQuery[] myCreateEdgesQueries)
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

            return Query(_CreateTypesQuery.ToString());

        }

        #endregion

        #region CreateEdges(myAction, params myCreateEdgesQueries)

        public QueryResult CreateEdges(Action<QueryResult> myAction, params CreateEdgeQuery[] myCreateEdgesQueries)
        {

            var _QueryResult = CreateEdges(myCreateEdgesQueries);

            QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region CreateEdges(mySuccessAction, myFailureAction, params myCreateEdgesQueries)

        public QueryResult CreateEdges(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params CreateEdgeQuery[] myCreateEdgesQueries)
        {

            var _QueryResult = CreateEdges(myCreateEdgesQueries);

            QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region CreateEdges(mySuccessAction, myPartialSuccessAction, myFailureAction, params myCreateEdgesQueries)

        public QueryResult CreateEdges(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params CreateEdgeQuery[] myCreateEdgesQueries)
        {

            var _QueryResult = CreateEdges(myCreateEdgesQueries);

            QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #endregion

        #region AlterEdge/-Edges
        #endregion


        #region CreateIndex/-Indices

        #region CreateIndex(myIndexName)

        /// <summary>
        /// Creates an index. If the name is null it will be later auto-generated!
        /// </summary>
        /// <param name="myIndexName">The name of the index</param>
        /// <returns>A CreateIndexQuery object</returns>
        public CreateIndexQuery CreateIndex(String myIndexName = null)
        {
            return new CreateIndexQuery(this, myIndexName);
        }

        #endregion

        #region CreateIndex(myCreateVertexQuery)

        public CreateIndexQuery CreateIndex(CreateVertexQuery myCreateVertexQuery)
        {
            return new CreateIndexQuery(this, myCreateVertexQuery.Name);
        }

        #endregion


        #region CreateIndices(params myCreateIndexQueries)

        public QueryResult CreateIndices(params CreateIndexQuery[] myCreateIndexQueries)
        {

            // myCreateVertexQuery.Name will never be null or its size zero!
            Debug.Assert(myCreateIndexQueries != null || myCreateIndexQueries.Length == 0);

            QueryResult _QueryResult = null;

            foreach (var _CreateIndexQuery in myCreateIndexQueries)
            {

                _QueryResult = Query(_CreateIndexQuery.GetGQLQuery());

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateIndices(myAction, params myCreateIndexQueries)

        public QueryResult CreateIndices(Action<QueryResult> myAction, params CreateIndexQuery[] myCreateIndexQueries)
        {

            // myCreateVertexQuery.Name will never be null or its size zero!
            Debug.Assert(myCreateIndexQueries != null || myCreateIndexQueries.Length == 0);

            QueryResult _QueryResult = null;

            foreach (var _CreateIndexQuery in myCreateIndexQueries)
            {

                _QueryResult = _CreateIndexQuery.Execute();

                QueryResultAction(_QueryResult, myAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateIndices(mySuccessAction, myFailureAction, params myCreateIndexQueries)

        public QueryResult CreateIndices(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, params CreateIndexQuery[] myCreateIndexQueries)
        {

            // myCreateVertexQuery.Name will never be null or its size zero!
            Debug.Assert(myCreateIndexQueries != null || myCreateIndexQueries.Length == 0);

            QueryResult _QueryResult = null;

            foreach (var _CreateIndexQuery in myCreateIndexQueries)
            {

                _QueryResult = _CreateIndexQuery.Execute();

                QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #region CreateIndices(mySuccessAction, myPartialSuccessAction, myFailureAction, params myCreateIndexQueries)

        public QueryResult CreateIndices(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, params CreateIndexQuery[] myCreateIndexQueries)
        {

            // myCreateVertexQuery.Name will never be null or its size zero!
            Debug.Assert(myCreateIndexQueries != null || myCreateIndexQueries.Length == 0);

            QueryResult _QueryResult = null;

            foreach (var _CreateIndexQuery in myCreateIndexQueries)
            {

                _QueryResult = _CreateIndexQuery.Execute();

                QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

                if (_QueryResult.Failed)
                    return _QueryResult;

            }

            return _QueryResult;

        }

        #endregion

        #endregion


        #region Insert(...)

        #region (protected) SetReturnValues(myDBObjectOfT, myQueryResult)

        protected void SetReturnValues(DBObject myDBObjectOfT, QueryResult myQueryResult)
        {

            Debug.Assert(myQueryResult["UUID"]      != null);
            //Debug.Assert(myQueryResult["EDITION"]   != null);
            Debug.Assert(myQueryResult["REVISION"]  != null);

            Debug.Assert(myQueryResult["UUID"]      as ObjectUUID != null);
            //Debug.Assert(myQueryResult["EDITION"]       as String != null);
            //Debug.Assert(myQueryResult["REVISION"]      as RevisionID != null); //ToDo: REVISION is String NOT RevisionID!!!

            myDBObjectOfT.UUID          = myQueryResult["UUID"]     as ObjectUUID;
            myDBObjectOfT.Edition       = myQueryResult["EDITION"]  as String;
            myDBObjectOfT.RevisionID    = myQueryResult["REVISION"] as ObjectRevisionID;

        }

        #endregion

        #region Insert(myAction, myDBObjects)

        public DBObject[] Insert(Action<QueryResult> myAction, params DBObject[] myDBObjects)
        {

            if (myDBObjects == null)
                throw new ArgumentNullException();

            if (!myDBObjects.Any())
                throw new ArgumentException();

            QueryResult _QueryResult = null;

            if (myDBObjects != null)
            {
                foreach (var _DBObject in myDBObjects)
                {

                    _QueryResult = Query("INSERT INTO " + _DBObject.GetType().Name + " VALUES (" + _DBObject.GetInsertValues(", ") + ")", myAction);

                    if (_QueryResult.ResultType != ResultType.Failed)
                        SetReturnValues(_DBObject, _QueryResult);

                }
            }

            return myDBObjects;

        }

        #endregion 

        #region Insert<T>(myAction, myDBObjectOfT)

        public T Insert<T>(Action<QueryResult> myAction, T myDBObjectOfT) where T : DBObject
        {

            if (myDBObjectOfT == null)
                throw new ArgumentNullException();

            var _QueryResult = Query("INSERT INTO " + typeof(T).Name + " VALUES (" + myDBObjectOfT.GetInsertValues(", ") + ")", myAction);

            if (_QueryResult.ResultType != ResultType.Failed)
                SetReturnValues(myDBObjectOfT, _QueryResult);

            return myDBObjectOfT;

        }

        #endregion

        #region Insert<T>(myAction, myDBObjectsOfT)

        public T[] Insert<T>(Action<QueryResult> myAction, params T[] myDBObjectsOfT) where T : DBObject
        {

            if (myDBObjectsOfT == null)
                throw new ArgumentNullException();

            if (!myDBObjectsOfT.Any())
                throw new ArgumentException();

            QueryResult _QueryResult = null;

            if (myDBObjectsOfT != null)
            {

                foreach (var _DBObject in myDBObjectsOfT)
                {

                    _QueryResult = Query("INSERT INTO " + typeof(T).Name + " VALUES (" + _DBObject.GetInsertValues(", ") + ")", myAction);

                    if (_QueryResult.ResultType != ResultType.Failed)
                        SetReturnValues(_DBObject, _QueryResult);

                }

            }

            return myDBObjectsOfT;

        }

        #endregion

        #region Insert(myAction, myGraphDBType, myAnonymousClass)

        public QueryResult Insert<T>(Action<QueryResult> myAction, CreateVertexQuery myGraphDBType, T myAnonymousClass) where T : class
        {

            var                 _StringBuilder      = new StringBuilder("INSERT INTO ").Append(myGraphDBType.Name).Append(" VALUES (");
            Object              _AttributeValue;
            String              _StringValue;
            IEnumerable<Object> _ListValue;
            Boolean             _HideFromDatabase   = false;


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

            return Query(_StringBuilder.ToString(), myAction);

        }

        #endregion

        #endregion


        #region Link

        #region Link(mySubject, myCreateEdgeQuery, params myObjects)

        public QueryResult Link(DBVertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params DBVertex[] myObjects)
        {
            return new QueryResult();
        }

        #endregion

        #region Link(myAction, mySubject, myCreateEdgeQuery, params myObjects)

        public QueryResult Link(Action<QueryResult> myAction, DBVertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params DBVertex[] myObjects)
        {

            var _QueryResult = Link(mySubject, myCreateEdgeQuery, myObjects);

            QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region Link(mySuccessAction, myFailureAction, mySubject, myCreateEdgeQuery, params myObjects)

        public QueryResult Link(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, DBVertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params DBVertex[] myObjects)
        {

            var _QueryResult = Link(mySubject, myCreateEdgeQuery, myObjects);

            QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region Link(mySuccessAction, myPartialSuccessAction, myFailureAction, mySubject, myCreateEdgeQuery, params myObjects)

        public QueryResult Link(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, DBVertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params DBVertex[] myObjects)
        {

            var _QueryResult = Link(mySubject, myCreateEdgeQuery, myObjects);

            QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion


        #region Link(mySubject, myEdge, params myObjects)

        public QueryResult Link(DBVertex mySubject, DBEdge myEdge, params DBVertex[] myObjects)
        {
            return new QueryResult();
        }

        #endregion

        #region Link(myAction, mySubject, myEdge, params myObjects)

        public QueryResult Link(Action<QueryResult> myAction, DBVertex mySubject, DBEdge myEdge, params DBVertex[] myObjects)
        {

            var _QueryResult = Link(mySubject, myEdge, myObjects);

            QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region Link(mySuccessAction, myFailureAction, mySubject, myEdge, params myObjects)

        public QueryResult Link(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, DBVertex mySubject, DBEdge myEdge, params DBVertex[] myObjects)
        {

            var _QueryResult = Link(mySubject, myEdge, myObjects);

            QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region Link(mySuccessAction, myPartialSuccessAction, myFailureAction, mySubject, myEdge, params myObjects)

        public QueryResult Link(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, DBVertex mySubject, DBEdge myEdge, params DBVertex[] myObjects)
        {

            var _QueryResult = Link(mySubject, myEdge, myObjects);

            QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #endregion

        #region Unlink

        #region Unlink(mySubject, myCreateEdgeQuery, params myObjects)

        public QueryResult Unlink(DBVertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params DBVertex[] myObjects)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Unlink(myAction, mySubject, myCreateEdgeQuery, params myObjects)

        public QueryResult Unlink(Action<QueryResult> myAction, DBVertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params DBVertex[] myObjects)
        {

            var _QueryResult = Unlink(mySubject, myCreateEdgeQuery, myObjects);

            QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region Unlink(mySuccessAction, myFailureAction, mySubject, myCreateEdgeQuery, params myObjects)

        public QueryResult Unlink(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, DBVertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params DBVertex[] myObjects)
        {

            var _QueryResult = Unlink(mySubject, myCreateEdgeQuery, myObjects);

            QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region Unlink(mySuccessAction, myPartialSuccessAction, myFailureAction, mySubject, myCreateEdgeQuery, params myObjects)

        public QueryResult Unlink(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, DBVertex mySubject, CreateEdgeQuery myCreateEdgeQuery, params DBVertex[] myObjects)
        {

            var _QueryResult = Unlink(mySubject, myCreateEdgeQuery, myObjects);

            QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion


        #region Unlink(mySubject, myEdge, params myObjects)

        public QueryResult Unlink(DBVertex mySubject, DBEdge myEdge, params DBVertex[] myObjects)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Unlink(myAction, mySubject, myEdge, params myObjects)

        public QueryResult Unlink(Action<QueryResult> myAction, DBVertex mySubject, DBEdge myEdge, params DBVertex[] myObjects)
        {

            var _QueryResult = Unlink(mySubject, myEdge, myObjects);

            QueryResultAction(_QueryResult, myAction);

            return _QueryResult;

        }

        #endregion

        #region Unlink(mySuccessAction, myFailureAction, mySubject, myEdge, params myObjects)

        public QueryResult Unlink(Action<QueryResult> mySuccessAction, Action<QueryResult> myFailureAction, DBVertex mySubject, DBEdge myEdge, params DBVertex[] myObjects)
        {

            var _QueryResult = Unlink(mySubject, myEdge, myObjects);

            QueryResultAction(_QueryResult, mySuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region Unlink(mySuccessAction, myPartialSuccessAction, myFailureAction, mySubject, myEdge, params myObjects)

        public QueryResult Unlink(Action<QueryResult> mySuccessAction, Action<QueryResult> myPartialSuccessAction, Action<QueryResult> myFailureAction, DBVertex mySubject, DBEdge myEdge, params DBVertex[] myObjects)
        {

            var _QueryResult = Unlink(mySubject, myEdge, myObjects);

            QueryResultAction(_QueryResult, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #endregion


        #region LinqQuery<T>()

        public LinqQueryable<T> LinqQuery<T>()
            where T : DBVertex, new()
        {
            return LinqQuery<T>("");
        }

        #endregion

        #region LinqQuery<T>(myTypeAlias)

        public LinqQueryable<T> LinqQuery<T>(String myTypeAlias)
            where T : DBVertex, new()
        {
            return new LinqQueryable<T>(new LinqQueryProvider(this, typeof(T), myTypeAlias));
        }

        #endregion


        #region Traverse(...)

        /// <summary>
        /// Starts a traversal and returns the found paths or an aggreagted result
        /// </summary>
        /// <typeparam name="T">The resulttype after applying the result transformation</typeparam>
        /// <param name="myStartVertex">The starting vertex</param>
        /// <param name="TraversalOperation">BreathFirst|DepthFirst</param>
        /// <param name="myFollowThisEdge">Follow this edge? Based on its TYPE or any other property/characteristic...</param>
        /// <param name="myFollowThisPath">Follow this path (== actual path + NEW edge + NEW dbobject? Based on edge/object UUID, TYPE or any other property/characteristic...</param>
        /// <param name="myMatchEvaluator">Mhm, this vertex/path looks interesting!</param>
        /// <param name="myMatchAction">Hey! I have found something interesting!</param>
        /// <param name="myStopEvaluator">Will stop the traversal on a condition</param>
        /// <param name="myWhenFinished">Finish this traversal by calling (a result transformation method and) an external method...</param>
        /// <returns></returns>
        public T Traverse<T>(DBVertex                               myStartVertex,
                             TraversalOperation                     TraversalOperation  = TraversalOperation.BreathFirst,
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

        #region BeginTransaction(myDistributed = false, myLongRunning = false, myIsolationLevel = IsolationLevel.Serializable, myName = "", myCreated = null)

        public abstract DBTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? myCreated = null);

        #endregion


    }

}

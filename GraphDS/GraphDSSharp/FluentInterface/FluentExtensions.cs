/*
 * sones GraphDS API - FluentExtensions
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

namespace sones.GraphDS.API.CSharp.Fluent
{


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

    }

}

/*
 * GraphDSSharp - LinqExtensions
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDS.API.CSharp.Linq
{

    /// <summary>
    /// Extension classes for the GraphDSSharp API to provide LINQ queries
    /// </summary>
    public static class LinqExtensions
    {


        #region LinqQuery<T>(this myAGraphDSSharp)

        public static LinqQueryable<T> LinqQuery<T>(this AGraphDSSharp myAGraphDSSharp)
            where T : Vertex, new()
        {
            return myAGraphDSSharp.LinqQuery<T>("");
        }

        #endregion

        #region LinqQuery<T>(this myAGraphDSSharp, myTypeAlias)

        public static LinqQueryable<T> LinqQuery<T>(this AGraphDSSharp myAGraphDSSharp, String myTypeAlias)
            where T : Vertex, new()
        {
            return new LinqQueryable<T>(new LinqQueryProvider(myAGraphDSSharp, typeof(T), myTypeAlias));
        }

        #endregion


    }

}

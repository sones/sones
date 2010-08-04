/* 
 * LinqQueryProvider
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace sones.GraphDS.API.CSharp.Linq
{

    /// <summary>
    /// Based on eBook: Linq-in-Action which is based on:
    /// http://blogs.msdn.com/b/mattwar/archive/2007/08/09/linq-building-an-iqueryable-provider-part-i.aspx
    /// http://iqtoolkit.codeplex.com/license => MS-PL (more or less like BSD)
    /// </summary>

    public class LinqQueryProvider : IQueryProvider
    // Sadly constraints on T will not work, as the interface
    // IQueryProvider does not define constraints!
    // where T : DBVertex, new()
    // Additionally T must not be the type of the query class (e.g. Website)
    // but the type of the resulting IEnumerable<T> (e.g. Website, DBVertex, String, AnonymousType)"
    {

        #region Data

        AGraphDSSharp _AGraphDSSharp;

        #endregion

        #region Properties

        public Type   Type      { get; private set; }
        public String TypeAlias { get; private set; }

        #endregion

        #region Constructor(s)

        #region LinqQueryProvider(myGraphDSSharp, myType)

        public LinqQueryProvider(AGraphDSSharp myAGraphDSSharp, Type myType)
            : this (myAGraphDSSharp, myType, "")
        {
        }

        #endregion

        #region LinqQueryProvider(myGraphDSSharp, myType, myTypeAlias)

        public LinqQueryProvider(AGraphDSSharp myAGraphDSSharp, Type myType, String myTypeAlias)
        {
            _AGraphDSSharp  = myAGraphDSSharp;
            Type            = myType;
            TypeAlias       = myTypeAlias;
        }

        #endregion

        #endregion


        #region GetGQLQuery(myExpression)

        public String GetGQLQuery(Expression myExpression)
        {

            // To some magic Expression => GQL

            var _TypeAlias = TypeAlias;

            if (_TypeAlias != "")
                _TypeAlias = " " + _TypeAlias;

            var _GQLQuery = "FROM " + Type.Name + _TypeAlias + " SELECT UUID, Name";

            return _GQLQuery;

        }

        #endregion


        #region IQueryable members

        #region CreateQuery<TResult>(myExpression)

        public IQueryable<TResult> CreateQuery<TResult>(Expression myExpression)
            // Sadly constraints on T will not work, as the interface
            // does not define constraints!
            // where T : DBVertex, new()
        {

            // Do not check types, as selction of base types or anonymous types
            // would not be possible afterwards!

            //if (!typeof(T).IsAssignableFrom(typeof(DBVertex)))
            //    throw new ArgumentOutOfRangeException("Invalid type T!");

            return (IQueryable<TResult>) new LinqQueryable<TResult>(this, myExpression);

        }

        #endregion

        #region CreateQuery(myExpression)

        public IQueryable CreateQuery(Expression myExpression)
        {

            //return new GraphDSQueryable(this, myExpression);
            //Type _ElementType = TypeSystem.GetElementType(myExpression.Type);

            //try
            //{
            //    return (IQueryable) Activator.CreateInstance(typeof(GraphDSQueryable<>).
            //        MakeGenericType(_ElementType), new Object[] { this, myExpression });
            //}

            //catch (TargetInvocationException _Exception)
            //{
            //    throw _Exception.InnerException;
            //}

            return null;

        }

        #endregion

        #region Execute(myExpression)

        public Object Execute(Expression myExpression)
        {
            var _SelectToObjectGraph = new SelectToObjectGraph(_AGraphDSSharp.Query(GetGQLQuery(myExpression)));
            return _SelectToObjectGraph;
        }

        #endregion

        #region Execute<TResult>(myExpression)

        public TResult Execute<TResult>(Expression myExpression)
        {

            // Do not use this method, as T set to e.g. DBVertex and returning
            // a DBVertex is not really what you want. Ienumerable<T> would be
            // more useful!

            //String url = GetQueryText(myExpression);
            //IEnumerable<AmazonBook> results = AmazonHelper.PerformWebQuery(url);
            //String _GQLQuery = "FROM Website w SELECT w.UUID, w.Name";
            //IEnumerable<T> results = _AGraphDSSharp.QuerySelect(_GQLQuery).GetAsGraph<DBVertex>();
            //return results;

            return default(TResult);

        }

        #endregion

        #endregion

    }

}

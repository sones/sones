/* 
 * LinqQueryable
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;

using sones.GraphDS.API.CSharp.Reflection;

#endregion

namespace sones.GraphDS.API.CSharp.Linq
{

    public class LinqQueryable<T> : IQueryable<T>, IQueryable,
                                    IOrderedQueryable, IOrderedQueryable<T>,
                                    IEnumerable, IEnumerable<T>
    // Sadly constraints on T will not work, as the interface
    // IQueryProvider does not define constraints!
    // where T : DBVertex, new()
    {

        #region Data

        LinqQueryProvider _QueryProvider;
        Expression           _Expression;

        #endregion

        #region Properties

        #region Expression

        Expression IQueryable.Expression
        {
            get
            {
                return _Expression;
            }
        }

        #endregion

        #region ElementType

        Type IQueryable.ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        #endregion

        #region Provider

        IQueryProvider IQueryable.Provider
        {
            get
            {
                return _QueryProvider;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region LinqQueryable(myQueryProvider)

        public LinqQueryable(LinqQueryProvider myQueryProvider)
        {

            if (myQueryProvider == null)
                throw new ArgumentNullException("myQueryProvider must not be null!");
            
            _QueryProvider = myQueryProvider;
            _Expression    = Expression.Constant(this);
        
        }

        #endregion

        #region LinqQueryable(myQueryProvider, myExpression)

        public LinqQueryable(LinqQueryProvider myQueryProvider, Expression myExpression)
        {

            if (myQueryProvider == null)
                throw new ArgumentNullException("myQueryProvider must not be null!");

            if (myExpression == null)
                throw new ArgumentNullException("myExpression must not be null!");

            // THis check is not useful, as you might want to return Strings or Int32s!
            //if (!typeof(IQueryable<DBVertex>).IsAssignableFrom(myExpression.Type))
            //    throw new ArgumentOutOfRangeException("Invalid myExpression type!");
            
            _QueryProvider = myQueryProvider;
            _Expression    = myExpression;
        
        }

        #endregion

        #endregion


        #region GetEnumerator()

        #region IEnumerable<T>.GetEnumerator()

        public IEnumerator<T> GetEnumerator()
        {

            var _Type                = typeof(T);
            var _SelectToObjectGraph = _QueryProvider.Execute(_Expression) as SelectToObjectGraph;

            // The user wants to receive DBVertices
            if (typeof(DBVertex).IsAssignableFrom(_Type))
                return _SelectToObjectGraph.ToVertexType<T>().GetEnumerator();

            // The user wants to receive new objects of an anonymous type
            if (_Type.Name.StartsWith("<>f__AnonymousType"))
                return _SelectToObjectGraph.ToAnonymousType<T>().GetEnumerator();

            // The user wants to receive objects of a base type (e.g. Int32, String)
            //ToDo: Get myAttributeName somehow from _Expression
            return _SelectToObjectGraph.ToDotNetObject<T>("myAttributeName").GetEnumerator();

        }

        #endregion

        #region IEnumerable.GetEnumerator()

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _QueryProvider.Execute(_Expression)).GetEnumerator();
        }

        #endregion

        #endregion

        #region ToString()

        public override String ToString()
        {
            return this._QueryProvider.GetGQLQuery(_Expression);
        }

        #endregion

    }

}

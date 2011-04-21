using System;
using sones.GraphDB.Expression.Tree;
using System.Collections;
using System.Collections.Generic;
using sones.GraphDB.Expression.Tree.Literals;

namespace sones.GraphDB.Expression
{
    /// <summary>
    /// A collection literal expression
    /// </summary>
    public sealed class CollectionLiteralExpression : IExpression, ILiteralExpression
    {
        #region data

        /// <summary>
        /// A collection literal expression... sth like [13, 14, 15] or ["Alice", "Bob", "Carol"]
        /// </summary>
        public readonly ICollectionWrapper CollectionLiteral;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new collection literal using a list
        /// </summary>
        /// <param name="myCollection">The IList collection</param>
        public CollectionLiteralExpression(IList<IComparable> myCollection)
        {
            CollectionLiteral = new ListCollectionWrapper(myCollection);
        }

        /// <summary>
        /// Creates a new collection literal using a set
        /// </summary>
        /// <param name="myCollection">The ISet collection</param>
        public CollectionLiteralExpression(ISet<IComparable> myCollection)
        {
            CollectionLiteral = new SetCollectionWrapper(myCollection);
        }

        #endregion

        #region IExpression Members

        public TypeOfExpression TypeOfExpression
        {
            get { return TypeOfExpression.Constant; }
        }

        #endregion

        #region ILiteralExpression Members

        public IComparable Value
        {
            get { return CollectionLiteral; }
        }

        #endregion
    }
}

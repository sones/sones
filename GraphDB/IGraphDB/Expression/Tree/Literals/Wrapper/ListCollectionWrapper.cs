using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ErrorHandling.Expression;
using System.Collections;


namespace sones.GraphDB.Expression.Tree.Literals
{
    /// <summary>
    /// A wrapper that wraps lists
    /// This data structure is needed, because generic lists do not implement IComparable
    /// </summary>
    public sealed class ListCollectionWrapper : ICollectionWrapper, IComparable<ListCollectionWrapper>, IComparable<IComparable>
    {
        #region data

        /// <summary>
        /// The actual value of the wrapper
        /// </summary>
        public readonly IList<IComparable> Value;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new collection wrapper
        /// </summary>
        /// <param name="myCollection">The collection that needs to be wrapped</param>
        public ListCollectionWrapper(IList<IComparable> myCollection)
        {
            Value = myCollection;
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is ListCollectionWrapper)
            {
                return CompareTo(obj as ListCollectionWrapper);
            }
            
            if (obj is IComparable)
            {
                return CompareTo(obj as IComparable);
            }

            throw new InvalidCollectionComparismException(String.Format("It is not allowed to compare a {0} to a List", obj.GetType().Name));
        }

        #endregion

        #region IEnumerable<IComparable> Members

        public IEnumerator<IComparable> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Value).GetEnumerator();
        }

        #endregion

        #region IComparable<ListCollectionWrapper> Members

        public int CompareTo(ListCollectionWrapper myOther)
        {
            #region count of objects

            //A list of comparables is greater than another one, if their count is also greater

            var countCompare = this.Value.Count.CompareTo(myOther.Value.Count);

            if (countCompare != 0)
            {
                return countCompare;
            }

            #endregion

            #region inner comparables

            //every member within the list is compared

            int memberCompare = 0;

            for (int i = 0; i < Value.Count; i++)
            {
                memberCompare = this.Value[i].CompareTo(myOther.Value[i]);
            }

            if (memberCompare != 0)
            {
                return memberCompare;
            }

            #endregion

            return 0;
        }

        #endregion

        #region IComparable<IComparable> Members

        public int CompareTo(IComparable myOther)
        {
            //A single IComparable is treated as a list with one element.
            if (this.Value.Count != 1)
                return this.Value.Count.CompareTo(1);

            return this.Value[0].CompareTo(myOther);
        }

        #endregion
    }
}

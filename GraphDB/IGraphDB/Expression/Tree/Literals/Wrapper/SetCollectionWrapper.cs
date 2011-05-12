/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ErrorHandling.Expression;
using System.Collections;
using sones.Library.NewFastSerializer;

namespace sones.GraphDB.Expression.Tree.Literals
{
    /// <summary>
    /// A wrapper that wraps sets
    /// This data structure is needed, because generic sets do not implement IComparable
    /// </summary>
    public sealed class SetCollectionWrapper : ICollectionWrapper
    {
        #region data

        /// <summary>
        /// The actual value of the wrapper
        /// </summary>
        public ISet<IComparable> Value;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new set collection wrapper
        /// </summary>
        /// <param name="myCollection">The collection that needs to be wrapped</param>
        public SetCollectionWrapper(IEnumerable<IComparable> myCollection)
        {
            Value = new HashSet<IComparable>( myCollection);
        }

        /// <summary>
        /// Creates a new collection wrapper
        /// </summary>
        public SetCollectionWrapper()
        {
            Value = new HashSet<IComparable>();
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            SetCollectionWrapper counterPart = obj as SetCollectionWrapper;

            if (counterPart != null)
            {
                #region count of objects

                //A set of comparables is greater than another one, if their count is also greater

                var countCompare = this.Value.Count.CompareTo(counterPart.Value.Count);

                if (countCompare != 0)
                {
                    return countCompare;
                }

                #endregion

                #region inner comparables

                var thisAsOrderedList = this.Value.OrderBy(member => member).ToList<IComparable>();
                var counterpartAsOrderedList = counterPart.Value.OrderBy(member => member).ToList<IComparable>();

                //every member within the list is compared

                int memberCompare = 0;

                for (int i = 0; i < Value.Count; i++)
                {
                    memberCompare = thisAsOrderedList[i].CompareTo(counterpartAsOrderedList[i]);
                }

                if (memberCompare != 0)
                {
                    return memberCompare;
                }

                #endregion

                return 0;
            }

            throw new InvalidCollectionComparismException(String.Format("It is not allowed to compare a {0} to a Set", obj.GetType().Name));
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

        #region ICollectionWrapper Members

        public void Add(IComparable myComparable)
        {
            Value.Add(myComparable);
        }

        public void Add(IEnumerable<IComparable> myComparables)
        {
            Value.UnionWith(myComparables);
        }

        public void Remove(IComparable myComparable)
        {
            Value.Remove(myComparable);
        }

        public void Remove(IEnumerable<IComparable> myComparables)
        {
            Value.ExceptWith(myComparables);
        }

        #endregion

        #region IFastSerialize Members

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            mySerializationWriter.WriteInt32(Value.Count);

            foreach (var item in Value)
            {
                mySerializationWriter.WriteObject(item);
            }
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            Value = new HashSet<IComparable>();

            var itemCnt = mySerializationReader.ReadInt32();

            for (int i = 0; i < itemCnt; i++)
            {
                Value.Add((IComparable)mySerializationReader.ReadObject());
            }
        }

        #endregion
    }
}

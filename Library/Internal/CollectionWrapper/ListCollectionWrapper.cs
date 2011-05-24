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
using System.Collections;
using sones.Library.NewFastSerializer;
using sones.Library.CollectionWrapper.ErrorHandling;


namespace sones.Library.CollectionWrapper
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
        public IList<IComparable> Value;


        #endregion

        #region constructor

        /// <summary>
        /// Creates a new collection wrapper
        /// </summary>
        /// <param name="myCollection">The collection that needs to be wrapped</param>
        public ListCollectionWrapper(IEnumerable<IComparable> myCollection)
        {
            Value = new List<IComparable>(myCollection);
        }

        /// <summary>
        /// Create a new list collection wrapper
        /// </summary>
        public ListCollectionWrapper()
        {
            Value = new List<IComparable>();
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

        #region IFastSerialize Members

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            //type code for the list collection wrapper
            mySerializationWriter.WriteInt16(0);
            
            mySerializationWriter.WriteInt32(Value.Count);

            foreach (var item in Value)
            {
                mySerializationWriter.WriteObject(item);
            }
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            Value = new List<IComparable>();

            var typeCode = mySerializationReader.ReadInt16();
            
            var itemCnt = mySerializationReader.ReadInt32();

            for (int i = 0; i < itemCnt; i++)
            {
                Value.Add((IComparable)mySerializationReader.ReadObject());
            }
        }

        #endregion

        public void Add(IComparable myValue)
        {
            Value.Add(myValue);
        }
    }
}

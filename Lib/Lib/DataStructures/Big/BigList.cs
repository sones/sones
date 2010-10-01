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

/* GraphLib - BigList
 * (c) Daniel Kirstenpfad, 2009
 * 
 * This class implements a generic List datastructure which
 * can go beyond the 2 GB per object limit for objects in .NET.
 * It can do this by partitioning different smaller Lists
 * into one management datastructure.
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.DataStructures.Big
{
    /// <summary>
    /// This is a BigList class for the .NET generic List data structure.
    /// </summary>
    /// <typeparam name="T">the type of this generic list</typeparam>
    public class BigList<T> : IEnumerable<T>, ICollection<T>
    {
        private Int64 _Length;
        internal List<List<T>> _Data;
        internal List<T> _DataElement;
        private Int64 _MaximumElementsPerPartition = 1000000; // 1 Mio elements by default


        #region Constructors
        /// <summary>
        /// the simple constructor, initalizes with the default value for MaximumElementsPerPartition
        /// </summary>
        public BigList()
        {
            _Data = new List<List<T>>();
            _DataElement = new List<T>();
            _Length = 0;
        }

        /// <summary>
        /// initializes the data structure with a user defined MaximumNumberOfElementsPerPartition.
        /// Important: Make sure that you're Data is going to be smaller than 2 Gbyte of memory per
        /// Partition.
        /// </summary>
        /// <param name="MaximumNumberOfElementsPerPartition"></param>
        public BigList(Int64 MaximumNumberOfElementsPerPartition)
        {
            _Data = new List<List<T>>();
            _DataElement = new List<T>();
            _MaximumElementsPerPartition = MaximumNumberOfElementsPerPartition;
            _Length = 0;
        }
        #endregion

        public Int64 MaximumElementsPerPartition { get { return _MaximumElementsPerPartition; } }

        public Int64 Length { get { return _Length; } }

        public Int64 Count {  get { return _Length; } }

        public int PartitionCount 
        { 
            get 
            {
               return _Data.Count + 1;
            } 
        }

        private List<T> CurrentDataElementList
        {
            get { return _DataElement; }
        }

        public bool Add(T element)
        {
            CurrentDataElementList.Add(element);
            _Length++;
            
            #region MaximumElementsPerPartition reached...
            if (CurrentDataElementList.Count == _MaximumElementsPerPartition)
            {
                _Data.Add(CurrentDataElementList);                    
                
                // start a new List...
                _DataElement = new List<T>();
            }
            #endregion

            return true;
        }

        public bool Contains(T element)
        {
            if (CurrentDataElementList.Contains(element))
                return true;
            else
            {
                foreach (List<T> _tDataElement in _Data)
                {
                    if (_tDataElement.Contains(element))
                        return true;
                }
            }
            return false;
        }

        public bool Remove(T element)
        {
            if (CurrentDataElementList.Remove(element))
            {
                _Length--;
                return true;
            }
            else
                foreach (List<T> _tDataElement in _Data)
                {
                    if (_tDataElement.Remove(element))
                    {
                        _Length--;
                        return true;
                    }
                }

            return false;
        }

        #region Index
        public T this[Int64 _Element]
        {
            get
            {
                Int32 partition = Convert.ToInt32((_Element / MaximumElementsPerPartition));

                if (partition == PartitionCount)
                {
                    // we're obviously in the _DataElement part
                    return _DataElement[(Int32)(_Element - ((partition - 1) * MaximumElementsPerPartition))];
                }
                else
                {
                    // we're obviously in the _Data Part...
                    return _Data[partition][(Int32)(_Element - ((partition) * MaximumElementsPerPartition))];
                }
            }
            set
            {
                Int32 partition = Convert.ToInt32((_Element / MaximumElementsPerPartition));

                if (partition == PartitionCount)
                {
                    // we're obviously in the _DataElement part
                    _DataElement[(Int32)(_Element - ((partition - 1) * MaximumElementsPerPartition))] = value;
                }
                else
                {
                    // we're obviously in the _Data Part...
                    _Data[partition][(Int32)(_Element - ((partition) * MaximumElementsPerPartition))] = value;
                }
                
            }
        }
        #endregion


        #region IEnumerable<T> Members
        public IEnumerator<T> GetEnumerator()
        {
            return new BigListEnumerator<T>(this);
        }
        #endregion

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new BigListEnumerator<T>(this);
        }
        #endregion

        #region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        public void Clear()
        {
            _Data = new List<List<T>>();
            _DataElement = new List<T>();
            _Length = 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<T>.Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion
    }

    #region BigListEnumerator
    public class BigListEnumerator<T> : IEnumerator<T>
    {
        private BigList<T> _collection; //enumerated collection

        private Int64 index;        //current overall index
        private Int32 partition;    // the current partition
        private Int32 elementindex; // the current index within the partition
        private T _current; //current enumerated object in the collection


        public BigListEnumerator()
        {
            // do nothing
        }

        public BigListEnumerator(BigList<T> collection)
        {
            _collection = collection;
            index = -1;
            partition = 0;
            elementindex = -1;
            _current = default(T);
        }
        
        #region IEnumerator<T> Members
        public T Current
        {
            get
            {
                return _current;
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            _collection = null;
            _current = default(T);
            index = 0;
        }
        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        public bool MoveNext()
        {
            //make sure we are within the bounds of the collection

            if (index >= _collection.Count)
            {
                //if not return false

                return false;
            }
            else
            {
                //if we are, then set the current element to the next object in the collection
                index++;
                elementindex++;

                // are we still checking the partitions?
                if (partition >= _collection.PartitionCount-1)
                {
                    // no partitions
                    #region we're just here for the elementindex
                    
                    // if there are not more elements left in the _DataElement List
                    if (_collection._DataElement.Count < elementindex)
                    {
                            return false;
                    }
                    else
                    {
                        // set current to the next element in the _dataElement of the _collection
                        _current = _collection._DataElement[elementindex];
                        return true;
                    }
                    
                    #endregion
                }
                else
                {
                    // we're seeking through the partitions, starting at 0 and upwards

                    // check if we're within the boundaries of the current partition
                    if (_collection._Data[partition].Count == elementindex)
                    {
                        // nope, we're outside of it...
                        elementindex = 0;
                        partition++;
                        if (partition >= _collection.PartitionCount-1)
                        {
                            // nope, not available
                            return false;
                        }
                    }

                    _current = _collection._Data[partition][elementindex];
                }

            }
            return true;

        }

        public void Reset()
        {
            _current = default(T); //reset current object
            partition = 0;
            index = -1;
            elementindex = -1;
        }

        #endregion
    }
    #endregion
}

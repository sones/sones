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


/* PandoraLib - BigDictionary
 * (c) Daniel Kirstenpfad, 2009
 * 
 * This class implements a generic Dictionary datastructure which
 * can go beyond the 2 GB per object limit for objects in .NET.
 * It can do this by partitioning different smaller Dictionaries
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
    /// This is a BigHashSet class for the .NET generic HashSet data structure.
    /// </summary>
    /// <typeparam name="T">the type of this generic HashSet</typeparam>
    public class BigDictionary<TKey,TValue> : IDictionary<TKey,TValue>
    {
        private ulong _Length;
        private List<Dictionary<TKey,TValue>> _Data;
        private Dictionary<TKey,TValue> _DataElement;
        private bool previouslyAdded = false;
        private int _MaximumElementsPerPartition = 1000000; // 1 Mio elements by default

        #region Constructors
        /// <summary>
        /// the simple constructor, initalizes with the default value for MaximumElementsPerPartition
        /// </summary>
        public BigDictionary()
        {
            _Data = new List<Dictionary<TKey,TValue>>();
            _DataElement = new Dictionary<TKey,TValue>();
            _Length = 0;
            previouslyAdded = false;
        }

        /// <summary>
        /// initializes the data structure with a user defined MaximumNumberOfElementsPerPartition.
        /// Important: Make sure that you're Data is going to be smaller than 2 Gbyte of memory per
        /// Partition.
        /// </summary>
        /// <param name="MaximumNumberOfElementsPerPartition"></param>
        public BigDictionary(int MaximumNumberOfElementsPerPartition)
        {
            _Data = new List<Dictionary<TKey,TValue>>();
            _DataElement = new Dictionary<TKey,TValue>();
            _MaximumElementsPerPartition = MaximumNumberOfElementsPerPartition;
            _Length = 0;
            previouslyAdded = false;
        }
        #endregion

        #region Properties
        public int MaximumElementsPerPartition { get { return _MaximumElementsPerPartition; } }

        public ulong Length { get { return _Length; } }

        public ulong Count { get { return _Length; } }

        public int PartitionCount
        {
            get
            {
                if (!previouslyAdded)
                    return _Data.Count + 1;
                else
                    return _Data.Count;
            }
        }

        private Dictionary<TKey,TValue> CurrentDataElementDictionary
        {
            get { return _DataElement; }
        }
        #endregion

        #region IDictionary<TKey,TValue> Members

        #region Add
        public void Add(TKey key, TValue value)
        {
            // this is the slowest version of this... by just testing if it's already in the whole Dictionary
            if (!this.ContainsKey(key))
            {
                CurrentDataElementDictionary.Add(key, value);
                _Length++;

                #region MaximumElementsPerPartition reached...
                if (CurrentDataElementDictionary.Count == _MaximumElementsPerPartition)
                {
                    if (!previouslyAdded)
                    {
                        _Data.Add(CurrentDataElementDictionary);
                    }

                    // switch to the next DataElementDictionary which is not filled up or create a new one
                    foreach (Dictionary<TKey,TValue> _Dictionary in _Data)
                    {
                        if (_Dictionary.Count < _MaximumElementsPerPartition)
                        {
                            // found a Dictionary which is not filled up
                            previouslyAdded = true;
                            _DataElement = _Dictionary;
                            return;
                        }
                    }

                    // we only get here if we filled up a Dictionary and haven't found any Dictionary that can
                    // take more elements elsewhere
                    previouslyAdded = false;

                    _DataElement = new Dictionary<TKey, TValue>();
                }
            }
            else
                throw new DuplicateWaitObjectException();
            #endregion
        }
        #endregion

        #region ContainsKey
        public bool ContainsKey(TKey key)
        {
            if (!previouslyAdded)
            {
                if (CurrentDataElementDictionary.ContainsKey(key))
                    return true;
            }

            foreach (Dictionary<TKey,TValue> _dictionary in _Data)
            {
                if (_dictionary.ContainsKey(key))
                    return true;
            }

            return false;
        }
        #endregion

        #region Keys Property
        /// <summary>
        /// May be slow
        /// </summary>
        public ICollection<TKey> Keys
        {
            get 
            {
                BigList<TKey> Output = new BigList<TKey>();

                if (!previouslyAdded)
                {
                    foreach (TKey key in _DataElement.Keys)
                    {
                        Output.Add(key);
                    }
                }

                foreach (Dictionary<TKey,TValue> _dictionary in _Data)
                {
                    foreach (TKey key in _DataElement.Keys)
                    {
                        Output.Add(key);
                    }
                }

                return Output;
            }
        }
        #endregion

        #region Remove
        public bool Remove(TKey key)
        {
            if (!previouslyAdded)
            {
                if (CurrentDataElementDictionary.ContainsKey(key))
                {
                    CurrentDataElementDictionary.Remove(key);
                    _Length--;
                    return true;
                }
            }

            foreach (Dictionary<TKey,TValue> _dictionary in _Data)
            {
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary.Remove(key);
                    _Length--;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region TryGetValue
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!previouslyAdded)
            {
                if (CurrentDataElementDictionary.TryGetValue(key, out value))
                    return true;
            }

            foreach (Dictionary<TKey,TValue> _dictionary in _Data)
            {
                if (_dictionary.TryGetValue(key, out value))
                    return true;
            }

            value = default(TValue);
            return false;
        }
        #endregion

        #region Values
        public ICollection<TValue> Values
        {
            get
            {
                BigList<TValue> Output = new BigList<TValue>();

                if (!previouslyAdded)
                {
                    foreach (TValue value in _DataElement.Values)
                    {
                        Output.Add(value);
                    }
                }

                foreach (Dictionary<TKey,TValue> _dictionary in _Data)
                {
                    foreach (TValue value in _DataElement.Values)
                    {
                        Output.Add(value);
                    }
                }

                return Output;
            }
        }
        #endregion

        #region Index
        public TValue this[TKey key]
        {
            get
            {
                // the slowest version of this...
                TValue Output;
                TryGetValue(key,out Output);
                return Output;
            }
            set
            {
                if (!previouslyAdded)
                {
                    if (CurrentDataElementDictionary.ContainsKey(key))
                    {
                        CurrentDataElementDictionary[key] = value;
                        return;
                    }
                }

                foreach (Dictionary<TKey,TValue> _dictionary in _Data)
                {
                    if (_dictionary.ContainsKey(key))
                    {
                        _dictionary[key] = value;
                        return;
                    }
                }
            }
        }
        #endregion

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        #region Add
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }
        #endregion

        #region Clear
        public void Clear()
        {
            _Data = new List<Dictionary<TKey, TValue>>();
            _DataElement = new Dictionary<TKey, TValue>();
            _Length = 0;
            previouslyAdded = false;
        }
        #endregion

        #region Contains
        /// <summary>
        /// this contains does not go into lengths when determine if something is in the dictionary,
        /// it rather works on Keys only.
        /// </summary>
        /// <param name="item">the item to search and check</param>
        /// <returns>true or false</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.ContainsKey(item.Key);
        }
        #endregion

        #region CopyTo  -   NOT IMPLEMENTED
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Count   -   NOT IMPLEMENTED
        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        #region IsReadOnly
        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion

        #region Remove
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return (this.Remove(item.Key));
        }
        #endregion

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            #region Yield current dict

            foreach (var keyVal in _DataElement)
            {
                yield return keyVal;
            }

            #endregion

            #region Yield all other dict except the current one

            foreach (var dict in _Data)
            {
                if (dict != _DataElement)
                {
                    foreach (var keyVal in dict)
                    {
                        yield return keyVal;
                    }
                }
            }

            #endregion
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

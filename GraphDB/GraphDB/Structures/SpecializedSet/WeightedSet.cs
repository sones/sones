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

/* <id name="GraphDB – WeightedSet<T>" />
 * <copyright file="WeightedSet.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This datastructure is an implementation of an weighted set, ...</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.DataStructures;
using sones.Lib.NewFastSerializer;
using sones.Lib.Serializer;

namespace sones.GraphDB.Structures
{

    
    public class WeightedSet<T> : IFastSerialize, ICollection<T>, IEnumerable<T>
        where T : IComparable, IFastSerialize, new()
    {

        #region Data

        private SortedDictionary<DBNumber, List<T>> _WeightedListEntriesWeights;
        private Dictionary<T, DBNumber> _WeightedListEntries;

        #endregion

        #region Properties

        #region SortDirection

        private SortDirection _SortDirection;
        public SortDirection SortDirection
        {
            get { return _SortDirection; }
        }

        #endregion

        #region DefaultWeight

        private DBNumber _DefaultWeight = new DBUInt64(0UL);
        public DBNumber DefaultWeight
        {
            get { return _DefaultWeight; }
        }

        #endregion

        #region Count

        private UInt64 _Count;
        public UInt64 Count
        {
            get { return _Count; }
        }

        #endregion

        #endregion

        #region Ctors

        /// <summary>
        /// Contructor of an desc sorted weighted set
        /// </summary>
        public WeightedSet()
            : this(SortDirection.Desc)
        { }

        public WeightedSet(SortDirection mySortDirection)
        {

            _WeightedListEntries = new Dictionary<T, DBNumber>();
            _SortDirection = mySortDirection;

            if (_SortDirection == SortDirection.Desc)
                _WeightedListEntriesWeights = new SortedDictionary<DBNumber, List<T>>(new SortDescendencyComparer());
            else
                _WeightedListEntriesWeights = new SortedDictionary<DBNumber, List<T>>();

            _Count = 0;

        }

        #endregion

        #region SetSortDirection

        /// <summary>
        /// Changes the sort direction
        /// </summary>
        /// <param name="mySortDirection"></param>
        public void SetSortDirection(SortDirection mySortDirection)
        {
            _SortDirection = mySortDirection;
            if (_WeightedListEntriesWeights.Count > 0)
            {

                SortedDictionary<DBNumber, List<T>> newWeightedListEntriesWeights;
                if (_SortDirection == SortDirection.Desc)
                    newWeightedListEntriesWeights = new SortedDictionary<DBNumber, List<T>>(new SortDescendencyComparer());
                else
                    newWeightedListEntriesWeights = new SortedDictionary<DBNumber, List<T>>();

                foreach (var val in _WeightedListEntriesWeights)
                {
                    newWeightedListEntriesWeights.Add(val.Key, val.Value);
                }

                _WeightedListEntriesWeights = newWeightedListEntriesWeights;

            }
        }
        
        #endregion

        #region SetWeightedDefaultValue

        /// <summary>
        /// Changes the default value of the weight
        /// </summary>
        /// <param name="myDefaultValue"></param>
        public void SetWeightedDefaultValue(DBNumber myDefaultValue)
        {
            _DefaultWeight = myDefaultValue;
        }
        
        #endregion

        #region Add

        /// <summary>
        /// Adds a new value with the defined default weight
        /// </summary>
        /// <param name="myValue"></param>
        public void Add(T myValue)
        {
            Add(myValue, _DefaultWeight);
        }

        /// <summary>
        /// Add the value with a weight, if the value already exist, the <paramref name="myWeight"/> will be add to the current weight
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myWeight"></param>
        public void Add(T myValue, DBNumber myWeight)
        {
            DBNumber curWeight;// = (DBNumber)_DefaultWeight.Clone(myWeight.Value);

            #region Add value entry with weight and update weight

            if (_WeightedListEntries.ContainsKey(myValue))
            {

                #region myValue already exist - update the weight

                #region Get current weight of myValue and remove it

                curWeight = _WeightedListEntries[myValue];
                _WeightedListEntriesWeights[curWeight].Remove(myValue);
                if (_WeightedListEntriesWeights[curWeight].Count == 0)
                {
                    _WeightedListEntriesWeights.Remove(curWeight);
                }

                #endregion

                #region Update weight and add it

                curWeight = (DBNumber)curWeight.Clone();
                curWeight.Add(myWeight);
                _WeightedListEntries[myValue] = curWeight;

                #endregion

                #endregion

            }
            else
            {

                #region myValue does not exist - add it

                curWeight = (DBNumber)myWeight.Clone();
                _WeightedListEntries.Add(myValue, curWeight);

                #endregion

            }

            #endregion

            #region Add weight entry

            if (!_WeightedListEntriesWeights.ContainsKey(curWeight))
            {
                _WeightedListEntriesWeights.Add(curWeight, new List<T>());
            }
            _WeightedListEntriesWeights[curWeight].Add(myValue);

            #endregion

            _Count++;

        }

        /// <summary>
        /// Adds a range of values with the same weight
        /// </summary>
        /// <param name="hashSet"></param>
        /// <param name="weight"></param>
        public void AddRange(IEnumerable<T> hashSet, DBNumber weight)
        {
            foreach (var elem in hashSet)
            {
                Add(elem, weight);
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// Get all values with their corresponding weight
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<T, DBNumber>> GetAll()
        {
            foreach (KeyValuePair<DBNumber, List<T>> dbo in _WeightedListEntriesWeights)
            {
                foreach (T t in dbo.Value)
                {
                    yield return new KeyValuePair<T, DBNumber>(t, dbo.Key);
                }
            }
        }

        /// <summary>
        /// Get all values
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAllValues()
        {
            foreach (List<T> dbos in _WeightedListEntriesWeights.Values)
            {
                foreach (var dbo in dbos)
                {
                    yield return dbo;
                }
            }
        }

        /// <summary>
        /// Get the top <paramref name="myNumOfEntries"/> values
        /// </summary>
        /// <param name="myNumOfEntries"></param>
        /// <returns></returns>
        public IEnumerable<T> GetTop(UInt64 myNumOfEntries)
        {

            SortedDictionary<DBNumber, List<T>>.Enumerator rator = _WeightedListEntriesWeights.GetEnumerator();

            while (myNumOfEntries > 0)
            {
                if (rator.MoveNext())
                {
                    foreach (var val in rator.Current.Value)
                    {
                        if (myNumOfEntries <= 0)
                            break;

                        yield return val;
                        myNumOfEntries--;
                    }
                }
            }

        }

        /// <summary>
        /// Get the top <paramref name="myNumOfEntries"/> values as a weighted set
        /// </summary>
        /// <param name="myNumOfEntries"></param>
        /// <returns></returns>
        public WeightedSet<T> GetTopAsWeightedSet(UInt64 myNumOfEntries)
        {

            var result = new WeightedSet<T>(_SortDirection);

            SortedDictionary<DBNumber, List<T>>.Enumerator rator = _WeightedListEntriesWeights.GetEnumerator();
            while (result.Count < myNumOfEntries && rator.MoveNext())
            {
                if (myNumOfEntries - result.Count >= (UInt64)rator.Current.Value.Count)
                {

                    #region All values of this weight fit into myNumOfEntries

                    //result.Add(rator.Current.Value, rator.Current.Key);
                    result._WeightedListEntriesWeights.Add(rator.Current.Key, rator.Current.Value);
                    foreach (var entry in rator.Current.Value)
                        result._WeightedListEntries.Add(entry, rator.Current.Key);

                    result._Count += (UInt64)rator.Current.Value.Count;

                    #endregion

                }
                else
                {

                    #region The values of this weight does not fit, take as much as needed from the values

                    var values = rator.Current.Value.GetRange(0, (Int32)(myNumOfEntries - result.Count));
                    result._WeightedListEntriesWeights.Add(rator.Current.Key, values);
                    foreach (var entry in values)
                        result._WeightedListEntries.Add(entry, rator.Current.Key);

                    result._Count += (UInt64)result._WeightedListEntriesWeights[rator.Current.Key].Count;

                    #endregion

                }
            }

            return result;
        }

        /// <summary>
        /// Get the maximum weight - the first element.
        /// </summary>
        /// <returns></returns>
        public DBNumber GetMaxWeight()
        {
            return _WeightedListEntriesWeights.ElementAt(0).Key;
        }

        /// <summary>
        /// Get the minimum weight - the last element.
        /// </summary>
        /// <returns></returns>
        public DBNumber GetMinWeight()
        {
            return _WeightedListEntriesWeights.ElementAt(_WeightedListEntriesWeights.Count - 1).Key;
        }

        public KeyValuePair<T, DBNumber> Get(T myKey)
        {
            return new KeyValuePair<T, DBNumber>(myKey, _WeightedListEntries[myKey]);
        }

        #endregion

        #region GetDescInfo()

        /// <summary>
        /// Get an decription info of this edge
        /// </summary>
        /// <returns></returns>
        public string GetDescInfo()
        {
            string retVal = "";

            retVal = _DefaultWeight.Value.ToString();
            switch (_SortDirection)
            {
                case SortDirection.Asc:
                    retVal += " ASC";
                    break;

                case SortDirection.Desc:
                    retVal += " DESC";
                    break;
            }

            return retVal;
        }

        #endregion
        
        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var dboList in _WeightedListEntriesWeights.Values)
            {
                foreach (var dbo in dboList)
                {
                    yield return dbo;
                }
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Clear()
        {
            _WeightedListEntries = new Dictionary<T, DBNumber>();
            _WeightedListEntriesWeights = new SortedDictionary<DBNumber, List<T>>();
            _Count = 0;
        }

        public bool Contains(T item)
        {
            return _WeightedListEntries.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _WeightedListEntries.Keys.ToArray().CopyTo(array, arrayIndex);
        }

        int ICollection<T>.Count
        {
            get { return (int)_Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (!_WeightedListEntries.ContainsKey(item))
                return true;

            if (_WeightedListEntries[item] != null)
                _WeightedListEntriesWeights[_WeightedListEntries[item]].Remove(item);

            return _WeightedListEntries.Remove(item);
        }

        public void RemoveWhere(Predicate<T> match)
        {
            foreach (var dbo in _WeightedListEntries)
            {
                if (match(dbo.Key))
                {
                    if (_WeightedListEntriesWeights.ContainsKey(dbo.Value))
                    {
                        _WeightedListEntriesWeights[dbo.Value].Remove(dbo.Key);
                        if (_WeightedListEntriesWeights[dbo.Value].Count == 0)
                            _WeightedListEntriesWeights.Remove(dbo.Value);
                    }
                    _WeightedListEntries.Remove(dbo.Key);
                    _Count--;
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _WeightedListEntries.GetEnumerator();
        }

        #endregion

        #region IFastSerialize Members

        bool IFastSerialize.isDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        DateTime IFastSerialize.ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {

            #region Write

            mySerializationWriter.WriteByte((Byte)_SortDirection);
            mySerializationWriter.WriteObject(_DefaultWeight);

            mySerializationWriter.WriteUInt32((UInt32)_WeightedListEntries.Count);

            foreach (var keyValPair in _WeightedListEntries)
            {
                ((IFastSerialize)keyValPair.Key).Serialize(ref mySerializationWriter);
                keyValPair.Value.Serialize(ref mySerializationWriter);
            }

            #endregion

        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {

            #region Read

            _SortDirection = (SortDirection)mySerializationReader.ReadOptimizedByte();

            if (_SortDirection == SortDirection.Desc)
                _WeightedListEntriesWeights = new SortedDictionary<DBNumber, List<T>>(new SortDescendencyComparer());
            else
                _WeightedListEntriesWeights = new SortedDictionary<DBNumber, List<T>>();

            _WeightedListEntries = new Dictionary<T, DBNumber>();

            _DefaultWeight = (DBNumber)mySerializationReader.ReadObject();

            UInt32 _WeightedListEntriesCount = mySerializationReader.ReadUInt32();

            for (UInt32 i = 0; i < _WeightedListEntriesCount; i++)
            {
                T newT = new T();
                ((IFastSerialize)newT).Deserialize(ref mySerializationReader);
                DBNumber weight = (DBNumber)_DefaultWeight.Clone();
                weight.Deserialize(ref mySerializationReader);

                Add(newT, weight);

            }

            #endregion

        }

        #endregion

        #region ToString() override

        public override string ToString()
        {
            return String.Concat("WeightedList<", typeof(T).Name, ">[", _WeightedListEntriesWeights.Count, "]");
        }

        #endregion
        
        #region Equals

        public override bool Equals(object obj)
        {
            var other = obj as WeightedSet<T>;
            if (other == null)
            {
                return false;
            }

            if (_Count != other._Count)
            {
                return false;
            }
            if (!_DefaultWeight.Equals(other._DefaultWeight))
            {
                return false;
            }
            if (_SortDirection != other._SortDirection)
            {
                return false;
            }
            if (_WeightedListEntries.Count != other._WeightedListEntries.Count)
            {
                return false;
            }
            foreach (var entry in _WeightedListEntries)
            {
                if (!other._WeightedListEntries.Contains(entry))
                {
                    return false;
                }
                if (!other._WeightedListEntries[entry.Key].Equals(entry.Value))
                {
                    return false;
                }
            }

            if (_WeightedListEntriesWeights.Count != other._WeightedListEntriesWeights.Count)
            {
                return false;
            }
            foreach (var entry in _WeightedListEntriesWeights)
            {
                if (!other._WeightedListEntriesWeights.Contains(entry))
                {
                    return false;
                }
                if (entry.Value.Count != other._WeightedListEntriesWeights[entry.Key].Count)
                {
                    return false;
                }
                foreach (var uuid in entry.Value)
                {
                    if (!other._WeightedListEntriesWeights[entry.Key].Contains(uuid))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region GetHashCode

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

    }

}

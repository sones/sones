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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures;

namespace sones.Libraries.Datastructures
{

    /// <summary>
    /// May be usefull some day ;)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="N"></typeparam>

    public class WeightedSet<T, N> : ICollection<T>//, IFastSerialize
        where T : IComparable, new()//, IFastSerialize
    {

        private SortedDictionary<N, List<T>> _WeightedListEntriesWeights;
        private Dictionary<T, N> _WeightedListEntries;
        private SortDirection _SortDirection;
        
        public SortDirection SortDirection
        {
            get { return _SortDirection; }
        }

        private Boolean _IsWeighted = false;
        private N _DefaultWeight = default(N);
        
        public N DefaultWeight
        {
            get { return _DefaultWeight; }
        }

        private UInt64 _Count;

        public UInt64 Count
        {
            get { return _Count; }
        }

        public Boolean IsWeighted
        {
            get
            {
                return _IsWeighted;
            }
        }

        public WeightedSet()
            : this(SortDirection.Desc, false)
        {
        }

        public WeightedSet(SortDirection mySortDirection, Boolean IsWeighted)
        {

            _WeightedListEntries = new Dictionary<T, N>();
            _SortDirection = mySortDirection;

            if (IsWeighted)
            {

                _IsWeighted = IsWeighted;

                if (_SortDirection == SortDirection.Desc)
                    _WeightedListEntriesWeights = new SortedDictionary<N, List<T>>();//new SortDescendencyComparer());
                else
                    _WeightedListEntriesWeights = new SortedDictionary<N, List<T>>();

            }

            _Count = 0;

        }

        public void SetSortDirection(SortDirection mySortDirection)
        {
            _SortDirection = mySortDirection;
        }

        public void SetWeighted(N myDefaultValue)
        {
            if (_IsWeighted)
                return;

            _IsWeighted = true;
            _DefaultWeight = myDefaultValue;

            _WeightedListEntries = _WeightedListEntries.ToDictionary(key => key.Key, value => (N)myDefaultValue);

            if (_SortDirection == SortDirection.Desc)
                _WeightedListEntriesWeights = new SortedDictionary<N, List<T>>();//new SortDescendencyComparer());
            else
                _WeightedListEntriesWeights = new SortedDictionary<N, List<T>>();

            _WeightedListEntriesWeights.Add(myDefaultValue, new List<T>(_WeightedListEntries.Keys));
        }

        public void SetWeightedDefaultValue(N myDefaultValue)
        {
            _DefaultWeight = myDefaultValue;
        }

        public void Add(T myValue)
        {
            if (_IsWeighted)
                throw new Exception("This SpezializedSet is defined as Weighted. You have to pass a weight as parameter!");

            if (!_WeightedListEntries.ContainsKey(myValue))
                _WeightedListEntries.Add(myValue, default(N)); // null!
        }

        public void Add(IEnumerable<T> myValues)
        {

            if (_IsWeighted)
                throw new Exception("This SpezializedSet is defined as Weighted. You have to pass a weight as parameter!");

            foreach (var val in myValues)
            {
                _WeightedListEntries.Add(val, default(N)); // null!
            }

        }

        public void Add(T myValue, N myWeight)
        {
            if (!_IsWeighted)
                throw new Exception("This SpezializedSet is not Weighted. You cannot pass a weight with the value");

            N curWeight;// = (DBNumber)_DefaultWeight.Clone(myWeight.Value);

            if (_WeightedListEntries.ContainsKey(myValue))
            {

                // In case, we changed a Hash to a Weighted we treat all null values as weight of 1
                if (_WeightedListEntries[myValue] == null)
                    _WeightedListEntries[myValue] = default(N);//.SetValue(1);

                curWeight = _WeightedListEntries[myValue];
                _WeightedListEntriesWeights[curWeight].Remove(myValue);

                if (_WeightedListEntriesWeights[curWeight].Count == 0)
                    _WeightedListEntriesWeights.Remove(curWeight);

                //curWeight = (N) curWeight.Clone();
                curWeight = myWeight; //curWeight.Add(myWeight);
                _WeightedListEntries[myValue] = curWeight;
            }
            else
            {
                curWeight = myWeight;//curWeight = (N)myWeight.Clone();
                _WeightedListEntries.Add(myValue, curWeight);
            }

            if (!_WeightedListEntriesWeights.ContainsKey(curWeight))
                _WeightedListEntriesWeights.Add(curWeight, new List<T>());
            _WeightedListEntriesWeights[curWeight].Add(myValue);

            _Count++;

        }

        public IEnumerable<KeyValuePair<T, N>> GetAll()
        {

            if (!_IsWeighted)
            {
                _WeightedListEntries.Keys.GetEnumerator();
            }
            else
            {
                foreach (KeyValuePair<N, List<T>> dbo in _WeightedListEntriesWeights)
                {
                    foreach (T t in dbo.Value)
                        yield return new KeyValuePair<T, N>(t, dbo.Key);
                }
            }
        }

        public IEnumerable<T> GetAllValues()
        {

            if (!_IsWeighted)
            {
                foreach (var dbo in _WeightedListEntries)
                    yield return dbo.Key;
            }
            else
            {
                foreach (List<T> dbos in _WeightedListEntriesWeights.Values)
                {
                    foreach (var dbo in dbos)
                        yield return dbo;
                }
            }
        }

        public IEnumerable<T> GetTop(UInt64 myNumOfEntries)
        {

            if (!_IsWeighted)
            {
                var rator = _WeightedListEntries.GetEnumerator();
                while (myNumOfEntries > 0 && rator.MoveNext())
                {
                    yield return rator.Current.Key;
                    myNumOfEntries--;
                }
            }
            else
            {

                SortedDictionary<N, List<T>>.Enumerator rator = _WeightedListEntriesWeights.GetEnumerator();

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

        }

        public WeightedSet<T, N> GetTopAsWeightedSet(UInt64 myNumOfEntries)
        {

            if (!_IsWeighted)
                throw new Exception("This SpezializedSet is not a Weighted!");

            var result = new WeightedSet<T, N>(_SortDirection, _IsWeighted);

            SortedDictionary<N, List<T>>.Enumerator rator = _WeightedListEntriesWeights.GetEnumerator();
            while (result.Count < myNumOfEntries && rator.MoveNext())
            {
                if (myNumOfEntries - result.Count >= (UInt64)rator.Current.Value.Count)
                {
                    //result.Add(rator.Current.Value, rator.Current.Key);
                    result._WeightedListEntriesWeights.Add(rator.Current.Key, rator.Current.Value);
                    result._Count += (UInt64)rator.Current.Value.Count;
                }
                else
                {
                    result._WeightedListEntriesWeights.Add(rator.Current.Key, rator.Current.Value.GetRange(0, (Int32)(myNumOfEntries - result.Count)));
                    result._Count += (UInt64)result._WeightedListEntriesWeights[rator.Current.Key].Count;
                }
            }

            return result;
        }

        public N GetMaxWeight()
        {

            if (!_IsWeighted)
                throw new Exception("This SpezializedSet is not a Weighted!");

            return _WeightedListEntriesWeights.ElementAt(0).Key;
        }

        public N GetMinWeight()
        {

            if (!_IsWeighted)
                throw new Exception("This SpezializedSet is not a Weighted!");

            return _WeightedListEntriesWeights.ElementAt(_WeightedListEntriesWeights.Count - 1).Key;
        }

        public override string ToString()
        {
            return String.Concat((_IsWeighted) ? "WeightedList<" : "HashSet<", typeof(T).Name, ">[", _WeightedListEntriesWeights.Count, "]");
        }

        public string GetDescInfo()
        {
            string retVal = "";

            if (_IsWeighted)
            {

                retVal = _DefaultWeight.ToString();//retVal = _DefaultWeight.Value.ToString();                

                switch (_SortDirection)
                { 
                    case SortDirection.Asc:
                        retVal += " ASC";
                        break;

                    case SortDirection.Desc:
                        retVal += " DESC";
                        break;
                }

            }

            return retVal;
        }
        
        public HashSet<T> GetAsHashSet()
        {
            return new HashSet<T>(_WeightedListEntries.Keys);
        }


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
            _WeightedListEntries = new Dictionary<T, N>();
            _WeightedListEntriesWeights = new SortedDictionary<N, List<T>>();
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

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _WeightedListEntries.GetEnumerator();
        }

        #endregion

        //#region IFastSerialize Members

        //bool IFastSerialize.isDirty
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //DateTime IFastSerialize.ModificationTime
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public void Serialize(ref SerializationWriter mySerializationWriter)
        //{

        //    #region Write

        //    mySerializationWriter.WriteObject(_IsWeighted);
        //    mySerializationWriter.WriteObject((byte)_SortDirection);
        //    this._DefaultWeight.ID.Serialize(ref mySerializationWriter);
        //    _DefaultWeight.Serialize(ref mySerializationWriter);

        //    mySerializationWriter.WriteObject(_WeightedListEntries.Count);

        //    foreach (KeyValuePair<T, N> keyValPair in _WeightedListEntries)
        //    {
        //        ((IFastSerialize)keyValPair.Key).Serialize(ref mySerializationWriter);
        //        if (_IsWeighted)
        //        {
        //            keyValPair.Value.Serialize(ref mySerializationWriter);
        //        }
        //    }

        //    #endregion

        //}

        //public void Deserialize(ref SerializationReader mySerializationReader)
        //{

        //    #region Read

        //    _IsWeighted = (Boolean)mySerializationReader.ReadObject();
        //    _SortDirection = (SortDirection)mySerializationReader.ReadObject();

        //    if (_SortDirection == SortDirection.Desc)
        //        _WeightedListEntriesWeights = new SortedDictionary<N, List<T>>(new SortDescendencyComparer());
        //    else
        //        _WeightedListEntriesWeights = new SortedDictionary<N, List<T>>();

        //    _WeightedListEntries = new Dictionary<T, N>();

        //    TypeUUID typeID = new TypeUUID();
        //    typeID.Deserialize(ref mySerializationReader);
        //    _DefaultWeight = (N) GraphTypeMapper.GetADBBaseObjectFromUUID(typeID, 0);
        //    _DefaultWeight.Deserialize(ref mySerializationReader);

        //    Int32 _WeightedListEntriesCount = (Int32)mySerializationReader.ReadObject();
        //    for (Int32 i = 0; i < _WeightedListEntriesCount; i++)
        //    {
        //        T newT = new T();
        //        ((IFastSerialize)newT).Deserialize(ref mySerializationReader);
        //        if (_IsWeighted)
        //        {
        //            N weight = (N)_DefaultWeight.Clone();
        //            weight.Deserialize(ref mySerializationReader);

        //            Add(newT, weight);
        //        }
        //        else
        //        {
        //            Add(newT);
        //        }

        //    }

        //    #endregion

        //}

        //#endregion

        public KeyValuePair<T, N> Get(T myKey)
        {
            return new KeyValuePair<T,N>(myKey, _WeightedListEntries[myKey]);
        }

        public void RemoveWhere(Predicate<T> match)
        {
            foreach (var dbo in _WeightedListEntries)
            {
                if (match(dbo.Key))
                {
                    if (_IsWeighted && _WeightedListEntriesWeights.ContainsKey(dbo.Value))
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

        public void AddRange(HashSet<T> hashSet, N dBNumber)
        {
            foreach (var elem in hashSet)
            {
                Add(elem, dBNumber);
            }
        }
    }

}



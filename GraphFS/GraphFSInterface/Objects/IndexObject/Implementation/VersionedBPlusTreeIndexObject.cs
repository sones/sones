/* 
 * VersionedBPlusTreeIndexObject
 * (c) Achim Friedland, 2009 - 2010
 * 
 * <developer>Martin Junghanns</developer>
 */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.Lib.NewFastSerializer;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.Indices;
using sones.Lib.DataStructures.Timestamp;
using sones.Lib.DataStructures.BPlusTree.Versioned;
using System.Runtime.Serialization;

#endregion


namespace sones.GraphFS.Objects
{
    public class VersionedBPlusTreeIndexObject
    {
        public const String Name = "BPLUSTREE";
    }

    public class VersionedBPlusTreeIndexObject<TKey, TValue> : AFSObject, IVersionedIndexObject<TKey, TValue>
        where TKey : IComparable
    {
        #region Data

        protected IVersionedBPlusTree<TKey, TValue> _VersionedBPlusTree;

        #endregion

        #region Properties

        #region HistorySize

        public UInt64 HistorySize
        {
            get
            {
                return _VersionedBPlusTree.HistorySize;
            }

            set
            {
                _VersionedBPlusTree.HistorySize = value;
            }
        }

        #endregion

        #endregion

        #region Constructors

        #region VersionedBPlusTreeIndexObject()

        public VersionedBPlusTreeIndexObject()
            : this (new VersionedBPlusTree<TKey, TValue>())
        {
        }

        #endregion

        #region VersionedBPlusTreeIndexObject(myIVersionedBPlusTree)

        /// <summary>
        /// This will create an empty AIndexObject using the given IDictionary object for the internal IDictionary&lt;TKey, HashSet&lt;TValue&gt;&gt; object.
        /// </summary>
        public VersionedBPlusTreeIndexObject(VersionedBPlusTree<TKey, TValue> myIVersionedBPlusTree)
        {

            // Members of AGraphStructure
            _StructureVersion       = 1;

            // Members of AGraphObject
            _ObjectStream           = FSConstants.DEFAULT_INDEXSTREAM;

            //datastructure
            _VersionedBPlusTree    = myIVersionedBPlusTree;
        }

        #endregion

        #region VersionedBPlusTreeIndexObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized IndexObject</param>
        public VersionedBPlusTreeIndexObject(String myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
            : this()
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);

        }

        #endregion

        #endregion


        #region Members of AGraphStructure

        #region SerializeInnerObject(ref mySerializationWriter)

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            SerializeObject(ref mySerializationWriter, 0);
        }

        #endregion

        #region SerializeObject(ref mySerializationWriter, myNotificationHandling)

        public void SerializeObject(ref SerializationWriter mySerializationWriter, UInt64 myNotificationHandling)
        {
            /* Format       := |NotificationHandling|HistorySize|TreeOrder|<Entries>|
             * Entry        := |Key|NoHistEntries|<HistEntries>|
             * HistEntry    := |Timestamp|NoAddSet|NoRemSet|AddSetVals|RemSetVals|
             */ 
            try
            {
                #region NotificationHandling

                mySerializationWriter.WriteUInt64(myNotificationHandling);

                #endregion

                #region HistorySize

                mySerializationWriter.WriteUInt64(_VersionedBPlusTree.HistorySize);

                #endregion

                #region Tree Order

                mySerializationWriter.WriteInt32(_VersionedBPlusTree.Order);

                #endregion

                #region Data

                var kvp_enumerator = _VersionedBPlusTree.GetKVPEnumerator();

                while(kvp_enumerator.MoveNext())
                {
                    #region data

                    var indexValueHistoryList = kvp_enumerator.Current.Value.InternalIndexValueHistoryList;

                    var numberOfHistoryEntries = indexValueHistoryList.Count;

                    #endregion

                    #region write key

                    mySerializationWriter.WriteObject(kvp_enumerator.Current.Key);

                    #endregion

                    #region write number of history entries

                    mySerializationWriter.WriteInt32(numberOfHistoryEntries);

                    #endregion

                    #region write history data

                    foreach (var indexValueHistory in indexValueHistoryList)
                    {
                        #region TimeStamp

                        mySerializationWriter.WriteUInt64(indexValueHistory.Timestamp);

                        #endregion

                        #region AddSetCount

                        mySerializationWriter.WriteInt32(indexValueHistory.AddSet.Count);

                        #endregion

                        #region RemSetCount

                        mySerializationWriter.WriteInt32(indexValueHistory.RemSet.Count);

                        #endregion

                        #region AddSet

                        foreach (var val in indexValueHistory.AddSet)
                        {
                            mySerializationWriter.WriteObject(val);  
                        }

                        #endregion

                        #region RemoveSet

                        foreach (var val in indexValueHistory.RemSet)
                        {
                            mySerializationWriter.WriteObject(val);
                        }

                        #endregion
                    }

                    #endregion
                }

                #endregion
            }
            catch (SerializationException e)
            {
                throw e;
            }
        }

        #endregion

        #region DeserializeInnerObject(ref mySerializationReader

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            /* Format       := |NotificationHandling|HistorySize|TreeOrder|<Entries>|
             * Entry        := |Key|NoHistEntries|<HistEntries>|
             * HistEntry    := |Timestamp|NoAddSet|NoRemSet|AddSetVals|RemSetVals|
             */ 
            try
            {
                #region NotificationHandling

                UInt64 _NotificationHandling = mySerializationReader.ReadUInt64();

                #endregion

                #region HistorySize

                var historySize = mySerializationReader.ReadUInt64();

                #endregion

                #region TreeOrder

                var treeOrder = mySerializationReader.ReadInt32();

                #endregion

                #region init tree

                var tree = new VersionedBPlusTree<TKey, TValue>(treeOrder);

                tree.HistorySize = historySize;

                #endregion

                #region key value pairs

                #region data

                LinkedList<IndexValueHistory<TValue>> historyList = null;
                var timeStamp   = 0UL;
                var addSetCount = 0;
                var remSetCount = 0;
                HashSet<TValue> addSet = null;
                HashSet<TValue> remSet = null;
                IndexValueHistoryList<TValue> tmp;
                
                #endregion

                while (mySerializationReader.BytesRemaining > 0)
                {
                    #region Key

                    var key = (TKey)mySerializationReader.ReadObject();

                    #endregion

                    #region contains check

                    if (tree.TryGetValue(key, out tmp, 0L))
                    {
                        historyList = tmp.InternalIndexValueHistoryList;
                    }
                    else
                    {
                        historyList = new LinkedList<IndexValueHistory<TValue>>();
                    }

                    #endregion

                    #region Number of History Entries

                    var numberOfHistEntries = mySerializationReader.ReadInt32();

                    #endregion

                    while (numberOfHistEntries > 0)
                    {
                        #region TimeStamp

                        timeStamp = mySerializationReader.ReadUInt64();

                        #endregion

                        #region AddSetCount

                        addSetCount = mySerializationReader.ReadInt32();

                        #endregion

                        #region RemSetCount

                        remSetCount = mySerializationReader.ReadInt32();

                        #endregion

                        #region AddSet

                        addSet = new HashSet<TValue>();
                        for (int i = 0; i < addSetCount; i++)
                        {
                            addSet.Add((TValue)mySerializationReader.ReadObject());
                        }

                        #endregion

                        #region RemoveSet

                        remSet = new HashSet<TValue>();
                        for (int i = 0; i < remSetCount; i++)
                        {
                            remSet.Add((TValue)mySerializationReader.ReadObject());
                        }

                        #endregion

                        //appended valuehistory has to be added as first
                        if ((historyList.First != null) && (timeStamp >= historyList.First.Value.Timestamp))
                        {
                            historyList.AddFirst(new IndexValueHistory<TValue>(timeStamp, addSet, remSet));
                        }
                        else
                        {
                            historyList.AddLast(new IndexValueHistory<TValue>(timeStamp, addSet, remSet));
                        }

                        numberOfHistEntries--;
                    }
                    
                    var value = new IndexValueHistoryList<TValue>();

                    value.InternalIndexValueHistoryList = historyList;

                    tree.Add(key, value);
                }

                #endregion

                #region assignment

                _VersionedBPlusTree = tree;

                #endregion
            }                
            catch (Exception e)
            {
                throw new Exception("VersionedBPlusTreeIndexObject<" + typeof(TKey).ToString() + ", " + typeof(TValue).ToString() + "> could not be deserialized!\n\n" + e);
            }
        }

        #endregion

        #endregion

        public override AFSObject Clone()
        {
            throw new NotImplementedException();
        }

        #region Members of IIndexObject

        #region Add

        #region Add(myKey, myValue)

        public void Add(TKey myKey, TValue myValue)
        {
            Set(myKey, myValue, IndexSetStrategy.MERGE);
        }

        #endregion

        #region Add(myKey, myValues)

        public void Add(TKey myKey, IEnumerable<TValue> myValues)
        {
            Set(myKey, myValues, IndexSetStrategy.MERGE);
        }

        #endregion

        #region Add(myKeyValuePair)

        public void Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            Add(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion

        #region Add(myKeyValuesPair)

        public void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair)
        {
            Add(myKeyValuesPair.Key, myKeyValuesPair.Value);
        }

        #endregion

        #region Add(myDictionary)

        public void Add(Dictionary<TKey, TValue> myDictionary)
        {
            Set(myDictionary, IndexSetStrategy.MERGE);
        }

        #endregion

        #region Add(myMultiValueDictionary)

        public void Add(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary)
        {
            Set(myMultiValueDictionary, IndexSetStrategy.MERGE);
        }

        #endregion

        #endregion

        #region Set

        #region Set(myKey, myValue, myIndexSetStrategy)

        #region (private) SetOnFileSystem(TKey myKey, HashSet<TValue> myValues, UInt64 myTimestamp, OP myOp)

        private void SetOnFileSystem(TKey myKey, HashSet<TValue> myValues, UInt64 myTimestamp, OP myOp)
        {
            #region data

            IGraphFSStream _IFSStream = null;

            bool serializeMe = false;

            #endregion

            #region persistency check

            if (_IGraphFSReference.IsPersistent())
            {
                _IFSStream = this._IGraphFSSessionReference.Value.OpenStream(ObjectLocation, _ObjectStream, _ObjectEdition, null, 0).Value;
                serializeMe = true;
            } else if (_IGraphFSSessionReference.IsPersistent())
            {
                _IFSStream = this._IGraphFSSessionReference.Value.OpenStream(ObjectLocation, _ObjectStream, _ObjectEdition, null, 0).Value;
                serializeMe = true;
            }

            #endregion

            #region serialize

            if (serializeMe)
            {
                var serializationWriter = new SerializationWriter();

                bool isAdd = (myOp == OP.ADD);

                #region prepare data

                #region data

                var numberOfHistoryEntries = 1;

                #endregion

                #region write key

                serializationWriter.WriteObject(myKey);

                #endregion

                #region write number of history entries

                serializationWriter.WriteObject(numberOfHistoryEntries);

                #endregion

                #region write history data

                #region TimeStamp

                serializationWriter.WriteObject(myTimestamp);

                #endregion

                #region AddSetCount

                serializationWriter.WriteObject((isAdd) ? myValues.Count : 0);

                #endregion

                #region RemSetCount

                serializationWriter.WriteObject((!isAdd) ? myValues.Count : 0);

                #endregion

                #region AddSet / RemoveSet

                foreach (var val in myValues)
                {
                    serializationWriter.WriteObject(val);
                }

                #endregion

                #endregion

                #endregion

                #region write

                var appendingData = serializationWriter.ToArray();

                _IFSStream.Write(appendingData, SeekOrigin.End);

                _IFSStream.Close();

                #endregion
            }

            #endregion
        }

        #endregion

        public void Set(TKey myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {
                _VersionedBPlusTree.Set(myKey, myValue, myIndexSetStrategy);

                SetOnFileSystem(myKey, new HashSet<TValue>() { myValue }, TimestampNonce.Ticks, OP.ADD);

                isDirty = true;
            }
        }

        #endregion

        #region Set(myKey, myValues, myIndexSetStrategy)

        public void Set(TKey myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {
                _VersionedBPlusTree.Set(myKey, myValues, myIndexSetStrategy);

                //SetOnFileSystem(myKey, new HashSet<TValue>(myValues), TimestampNonce.Ticks, OP.ADD);

                isDirty = true;
            }
        }

        #endregion

        #region Set(myKeyValuePair, myIndexSetStrategy)

        public void Set(KeyValuePair<TKey, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
        {
            Set(myKeyValuePair.Key, myKeyValuePair.Value, myIndexSetStrategy);
        }

        #endregion

        #region Set(myKeyValuesPair, myIndexSetStrategy)

        public void Set(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy)
        {
            Set(myKeyValuesPair.Key, myKeyValuesPair.Value, myIndexSetStrategy);
        }

        #endregion

        #region Set(myKeyValuePairs, myIndexSetStrategy)

        public void Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {
                _VersionedBPlusTree.Set(myKeyValuePairs, myIndexSetStrategy);

                var now = TimestampNonce.Ticks;

                foreach (var kvp in myKeyValuePairs)
                {
                    SetOnFileSystem(kvp.Key, new HashSet<TValue>() { kvp.Value }, now, OP.ADD);
                }

                isDirty = true;
            }
        }

        #endregion

        #region Set(myDictionary, myIndexSetStrategy)

        public void Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {
                _VersionedBPlusTree.Set(myDictionary, myIndexSetStrategy);

                var now = TimestampNonce.Ticks;

                foreach (var kvp in myDictionary)
                {
                    SetOnFileSystem(kvp.Key, new HashSet<TValue>() { kvp.Value }, now, OP.ADD);
                }

                isDirty = true;
            }
        }

        #endregion

        #region Set(myMultiValueDictionary, myIndexSetStrategy)

        public void Set(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {
                _VersionedBPlusTree.Set(myMultiValueDictionary, myIndexSetStrategy);

                var now = TimestampNonce.Ticks;

                foreach (var kvp in myMultiValueDictionary)
                {
                    SetOnFileSystem(kvp.Key, new HashSet<TValue>(kvp.Value), now, OP.ADD);
                }

                isDirty = true;
            }
        }

        #endregion

        #endregion

        #region Contains

        #region ContainsKey(myKey)

        public Trinary ContainsKey(TKey myKey)
        {
            return ContainsKey(myKey, 0);
        }

        #endregion

        #region ContainsValue(myValue)

        public Trinary ContainsValue(TValue myValue)
        {
            return ContainsValue(myValue, 0);
        }

        #endregion

        #region Contains(myKey, myValue)

        public Trinary Contains(TKey myKey, TValue myValue)
        {
            return Contains(myKey, myValue, 0);
        }

        #endregion

        #region Contains(myFunc)

        public Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {
                return _VersionedBPlusTree.Contains(myFunc);
            }
        }

        #endregion

        #endregion

        #region Get/Keys/Values/Enumerator

        #region this[myKey]

        public HashSet<TValue> this[TKey myKey]
        {
            get
            {
                lock (this)
                {
                    return _VersionedBPlusTree[myKey];
                }
            }
            set
            {
                _VersionedBPlusTree[myKey] = value;
            }
        }

        #endregion

        #region TryGetValue(myKey, out myValue)

        public Boolean TryGetValue(TKey myKey, out HashSet<TValue> myValue)
        {
            lock (this)
            {
                return _VersionedBPlusTree.TryGetValue(myKey, out myValue);
            }
        }

        #endregion


        #region Keys()

        public IEnumerable<TKey> Keys()
        {
            return Keys(0);
        }

        #endregion

        #region Keys(myFunc)

        public IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return Keys(myFunc, 0);
        }

        #endregion

        #region KeyCount()

        public UInt64 KeyCount()
        {
            return KeyCount(0);
        }

        #endregion

        #region KeyCount(myFunc)

        public UInt64 KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return KeyCount(myFunc, 0);
        }

        #endregion


        #region Values()

        public IEnumerable<HashSet<TValue>> Values()
        {
            return Values(0);
        }

        #endregion

        #region Values(myFunc)

        public IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return Values(myFunc, 0);
        }

        #endregion

        #region ValueCount()

        public UInt64 ValueCount()
        {
            return ValueCount(0);
        }

        #endregion

        #region ValueCount(myFunc)

        public UInt64 ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return ValueCount(myFunc, 0);
        }

        #endregion


        #region GetIDictionary()

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary()
        {
            return GetIDictionary(0);
        }

        #endregion

        #region GetIDictionary(myFunc)

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return GetIDictionary(myFunc, 0);
        }

        #endregion


        #region GetEnumerator()

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
        {
            return GetEnumerator(0);
        }

        #endregion

        #region GetEnumerator(myFunc)

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return GetEnumerator(myFunc, 0);
        }

        #endregion

        #endregion

        #region Remove/Clear

        #region Remove(myKey)

        public Boolean Remove(TKey myKey)
        {
            lock (this)
            {
                HashSet<TValue> values;
                if (_VersionedBPlusTree.TryGetValue(myKey, out values))
                {
                    _VersionedBPlusTree.Remove(myKey);

                    SetOnFileSystem(myKey, values, TimestampNonce.Ticks, OP.REM);

                    return true;
                }
                return false;

            }
        }

        #endregion

        #region Remove(myKey, myValue)

        public Boolean Remove(TKey myKey, TValue myValue)
        {
            lock (this)
            {
                if(_VersionedBPlusTree.Remove(myKey, myValue))
                {
                    SetOnFileSystem(myKey, new HashSet<TValue>() { myValue }, TimestampNonce.Ticks, OP.REM);

                    return true;
                }
                return false;
            }
        }

        #endregion

        #region Remove(myFunc)

        public Boolean Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {
                return _VersionedBPlusTree.Remove(myFunc);
            }
        }

        #endregion

        #region Clear()

        public void Clear()
        {
            _VersionedBPlusTree.Clear();
        }

        #endregion

        #endregion

        #region Range methods

        public IEnumerable<TValue> GreaterThan(TKey myKey, bool myOrEqual = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.GreaterThan(myKey, myOrEqual);
            }
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.GreaterThan(myKey, myFunc, myOrEqual);
            }
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, bool myOrEqual = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.LowerThan(myKey, myOrEqual);
            }
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.LowerThan(myKey, myFunc, myOrEqual);
            }
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.InRange(myFromKey, myToKey, myOrEqualFromKey, myOrEqualToKey);
            }
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.InRange(myFromKey, myToKey, myFunc, myOrEqualFromKey, myOrEqualToKey);
            }
        }

        #endregion

        #endregion

        #region IVersionedIndexObject<TKey,TValue> Members

        #region Contains

        #region ContainsKey(myKey, myVersion)

        public virtual Trinary ContainsKey(TKey myKey, Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.ContainsKey(myKey, myVersion);
            }
        }

        #endregion

        #region ContainsValue(myValue, myVersion)

        public virtual Trinary ContainsValue(TValue myValue, Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.ContainsValue(myValue, 0);
            }
        }

        #endregion

        #region Contains(myKey, myValue, myVersion)

        public virtual Trinary Contains(TKey myKey, TValue myValue, Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.Contains(myKey, myValue, myVersion);
            }
        }

        #endregion

        #region Contains(myFunc)

        public virtual Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.Contains(myFunc, myVersion);
            }
        }

        #endregion

        #endregion

        #region Get/Keys/Values/Enumerator

        #region this[myKey, myVersion]  // Int64

        public virtual HashSet<TValue> this[TKey myKey, Int64 myVersion]
        {
            get
            {
                lock (this)
                {
                    return _VersionedBPlusTree[myKey, myVersion];
                }
            }
        }

        #endregion

        #region this[myKey, myVersion]  // UInt64

        public virtual HashSet<TValue> this[TKey myKey, UInt64 myVersion]
        {
            get
            {
                lock (this)
                {
                    return _VersionedBPlusTree[myKey, (long) myVersion];
                }
            }
        }

        #endregion

        #region TryGetValue(myKey, out myValue, myVersion)

        public virtual Boolean TryGetValue(TKey myKey, out HashSet<TValue> myValue, Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.TryGetValue(myKey, out myValue, myVersion);
            }
        }

        #endregion

        #region Keys(myVersion)

        public virtual IEnumerable<TKey> Keys(Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.Keys(myVersion);
            }
        }

        #endregion

        #region Keys(myFunc, myVersion)

        public virtual IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.Keys(myFunc, myVersion);
            }
        }

        #endregion

        #region KeyCount(myVersion)

        public virtual UInt64 KeyCount(Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.KeyCount(myVersion);
            }
        }

        #endregion

        #region KeyCount(myFunc, myVersion)

        public virtual UInt64 KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.KeyCount(myFunc, myVersion);
            }
        }

        #endregion

        #region Values(myVersion)

        public virtual IEnumerable<HashSet<TValue>> Values(Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.Values(myVersion);
            }
        }

        #endregion

        #region Values(myFunc, myVersion)

        public virtual IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.Values(myFunc, myVersion);
            }
        }

        #endregion

        #region ValueCount(myVersion)

        public virtual UInt64 ValueCount(Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.ValueCount(myVersion);
            }
        }

        #endregion

        #region ValueCount(myFunc, myVersion)

        public virtual UInt64 ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.ValueCount(myFunc, myVersion);
            }
        }

        #endregion


        #region GetIDictionary(myVersion)

        public virtual IDictionary<TKey, HashSet<TValue>> GetIDictionary(Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.GetIDictionary(myVersion);
            }
        }

        #endregion

        #region GetIDictionary(myFunc, myVersion)

        public virtual IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.GetIDictionary(myFunc, myVersion);
            }
        }

        #endregion

        #region GetEnumerator(myVersion)

        public virtual IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.GetEnumerator(myVersion); 
            }
        }

        #endregion

        #region GetEnumerator(myFunc, myVersion)

        public virtual IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {
                return _VersionedBPlusTree.GetEnumerator(myFunc, myVersion);
            }
        }

        #endregion

        #endregion

        #region Additional methods

        #region VersionCount(myKey)

        public virtual UInt64 VersionCount(TKey myKey)
        {
            lock (this)
            {
                return _VersionedBPlusTree.VersionCount(myKey);
            }
        }

        #endregion

        #region ClearHistory(myKey)

        public virtual void ClearHistory(TKey myKey)
        {
            lock (this)
            {
                _VersionedBPlusTree.ClearHistory(myKey);
            }
        }

        #endregion

        #endregion

        #region Range methods

        public IEnumerable<TValue> GreaterThan(TKey myKey, long myVersion, bool myOrEqual = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.GreaterThan(myKey, myVersion, myOrEqual);
            }
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqual = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.GreaterThan(myKey, myFunc, myVersion, myOrEqual);
            }
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, long myVersion, bool myOrEqual = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.LowerThan(myKey, myVersion, myOrEqual);
            }
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqual = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.LowerThan(myKey, myFunc, myVersion, myOrEqual);
            }
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, long myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.InRange(myFromKey, myToKey, myVersion, myOrEqualFromKey, myOrEqualToKey);
            }
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            lock (this)
            {
                return _VersionedBPlusTree.InRange(myFromKey, myToKey, myFunc, myVersion, myOrEqualFromKey, myOrEqualToKey);
            }
        }

        #endregion

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this)
            {
                return _VersionedBPlusTree.GetEnumerator();
            }
        }

        #endregion

        #region Additional IIndexObject<TKey,TValue> Members

        public IIndexObject<TKey, TValue> GetNewInstance()
        {
            return new VersionedBPlusTreeIndexObject<TKey, TValue>();
        }

        #endregion

        #region Additional IIndexObject2<TKey,TValue> Members

        public IVersionedIndexObject<TKey, TValue> GetNewInstance2()
        {
            return new VersionedBPlusTreeIndexObject<TKey, TValue>();
        }

        #endregion

        #region Additional IIndexInterface<TKey,TValue> Members

        public string IndexName
        {
            get { return VersionedBPlusTreeIndexObject.Name; }
        }

        public IEnumerable<TValue> GetValues()
        {
            lock (this)
            {
                return _VersionedBPlusTree.GetValues();
            }
        }

        #endregion
    }
}

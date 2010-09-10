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

/* 
 * MetadataObject
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.StorageEngines;
using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.Indices;
using sones.GraphFS.InternalObjects;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The abstract class for all Graph metadata objects
    /// and virtual metadata objects.
    /// </summary>
    public class MetadataObject<TValue> : AIndexObject<String, TValue>, IMetadataObject<TValue>, IDirectoryListing
    {

        #region Constructor

        #region MetadataObject()

        /// <summary>
        /// This will create an empty MetadataObject
        /// </summary>
        public MetadataObject()
        {

            // Members of AGraphStructure
            _StructureVersion   = 1;

            // Members of AGraphObject
            _ObjectStream       = FSConstants.DIRECTORYSTREAM;

            // Object specific data...
            _IIndexObject       = new HashIndexObject<String, TValue>();

        }

        #endregion


        #region MetadataObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">The ObjectLocation</param>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized MetadataObject</param>
        public MetadataObject(Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);
            _isNew = false;

        }

        #endregion

        #endregion


        #region Members of AGraphObject

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new MetadataObject<TValue>();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion


        #region IMetadataObject<TValue> Members

        #region Add

        #region Add(myKey, myValue)

        public new void Add(String myKey, TValue myValue)
        {
            base.Add(myKey, myValue);
        }

        #endregion

        #region Add(params myFunc)

        /// <summary>
        /// Another Add-method using reflection on the name of the Func parameter
        /// </summary>
        /// <param name="myFunc"></param>
        public void Add(params Func<String, TValue>[] myFunc)
        {
            foreach (var _Func in myFunc)
                base.Add(_Func.Method.GetParameters()[0].Name, _Func(null));
        }

        #endregion

        #region Add(myValues)

        public new void Add(String myKey, IEnumerable<TValue> myValues)
        {
            base.Add(myKey, myValues);
        }

        #endregion

        #region Add(myKeyValuePair)

        public new void Add(KeyValuePair<String, TValue> myKeyValuePair)
        {
            base.Add(myKeyValuePair);
        }

        #endregion

        #region Add(myKeyValuesPair)

        public new void Add(KeyValuePair<String, IEnumerable<TValue>> myKeyValuesPair)
        {
            base.Add(myKeyValuesPair);
        }

        #endregion

        #region Add(myDictionary)

        public new void Add(Dictionary<String, TValue> myDictionary)
        {
            base.Add(myDictionary);
        }

        #endregion

        #region Add(myMultiValueDictionary)

        public new void Add(Dictionary<String, IEnumerable<TValue>> myMultiValueDictionary)
        {
            base.Add(myMultiValueDictionary);
        }

        #endregion

        #endregion

        #region Set

        #region Set(myKey, myValue, myIndexSetStrategy)

        public new void Set(String myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy)
        {
            base.Set(myKey, myValue, myIndexSetStrategy);
        }

        #endregion

        #region Set(myKey, myValues, myIndexSetStrategy)

        public new void Set(String myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
        {
            base.Set(myKey, myValues, myIndexSetStrategy);
        }

        #endregion

        #region Set(myKeyValuePair, myIndexSetStrategy)

        public new void Set(KeyValuePair<String, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
        {
            Set(myKeyValuePair.Key, myKeyValuePair.Value, myIndexSetStrategy);
        }

        #endregion

        #region Set(myKeyValuesPair, myIndexSetStrategy)

        public new void Set(KeyValuePair<String, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy)
        {
            base.Set(myKeyValuesPair, myIndexSetStrategy);
        }

        #endregion

        #region Set(myKeyValuePairs, myIndexSetStrategy)

        public new void Set(IEnumerable<KeyValuePair<String, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
        {
            base.Set(myKeyValuePairs, myIndexSetStrategy);
        }

        #endregion

        #region Set(myDictionary, myIndexSetStrategy)

        public new void Set(Dictionary<String, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            base.Set(myDictionary, myIndexSetStrategy);
        }

        #endregion

        #region Set(myMultiValueDictionary, myIndexSetStrategy)

        public new void Set(Dictionary<String, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            base.Set(myMultiValueDictionary, myIndexSetStrategy);
        }

        #endregion

        #endregion



        #region Contains

        #region ContainsKey(myKey)

        public new Trinary ContainsKey(String myKey)
        {
            return base.ContainsKey(myKey);
        }

        #endregion

        #region ContainsValue(myValue)

        public new Trinary ContainsValue(TValue myValue)
        {
            return base.ContainsValue(myValue);
        }

        #endregion

        #region Contains(myKey, myValue)

        public new Trinary Contains(String myKey, TValue myValue)
        {
            return base.Contains(myKey, myValue);
        }

        #endregion

        #region Contains(myFunc)

        public new Trinary Contains(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return base.Contains(myFunc);
        }

        #endregion

        #endregion

        #region Get/GetXInRange/Count

        #region this[myKey]

        public new HashSet<TValue> this[String myKey]
        {

            get
            {
                return base[myKey];
            }

            set
            {
                base[myKey] = value;
            }

        }

        #endregion

        #region TryGetValue(myKey, out myValue)

        public new Boolean TryGetValue(String myKey, out HashSet<TValue> myValue)
        {
            return base.TryGetValue(myKey, out myValue);
        }

        #endregion


        #region Keys()

        public new IEnumerable<String> Keys()
        {
            return base.Keys();
        }

        #endregion

        #region Keys(myFunc)

        public new IEnumerable<String> Keys(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return base.Keys(myFunc);
        }

        #endregion

        #region KeyCount()

        public new UInt64 KeyCount()
        {
            return base.KeyCount();
        }

        #endregion

        #region KeyCount(myFunc)

        public new UInt64 KeyCount(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return base.KeyCount(myFunc);
        }

        #endregion


        #region Values()

        public new IEnumerable<HashSet<TValue>> Values()
        {
            return base.Values();
        }

        #endregion

        #region Values(myFunc)

        public new IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return base.Values(myFunc);
        }

        #endregion

        #region ValueCount()

        public new UInt64 ValueCount()
        {
            return base.ValueCount();
        }

        #endregion

        #region ValueCount(myFunc)

        public new UInt64 ValueCount(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return base.ValueCount(myFunc);
        }

        #endregion


        #region GetIDictionary()

        public new IDictionary<String, HashSet<TValue>> GetIDictionary()
        {
            return base.GetIDictionary();
        }

        #endregion

        #region GetIDictionary(myFunc)

        public new IDictionary<String, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return base.GetIDictionary(myFunc);
        }

        #endregion


        #region GetEnumerator()

        public new IEnumerator<KeyValuePair<String, HashSet<TValue>>> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion

        #region GetEnumerator()

        public new IEnumerator<KeyValuePair<String, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return base.GetEnumerator(myFunc);
        }

        #endregion

        #endregion

        #region Remove/Clear

        #region Remove(myKey)

        public new Boolean Remove(String myKey)
        {
            return base.Remove(myKey);
        }

        #endregion

        #region Remove(myKey, myValue)

        public new Boolean Remove(String myKey, TValue myValue)
        {
            return base.Remove(myKey, myValue);
        }

        #endregion

        #region Remove(myFunc)

        public new Boolean Remove(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return base.Remove(myFunc);
        }

        #endregion

        #region Clear()

        public new void Clear()
        {
            base.Clear();
        }

        #endregion

        #endregion

        #region NotificationHandling

        #region NotificationHandling

        private Object _IMetadataObject_NotificationHandlingLock = new Object();

        private NHIMetadataObject _IMetadataObject_NotificationHandling;

        NHIMetadataObject IMetadataObject<TValue>.NotificationHandling
        {
            get
            {
                return _IMetadataObject_NotificationHandling;
            }
        }

        #endregion

        #region SubscribeNotification(myNotificationHandling)

        public void SubscribeNotification(NHIMetadataObject myNotificationHandling)
        {
            lock (_IMetadataObject_NotificationHandlingLock)
            {
                _IMetadataObject_NotificationHandling |= myNotificationHandling;
            }
        }

        #endregion

        #region UnsubscribeNotification(myNotificationHandling)

        public void UnsubscribeNotification(NHIMetadataObject myNotificationHandling)
        {
            lock (_IMetadataObject_NotificationHandlingLock)
            {
                _IMetadataObject_NotificationHandling &= ~myNotificationHandling;
            }
        }

        #endregion

        #endregion

        #endregion


        #region IDirectoryObject Members

        #region IGraphFSReference

        private IGraphFS _IGraphFSReference;

        public IGraphFS IGraphFSReference
        {

            get
            {
                return _IGraphFSReference;
            }

            set
            {
                _IGraphFSReference = value;
            }

        }

        #endregion

        public void AddObjectStream(String myObjectName, String myObjectStream)
        {
            throw new NotImplementedException();
        }

        #region AddObjectStream(myObjectName, myObjectStream, myINodePositions)

        public void AddObjectStream(String myObjectName, String myObjectStream, IEnumerable<ExtendedPosition> myINodePositions)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region ObjectExists(myObjectName)

        public Trinary ObjectExists(String myObjectName)
        {
            return base.ContainsKey(myObjectName);
        }

        #endregion

        #region  ObjectStreamExists(myObjectName, myObjectStream)

        public Trinary ObjectStreamExists(String myObjectName, String myObjectStream)
        {

            if (base.ContainsKey(myObjectName) == Trinary.TRUE && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region GetObjectStreamsList(myObjectName)

        public IEnumerable<String> GetObjectStreamsList(String myObjectName)
        {
            return new List<String> { FSConstants.INLINEDATA };
        }

        #endregion


        #region RemoveObjectStream(myObjectName, myObjectStream)

        public void RemoveObjectStream(String myObjectName, String myObjectStream)
        {

            if (myObjectStream == FSConstants.INLINEDATA)
                Remove(myObjectName);

        }

        #endregion

        #region GetObjectINodePositions(myObjectName)

        public IEnumerable<ExtendedPosition> GetObjectINodePositions(String myObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion 

        #region GetDirectoryEntry(myObjectName)

        public DirectoryEntry GetDirectoryEntry(String myObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region StoreInlineData(myObjectName, myInlineData, myAllowOverwritting)

        public void StoreInlineData(String myObjectName, Byte[] myInlineData, Boolean myAllowOverwritting)
        {
            //Set(myObjectName, (T) myInlineData);
        }

        #endregion

        #region GetInlineData(myObjectName)

        public Byte[] GetInlineData(String myObjectName)
        {
            return Encoding.UTF8.GetBytes(base[myObjectName].ToString());
        }

        #endregion

        #region hasInlineData(String myObjectName)

        public Trinary hasInlineData(String myObjectName)
        {

            if (base.ContainsKey(myObjectName) == Trinary.TRUE)
                return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region DeleteInlineData(String myObjectName)

        public void DeleteInlineData(String myObjectName)
        {
            base.Remove(myObjectName);
        }

        #endregion


        #region AddSymlink(myObjectName, myTargetObject)

        public void AddSymlink(String myObjectName, String myTargetObject)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetSymlink(myObjectName)

        public ObjectLocation GetSymlink(String myObjectName)
        {
            return null;
        }

        #endregion

        #region isSymlink(myObjectName)

        public Trinary isSymlink(String myObjectName)
        {
            return Trinary.FALSE;
        }

        #endregion

        #region RemoveSymlink(myObjectName)

        public void RemoveSymlink(String myObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region GetDirectoryListing()

        public IEnumerable<String> GetDirectoryListing()
        {

            var _DirectoryListing = new List<String>();

            foreach (var _DirectoryEntry in base.Keys())
                _DirectoryListing.Add(_DirectoryEntry);

            return _DirectoryListing;

        }

        #endregion

        #region GetDirectoryListing(myFunc)

        public IEnumerable<String> GetDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetDirectoryListing(myLogin, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

        public IEnumerable<String> GetDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
        {
            return GetDirectoryListing();
        }

        #endregion

        #region GetExtendedDirectoryListing()

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing()
        {

            var _Output           = new List<DirectoryEntryInformation>();
            var _OutputParameter  = new DirectoryEntryInformation();

            foreach (var _DirectoryEntry in base.Keys())
            {
                _OutputParameter.Name         = _DirectoryEntry;
                _OutputParameter.Streams  = new HashSet<String> { FSConstants.INLINEDATA };
                _Output.Add(_OutputParameter);
            }

            return _Output;

        }

        #endregion

        #region GetExtendedDirectoryListing(myFunc)

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetExtendedDirectoryListing(myLogin, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
        {
            return GetExtendedDirectoryListing();
        }

        #endregion


        #region DirCount()

        public UInt64 DirCount
        {
            get
            {
                return (UInt64)GetDirectoryListing().LongCount();
            }
        }

        #endregion


        #region NotificationHandling

        private Object _IDirectoryObject_NotificationHandlingLock = new Object();

        private NHIDirectoryObject _IDirectoryObject_NotificationHandling;

        /// <summary>
        /// Returns the NotificationHandling bitfield that indicates which
        /// notifications should be triggered.
        /// </summary>
        NHIDirectoryObject IDirectoryListing.NotificationHandling
        {
            get
            {
                return _IDirectoryObject_NotificationHandling;
            }
        }

        #endregion

        #region SubscribeNotification(myNotificationHandling)

        /// <summary>
        /// This method adds the given NotificationHandling flags.
        /// </summary>
        /// <param name="myNotificationHandling">The NotificationHandlings to be added.</param>
        public void SubscribeNotification(NHIDirectoryObject myNotificationHandling)
        {
            lock (_IMetadataObject_NotificationHandlingLock)
            {
                _IDirectoryObject_NotificationHandling |= myNotificationHandling;
            }
        }

        #endregion

        #region UnsubscribeNotification(myNotificationHandling)

        /// <summary>
        /// This method removes the given NotificationHandling flags.
        /// </summary>
        /// <param name="myNotificationHandling">The NotificationHandlings to be removed.</param>
        public void UnsubscribeNotification(NHIDirectoryObject myNotificationHandling)
        {
            lock (_IMetadataObject_NotificationHandlingLock)
            {
                _IDirectoryObject_NotificationHandling &= ~myNotificationHandling;
            }
        }

        #endregion

        #endregion

    }

}

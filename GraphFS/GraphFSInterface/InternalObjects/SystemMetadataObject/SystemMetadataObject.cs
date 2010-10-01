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

///* GraphFS - UserMetadataObject
// * (c) Achim Friedland, 2008 - 2009
// * 
// * Lead programmer:
// *      Achim Friedland
// * 
// * */

//#region Usings

//using System;
//using System.Text;

//using sones.Graph.Storage.GraphFS.Datastructures;
//using sones.Graph.Storage.GraphFS.Objects;

//using sones.Lib.Cryptography.IntegrityCheck;
//using sones.Lib.Cryptography.SymmetricEncryption;
//using sones.Graph.Storage.GraphFS.InternalObjects;
//using sones.Lib.DataStructures;
//using sones.GraphFS.Objects;

//#endregion

//namespace sones.GraphFS.InternalObjects
//{

//    /// <summary>
//    ///  A MetadataObject to store user defined information.
//    /// </summary>

//    public class SystemMetadataObject : MetadataObject<Object>, IDirectoryObject
//    {


//        #region Constructor

//        #region SystemMetadataObject()

//        /// <summary>
//        /// This will create an empty MetadataObject
//        /// </summary>
//        public SystemMetadataObject()
//        {

//            // Members of AGraphStructure
//            _StructureVersion   = 1;

//            // Members of AGraphObject
//            _ObjectStream       = FSConstants.SYSTEMMETADATASTREAM;

//            // Object specific data...
////            _IndexHashTable     = new Dictionary<String, List<T>>();

//        }

//        #endregion


//        #region SystemMetadataObject(myObjectLocation)

//        /// <summary>
//        /// This will create an empty SystemMetadataObject
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        public SystemMetadataObject(ObjectLocation myObjectLocation)
//            : this()
//        {

//            if (myObjectLocation == null || myObjectLocation.Length < FSPathConstants.PathDelimiter.Length)
//                throw new ArgumentNullException("Invalid ObjectLocation!");

//            // Set the property in order to automagically set the
//            // ObjectPath and ObjectName
//            ObjectLocation      = myObjectLocation;

//        }

//        #endregion

//        #region SystemMetadataObject(myObjectLocation, myObjectStream)

//        /// <summary>
//        /// This will create a SystemMetadataObject with the given ObjectLocation and ObjectStream.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        public SystemMetadataObject(ObjectLocation myObjectLocation, String myObjectStream)
//            : this(myObjectLocation)
//        {

//            if (myObjectStream == null || myObjectStream.Length == 0)
//                throw new ArgumentNullException("Invalid ObjectStream!");

//            _ObjectStream = myObjectStream;

//        }

//        #endregion

//        #region SystemMetadataObject(myObjectLocation, myObjectStream, myObjectEdition)

//        /// <summary>
//        /// This will create a SystemMetadataObject with the given ObjectLocation, ObjectStream and ObjectEdition.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        /// <param name="myObjectEdition">the ObjectEdition</param>
//        public SystemMetadataObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition)
//            : this(myObjectLocation, myObjectStream)
//        {

//            if (myObjectEdition == null || myObjectEdition.Length == 0)
//                _ObjectEdition = FSConstants.DefaultEdition;

//            else
//                _ObjectEdition = myObjectEdition;

//        }

//        #endregion

//        #region SystemMetadataObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID)

//        /// <summary>
//        /// This will create a SystemMetadataObject with the given ObjectLocation, ObjectStream, ObjectEdition and ObjectRevisionID.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        /// <param name="myObjectEdition">the ObjectEdition</param>
//        /// <param name="myObjectRevision">the RevisionID of the AGraphObject</param>
//        public SystemMetadataObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID)
//            : this(myObjectLocation, myObjectStream, myObjectEdition)
//        {

//            if (myObjectRevisionID == null)
//                throw new ArgumentNullException("Invalid ObjectRevisionID!");

//            else
//            {

//                if (myObjectRevisionID.UUID == null)
//                    throw new ArgumentNullException("Invalid ObjectRevisionID UUID!");

//                else
//                    _ObjectRevisionID = myObjectRevisionID;

//            }

//        }

//        #endregion

//        #region SystemMetadataObject(myObjectLocation, myObjectRevisionID)

//        /// <summary>
//        /// This will create a SystemMetadataObject with the given ObjectLocation and ObjectRevisionID.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectRevision">the RevisionID of the AGraphObject</param>
//        public SystemMetadataObject(ObjectLocation myObjectLocation, RevisionID myObjectRevisionID)
//            : this(myObjectLocation)
//        {

//            if (myObjectRevisionID == null)
//                throw new ArgumentNullException("Invalid ObjectRevisionID!");

//            else
//            {

//                if (myObjectRevisionID.UUID == null)
//                    throw new ArgumentNullException("Invalid ObjectRevisionID UUID!");

//                else
//                    _ObjectRevisionID = myObjectRevisionID;

//            }

//        }

//        #endregion


//        #region SystemMetadataObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

//        /// <summary>
//        /// A constructor used for fast deserializing
//        /// </summary>
//        /// <param name="myObjectLocation">The ObjectLocation</param>
//        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized MetadataObject</param>
//        public SystemMetadataObject(ObjectLocation myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
//            : this(myObjectLocation)
//        {

//            if (mySerializedData == null || mySerializedData.Length == 0)
//                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

//            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);
//            _isNew = false;

//        }

//        #endregion

//        #endregion


//        #region Members of AGraphObject

//        #region Clone()

//        public override AGraphObject Clone()
//        {

//            var newT = new UserMetadataObject();
//            newT.Deserialize(Serialize(null, null, false), null, null, this);

//            return newT;

//        }

//        #endregion

//        #endregion

//        //#region IMetadataObject<T> Members

//        //#region Set

//        //#region Set(myKey, myValue, myIndexSetStrategy)

//        //public new void Set(String myKey, T myValue, IndexSetStrategy myIndexSetStrategy)
//        //{
//        //    base.Set(myKey, myValue, myIndexSetStrategy);
//        //}

//        //#endregion

//        //#region Set(myKey, myListOfValues, myIndexSetStrategy)

//        //public new void Set(String myKey, List<T> myListOfValues, IndexSetStrategy myIndexSetStrategy)
//        //{
//        //    base.Set(myKey, myListOfValues, myIndexSetStrategy);
//        //}

//        //#endregion

//        //#region Set(myKeyValuePair, myIndexSetStrategy)

//        //public new void Set(KeyValuePair<String, T> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
//        //{
//        //    Set(myKeyValuePair.Key, myKeyValuePair.Value, myIndexSetStrategy);
//        //}

//        //#endregion

//        //#region Set(myKeyListOfValuesPair, myIndexSetStrategy)

//        //public new void Set(KeyValuePair<String, List<T>> myKeyListOfValuesPair, IndexSetStrategy myIndexSetStrategy)
//        //{
//        //    base.Set(myKeyListOfValuesPair, myIndexSetStrategy);
//        //}

//        //#endregion


//        //#region Set(myListOfKeyListOfValuesPairs, myIndexSetStrategy)

//        //public new void Set(List<KeyValuePair<String, T>> myListOfKeyListOfValuesPairs, IndexSetStrategy myIndexSetStrategy)
//        //{
//        //    base.Set(myListOfKeyListOfValuesPairs, myIndexSetStrategy);
//        //}

//        //#endregion

//        //#region Set(myListOfKeyListOfValuesPairs, myIndexSetStrategy)

//        //public new void Set(List<KeyValuePair<String, List<T>>> myListOfKeyListOfValuesPairs, IndexSetStrategy myIndexSetStrategy)
//        //{
//        //    base.Set(myListOfKeyListOfValuesPairs, myIndexSetStrategy);
//        //}

//        //#endregion

//        //#region Set(myDictionary, myIndexSetStrategy)

//        //public new void Set(Dictionary<String, T> myDictionary, IndexSetStrategy myIndexSetStrategy)
//        //{
//        //    base.Set(myDictionary, myIndexSetStrategy);
//        //}

//        //#endregion

//        //#region Set(myDictionary, myIndexSetStrategy)

//        //public new void Set(Dictionary<String, List<T>> myDictionary, IndexSetStrategy myIndexSetStrategy)
//        //{
//        //    base.Set(myDictionary, myIndexSetStrategy);
//        //}

//        //#endregion

//        //#endregion

//        //#region Contains/Keys/Values/GetEnumerator

//        //#region ContainsKey(myKey)

//        //public new Boolean ContainsKey(String myKey)
//        //{
//        //    return base.ContainsKey(myKey);
//        //}

//        //#endregion

//        //#region ContainsValue(myValue)

//        //public new Boolean ContainsValue(T myValue)
//        //{
//        //    return base.ContainsValue(myValue);
//        //}

//        //#endregion

//        //#region Contains(myKeyValuePair)

//        //public new Boolean Contains(KeyValuePair<String, T> myKeyValuePair)
//        //{
//        //    return base.Contains(myKeyValuePair);
//        //}

//        //#endregion

//        //#region Keys

//        //public new ICollection<String> Keys
//        //{
//        //    get
//        //    {
//        //        return base.Keys;
//        //    }
//        //}

//        //#endregion

//        //#region Values

//        //public new ICollection<List<T>> Values
//        //{
//        //    get
//        //    {
//        //        return base.Values;
//        //    }
//        //}

//        //#endregion

//        //#region GetEnumerator()

//        //public new IEnumerator<KeyValuePair<String, List<T>>> GetEnumerator()
//        //{
//        //    return base.GetEnumerator();
//        //}

//        //#endregion

//        //#endregion

//        //#region Get/GetXInRange/Count

//        //#region this[myKey]

//        //public new List<T> this[String myKey]
//        //{

//        //    get
//        //    {
//        //        return base[myKey];
//        //    }

//        //    set
//        //    {
//        //        base[myKey] = value;
//        //    }

//        //}

//        //#endregion

//        //#region TryGetValue(myKey, out myValue)

//        //public new Boolean TryGetValue(String myKey, out List<T> myValue)
//        //{
//        //    return base.TryGetValue(myKey, out myValue);
//        //}

//        //#endregion

//        //#region GetListOfKeyValuePairs()

//        //public new List<KeyValuePair<String, List<T>>> GetListOfKeyValuePairs()
//        //{
//        //    return base.GetListOfKeyValuePairs();
//        //}

//        //#endregion

//        //#region GetListOfKeyValuePairs(myPrefix)

//        //public List<KeyValuePair<String, List<T>>> GetListOfKeyValuePairs(String myPrefix)
//        //{

//        //    List<KeyValuePair<String, List<T>>> returnValue         = new List<KeyValuePair<String, List<T>>>();
//        //    List<KeyValuePair<String, List<T>>> ListOfKeyValuePairs = base.GetListOfKeyValuePairs();

//        //    foreach (KeyValuePair<String, List<T>> _KeyValuePair in ListOfKeyValuePairs)
//        //        if (_KeyValuePair.Key.StartsWith(myPrefix))
//        //            returnValue.Add(_KeyValuePair);

//        //    return returnValue;

//        //}

//        //#endregion

//        //#region GetDictionary()

//        //public new Dictionary<String, List<T>> GetDictionary()
//        //{
//        //    return base.GetDictionary();
//        //}

//        //#endregion

//        //#region GetDictionary(myPrefix)

//        //public Dictionary<String, List<T>> GetDictionary(String myPrefix)
//        //{

//        //    Dictionary<String, List<T>> returnValue  = new Dictionary<String, List<T>>();
//        //    Dictionary<String, List<T>> _Dictionary  = base.GetDictionary();

//        //    foreach (KeyValuePair<String, List<T>> _KeyValuePair in _Dictionary)
//        //        if (_KeyValuePair.Key.StartsWith(myPrefix))
//        //            returnValue.Add(_KeyValuePair.Key, _KeyValuePair.Value);

//        //    return returnValue;

//        //}

//        //#endregion



//        //#region GetKeysInRange(myMinValue, myMaxValue)

//        //public new List<String> GetKeysInRange(String myMinKey, String myMaxKey)
//        //{
//        //    return base.GetKeysInRange(myMinKey, myMaxKey);
//        //}

//        //#endregion

//        //#region GetValuesInRange(myMinValue, myMaxValue)

//        //public new List<T> GetValuesInRange(String myMinValue, String myMaxValue)
//        //{
//        //    return base.GetValuesInRange(myMinValue, myMaxValue);
//        //}

//        //#endregion

//        //#region GetListOfKeyValuePairsInRange(myMinKey, myMaxKey)

//        //public new List<KeyValuePair<String, List<T>>> GetListOfKeyValuePairsInRange(String myMinKey, String myMaxKey)
//        //{
//        //    return base.GetListOfKeyValuePairsInRange(myMinKey, myMaxKey);
//        //}

//        //#endregion

//        //#region GetDictionaryInRange(myMinKey, myMaxKey)

//        //public new Dictionary<String, List<T>> GetDictionaryInRange(String myMinKey, String myMaxKey)
//        //{
//        //    return base.GetDictionaryInRange(myMinKey, myMaxKey);
//        //}

//        //#endregion


//        //#region Count

//        //public new Int32 Count
//        //{
//        //    get
//        //    {
//        //        return base.Count;
//        //    }
//        //}

//        //#endregion

//        //#region Count64

//        //public new UInt64 Count64
//        //{
//        //    get
//        //    {
//        //        return (UInt64) base.Count;
//        //    }
//        //}

//        //#endregion

//        //#endregion

//        //#region Remove/Clear

//        //#region Remove(myKey)

//        //public new Boolean Remove(String myKey)
//        //{
//        //    return base.Remove(myKey);
//        //}

//        //#endregion

//        //#region Remove(myListOfKeys)

//        //public new Boolean Remove(List<String> myListOfKeys)
//        //{
//        //    return base.Remove(myListOfKeys);
//        //}

//        //#endregion

//        //#region Remove(myKey, myValue)

//        //public new Boolean Remove(String myKey, T myValue)
//        //{
//        //    return base.Remove(myKey, myValue);
//        //}

//        //#endregion

//        //#region Remove(myKey, myValue)

//        //public new Boolean Remove(String myKey, List<T> myValue)
//        //{
//        //    return base.Remove(myKey, myValue);
//        //}

//        //#endregion

//        //#region Remove(myKeyValuePair)

//        //public new Boolean Remove(KeyValuePair<String, T> myKeyValuePair)
//        //{
//        //    return base.Remove(myKeyValuePair.Key, myKeyValuePair.Value);
//        //}

//        //#endregion

//        //#region Remove(myKeyValuePair)

//        //public new Boolean Remove(KeyValuePair<String, List<T>> myKeyValuePair)
//        //{
//        //    return base.Remove(myKeyValuePair.Key, myKeyValuePair.Value);
//        //}

//        //#endregion

//        //#region Remove(myListOfKeyValuePairs)

//        //public new Boolean Remove(List<KeyValuePair<String, T>> myListOfKeyValuePairs)
//        //{
//        //    return base.Remove(myListOfKeyValuePairs);
//        //}

//        //#endregion

//        //#region Remove(myListOfKeyValuePairs)

//        //public new Boolean Remove(List<KeyValuePair<String, List<T>>> myListOfKeyListOfValuePairs)
//        //{
//        //    return base.Remove(myListOfKeyListOfValuePairs);
//        //}

//        //#endregion

//        //#region Clear()

//        //public new void Clear()
//        //{
//        //    _IndexHashTable.Clear();
//        //}

//        //#endregion

//        //#endregion

//        //#region NotificationHandling

//        //#region NotificationHandling

//        //private Object _IMetadataObject_NotificationHandlingLock = new Object();

//        //private NHIMetadataObject _IMetadataObject_NotificationHandling;

//        //NHIMetadataObject IMetadataObject<T>.NotificationHandling
//        //{
//        //    get
//        //    {
//        //        return _IMetadataObject_NotificationHandling;
//        //    }
//        //}

//        //#endregion

//        //#region SubscribeNotification(myNotificationHandling)

//        //public void SubscribeNotification(NHIMetadataObject myNotificationHandling)
//        //{
//        //    lock (_IMetadataObject_NotificationHandlingLock)
//        //    {
//        //        _IMetadataObject_NotificationHandling |= myNotificationHandling;
//        //    }
//        //}

//        //#endregion

//        //#region UnsubscribeNotification(myNotificationHandling)

//        //public void UnsubscribeNotification(NHIMetadataObject myNotificationHandling)
//        //{
//        //    lock (_IMetadataObject_NotificationHandlingLock)
//        //    {
//        //        _IMetadataObject_NotificationHandling &= ~myNotificationHandling;
//        //    }
//        //}

//        //#endregion

//        //#endregion

//        //#endregion

//        //#region IDirectoryObject Members

//        //#region AddObjectStream(myObjectName, myObjectStream, myINodePositions)

//        //public void AddObjectStream(String myObjectName, String myObjectStream, List<ExtendedPosition> myINodePositions)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //#endregion


//        //#region ObjectExists(myObjectName)

//        //public Boolean ObjectExists(String myObjectName)
//        //{
//        //    return base.ContainsKey(myObjectName);
//        //}

//        //#endregion

//        //#region  ObjectStreamExists(myObjectName, myObjectStream)

//        //public Boolean ObjectStreamExists(String myObjectName, String myObjectStream)
//        //{

//        //    if (base.ContainsKey(myObjectName) && myObjectStream == FSConstants.INLINEDATA)
//        //        return true;

//        //    return false;

//        //}

//        //#endregion

//        //#region GetObjectStreamsList(myObjectName)

//        //public List<String> GetObjectStreamsList(String myObjectName)
//        //{
//        //    return new List<String> { FSConstants.INLINEDATA };
//        //}

//        //#endregion


//        //#region RemoveObjectStream(myObjectName, myObjectStream)

//        //public void RemoveObjectStream(String myObjectName, String myObjectStream)
//        //{

//        //    if (myObjectStream == FSConstants.INLINEDATA)
//        //        Remove(myObjectName);

//        //}

//        //#endregion

//        //#region GetObjectINodePositions(myObjectName)

//        //public List<ExtendedPosition> GetObjectINodePositions(String myObjectName)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //#endregion 

//        //#region GetDirectoryEntry(myObjectName)

//        //public DirectoryEntry GetDirectoryEntry(String myObjectName)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //#endregion


//        //#region StoreInlineData(myObjectName, myInlineData, myAllowOverwritting)

//        //public void StoreInlineData(String myObjectName, Byte[] myInlineData, Boolean myAllowOverwritting)
//        //{
//        //    //Set(myObjectName, (T) myInlineData);
//        //}

//        //#endregion

//        //#region GetInlineData(myObjectName)

//        //public Byte[] GetInlineData(String myObjectName)
//        //{
//        //    return Encoding.UTF8.GetBytes(base[myObjectName].ToString());
//        //}

//        //#endregion

//        //#region hasInlineData(String myObjectName)

//        //public Boolean hasInlineData(String myObjectName)
//        //{

//        //    if (base.ContainsKey(myObjectName))
//        //        return true;

//        //    return false;

//        //}

//        //#endregion

//        //#region DeleteInlineData(String myObjectName)

//        //public void DeleteInlineData(String myObjectName)
//        //{
//        //    base.Remove(myObjectName);
//        //}

//        //#endregion


//        //#region AddSymlink(myObjectName, myTargetObject)

//        //public void AddSymlink(String myObjectName, String myTargetObject)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //#endregion

//        //#region GetSymlink(myObjectName)

//        //public String GetSymlink(String myObjectName)
//        //{
//        //    return "";
//        //}

//        //#endregion

//        //#region isSymlink(myObjectName)

//        //public Boolean isSymlink(String myObjectName)
//        //{
//        //    return false;
//        //}

//        //#endregion

//        //#region RemoveSymlink(myObjectName)

//        //public void RemoveSymlink(String myObjectName)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //#endregion


//        //#region GetDirectoryListing()

//        //public List<String> GetDirectoryListing()
//        //{

//        //    List<String> _DirectoryListing = new List<String>();

//        //    foreach (String _DirectoryEntry in base.Keys)
//        //        _DirectoryListing.Add(_DirectoryEntry);

//        //    return _DirectoryListing;

//        //}

//        //#endregion

//        //#region GetDirectoryListing(myLogin, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

//        //public List<String> GetDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
//        //{
//        //    return GetDirectoryListing();
//        //}

//        //#endregion

//        //#region GetExtendedDirectoryListing()

//        //public List<DirectoryEntryInformation> GetExtendedDirectoryListing()
//        //{

//        //    List<DirectoryEntryInformation> _Output           = new List<DirectoryEntryInformation>();
//        //    DirectoryEntryInformation       _OutputParameter  = new DirectoryEntryInformation();

//        //    foreach (String _DirectoryEntry in base.Keys)
//        //    {
//        //        _OutputParameter.Name         = _DirectoryEntry;
//        //        _OutputParameter.StreamTypes  = new List<String> { FSConstants.INLINEDATA };
//        //        _Output.Add(_OutputParameter);
//        //    }

//        //    return _Output;

//        //}

//        //#endregion

//        //#region GetExtendedDirectoryListing(myLogin, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

//        //public List<DirectoryEntryInformation> GetExtendedDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
//        //{
//        //    return GetExtendedDirectoryListing();
//        //}

//        //#endregion


//        //#region DirCount()

//        //public Int32 DirCount
//        //{
//        //    get
//        //    {
//        //        return GetDirectoryListing().Count;
//        //    }
//        //}

//        //#endregion

//        //#region DirCount64()

//        //public UInt64 DirCount64
//        //{
//        //    get
//        //    {
//        //        return (UInt64)GetDirectoryListing().Count;
//        //    }
//        //}

//        //#endregion


//        //#region NotificationHandling

//        //private Object _IDirectoryObject_NotificationHandlingLock = new Object();

//        //private NHIDirectoryObject _IDirectoryObject_NotificationHandling;

//        ///// <summary>
//        ///// Returns the NotificationHandling bitfield that indicates which
//        ///// notifications should be triggered.
//        ///// </summary>
//        //NHIDirectoryObject IDirectoryObject.NotificationHandling
//        //{
//        //    get
//        //    {
//        //        return _IDirectoryObject_NotificationHandling;
//        //    }
//        //}

//        //#endregion

//        //#region SubscribeNotification(myNotificationHandling)

//        ///// <summary>
//        ///// This method adds the given NotificationHandling flags.
//        ///// </summary>
//        ///// <param name="myNotificationHandling">The NotificationHandlings to be added.</param>
//        //public void SubscribeNotification(NHIDirectoryObject myNotificationHandling)
//        //{
//        //    lock (_IMetadataObject_NotificationHandlingLock)
//        //    {
//        //        _IDirectoryObject_NotificationHandling |= myNotificationHandling;
//        //    }
//        //}

//        //#endregion

//        //#region UnsubscribeNotification(myNotificationHandling)

//        ///// <summary>
//        ///// This method removes the given NotificationHandling flags.
//        ///// </summary>
//        ///// <param name="myNotificationHandling">The NotificationHandlings to be removed.</param>
//        //public void UnsubscribeNotification(NHIDirectoryObject myNotificationHandling)
//        //{
//        //    lock (_IMetadataObject_NotificationHandlingLock)
//        //    {
//        //        _IDirectoryObject_NotificationHandling &= ~myNotificationHandling;
//        //    }
//        //}

//        //#endregion

//        //#endregion


//    }

//}

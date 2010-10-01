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

///* GraphFS - ATypedMetadataObject
// * (c) Achim Friedland, 2008 - 2009
// * 
// * The abstract class for all Graph  metadata objects
// * and virtual metadata objects.
// * 
// * Lead programmer:
// *      Achim Friedland
// * 
// * */

//#region Usings

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Diagnostics;
//using System.IO;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

//using sones.Lib.BTree;
//using sones.Graph.Storage.Exceptions;
//
//using sones.Lib.Serializer;

//using sones.Lib;
//using sones.Graph.Storage.GraphFS.GenericObjects;

//#endregion

//namespace sones.GraphFS.Objects
//{

//    /// <summary>
//    /// The abstract class for all Graph metadata objects
//    /// and virtual metadata objects.
//    /// </summary>
//    public class TypedMetadataObject<T> : AGraphObject, IMetadataObject<T>, IDirectoryObject
//    {

//        #region Data

//        AIndexObject<String, T> _IndexObject;

//        #endregion


//        #region Constructor

//        #region TypedMetadataObject()

//        /// <summary>
//        /// This will create an empty TypedMetadataObject
//        /// </summary>
//        public TypedMetadataObject()
//        {

//            // Members of AGraphStructure
//            _StructureVersion   = 1;

//            // Members of AGraphObject
//            _ObjectStream       = FSConstants.DIRECTORYSTREAM;

//            // Object specific data...
//            _IndexObject        = new IndexObject_HashTable<String, T>();

//        }

//        #endregion


//        #region TypedMetadataObject(myObjectLocation)

//        /// <summary>
//        /// This will create an empty TypedMetadataObject
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        public TypedMetadataObject(String myObjectLocation)
//            : this()
//        {

//            if (myObjectLocation == null || myObjectLocation.Length < FSPathConstants.PathDelimiter.Length)
//                throw new ArgumentNullException("Invalid ObjectLocation!");

//            // Set the property in order to automagically set the
//            // ObjectPath and ObjectName
//            ObjectLocation      = myObjectLocation;

//        }

//        #endregion

//        #region TypedMetadataObject(myObjectLocation, myObjectStream)

//        /// <summary>
//        /// This will create a TypedMetadataObject with the given ObjectLocation and ObjectStream.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        public TypedMetadataObject(String myObjectLocation, String myObjectStream)
//            : this(myObjectLocation)
//        {

//            if (myObjectStream == null || myObjectStream.Length == 0)
//                throw new ArgumentNullException("Invalid ObjectStream!");

//            _ObjectStream = myObjectStream;

//        }

//        #endregion

//        #region TypedMetadataObject(myObjectLocation, myObjectStream, myObjectEdition)

//        /// <summary>
//        /// This will create a TypedMetadataObject with the given ObjectLocation, ObjectStream and ObjectEdition.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        /// <param name="myObjectEdition">the ObjectEdition</param>
//        public TypedMetadataObject(String myObjectLocation, String myObjectStream, String myObjectEdition)
//            : this(myObjectLocation, myObjectStream)
//        {

//            if (myObjectEdition == null || myObjectEdition.Length == 0)
//                _ObjectEdition = FSConstants.DefaultEdition;

//            else
//                _ObjectEdition = myObjectEdition;

//        }

//        #endregion

//        #region TypedMetadataObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID)

//        /// <summary>
//        /// This will create a TypedMetadataObject with the given ObjectLocation, ObjectStream, ObjectEdition and ObjectRevisionID.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        /// <param name="myObjectEdition">the ObjectEdition</param>
//        /// <param name="myObjectRevision">the RevisionID of the AGraphObject</param>
//        public TypedMetadataObject(String myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID)
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

//        #region TypedMetadataObject(myObjectLocation, myObjectRevisionID)

//        /// <summary>
//        /// This will create a TypedMetadataObject with the given ObjectLocation and ObjectRevisionID.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectRevision">the RevisionID of the AGraphObject</param>
//        public TypedMetadataObject(String myObjectLocation, RevisionID myObjectRevisionID)
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


//        #region TypedMetadataObject(myIndexObjectType, myObjectLocation)

//        /// <summary>
//        /// This will create an empty TypedMetadataObject
//        /// </summary>
//        /// <param name="myIndexObjectType">The type of the IIndexObject</param>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        public TypedMetadataObject(DirectoryObjectTypes myIndexObjectType, String myObjectLocation)
//            : this(myObjectLocation)
//        {

//            switch (myIndexObjectType)
//            {
//                case DirectoryObjectTypes.DEFAULT:   _IndexObject = new IndexObject_HashTable<String, T>(myObjectLocation); break;
//                case DirectoryObjectTypes.HashTable: _IndexObject = new IndexObject_HashTable<String, T>(myObjectLocation); break;
//                case DirectoryObjectTypes.BStarTree: _IndexObject = new IndexObject_BStarTree<String, T>(myObjectLocation); break;
//            }

//        }

//        #endregion

//        #region TypedMetadataObject(myIndexObjectType, myObjectLocation, myObjectStream)

//        /// <summary>
//        /// This will create a TypedMetadataObject with the given ObjectLocation and ObjectStream.
//        /// </summary>
//        /// <param name="myIndexObjectType">The type of the IIndexObject</param>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        public TypedMetadataObject(DirectoryObjectTypes myIndexObjectType, String myObjectLocation, String myObjectStream)
//            : this(myIndexObjectType, myObjectLocation)
//        {

//            if (myObjectStream == null || myObjectStream.Length == 0)
//                throw new ArgumentNullException("Invalid ObjectStream!");

//            _ObjectStream = myObjectStream;

//        }

//        #endregion

//        #region TypedMetadataObject(myIndexObjectType, myObjectLocation, myObjectStream, myObjectEdition)

//        /// <summary>
//        /// This will create a TypedMetadataObject with the given ObjectLocation, ObjectStream and ObjectEdition.
//        /// </summary>
//        /// <param name="myIndexObjectType">The type of the IIndexObject</param>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        /// <param name="myObjectEdition">the ObjectEdition</param>
//        public TypedMetadataObject(DirectoryObjectTypes myIndexObjectType, String myObjectLocation, String myObjectStream, String myObjectEdition)
//            : this(myIndexObjectType, myObjectLocation, myObjectStream)
//        {

//            if (myObjectEdition == null || myObjectEdition.Length == 0)
//                _ObjectEdition = FSConstants.DefaultEdition;

//            else
//                _ObjectEdition = myObjectEdition;

//        }

//        #endregion

//        #region TypedMetadataObject(myIndexObjectType, myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID)

//        /// <summary>
//        /// This will create a TypedMetadataObject with the given ObjectLocation, ObjectStream, ObjectEdition and ObjectRevisionID.
//        /// </summary>
//        /// <param name="myIndexObjectType">The type of the IIndexObject</param>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        /// <param name="myObjectEdition">the ObjectEdition</param>
//        /// <param name="myObjectRevision">the RevisionID of the AGraphObject</param>
//        public TypedMetadataObject(DirectoryObjectTypes myIndexObjectType, String myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID)
//            : this(myIndexObjectType, myObjectLocation, myObjectStream, myObjectEdition)
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

//        #region TypedMetadataObject(myIndexObjectType, myObjectLocation, myObjectRevisionID)

//        /// <summary>
//        /// This will create a TypedMetadataObject with the given ObjectLocation and ObjectRevisionID.
//        /// </summary>
//        /// <param name="myIndexObjectType">The type of the IIndexObject</param>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectRevision">the RevisionID of the AGraphObject</param>
//        public TypedMetadataObject(DirectoryObjectTypes myIndexObjectType, String myObjectLocation, RevisionID myObjectRevisionID)
//            : this(myIndexObjectType, myObjectLocation)
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


//        #region TypedMetadataObject(myObjectLocation, mySerializedData)

//        /// <summary>
//        /// A constructor used for fast deserializing
//        /// </summary>
//        /// <param name="myObjectLocation">The ObjectLocation</param>
//        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized TypedMetadataObject</param>
//        public TypedMetadataObject(String myObjectLocation, Byte[] mySerializedData)
//            : this(myObjectLocation)
//        {

//            if (mySerializedData == null || mySerializedData.Length == 0)
//                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

//            Deserialize(mySerializedData);
//            _isNew = false;

//        }

//        #endregion

//        #endregion


//        #region Object-specific methods

//        #region ITypedMetadataObject<T> Members

//        #region Set(myKey, myValue)

//        /// <summary>
//        /// Stores the given object as metadatum using the given MetadataKey.
//        /// If the object is null the given MetadataKey will be removes from
//        /// the TypedMetadataObject.
//        /// </summary>
//        /// <param name="myKey">a key identifying the metadata</param>
//        /// <param name="myValue">an object</param>
//        public void Set(String myKey, T myValue)
//        {

//            if (myValue != null)
//                _IndexObject.Set(myKey, myValue);

//            else
//                _IndexObject.Remove(myKey);

//        }

//        #endregion

//        #region Set(myKey, List<T> myValue)

//        public void Set(string myKey, List<T> myValue)
//        {

//            if (myValue != null)
//                _IndexObject.Set(myKey, myValue);

//            else
//                _IndexObject.Remove(myKey);

//        }
        
//        #endregion

//        #region Set(myKeyValuePair)

//        public void Set(KeyValuePair<String, T> myKeyValuePair)
//        {
//            _IndexObject.Set(myKeyValuePair);
//        }

//        #endregion

//        #region Set(myListOfKeyValuePairs)

//        public void Set(List<KeyValuePair<String, T>> myListOfKeyValuePairs)
//        {
//            _IndexObject.Set(myListOfKeyValuePairs);
//        }

//        #endregion

//        #region Set(myDictionary)

//        public void Set(Dictionary<String, List<T>> myDictionary)
//        {
//            _IndexObject.Set(myDictionary);
//        }

//        #endregion


//        #region ContainsKey(myKey)

//        public Boolean ContainsKey(String myKey)
//        {
//            return _IndexObject.ContainsKey(myKey);
//        }

//        #endregion

//        #region ContainsValue(myValue)

//        public Boolean ContainsValue(T myValue)
//        {
//            return _IndexObject.ContainsValue(myValue);
//        }

//        #endregion

//        #region Contains(myKeyValuePair)

//        public Boolean Contains(KeyValuePair<String, List<T>> myKeyValuePair)
//        {
//            return _IndexObject.Contains(myKeyValuePair);
//        }

//        #endregion


//        #region Keys

//        public ICollection<String> Keys
//        {
//            get
//            {
//                return _IndexObject.Keys;
//            }
//        }

//        #endregion

//        #region Values

//        public ICollection<List<T>> Values
//        {
//            get
//            {
//                return _IndexObject.Values;
//            }
//        }

//        #endregion

//        #region GetEnumerator()

//        public IEnumerator<KeyValuePair<String, T>> GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }

//        #endregion


//        #region this[myKey]

//        public List<T> this[String myKey]
//        {
//            get
//            {
//                return _IndexObject[myKey];
//            }
//            set
//            {
//                _IndexObject[myKey] = value;
//            }
//        }

//        #endregion

//        #region GetValue(myKey)

//        public List<T> GetValues(String myKey)
//        {
//            return _IndexObject.GetValue(myKey);
//        }

//        #endregion

//        #region TryGetValue(myKey, myValue)

//        public Boolean TryGetValues(String myKey, out List<T> myValue)
//        {
//            return _IndexObject.TryGetValue(myKey, out myValue);
//        }

//        #endregion

//        #region GetListOfKeyValuePairs()

//        public List<KeyValuePair<String, List<T>>> GetListOfKeyValuePairs()
//        {
//            return _IndexObject.GetListOfKeyValuePairs();
//        }

//        #endregion

//        #region GetListOfKeyValuePairs(myPrefix)

//        public List<KeyValuePair<String, List<T>>> GetListOfKeyValuePairs(String myPrefix)
//        {
//            //ToDo: Filter GetListOfKeyValuePairs() by prefix!
//            return _IndexObject.GetListOfKeyValuePairs();
//        }

//        #endregion

//        #region GetDictionary()

//        public Dictionary<String, List<T>> GetDictionary()
//        {
//            return _IndexObject.GetDictionary();
//        }

//        #endregion

//        #region GetDictionary(myPrefix)

//        public Dictionary<String, List<T>> GetDictionary(String myPrefix)
//        {

//            Dictionary<String, List<T>> _Dictionary = new Dictionary<String, List<T>>();

//            foreach (KeyValuePair<String, List<T>> _KeyValuePair in _IndexObject)
//                if (_KeyValuePair.Key.StartsWith(myPrefix))
//                    _Dictionary.Add(_KeyValuePair.Key, _KeyValuePair.Value);

//            return _Dictionary;

//        }

//        #endregion


//        #region Remove(myKey)

//        public Boolean Remove(String myKey)
//        {
//            return _IndexObject.Remove(myKey);
//        }

//        #endregion

//        #region Remove(myKeyValuePair)

//        public Boolean Remove(KeyValuePair<String, T> myKeyValuePair)
//        {
//            return _IndexObject.Remove(myKeyValuePair);
//        }

//        #endregion

//        #region Remove(myListOfKeys)

//        public Boolean Remove(List<String> myListOfKeys)
//        {
//            return _IndexObject.Remove(myListOfKeys);
//        }

//        #endregion

//        #region Remove(myListOfKeyValuePairs)

//        public Boolean Remove(List<KeyValuePair<String, T>> myListOfKeyValuePairs)
//        {
//            return _IndexObject.Remove(myListOfKeyValuePairs);
//        }

//        #endregion

//        #region RemoveKeyWithPrefix(myPrefix)

//        public Boolean RemoveKeyWithPrefix(String myPrefix)
//        {

//            List<KeyValuePair<String, List<T>>> _Dictionary = new List<KeyValuePair<String, List<T>>>();

//            foreach (KeyValuePair<String, List<T>> _KeyValuePair in _IndexObject)
//                if (_KeyValuePair.Key.StartsWith(myPrefix))
//                    _Dictionary.Add(_KeyValuePair);

//            return _IndexObject.Remove(_Dictionary);

//        }

//        #endregion

//        #region Clear()

//        public void Clear()
//        {
//            _IndexObject.Clear();
//        }

//        #endregion


//        #region Count

//        public Int32 Count
//        {
//            get
//            {
//                return _IndexObject.Count;
//            }
//        }

//        #endregion

//        #region Count64

//        public UInt64 Count64
//        {
//            get
//            {
//                return _IndexObject.Count64;
//            }
//        }

//        #endregion


//        #region Clone()

//        public override AGraphStructure Clone()
//        {
//            return Clone<TypedMetadataObject<T>>();
//        }

//        #endregion


//        #region NotificationHandling

//        #region NotificationHandling

//        private Object _IMetadataObject_NotificationHandlingLock = new Object();

//        private NHIMetadataObject _IMetadataObject_NotificationHandling;

//        /// <summary>
//        /// Returns the NotificationHandling bitfield that indicates which
//        /// notifications should be triggered.
//        /// </summary>
//        public NHIMetadataObject NotificationHandling
//        {

//            get
//            {
//                return _IMetadataObject_NotificationHandling;
//            }

//        }

//        #endregion

//        #region SubscribeNotification(myNotificationHandling)

//        /// <summary>
//        /// This method adds the given NotificationHandling flags.
//        /// </summary>
//        /// <param name="myNotificationHandling">The NotificationHandlings to be added.</param>
//        public void SubscribeNotification(NHIMetadataObject myNotificationHandling)
//        {

//            lock (_IMetadataObject_NotificationHandlingLock)
//            {
//                _IMetadataObject_NotificationHandling |= myNotificationHandling;
//            }

//        }

//        #endregion

//        #region UnsubscribeNotification(myNotificationHandling)

//        /// <summary>
//        /// This method removes the given NotificationHandling flags.
//        /// </summary>
//        /// <param name="myNotificationHandling">The NotificationHandlings to be removed.</param>
//        public void UnsubscribeNotification(NHIMetadataObject myNotificationHandling)
//        {

//            lock (_IMetadataObject_NotificationHandlingLock)
//            {
//                _IMetadataObject_NotificationHandling &= ~myNotificationHandling;
//            }

//        }

//        #endregion

//        #endregion

//        #endregion


//        #region IDirectoryObject Members

//        #region AddObjectStream(myObjectName, myObjectStream, myINodePositions)

//        public void AddObjectStream(String myObjectName, String myObjectStream, List<ExtendedPosition> myINodePositions)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion


//        #region ObjectExists(myObjectName)

//        public Boolean ObjectExists(String myObjectName)
//        {
//            return _IndexObject.ContainsKey(myObjectName);
//        }

//        #endregion

//        #region  ObjectStreamExists(myObjectName, myObjectStream)

//        public Boolean ObjectStreamExists(String myObjectName, String myObjectStream)
//        {

//            if (_IndexObject.ContainsKey(myObjectName) && myObjectStream == FSConstants.INLINEDATA)
//                return true;

//            return false;

//        }

//        #endregion

//        #region GetObjectStreamsList(myObjectName)

//        public List<String> GetObjectStreamsList(String myObjectName)
//        {
//            return new List<String> { FSConstants.INLINEDATA };
//        }

//        #endregion


//        #region RemoveObjectStream(myObjectName, myObjectStream)

//        public void RemoveObjectStream(String myObjectName, String myObjectStream)
//        {

//            if (myObjectStream == FSConstants.INLINEDATA)
//                Remove(myObjectName);

//        }

//        #endregion

//        #region GetObjectINodePositions(myObjectName)

//        public List<ExtendedPosition> GetObjectINodePositions(String myObjectName)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion 

//        #region GetDirectoryEntry(myObjectName)

//        public DirectoryEntry GetDirectoryEntry(String myObjectName)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion


//        #region StoreInlineData(myObjectName, myInlineData, myAllowOverwritting)

//        public void StoreInlineData(String myObjectName, Byte[] myInlineData, Boolean myAllowOverwritting)
//        {
//            //Set(myObjectName, (T) myInlineData);
//        }

//        #endregion

//        #region GetInlineData(myObjectName)

//        public Byte[] GetInlineData(String myObjectName)
//        {
//            return Encoding.UTF8.GetBytes(_IndexObject[myObjectName].ToString());
//        }

//        #endregion

//        #region hasInlineData(String myObjectName)

//        public Boolean hasInlineData(String myObjectName)
//        {

//            if (_IndexObject.ContainsKey(myObjectName))
//                return true;

//            return false;

//        }

//        #endregion

//        #region DeleteInlineData(String myObjectName)

//        public void DeleteInlineData(String myObjectName)
//        {
//            _IndexObject.Remove(myObjectName);
//        }

//        #endregion


//        #region AddSymlink(myObjectName, myTargetObject)

//        public void AddSymlink(String myObjectName, String myTargetObject)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion

//        #region GetSymlink(myObjectName)

//        public String GetSymlink(String myObjectName)
//        {
//            return "";
//        }

//        #endregion

//        #region isSymlink(myObjectName)

//        public Boolean isSymlink(String myObjectName)
//        {
//            return false;
//        }

//        #endregion

//        #region RemoveSymlink(myObjectName)

//        public void RemoveSymlink(String myObjectName)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion


//        #region GetDirectoryListing()

//        public List<String> GetDirectoryListing()
//        {

//            List<String> _DirectoryListing = new List<String>();

//            foreach (String _DirectoryEntry in _IndexObject.Keys)
//                _DirectoryListing.Add(_DirectoryEntry);

//            return _DirectoryListing;

//        }

//        #endregion

//        #region GetDirectoryListing(myLogin, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

//        public List<String> GetDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
//        {
//            return GetDirectoryListing();
//        }

//        #endregion

//        #region GetExtendedDirectoryListing()

//        public List<DirectoryEntryInformation> GetExtendedDirectoryListing()
//        {

//            List<DirectoryEntryInformation> _Output           = new List<DirectoryEntryInformation>();
//            DirectoryEntryInformation       _OutputParameter  = new DirectoryEntryInformation();

//            foreach (String _DirectoryEntry in _IndexObject.Keys)
//            {
//                _OutputParameter.Name         = _DirectoryEntry;
//                _OutputParameter.StreamTypes  = new List<String> { FSConstants.INLINEDATA };
//                _Output.Add(_OutputParameter);
//            }

//            return _Output;

//        }

//        #endregion

//        #region GetExtendedDirectoryListing(myLogin, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

//        public List<DirectoryEntryInformation> GetExtendedDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
//        {
//            return GetExtendedDirectoryListing();
//        }

//        #endregion


//        #region DirCount()

//        public Int32 DirCount
//        {
//            get
//            {
//                return GetDirectoryListing().Count;
//            }
//        }

//        #endregion

//        #region DirCount64()

//        public UInt64 DirCount64
//        {
//            get
//            {
//                return (UInt64)GetDirectoryListing().Count;
//            }
//        }

//        #endregion


//        #region NotificationHandling

//        #region NotificationHandling

//        private Object _IDirectoryObject_NotificationHandlingLock = new Object();

//        private NHIDirectoryObject _IDirectoryObject_NotificationHandling;

//        /// <summary>
//        /// Returns the NotificationHandling bitfield that indicates which
//        /// notifications should be triggered.
//        /// </summary>
//        NHIDirectoryObject IDirectoryObject.NotificationHandling
//        {

//            get
//            {
//                return _IDirectoryObject_NotificationHandling;
//            }

//        }

//        #endregion

//        #region SubscribeNotification(myNotificationHandling)

//        /// <summary>
//        /// This method adds the given NotificationHandling flags.
//        /// </summary>
//        /// <param name="myNotificationHandling">The NotificationHandlings to be added.</param>
//        public void SubscribeNotification(NHIDirectoryObject myNotificationHandling)
//        {

//            lock (_IMetadataObject_NotificationHandlingLock)
//            {
//                _IDirectoryObject_NotificationHandling |= myNotificationHandling;
//            }

//        }

//        #endregion

//        #region UnsubscribeNotification(myNotificationHandling)

//        /// <summary>
//        /// This method removes the given NotificationHandling flags.
//        /// </summary>
//        /// <param name="myNotificationHandling">The NotificationHandlings to be removed.</param>
//        public void UnsubscribeNotification(NHIDirectoryObject myNotificationHandling)
//        {

//            lock (_IMetadataObject_NotificationHandlingLock)
//            {
//                _IDirectoryObject_NotificationHandling &= ~myNotificationHandling;
//            }

//        }

//        #endregion

//        #endregion

//        #endregion


//        #region Clone<TClone>

//        public override TClone Clone<TClone>()
//        {

//            if (typeof(TClone) != typeof(TypedMetadataObject<T>))
//                throw new ArgumentException("TClone is not a TypedMetadataObject<T>");

//            if (isDirty || _SerializedAGraphStructure == null)
//                Serialize();

//            TClone newT = new TClone();
//            newT.Deserialize(_SerializedAGraphStructure, this);

//            return newT;

//        }

//        #endregion

//        #endregion


//        #region IFastSerialize Members

//        #region Serialize()

//        public override Byte[] Serialize()
//        {

//            _SerializedAGraphStructure = _IndexObject.Serialize( (UInt64) _IMetadataObject_NotificationHandling);
//            //isDirty = false;

//            return _SerializedAGraphStructure;

//        }

//        #endregion

//        #region Deserialize(mySerializedData)

//        public override void Deserialize(Byte[] mySerializedData)
//        {

//            _IndexObject.Deserialize(mySerializedData);

//            _SerializedAGraphStructure = mySerializedData;

//        }

//        #endregion

//        #endregion


//        #region IMetadataObject<T> Members


//        public void Set(Dictionary<string, T> myDictionary)
//        {
//            throw new NotImplementedException();
//        }

//        public void Set(KeyValuePair<string, List<T>> myKeyValuePair)
//        {
//            throw new NotImplementedException();
//        }

//        public void Set(List<KeyValuePair<string, List<T>>> myListOfKeyValuePairs)
//        {
//            throw new NotImplementedException();
//        }

//        ICollection<List<T>> IMetadataObject<T>.Values
//        {
//            get { throw new NotImplementedException(); }
//        }

//        IEnumerator<KeyValuePair<string, List<T>>> IMetadataObject<T>.GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }

//        List<T> IMetadataObject<T>.this[string myKey]
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        List<KeyValuePair<string, List<T>>> IMetadataObject<T>.GetListOfKeyValuePairs()
//        {
//            throw new NotImplementedException();
//        }

//        List<KeyValuePair<string, List<T>>> IMetadataObject<T>.GetListOfKeyValuePairs(string myPrefix)
//        {
//            throw new NotImplementedException();
//        }

//        Dictionary<string, List<T>> IMetadataObject<T>.GetDictionary()
//        {
//            throw new NotImplementedException();
//        }

//        Dictionary<string, List<T>> IMetadataObject<T>.GetDictionary(string myPrefix)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }

//}

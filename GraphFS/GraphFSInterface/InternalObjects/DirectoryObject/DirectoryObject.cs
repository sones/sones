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
 * GraphFS - DirectoryObject
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using sones.Lib;
using sones.Lib.BTree;
using sones.Lib.Serializer;
using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.StorageEngines;
using sones.GraphFS.Objects;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Exceptions;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.InternalObjects
{

    /// <summary>
    /// This implements the directory data structure with all needed
    /// methods for directory object handling.
    /// </summary>
    public class DirectoryObject : AVersionedDictionaryObject<String, DirectoryEntry>, IDirectoryObject
    {


        #region Constructors

        #region DirectoryObject()

        /// <summary>
        /// This will create an empty DirectoryObject
        /// </summary>
        public DirectoryObject()
            : base()
        {

            // Members of AGraphStructure
            _StructureVersion       = 1;

            // Members of AGraphObject
            _ObjectStream           = FSConstants.DIRECTORYSTREAM;

        }

        #endregion

        #region DirectoryObject()

        /// <summary>
        /// This will create an empty DirectoryObject
        /// </summary>
        public DirectoryObject(ObjectUUID myObjectUUID)
            : this()
        {
            ObjectUUID = myObjectUUID;
        }

        #endregion

        #region DirectoryObject(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
        /// <param name="mySerializedData">An array of bytes[] containing a serialized DirectoryObject</param>
        public DirectoryObject(Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
            : this()
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

            var newT = new DirectoryObject();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            //if (_EncryptionParameters != null)
            //    newT._EncryptionParameters = (Byte[])_EncryptionParameters.Clone();

            //foreach (var keyValPair in _IDictionary)
            //    newT.Add(keyValPair.Key, keyValPair.Value.Clone());

            //newT._INodeReference = _INodeReference;

            //if (_IntegrityCheckValue != null)
            //    newT._IntegrityCheckValue = (Byte[])_IntegrityCheckValue.Clone();

            //newT._isNew = _isNew;
            //newT._NotificationHandling = _NotificationHandling;

            //newT._ObjectEdition = _ObjectEdition;
            //newT.ObjectLocation = ObjectLocation;
            //newT._ObjectLocatorReference = _ObjectLocatorReference;
            //newT._INodeReference = _INodeReference;

            //if (_ObjectRevisionID != null)
            //    newT._ObjectRevisionID = _ObjectRevisionID.Clone();

            //newT._ObjectSize = _ObjectSize;
            //newT._ObjectSizeOnDisc = _ObjectSizeOnDisc;
            //newT._ObjectStream = _ObjectStream;

            //if (_ObjectUUID != null)
            //    newT._ObjectUUID = _ObjectUUID.Clone();

            //newT._SerializedAGraphStructure = null;
            //newT._StructureVersion = _StructureVersion;
            //newT._TransactionUUID = _TransactionUUID;

            return newT;

        }

        #endregion

        #endregion


        #region OverwriteObjectUUID(myObjectUUID) <- Obsolete "Remove me!"

        /// <summary>
        /// Overwrite the readonly ObjectUUID!
        /// </summary>
        /// <param name="myObjectUUID"></param>
        [Obsolete("Remove me!")]
        public void OverwriteObjectUUID(ObjectUUID myObjectUUID)
        {
            ObjectUUID = myObjectUUID;
        }

        #endregion


        #region IDirectoryObject Members

        #region AddObjectStream(myObjectName, myObjectStream)

        /// <summary>
        /// This method adds a new virtual ObjectStream to an object in the DirectoryTree
        /// </summary>
        /// <param name="myObjectName">the name of the object</param>
        /// <param name="myObjectStream">the ObjectStream of the object</param>
        public void AddObjectStream(String myObjectName, String myObjectStream)
        {

            lock (this)
            {

                DirectoryEntry _DirectoryEntry = null;

                var _found = base.TryGetValue(myObjectName, out _DirectoryEntry);

                #region Create new directory entry if no one exists

                if (!_found)
                {

                    _DirectoryEntry = new DirectoryEntry
                    {
                        Virtual = new HashSet<String>{ myObjectStream }
                    };

                    base.Add(myObjectName, _DirectoryEntry);

                    // Mark this DirectoryObject dirty...
                    isDirty = true;

                    return;

                }

                #endregion

                #region Register an additional ObjectStream within the ObjectStreamList

                if (!_DirectoryEntry.isVirtual)
                    throw new GraphFSException_ObjectStreamNotAllowed("Objectstream '" + myObjectStream.ToString() + "' could not be set as '" + myObjectName + "' is not a virtual ObjectStream!");

                if (_DirectoryEntry.ObjectStreamsList.Contains(myObjectStream))
                    throw new GraphFSException_ObjectStreamAlreadyExists("Objectstream '" + myObjectStream.ToString() + "' already exists within '" + myObjectName + "'!");

                _DirectoryEntry.ObjectStreamsList.Add(myObjectStream);

                #endregion

            }

            // Mark this DirectoryObject dirty...
            isDirty = true;

        }

        #endregion

        #region AddObjectStream(myObjectName, myObjectStream, myINodePositions)

        /// <summary>
        /// This method adds a new ObjectStream to an object in the DirectoryTree
        /// </summary>
        /// <param name="myObjectName">the Name of the object</param>
        /// <param name="myObjectStream">the ObjectStream of the object</param>
        /// <param name="myINodePositions">the filesystem positions of the corresponding streams</param>
        public void AddObjectStream(String myObjectName, String myObjectStream, IEnumerable<ExtendedPosition> myINodePositions)
        {

            lock (this)
            {

                DirectoryEntry _DirectoryEntry = null;

                var _found = base.TryGetValue(myObjectName, out _DirectoryEntry);

                #region Create new directory entry if no one exists

                // If no DirectoryEntry exists OR it was deleted...
                if (!_found || _DirectoryEntry == null)
                {

                    _DirectoryEntry = new DirectoryEntry
                    {
                        ObjectStreamsList = new HashSet<String>{ myObjectStream },
                        INodePositions    = new HashSet<ExtendedPosition>(myINodePositions)
                    };

                    base.Add(myObjectName, _DirectoryEntry);

                    // Mark this DirectoryObject dirty...
                    isDirty = true;

                    return;

                }

                #endregion

                #region Verify INodePositions

                foreach (var _ExtendedPosition in myINodePositions)

                    // The INodePositions must not be changed on adding another ObjectStream!
                    if (!_DirectoryEntry.INodePositions.Contains(_ExtendedPosition))
                        throw new GraphFSException("New INodePositions differ from already stored positions!");

                #endregion

                #region Register an additional ObjectStream within the ObjectStreamList

                if (_DirectoryEntry.isSymlink)
                    throw new GraphFSException_ObjectStreamNotAllowed("Objectstream '" + myObjectStream.ToString() + "' could not be set as '" + myObjectName + "' is a symlink!");

                if (_DirectoryEntry.isVirtual)
                    throw new GraphFSException_ObjectStreamNotAllowed("Objectstream '" + myObjectStream.ToString() + "' could not be set as '" + myObjectName + "' is a virtual ObjectStream!");

                if (_DirectoryEntry.hasInlineData)
                    throw new GraphFSException_ObjectStreamNotAllowed("Objectstream '" + myObjectStream.ToString() + "' could not be set as '" + myObjectName + "' has InlineData stored!");

                if (_DirectoryEntry.ObjectStreamsList.Contains(myObjectStream))
                    throw new GraphFSException_ObjectStreamAlreadyExists("Objectstream '" + myObjectStream.ToString() + "' already exists within '" + myObjectName + "'!");

                _DirectoryEntry.ObjectStreamsList.Add(myObjectStream);

                base.Add(myObjectName, _DirectoryEntry);

                #endregion

            }

            // Mark this DirectoryObject dirty...
            isDirty = true;

        }

        #endregion


        #region ObjectExists(myObjectName)

        /// <summary>
        /// Checks if an object exists within the object hashmap or binary search tree
        /// </summary>
        /// <param name="myObjectName">the name of the requested object</param>
        /// <returns>true if found, false if not found</returns>
        public Trinary ObjectExists(String myObjectName)
        {

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (myObjectName.Equals(FSPathConstants.PathDelimiter))
                // Otherwise the root directory will not be found!
                return Trinary.TRUE;

            else
            {
                
                if (base.ContainsKey(myObjectName))
                    return Trinary.TRUE;

                return Trinary.FALSE;

            }

        }

        #endregion

        #region ObjectStreamExists(myObjectName, myObjectStream)

        /// <summary>
        /// Checks if an object and the given ObjectStream exists within the filesystem hierarchy
        /// </summary>
        /// <param name="myObjectName">the Name of the object in question</param>
        /// <param name="myObjectStream">the ObjectStream in question</param>
        /// <returns>true if found, false if not found</returns>
        public Trinary ObjectStreamExists(String myObjectName, String myObjectStream)
        {

            DirectoryEntry _DirectoryEntry;

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (base.TryGetValue(myObjectName, out _DirectoryEntry))
            {
                if (_DirectoryEntry == null)
                    return Trinary.FALSE;
                if (_DirectoryEntry.ObjectStreamsList.Contains(myObjectStream))
                    return Trinary.TRUE;
            }

            return Trinary.FALSE;

        }

        #endregion

        #region GetObjectStreamsList(myObjectName)

        public IEnumerable<String> GetObjectStreamsList(String myObjectName)
        {

            DirectoryEntry _DirectoryEntry;

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (base.TryGetValue(myObjectName, out _DirectoryEntry))
                if (_DirectoryEntry != null)
                    return _DirectoryEntry.ObjectStreamsList;

            return new List<String>();

        }

        #endregion


        #region RemoveObjectStream(myObjectName, myObjectStream)

        /// <summary>
        /// This method deletes an ObjectStream from an object in the _DirectoryTree.
        /// It will also remove the entire object if no ObjectStream is left.
        /// </summary>
        /// <param name="myObjectName">the Name of the object</param>
        /// <param name="myObjectStream">the ObjectStream of the object</param>
        public void RemoveObjectStream(String myObjectName, String myObjectStream)
        {

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (ObjectStreamExists(myObjectName, myObjectStream) == Trinary.TRUE)
            {

                // Unregister ObjectStream within the ObjectStreamBitfield of the _DirectoryTree

                if (base[myObjectName].ObjectStreamsList.Contains(myObjectStream))
                    base[myObjectName].ObjectStreamsList.Remove(myObjectStream);

                // Remove the entire object if not ObjectStream is left
                if (base[myObjectName].ObjectStreamsList.Count == 0)
                    base.Remove(myObjectName);

                // Mark this directory dirty...
                isDirty = true;


            }

        }

        #endregion

        #region GetDirectoryEntry(myObjectName)

        /// <summary>
        /// Checks if an object exists within the object hashmap or binary search tree
        /// </summary>
        /// <param name="myObjectName">the Name of the requested object</param>
        /// <returns>true if found, false if not found</returns>
        public DirectoryEntry GetDirectoryEntry(String myObjectName)
        {
            
            if (myObjectName == FSPathConstants.PathDelimiter)
                myObjectName = FSConstants.DotLink;

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            DirectoryEntry _DirectoryEntry;

            if (base.TryGetValue(myObjectName, out _DirectoryEntry))
                return _DirectoryEntry;

            return null;

        }

        #endregion

        #region RenameDirectoryEntry(myObjectName, myNewObjectName)

        public Boolean RenameDirectoryEntry(String myObjectName, String myNewObjectName)
        {

            #region Initial checks

            if (myObjectName == null)
                throw new ArgumentNullException("myObjectName must not be null!");

            if (myNewObjectName == null)
                throw new ArgumentNullException("myNewObjectName must not be null!");

            if (myObjectName == myNewObjectName)
                return true;

            #endregion

            DirectoryEntry _DirectoryEntry;

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (base.TryGetValue(myObjectName, out _DirectoryEntry))
            {
                base.Remove(myObjectName);
                base.Add(myNewObjectName, _DirectoryEntry);
                return true;
            }

            return false;

        }

        #endregion


        #region GetObjectINodePositions(myObjectName)

        /// <summary>
        /// Returns the positions of the requested ObjectStream
        /// </summary>
        /// <param name="myObjectName">the Name of the object</param>
        /// <returns>a list of INode positions or an empty list if the object does not exist</returns>
        public IEnumerable<ExtendedPosition> GetObjectINodePositions(String myObjectName)
        {

            DirectoryEntry _DirectoryEntry;

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (base.TryGetValue(myObjectName, out _DirectoryEntry))
            {
                if (_DirectoryEntry != null)
                {
                    if (_DirectoryEntry.INodePositions != null)
                    {
                        return _DirectoryEntry.INodePositions;
                    }
                    else
                        Debug.WriteLine(String.Format("No INodePositions for {0}{1}{2} found!", ObjectLocation.ToString(), FSPathConstants.PathDelimiter, myObjectName));

                }
                else
                    Debug.WriteLine(String.Format("No DirectoryEntry for {0}{1}{2} found!", ObjectLocation.ToString(), FSPathConstants.PathDelimiter, myObjectName));
            }

            return new List<ExtendedPosition>();

        }

        #endregion


        #region StoreInlineData(myObjectName, myInlineData, myAllowOverwritting)

        /// <summary>
        /// Adds inline data which will be stored within the directory object
        /// </summary>
        /// <param name="myObjectName">the Name of the inline data object</param>
        /// <param name="myInlineData">the online data as array of bytes</param>
        /// <param name="myAllowOverwritting">allows overwritting</param>
        public void StoreInlineData(String myObjectName, Byte[] myInlineData, Boolean myAllowOverwritting)
        {

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (base.ContainsKey(myObjectName))
            {

                if (base[myObjectName].ObjectStreamsList.Contains(FSConstants.INLINEDATA) && myAllowOverwritting)
                    base[myObjectName].InlineData = myInlineData;

                else
                    throw new GraphFSException_ObjectAlreadyExists("Inline data could not ba added as '" + myObjectName + "' already exists!");

            }

            else
            {

                var _DirectoryEntry                = new DirectoryEntry();
                _DirectoryEntry.InlineData         = myInlineData;
                _DirectoryEntry.ObjectStreamsList  = new HashSet<String> { FSConstants.INLINEDATA };

                base.Add(myObjectName, _DirectoryEntry);

            }

        }

        #endregion

        #region GetInlineData(myObjectName)

        /// <summary>
        /// Returns the inline data stored within the directory object
        /// </summary>
        /// <param name="myObjectName">the Name of the symlink</param>
        /// <returns>the array of bytes stored inline within the directory object</returns>
        public Byte[] GetInlineData(String myObjectName)
        {

            DirectoryEntry _DirectoryEntry;

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (base.TryGetValue(myObjectName, out _DirectoryEntry))
                if (_DirectoryEntry.ObjectStreamsList.Contains(FSConstants.INLINEDATA))
                    return _DirectoryEntry.InlineData;

            return null;

        }

        #endregion

        #region hasInlineData(myObjectName)

        /// <summary>
        /// Returns if myObjectName has inline data
        /// </summary>
        /// <param name="myObjectName">the Name of the inline data object to probe</param>
        /// <returns>yes|no</returns>
        public Trinary hasInlineData(String myObjectName)
        {

            DirectoryEntry _DirectoryEntry;

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (base.TryGetValue(myObjectName, out _DirectoryEntry))
                if (_DirectoryEntry.ObjectStreamsList.Contains(FSConstants.INLINEDATA) && (base[myObjectName].InlineData.Length > 0))
                    return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region DeleteInlineData(myObjectName)

        /// <summary>
        /// Removes inline data stored within the directory object
        /// </summary>
        /// <param name="myObjectName">the Name of the inline data</param>
        public void DeleteInlineData(String myObjectName)
        {

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (base.ContainsKey(myObjectName))
                if (base[myObjectName].ObjectStreamsList.Contains(FSConstants.INLINEDATA))

                    base.Remove(myObjectName);

            // Mark this directory dirty...
            isDirty = true;

        }

        #endregion


        #region AddSymlink(myObjectName, myTargetLocation)

        /// <summary>
        /// Adds a symlink to another object within the filesystem
        /// </summary>
        /// <param name="myObjectName">the Name of the symlink</param>
        /// <param name="myTargetObject">the myPath to another object within the filesystem</param>
        public void AddSymlink(String myObjectName, ObjectLocation myTargetLocation)
        {

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (!base.ContainsKey(myObjectName))
            {

                var _TargetLocation = myTargetLocation.ToString();

                base.Add(myObjectName, new DirectoryEntry
                                            {
                                                Symlink = myTargetLocation
                                            });

                // Mark this directory dirty...
                isDirty = true;

            }

            else
                throw new GraphFSException_ObjectAlreadyExists("Symlink could not ba added as '" + myObjectName + "' already exists!");

        }

        #endregion

        #region GetSymlink(myObjectName)

        /// <summary>
        /// Returns the target of a symlink
        /// </summary>
        /// <param name="myObjectName">the Name of the symlink</param>
        /// <returns>an ObjectLocation representing the myPath to another object within the filesystem</returns>
        public ObjectLocation GetSymlink(String myObjectName)
        {

            DirectoryEntry _DirectoryEntry;

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (base.TryGetValue(myObjectName, out _DirectoryEntry))
                if (_DirectoryEntry != null)
                    return _DirectoryEntry.Symlink;

            return null;

        }

        #endregion

        #region isSymlink(myObjectName)

        /// <summary>
        /// Returns if myObjectName is a symlink
        /// </summary>
        /// <param name="myObjectName">the Name of the symlink to probe</param>
        /// <returns>yes|no</returns>
        public Trinary isSymlink(String myObjectName)
        {

            DirectoryEntry _DirectoryEntry;

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (base.TryGetValue(myObjectName, out _DirectoryEntry))
                if (_DirectoryEntry == null)
                    return Trinary.DELETED;
                else if (_DirectoryEntry.isSymlink) 
                    return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region RemoveSymlink(myObjectName)

        /// <summary>
        /// Removes a symlink
        /// </summary>
        /// <param name="myObjectName">the Name of the symlink</param>
        public void RemoveSymlink(String myObjectName)
        {

            DirectoryEntry _DirectoryEntry;

            if (myObjectName.StartsWith("./"))
                myObjectName = myObjectName.Substring(2, myObjectName.Length - 2);

            if (base.TryGetValue(myObjectName, out _DirectoryEntry))
                if (_DirectoryEntry.isSymlink)
                {
                    base.Remove(myObjectName);

                    // Mark this directory dirty...
                    isDirty = true;
                }

        }

        #endregion


        #region GetDirectoryListing()

        public IEnumerable<String> GetDirectoryListing()
        {

            var _ReturnValue = new List<String>();

            foreach (var __DirectoryEntry in _IDictionary)
                if (!__DirectoryEntry.Value.isDeleted)
                    _ReturnValue.Add(__DirectoryEntry.Key);

            return _ReturnValue;

        }

        #endregion

        #region GetDirectoryListing(myFunc)

        public IEnumerable<String> GetDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {

            var _ReturnValue = new List<String>();
            var _Enumerator  = base.GetEnumerator(myFunc);

            while (_Enumerator.MoveNext())
                if (_Enumerator.Current.Value != null)
                    _ReturnValue.Add(_Enumerator.Current.Key);

            return _ReturnValue;

        }

        #endregion

        #region GetDirectoryListing(myLogin, myIgnoreName, myRegExpr, myObjectStream, myIgnoreObjectStreams)

        public IEnumerable<String> GetDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
        {

            #region Data

            List<String> _Output  = new List<String>();
            Boolean      _AddEntry;

            #endregion

            foreach (var __DirectoryEntry in _IDictionary)
            {

                if (!__DirectoryEntry.Value.isDeleted)
                {

                    if (myName                  != null ||
                        myIgnoreName            != null ||
                        myRegExpr               != null ||
                        myObjectStreams         != null ||
                        myIgnoreObjectStreams   != null)
                    {

                        _AddEntry = false;

                        #region Match ObjectName

                        if (myName != null)
                            foreach (String _Name in myName)
                            {
                                if (_Name.EndsWith("*") && __DirectoryEntry.Key.StartsWith(_Name.Substring(0, _Name.Length - 1))) _AddEntry = true;
                                if (_Name.StartsWith("*") && __DirectoryEntry.Key.EndsWith(_Name.Substring(1, _Name.Length - 1))) _AddEntry = true;
                            }

                        if (myIgnoreName != null)
                        {

                            if (myName == null) _AddEntry = true;

                            foreach (String _IgnoreName in myIgnoreName)
                            {
                                if (_IgnoreName.EndsWith("*") && __DirectoryEntry.Key.StartsWith(_IgnoreName.Substring(0, _IgnoreName.Length - 1))) _AddEntry = false;
                                if (_IgnoreName.StartsWith("*") && __DirectoryEntry.Key.EndsWith(_IgnoreName.Substring(1, _IgnoreName.Length - 1))) _AddEntry = false;
                            }

                        }

                        #endregion

                        #region Match RegularExpression

                        if (myRegExpr != null)
                            foreach (String _RegExpr in myRegExpr)
                                if (Regex.IsMatch(__DirectoryEntry.Key, _RegExpr)) _AddEntry = true;

                        #endregion

                        #region Match ObjectStreamTypes

                        if (myObjectStreams != null)
                            foreach (String _ObjectStreamType in myObjectStreams)
                                if (__DirectoryEntry.Value.LatestValue != null)
                                    if (__DirectoryEntry.Value.LatestValue.ObjectStreamsList.Contains(_ObjectStreamType))
                                        _AddEntry = true;
                        //if (__DirectoryEntry.Value.ObjectStreamsList.Contains(_ObjectStreamType))
                        //_AddEntry = true;

                        if (myIgnoreObjectStreams != null)
                        {

                            if (myObjectStreams == null) _AddEntry = true;

                            foreach (String _IgnoreObjectStreamType in myIgnoreObjectStreams)
                                if (__DirectoryEntry.Value.LatestValue != null)
                                    if (__DirectoryEntry.Value.LatestValue.ObjectStreamsList.Contains(_IgnoreObjectStreamType))
                                        _AddEntry = false;
                            //if (__DirectoryEntry.Value.ObjectStreamsList.Contains(_IgnoreObjectStreamType))
                            //_AddEntry = false;

                        }

                        #endregion


                        if (_AddEntry) _Output.Add(__DirectoryEntry.Key);

                    }

                    else
                        _Output.Add(__DirectoryEntry.Key);
                
                }

            }

            return _Output;

        }

        #endregion

        #region GetExtendedDirectoryListing()

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing()
        {

            var _Output          = new List<DirectoryEntryInformation>();
            var _OutputParameter = new DirectoryEntryInformation();

            foreach (var __DirectoryEntry in _IDictionary)
            {
                if (__DirectoryEntry.Value != null)
                {
                    if (__DirectoryEntry.Value.LatestValue != null)
                    {
                        _OutputParameter.Name        = __DirectoryEntry.Key;
                        _OutputParameter.Streams = __DirectoryEntry.Value.LatestValue.ObjectStreamsList;
                        //_OutputParameter.StreamTypes  = __DirectoryEntry.Value.ObjectStreamsList;
                        _Output.Add(_OutputParameter);
                    }
                }
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

        #region GetExtendedDirectoryListing(myLogin, myIgnoreName, myRegExpr, myObjectStream, myIgnoreObjectStreamType)

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
        {

            #region Data

            List<DirectoryEntryInformation> _Output           = new List<DirectoryEntryInformation>();
            Boolean                         _AddEntry;
            DirectoryEntryInformation       _OutputParameter  = new DirectoryEntryInformation();

            #endregion

            foreach (var __DirectoryEntry in _IDictionary)
            {

                if (!__DirectoryEntry.Value.isDeleted)
                {

                    if (myName != null ||
                        myIgnoreName != null ||
                        myRegExpr != null ||
                        myObjectStreams != null ||
                        myIgnoreObjectStreams != null)
                    {

                        _AddEntry = false;

                        #region Match ObjectName

                        if (myName != null)
                            foreach (String _Name in myName)
                            {
                                if (_Name.EndsWith("*") && __DirectoryEntry.Key.StartsWith(_Name.Substring(0, _Name.Length - 1))) _AddEntry = true;
                                if (_Name.StartsWith("*") && __DirectoryEntry.Key.EndsWith(_Name.Substring(1, _Name.Length - 1))) _AddEntry = true;
                            }

                        if (myIgnoreName != null)
                        {

                            if (myName == null) _AddEntry = true;

                            foreach (String _IgnoreName in myIgnoreName)
                            {

                                if (_IgnoreName.EndsWith("*") && __DirectoryEntry.Key.StartsWith(_IgnoreName.Substring(0, _IgnoreName.Length - 1))) _AddEntry = false;
                                if (_IgnoreName.StartsWith("*") && __DirectoryEntry.Key.EndsWith(_IgnoreName.Substring(1, _IgnoreName.Length - 1))) _AddEntry = false;

                                if ((!_IgnoreName.EndsWith("*") && !_IgnoreName.StartsWith("*")) && (__DirectoryEntry.Key == _IgnoreName))
                                {
                                    _AddEntry = false;
                                }

                                if (!_AddEntry) break;
                            }

                            if (!_AddEntry) continue;

                        }

                        #endregion

                        #region Match RegularExpression

                        if (myRegExpr != null)
                            foreach (String _RegExpr in myRegExpr)
                                if (Regex.IsMatch(__DirectoryEntry.Key, _RegExpr)) _AddEntry = true;

                        #endregion

                        #region Match ObjectStreamTypes

                        if (myObjectStreams != null)
                            foreach (String _ObjectStreamType in myObjectStreams)
                                if (__DirectoryEntry.Value.LatestValue != null)
                                    if (__DirectoryEntry.Value.LatestValue.ObjectStreamsList.Contains(_ObjectStreamType))
                                        //if (__DirectoryEntry.Value.ObjectStreamsList.Contains(_ObjectStreamType))
                                        _AddEntry = true;

                        if (myIgnoreObjectStreams != null)
                        {

                            if (myObjectStreams == null) _AddEntry = true;

                            foreach (String _IgnoreObjectStreamType in myIgnoreObjectStreams)
                                if (__DirectoryEntry.Value.LatestValue != null)
                                    if (__DirectoryEntry.Value.LatestValue.ObjectStreamsList.Contains(_IgnoreObjectStreamType))
                                        //if (__DirectoryEntry.Value.ObjectStreamsList.Contains(_IgnoreObjectStreamType))
                                        _AddEntry = false;

                        }

                        #endregion


                        if (_AddEntry)
                        {
                            _OutputParameter.Name = __DirectoryEntry.Key;
                            _OutputParameter.Streams = __DirectoryEntry.Value.LatestValue.ObjectStreamsList;
                            //_OutputParameter.StreamTypes  = __DirectoryEntry.Value.ObjectStreamsList;
                            _Output.Add(_OutputParameter);
                        }

                    }

                    else
                    {
                        _OutputParameter.Name = __DirectoryEntry.Key;
                        _OutputParameter.Streams = __DirectoryEntry.Value.LatestValue.ObjectStreamsList;
                        //_OutputParameter.StreamTypes  = __DirectoryEntry.Value.ObjectStreamsList;
                        _Output.Add(_OutputParameter);
                    }
                
                }

            }


            return _Output;

        }

        #endregion


        #region DirCount()

        public UInt64 DirCount
        {
            get
            {
                return base.KeyCount();
            }
        }

        #endregion


        #region NotificationHandling

        private Object _NotificationHandlingLock = new Object();

        private NHIDirectoryObject _NotificationHandling;

        /// <summary>
        /// Returns the NotificationHandling bitfield that indicates which
        /// notifications should be triggered.
        /// </summary>
        public NHIDirectoryObject NotificationHandling
        {

            get
            {
                return _NotificationHandling;
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

            lock (_NotificationHandlingLock)
            {
                _NotificationHandling |= myNotificationHandling;
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

            lock (_NotificationHandlingLock)
            {
                _NotificationHandling &= ~myNotificationHandling;
            }

        }

        #endregion

        #endregion


        #region ToString()

        public override String ToString()
        {
            return String.Concat("\"", ObjectLocation, "\", UUID=", ObjectUUID.ToHexString(), ", ", base.KeyCount(), " entries");
        }

        #endregion


    }

}

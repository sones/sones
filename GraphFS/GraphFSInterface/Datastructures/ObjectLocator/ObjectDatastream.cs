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
 * GraphFSInterface - ObjectDatastream
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using sones.Lib;
using sones.Lib.Serializer;
using sones.Lib.DataStructures;
using sones.StorageEngines;
using sones.GraphFS.Objects;
using sones.GraphFS.InternalObjects;
#endregion

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    ///  An object stream is part of the ObjectLocator describing
    ///  where to find the extents belonging to a stream of data.
    /// </summary>

    public class ObjectDatastream : ObjectExtentsList, IEnumerable<ObjectExtent>, IDirectoryListing
    {

        #region Data

        public    static          String _ThisObjectName                = "Object";

        protected static readonly String _ObjectUUIDName                = "ObjectUUID";
        protected static readonly String _StreamLengthName              = "StreamLength";
        protected static readonly String _ReservedLengthName            = "ReservedLength";
        protected static readonly String _CompressionAlgorithmName      = "CompressionAlgorithm";
        protected static readonly String _IntegrityCheckValueName       = "IntegrityCheckValue";
        protected static readonly String _BlockIntegrityArraysName      = "BlockIntegrityArrays";
        protected static readonly String _ForwardErrorCorrectionName    = "ForwardErrorCorrection";
        protected static readonly String _AvailableStorageUUIDsName     = "AvailableStorageUUIDs";
        protected static readonly String _RedundancyName                = "Redundancy";
        protected static readonly String _ExtentsName                   = "Extents";

        #endregion

        #region Properties

        #region StorageUUIDs

        private List<StorageUUID> _StorageUUIDs;

        public List<StorageUUID> StorageUUIDs
        {

            get
            {
                return _StorageUUIDs;
            }

            set
            {
                _StorageUUIDs = value;
            }

        }

        #endregion

        #region AvailableStorageUUIDs

        private List<StorageUUID> _AvailableStorageUUIDs;

        public List<StorageUUID> AvailableStorageUUIDs
        {

            get
            {
                return _AvailableStorageUUIDs;
            }

            set
            {
                _AvailableStorageUUIDs = value;
            }

        }

        #endregion

        #region Compression

        private ObjectCompression _Compression;

        public ObjectCompression Compression
        {

            get
            {
                return _Compression;
            }

            set
            {
                _Compression = value;
            }

        }

        #endregion

        #region IntegrityCheckValue

        private Byte[] _IntegrityCheckValue;

        public Byte[] IntegrityCheckValue
        {
            get { return _IntegrityCheckValue; }
            set { _IntegrityCheckValue = value; }
        }

        #endregion

        #region ForwardErrorCorrection

        private ObjectFEC _ForwardErrorCorrection;

        public ObjectFEC ForwardErrorCorrection
        {
            
            get
            {
                return _ForwardErrorCorrection;
            }

            set
            {
                _ForwardErrorCorrection = value;
            }

        }

        #endregion

        #region Redundancy

        private ObjectRedundancy _Redundancy;

        public ObjectRedundancy Redundancy
        {
            
            get
            {
                return _Redundancy;
            }

            set
            {
                _Redundancy = value;
            }

        }

        #endregion

        #region Blocksize

        private UInt64 _Blocksize;

        public UInt64 Blocksize
        {

            get
            {
                return _Blocksize;
            }

            set
            {
                _Blocksize = value;
            }

        }

        #endregion

        #endregion

        #region Constructors

        #region ObjectDatastream()

        /// <summary>
        /// Basic constructor
        /// </summary>
        public ObjectDatastream()
        {

            _ObjectPath                 = null;
            _ObjectName                 = "";
            _ObjectLocation             = null;

            _ReservedLength             = 0;
            _Compression                = new ObjectCompression();
            _IntegrityCheckValue        = new Byte[0];
            _ForwardErrorCorrection     = new ObjectFEC();
            _AvailableStorageUUIDs      = new List<StorageUUID>();
            _Redundancy                 = new ObjectRedundancy();
            _ObjectExtents              = new LinkedList<ObjectExtent>();

        }

        #endregion

        #region ObjectDatastream(myObjectExtent)

        /// <summary>
        /// Additional constructor
        /// </summary>
        public ObjectDatastream(ObjectExtent myObjectExtent)
            : this()
        {
            Add2(myObjectExtent);
        }

        #endregion

        #region ObjectDatastream(myObjectDatastream)

        /// <summary>
        /// Creates a new ObjectDatastream based on the content of myObjectDatastream
        /// </summary>
        /// <param name="myObjectDatastream">The ObjectDatastream to be cloned</param>
        public ObjectDatastream(ObjectDatastream myObjectDatastream)
        {

            _ObjectPath                 = myObjectDatastream.ObjectPath;

            if (myObjectDatastream == null)
                throw new ArgumentNullException();

            _ReservedLength             = myObjectDatastream.ReservedLength;
            _Blocksize                  = myObjectDatastream.Blocksize;

            _AvailableStorageUUIDs      = new List<StorageUUID>();

            #if(__MonoCS__)
            foreach (var _StorageUUID in myObjectDatastream.AvailableStorageUUIDs)
                _AvailableStorageUUIDs.Add((StorageUUID)_StorageUUID.Clone());
            #else
            foreach (var _StorageUUID in myObjectDatastream.AvailableStorageUUIDs)
                _AvailableStorageUUIDs.Add(new StorageUUID(_StorageUUID));
            #endif

            _Compression                = myObjectDatastream.Compression;
            _IntegrityCheckValue        = myObjectDatastream.IntegrityCheckValue;
            _ForwardErrorCorrection     = myObjectDatastream.ForwardErrorCorrection;
            _Redundancy                 = myObjectDatastream.Redundancy;

            _ObjectExtents              = new LinkedList<ObjectExtent>();

            foreach (var _ObjectExtent in myObjectDatastream)
                Add2(new ObjectExtent(_ObjectExtent));

        }

        #endregion

        #endregion


        #region Object-specific methods

        #region Add(myObjectExtent)

        public new Boolean Add(ObjectExtent myObjectExtent)
        {

            _ObjectExtents.AddLast(myObjectExtent);
            _ObjectExtentCount++;
            return true;

        }

        public new Boolean Add2(ObjectExtent myObjectExtent)
        {

            // Endposition = Startposition + Length

            #region Initial checks

            if (myObjectExtent == null)
                throw new ArgumentNullException();

            if (myObjectExtent.Length == 0)
                throw new ArgumentException("The lenght of the given ObjectExtent is invalid!");

            #endregion


            lock (this)
            {

                #region Adding the first extent

                if (_ObjectExtents.Count == 0)
                {

                    if (myObjectExtent.LogicalPosition != 0)
                        throw new ArgumentException("The first extent must start at LogicalPosition 0!");

                    _ObjectExtents.AddFirst(myObjectExtent);
                    _ObjectExtentCount++;

                    
                    
                        

                    return true;

                }

                #endregion

                #region Appending the given ObjectExtent

                else if (myObjectExtent.LogicalPosition == _ObjectExtents.Last.Value.LogicalPosition + _ObjectExtents.Last.Value.Length)
                {
                    _ObjectExtents.AddLast(myObjectExtent);
                    _ObjectExtentCount++;
                    
                    
                        
                    return true;
                }

                #endregion

                else
                {

                    var _ActualNode = _ObjectExtents.First;

                    do
                    {

                        #region Equals

                        // Equals:    |----------------ActualExtent----------------|
                        //            |---------------myObjectExtent---------------|
                        // Result:    |---------------myObjectExtent---------------|
                        if (myObjectExtent.LogicalPosition == _ActualNode.Value.LogicalPosition &&
                            myObjectExtent.Length          == _ActualNode.Value.Length)
                        {

                            // Previous Extent
                            if (_ActualNode.Previous != null)
                                _ActualNode.Previous.Value.NextExtent = new ExtendedPosition(myObjectExtent.StorageUUID, myObjectExtent.PhysicalPosition);

                            _ObjectExtents.AddAfter(_ActualNode, myObjectExtent);
                            _ObjectExtents.Remove(_ActualNode);
                            _ObjectExtentCount++;
                            
                            
                                
                            return true;

                        }

                        #endregion


                        #region Inner

                        // Inner:     |----------------ActualExtent----------------|
                        //            :            |--myObjectExtent--|            :
                        // Result:    |-OldExtent-||--myObjectExtent--||-NewExtent-|
                        // Tested by: OneBigExtent_AddingASmallInTheMiddle()
                        if (myObjectExtent.LogicalPosition > _ActualNode.Value.LogicalPosition &&
                            myObjectExtent.LogicalPosition + myObjectExtent.Length < _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length)
                        {

                            // OldExtent
                            var OldExtent = new ObjectExtent();
                            OldExtent.LogicalPosition               = _ActualNode.Value.LogicalPosition;
                            OldExtent.Length                        = myObjectExtent.LogicalPosition - _ActualNode.Value.LogicalPosition;
                            OldExtent.PhysicalPosition              = _ActualNode.Value.PhysicalPosition;
                            OldExtent.StorageUUID                     = _ActualNode.Value.StorageUUID;
                            OldExtent.NextExtent                    = new ExtendedPosition(myObjectExtent.StorageUUID, myObjectExtent.PhysicalPosition);

                            // NewExtent
                            var NewExtent = new ObjectExtent();
                            NewExtent.LogicalPosition               = myObjectExtent.LogicalPosition + myObjectExtent.Length;
                            NewExtent.Length                        = _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length - myObjectExtent.Length - OldExtent.Length;
                            NewExtent.PhysicalPosition              = _ActualNode.Value.PhysicalPosition + myObjectExtent.Length + OldExtent.Length;
                            NewExtent.StorageUUID                     = _ActualNode.Value.StorageUUID;
                            NewExtent.NextExtent                    = _ActualNode.Value.NextExtent;

                            // myObjectExtent
                            myObjectExtent.NextExtent               = new ExtendedPosition(NewExtent.StorageUUID, NewExtent.PhysicalPosition);

                            // Add all extents after the _ActualNode and remove it afterwards
                            _ObjectExtents.AddAfter(_ActualNode, NewExtent);
                            _ObjectExtents.AddAfter(_ActualNode, myObjectExtent);
                            _ObjectExtents.AddAfter(_ActualNode, OldExtent);
                            _ObjectExtents.Remove(_ActualNode);
                            _ObjectExtentCount++;
                            _ObjectExtentCount++;
                            
                            
                                
                            return true;

                        }

                        #endregion

                        #region InnerLeft

                        // InnerLeft: |----------------ActualExtent----------------|
                        //            |------myObjectExtent------|                 :
                        // Result:    |------myObjectExtent------||---NewExtent----|
                        // Tested by: OneBigExtent_AddingASmallOneIntheBeginning()
                        if (myObjectExtent.LogicalPosition == _ActualNode.Value.LogicalPosition &&
                            myObjectExtent.LogicalPosition + myObjectExtent.Length < _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length)
                        {

                            // NewExtent
                            var NewExtent = new ObjectExtent();
                            NewExtent.LogicalPosition               = myObjectExtent.LogicalPosition + myObjectExtent.Length;
                            NewExtent.Length                        = _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length - myObjectExtent.Length;
                            NewExtent.PhysicalPosition              = _ActualNode.Value.PhysicalPosition + myObjectExtent.Length;
                            NewExtent.StorageUUID                     = _ActualNode.Value.StorageUUID;
                            NewExtent.NextExtent                    = _ActualNode.Value.NextExtent;

                            // Previous Extent
                            if (_ActualNode.Previous != null)
                                _ActualNode.Previous.Value.NextExtent = new ExtendedPosition(myObjectExtent.StorageUUID, myObjectExtent.PhysicalPosition);

                            // myObjectExtent
                            myObjectExtent.NextExtent               = new ExtendedPosition(NewExtent.StorageUUID, NewExtent.PhysicalPosition);

                            // Add all extents after the _ActualNode and remove it afterwards
                            _ObjectExtents.AddAfter(_ActualNode, NewExtent);
                            _ObjectExtents.AddAfter(_ActualNode, myObjectExtent);
                            _ObjectExtents.Remove(_ActualNode);
                            _ObjectExtentCount++;
                            
                            
                                
                            return true;

                        }

                        #endregion

                        #region InnerRight

                        // InnerRight: |----------------ActualExtent----------------|
                        //             :                 |------myObjectExtent------|
                        // Result:     |---OldExtent----||------myObjectExtent------|
                        // Tested by: OneBigExtent_AddingASmallOneAtTheEnd()
                        if (myObjectExtent.LogicalPosition + myObjectExtent.Length == _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length &&
                            myObjectExtent.LogicalPosition                          > _ActualNode.Value.LogicalPosition)
                        {

                            // OldExtent
                            var OldExtent = new ObjectExtent();
                            OldExtent.LogicalPosition               = _ActualNode.Value.LogicalPosition;
                            OldExtent.Length                        = myObjectExtent.LogicalPosition - _ActualNode.Value.LogicalPosition;
                            OldExtent.PhysicalPosition              = _ActualNode.Value.PhysicalPosition;
                            OldExtent.StorageUUID                     = _ActualNode.Value.StorageUUID;
                            OldExtent.NextExtent                    = new ExtendedPosition(myObjectExtent.StorageUUID, myObjectExtent.PhysicalPosition);

                            // myObjectExtent
                            myObjectExtent.NextExtent               = _ActualNode.Value.NextExtent;

                            // Add all extents after the _ActualNode and remove it afterwards
                            _ObjectExtents.AddAfter(_ActualNode, myObjectExtent);
                            _ObjectExtents.AddAfter(_ActualNode, OldExtent);
                            _ObjectExtents.Remove(_ActualNode);
                            _ObjectExtentCount++;
                                
                            return true;

                        }

                        #endregion


                        #region Bigger

                        // Bigger:   |------ActualExtent------|         :
                        //           |----------myObjectExtent----------|
                        // Result:   |XXXXXXX REMOVED XXXXXXXX|         :
                        //
                        // Bigger:   :     |------ActualExtent------|   :
                        //           |----------myObjectExtent----------|
                        // Result:         |XXXXXXX REMOVED XXXXXXXX|   :
                        //
                        // Bigger:   :         |------ActualExtent------|
                        //           |----------myObjectExtent----------|
                        // Result:   :         |XXXXXXX REMOVED XXXXXXXX|
                        if (_ActualNode.Value.LogicalPosition >= myObjectExtent.LogicalPosition &&
                            _ActualNode.Value.LogicalPosition + _ActualNode.Value.Length <= myObjectExtent.Length)
                        {


                            // Previous Extent
                            if (_ActualNode.Previous != null)
                                _ActualNode.Previous.Value.NextExtent = new ExtendedPosition(myObjectExtent.StorageUUID, myObjectExtent.PhysicalPosition);

                        }

                        #endregion


                        // Greater: |---ActualExtent---|               :
                        //          :               |--myObjectExtent--|
                        // Result:  |-ActualExtent-||--myObjectExtent--|


                        // Smaller: :               |---ActualExtent---|
                        //          |--myObjectExtent--|               :
                        // Result:  |--myObjectExtent--||-ActualExtent-|


                        _ActualNode = _ActualNode.Next;

                    } while (_ActualNode != null);

                }

                isDirty = true;
                return true;
            
            }

        }


        public Boolean AddReserve(ObjectExtent myObjectExtent)
        {
            
            var _ReturnValue = Add2(myObjectExtent);

            if (_ReturnValue)
                ReservedLength = myObjectExtent.Length;

            return _ReturnValue;

        }

        #endregion

        #region Add(myObjectExtents)

        public Boolean Add(IEnumerable<ObjectExtent> myObjectExtents)
        {
            lock (this)
            {

                foreach (var _ObjectExtent in myObjectExtents)
                    Add(_ObjectExtent);                    

                return true;
            
            }
        }

        #endregion

        #region Replace(myObjectExtent, OldLength)

        public Boolean Replace(ObjectExtent myObjectExtent, UInt64 OldLength)
        {
            return false;
        }

        #endregion

        #region Remove(LogicalPosition, Length)

        public Boolean Remove(UInt64 LogicalPosition, UInt64 Length)
        {
            return false;
        }

        #endregion


        #region this[myNumber]

        public ObjectExtent this[UInt64 myNumber]
        {

            get
            {

                if (myNumber < _ObjectExtentCount)//_ObjectExtents.ULongCount())
                {

                    try
                    {

                        var _Node = _ObjectExtents.First;

                        for (var i = myNumber; i != 0; i--)
                            _Node = _Node.Next;

                        return _Node.Value;

                    }

                    catch (Exception e)
                    {
                        throw new ArgumentException("Something is wrong!" + e);
                    }

                }

                throw new ArgumentException("Parameter myNumber is invalid!");

            }

        }

        #endregion

        #region Count

        public UInt64 Count
        {
            get
            {
                //return _ObjectExtents.ULongCount();
                return _ObjectExtentCount;
            }
        }

        #endregion


        #region Duplicate()

        public void Duplicate()
        {

            var _NewObjectDatastream = new ObjectDatastream();
            _NewObjectDatastream.ReservedLength             = _ReservedLength;
            _NewObjectDatastream.Blocksize                  = _Blocksize;
            _NewObjectDatastream.AvailableStorageUUIDs      = _AvailableStorageUUIDs;
            _NewObjectDatastream.Compression                = _Compression;
            _NewObjectDatastream.IntegrityCheckValue        = _IntegrityCheckValue;
            _NewObjectDatastream.ForwardErrorCorrection     = _ForwardErrorCorrection;
            _NewObjectDatastream.Redundancy                 = _Redundancy;

            foreach (var _ObjectExtent in _ObjectExtents)
                _NewObjectDatastream.Add2(new ObjectExtent(_ObjectExtent));

        }

        #endregion


        //public ObjectDatastream ChangeReservedSize(UInt64 myNumber)
        //{

        //    var _ObjectDatastream = new ObjectDatastream();

        //    _ObjectPath     = _ObjectDatastream.ObjectPath;
        //    _ReservedLength = _ObjectDatastream.ReservedLength - myNumber;

        //    _AvailableStorageUUIDs = new List<StorageUUID>();

        //    foreach (var _StorageUUID in _ObjectDatastream.AvailableStorageUUIDs)
        //        _AvailableStorageUUIDs.Add(new StorageUUID(_StorageUUID));

        //    _Compression = _ObjectDatastream.Compression;
        //    _IntegrityCheckValue = _ObjectDatastream.IntegrityCheckValue;
        //    _ForwardErrorCorrection = _ObjectDatastream.ForwardErrorCorrection;
        //    _Redundancy = _ObjectDatastream.Redundancy;

        //    _ObjectExtents = new LinkedList<ObjectExtent>();

        //    foreach (var _ObjectExtent in _ObjectDatastream)
        //        Add2(new ObjectExtent(_ObjectExtent));

        //    return _ObjectDatastream;

        //}

        public IEnumerable<ObjectExtent> UseReserve(UInt64 myNumberOfBytesToUse, UInt64 myLogicalPosition)
        {

            if (ReservedLength < myNumberOfBytesToUse)
                throw new ArgumentException("ReservedLength < myNumberOfBytesToUse");

            lock (this)
            {

                var _ListOfObjectExtents = new List<ObjectExtent>();

                foreach (var _ObjectExtent in _ObjectExtents)
                {

                    if (myLogicalPosition >= _ObjectExtent.LogicalPosition && myLogicalPosition < _ObjectExtent.LogicalPosition + _ObjectExtent.Length)
                    {

                        // Found a useable starting ObjectExtent!
                        var _BytesUsed = Math.Min(myNumberOfBytesToUse, _ObjectExtent.Length);
                        _ListOfObjectExtents.Add(new ObjectExtent(myLogicalPosition, _BytesUsed, _ObjectExtent.PhysicalPosition + myLogicalPosition, _ObjectExtent.StorageUUID));

                        if (myNumberOfBytesToUse == _BytesUsed)
                            break;

                        myNumberOfBytesToUse -= _BytesUsed;
                        myLogicalPosition    += _BytesUsed;

                    }

                }

                ReservedLength -= myNumberOfBytesToUse;

                return _ListOfObjectExtents;

            }

        }

        #endregion


        #region IEnumerable<ObjectExtent> Members

        public IEnumerator<ObjectExtent> GetEnumerator()
        {
            return _ObjectExtents.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _ObjectExtents.GetEnumerator();
        }

        #endregion

        #region IObjectLocation Members

        #region ObjectPath

        [NonSerialized]
        private ObjectLocation _ObjectPath;

        [NotIFastSerialized]
        public ObjectLocation ObjectPath
        {

            get
            {
                return _ObjectPath;
            }

            set
            {
                _ObjectPath     = value;
                _ObjectLocation = new ObjectLocation(_ObjectPath, _ObjectName);
            }

        }

        #endregion

        #region ObjectName

        [NonSerialized]
        private String _ObjectName;

        [NotIFastSerialized]
        public String ObjectName
        {

            get
            {
                return _ObjectName;
            }

            set
            {
                _ObjectName      = value;
                _ObjectLocation = new ObjectLocation(_ObjectPath, _ObjectName);
            }

        }

        #endregion

        #region ObjectLocation

        [NonSerialized]
        private ObjectLocation _ObjectLocation;

        [NotIFastSerialized]
        public ObjectLocation ObjectLocation
        {

            get
            {
                return _ObjectLocation;
            }

            set
            {
                _ObjectLocation  = value;
                _ObjectPath      = _ObjectLocation.Path;
                _ObjectName      = _ObjectLocation.Name;

            }

        }

        #endregion

        #endregion

        #region IDirectoryListing Members

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

        #region ObjectExists(myObjectName)

        public Trinary ObjectExists(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.DotLink))
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.DotDotLink))
                return Trinary.TRUE;

            if (myObjectName.Equals(_ThisObjectName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_ObjectUUIDName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_StreamLengthName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_ReservedLengthName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_CompressionAlgorithmName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_IntegrityCheckValueName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_BlockIntegrityArraysName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_ForwardErrorCorrectionName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_AvailableStorageUUIDsName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_RedundancyName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_ExtentsName))
                return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region ObjectStreamExists(myObjectName, myObjectStream)

        public Trinary ObjectStreamExists(String myObjectName, String myObjectStream)
        {

            if (myObjectName.Equals(FSConstants.DotLink)     && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.DotDotLink)  && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            var _TmpString = _ObjectLocation.Substring(_ObjectLocation.IndexOf(FSConstants.DotStreams) + FSConstants.DotStreams.Length + FSPathConstants.PathDelimiter.Length);

            if (myObjectName.Equals(_ThisObjectName)             && myObjectStream == _TmpString.Substring(0, _TmpString.IndexOf(FSPathConstants.PathDelimiter)))
                return Trinary.TRUE;

            if (myObjectName.Equals(_ObjectUUIDName)             && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            if (myObjectName.Equals(_StreamLengthName)           && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            if (myObjectName.Equals(_ReservedLengthName)         && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            if (myObjectName.Equals(_CompressionAlgorithmName)   && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(_IntegrityCheckValueName)    && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            if (myObjectName.Equals(_BlockIntegrityArraysName)   && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(_ForwardErrorCorrectionName) && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(_AvailableStorageUUIDsName)    && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(_RedundancyName)             && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(_ExtentsName)                && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region GetObjectStreamsList(myObjectName)

        public IEnumerable<String> GetObjectStreamsList(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.DotLink))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            if (myObjectName.Equals(FSConstants.DotDotLink))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            var _TmpString = _ObjectLocation.Substring(_ObjectLocation.IndexOf(FSConstants.DotStreams) + FSConstants.DotStreams.Length + FSPathConstants.PathDelimiter.Length);

            if (myObjectName.Equals(_ThisObjectName))
                return new List<String> { _TmpString.Substring(0, _TmpString.IndexOf(FSPathConstants.PathDelimiter)) };

            if (myObjectName.Equals(_ObjectUUIDName))
                return new List<String> { FSConstants.INLINEDATA };

            if (myObjectName.Equals(_StreamLengthName))
                return new List<String> { FSConstants.INLINEDATA };

            if (myObjectName.Equals(_ReservedLengthName))
                return new List<String> { FSConstants.INLINEDATA };

            if (myObjectName.Equals(_CompressionAlgorithmName))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            if (myObjectName.Equals(_IntegrityCheckValueName))
                return new List<String> { FSConstants.INLINEDATA };

            if (myObjectName.Equals(_BlockIntegrityArraysName))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            if (myObjectName.Equals(_ForwardErrorCorrectionName))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            if (myObjectName.Equals(_AvailableStorageUUIDsName))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            if (myObjectName.Equals(_RedundancyName))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            if (myObjectName.Equals(_ExtentsName))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            return new List<String>();

        }

        #endregion

        #region GetObjectINodePositions(myObjectName)

        public IEnumerable<ExtendedPosition> GetObjectINodePositions(String myObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetInlineData(myObjectName)

        public Byte[] GetInlineData(String myObjectName)
        {

            //if (myObjectName.Equals(_ObjectUUIDName))
            //    return Encoding.UTF8.GetBytes(ObjectUUID.ToHexString(SeperatorTypes.COLON));

            if (myObjectName.Equals(_StreamLengthName))
                return Encoding.UTF8.GetBytes(StreamLength.ToString());

            if (myObjectName.Equals(_ReservedLengthName))
                return Encoding.UTF8.GetBytes(ReservedLength.ToString());

            if (myObjectName.Equals(_IntegrityCheckValueName))
                return Encoding.UTF8.GetBytes(IntegrityCheckValue.ToHexString(SeperatorTypes.COLON));

            return new Byte[0];

        }

        #endregion

        #region hasInlineData(myObjectName)

        public Trinary hasInlineData(String myObjectName)
        {
            return ObjectStreamExists(myObjectName, FSConstants.INLINEDATA);
        }

        #endregion

        #region GetSymlink(myObjectName)

        public ObjectLocation GetSymlink(String myObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region isSymlink(myObjectName)

        public Trinary isSymlink(String myObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetDirectoryListing()

        public IEnumerable<String> GetDirectoryListing()
        {

            var _DirectoryListing = new List<String>();

            _DirectoryListing.Add(".");
            _DirectoryListing.Add("..");
            _DirectoryListing.Add(_ThisObjectName);
            _DirectoryListing.Add(_ObjectUUIDName);
            _DirectoryListing.Add(_StreamLengthName);
            _DirectoryListing.Add(_ReservedLengthName);
            _DirectoryListing.Add(_CompressionAlgorithmName);
            _DirectoryListing.Add(_IntegrityCheckValueName);
            _DirectoryListing.Add(_BlockIntegrityArraysName);
            _DirectoryListing.Add(_ForwardErrorCorrectionName);
            _DirectoryListing.Add(_AvailableStorageUUIDsName);
            _DirectoryListing.Add(_RedundancyName);
            _DirectoryListing.Add(_ExtentsName);

            return _DirectoryListing;

        }

        #endregion

        #region GetDirectoryListing(myFunc)

        public IEnumerable<String> GetDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetDirectoryListing(myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

        public IEnumerable<String> GetDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreamTypes)
        {
            return GetDirectoryListing();
        }

        #endregion

        #region GetExtendedDirectoryListing()

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing()
        {

            var _ExtendedDirectoryListing = new List<DirectoryEntryInformation>();
            var _OutputParameter = new DirectoryEntryInformation();

            _OutputParameter.Name = ".";
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name = "..";
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            var _TmpString = _ObjectLocation.Substring(_ObjectLocation.IndexOf(FSConstants.DotStreams) + FSConstants.DotStreams.Length + FSPathConstants.PathDelimiter.Length);

            _OutputParameter.Name = _ThisObjectName;
            _OutputParameter.Streams = new HashSet<String> { _TmpString.Substring(0, _TmpString.IndexOf(FSPathConstants.PathDelimiter)) };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name = _ObjectUUIDName;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.INLINEDATA };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name = _StreamLengthName;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.INLINEDATA };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name = _ReservedLengthName;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.INLINEDATA };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name = _CompressionAlgorithmName;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name = _IntegrityCheckValueName;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.INLINEDATA };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name = _BlockIntegrityArraysName;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name = _ForwardErrorCorrectionName;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name = _AvailableStorageUUIDsName;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name = _RedundancyName;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name = _ExtentsName;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            return _ExtendedDirectoryListing;

        }

        #endregion

        #region GetExtendedDirectoryListing(myFunc)

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetExtendedDirectoryListing(myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing(string[] myName, string[] myIgnoreName, string[] myRegExpr, List<String> myObjectStream, List<String> myIgnoreObjectStreamTypes)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DirCount()

        public UInt64 DirCount
        {
            get
            {
                return GetDirectoryListing().ULongCount();
            }
        }

        #endregion


        public NHIDirectoryObject NotificationHandling
        {
            get { throw new NotImplementedException(); }
        }

        public void SubscribeNotification(NHIDirectoryObject myNotificationHandling)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeNotification(NHIDirectoryObject myNotificationHandling)
        {
            throw new NotImplementedException();
        }


        #region GetDirectoryEntry(myObjectName)

        public DirectoryEntry GetDirectoryEntry(String myObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion


        #region isNew

        [NonSerialized]
        private Boolean _isNew;

        [NotIFastSerialized]
        public Boolean isNew
        {

            get
            {
                return _isNew;
            }

            set
            {
                _isNew = value;
            }

        }

        #endregion

        #region isDirty

        [NonSerialized]
        private Boolean _isDirty;

        [NotIFastSerialized]
        public Boolean isDirty
        {

            get
            {
                return _isDirty;
            }

            set
            {
                _isDirty = value;
            }

        }

        #endregion

    
    }

}

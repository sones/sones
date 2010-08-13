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


/*
 * GraphFSInterface - INode
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using sones.Lib.Serializer;
using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures.Timestamp;
using sones.Lib.XML;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures;
using sones.StorageEngines;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Exceptions;

#endregion

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    /// The INode stores the common attributes of an object, object
    /// safety and security options and the ObjectLocatorPositions.
    /// It has a fixed length of 256 byte!
    /// </summary>

    public class INode : AFSStructure
    {


        #region DefaultValues
        // More default values are defined within the INode constructor!

        private const UInt16 actualStructureVersion             = 1;

        public static UInt16 _defaultINodeSize                  = 256;

        private const UInt16 MaxNumberOfObjectLocatorPositions  = 5;    // The rule of five ;)
        private const UInt16 MaxNumberOfINodePositions          = 5;    // The rule of five ;)

        #endregion


        #region Constructors

        #region INode()

        /// <summary>
        /// This will create an (mostly) empty INode
        /// </summary>
        public INode()
        {

            // Members of AGraphStructure
            _StructureVersion             = actualStructureVersion;
            _IntegrityCheckValue          = null;
            _EncryptionParameters         = null;
            ObjectUUID                    = new ObjectUUID();
            _INodePositions               = new List<ExtendedPosition>();
            _INodeReference               = this;
            _ObjectLocatorReference       = null;

            // Common attributes
            _CreationTime                 = TimestampNonce.Ticks;
            _LastAccessTime               = _CreationTime;
            _LastModificationTime         = _CreationTime;
            _DeletionTime                 = UInt64.MinValue;

            // Object Safety and Security
            _IntegrityCheckAlgorithm      = IntegrityCheckTypes.NULLAlgorithm;
            _EncryptionAlgorithm          = SymmetricEncryptionTypes.NULLAlgorithm;

            // ObjectLocatorPositions
            _ObjectLocatorLength          = 0;
            _ObjectLocatorCopies          = FSConstants.ObjectLocatorsCopies;
            _ObjectLocatorPositions       = new List<ExtendedPosition>();


            // Mark INode dirty
            isDirty                       = true;

        }

        #endregion

        #region INode(myObjectUUID)

        /// <summary>
        /// This will create an (mostly) empty INode using the given ObjectUUID
        /// </summary>
        public INode(ObjectUUID myObjectUUID)
            : this()
        {
            ObjectUUID = myObjectUUID;
        }

        #endregion

        #region INode(mySerializedData)

        /// <summary>
        /// A constructor of the INode used for fast deserializing
        /// </summary>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized INode</param>
        public INode(Byte[] mySerializedData)
        {
            Deserialize(mySerializedData);
            _isNew = false;
        }

        #endregion

        #region INode(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor of the INode used for fast deserializing
        /// </summary>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized INode</param>
        public INode(Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
        {
            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm);
            _isNew = false;
        }

        #endregion

        #endregion


        #region INodePositions - Points to the ExtendedPositions of this INode

        protected List<ExtendedPosition> _INodePositions;

        /// <summary>
        /// Points to the ExtendedPositions of this INode.
        /// </summary>
        public List<ExtendedPosition> INodePositions
        {

            get
            {
                return _INodePositions;
            }

            set
            {
                _INodePositions  = value;
                isDirty          = true;
            }

        }

        #endregion


        #region Common attributes

        #region CreationTime

        private UInt64 _CreationTime;

        /// <summary>
        /// Documents the timestamp of the initial object creation.
        /// </summary>
        public UInt64 CreationTime
        {
            get
            {
                return _CreationTime;
            }

        }

        #endregion

        #region LastAccessTime

        private UInt64 _LastAccessTime;

        /// <summary>
        /// Documents the timestamp of the last object access.
        /// This feature may be deactivated in order to increase the
        /// overall performance of the filesystem.
        /// </summary>
        public UInt64 LastAccessTime
        {
            get
            {
                return _LastAccessTime;
            }

            set
            {
                _LastAccessTime  = value;
                isDirty          = true;
            }

        }

        #endregion
        
        #region LastModificationTime

        private UInt64 _LastModificationTime;

        /// <summary>
        /// Documents the timestamp of the last object modification.
        /// </summary>
        public UInt64 LastModificationTime
        {

            get
            {
                return _LastModificationTime;
            }

            set
            {
                _LastModificationTime  = value;
                isDirty                = true;
            }

        }

        #endregion

        #region DeletionTime

        private UInt64 _DeletionTime;

        /// <summary>
        /// Documents the timestamp of the object deletion
        /// </summary>
        public UInt64 DeletionTime
        {
            get
            {
                return _DeletionTime;
            }
        }

        #endregion

        #region ReferenceCount - Counts the number of directory entries pointing to this INode

        protected UInt16 _ReferenceCount;

        /// <summary>
        /// Counts the number of directory entries pointing to this INode.
        /// </summary>
        [NotIFastSerialized]
        public UInt16 ReferenceCount
        {

            get
            {
                return _ReferenceCount;
            }

            set
            {
                _ReferenceCount  = value;
                isDirty          = true;
            }

        }

        #endregion

        #region ObjectSize

        private UInt64 _ObjectSize;

        /// <summary>
        /// Documents the size of the object, which means the accumulated
        /// size of all actual ObjectStreams.
        /// </summary>
        public UInt64 ObjectSize
        {

            get
            {
                return _ObjectSize;
            }

            set
            {
                _ObjectSize  = value;
                isDirty      = true;
            }

        }

        #endregion

        #endregion


        #region ObjectSafety and -Security

        #region IntegrityCheckAlgorithm

        private IntegrityCheckTypes _IntegrityCheckAlgorithm;

        /// <summary>
        /// This chooses the algorithm for saving the integrity of all
        /// ObjectStreams. There is also a special ObjectStream called
        /// BlockIntegrityObject which divides your ObjectStream in
        /// smaller pieces and saves the integrity of every piece.
        /// </summary>
        public IntegrityCheckTypes IntegrityCheckAlgorithm
        {

            get
            {
                return _IntegrityCheckAlgorithm;
            }

            set
            {
                _IntegrityCheckAlgorithm  = value;
                isDirty                   = true;
            }

        }
        
        #endregion

        #region EncryptionAlgorithm

        private SymmetricEncryptionTypes _EncryptionAlgorithm;

        /// <summary>
        /// This chooses the algorithm for encrypting all ObjectStreams.
        /// The crypto key can be stored within the EncryptionKey property
        /// of the different User(-groups). Additional crypto material
        /// (seeds etc.) can be stored with the EncryptionValue property
        /// of every ObjectStream.
        /// Best practic at this point is to use a fast symmetric crypto
        /// algorithm like AES.
        /// </summary>
        public SymmetricEncryptionTypes EncryptionAlgorithm
        {

            get
            {
                return _EncryptionAlgorithm;
            }

            set
            {
                _EncryptionAlgorithm  = value;
                isDirty      = true;
            }

        }

        #endregion

        #endregion


        #region ObjectLocator

        #region ObjectLocatorLength

        UInt64 _ObjectLocatorLength;

        /// <summary>
        /// The size of the serialized ObjectLocator
        /// </summary>
        public UInt64 ObjectLocatorLength
        {

            get
            {
                return _ObjectLocatorLength;
            }

            set
            {
                _ObjectLocatorLength  = value;
                isDirty               = true;
            }

        }

        #endregion

        #region ObjectLocatorCopies

        UInt64 _ObjectLocatorCopies;

        /// <summary>
        /// Number of ObjectLocator copies to store.
        /// </summary>
        public UInt64 ObjectLocatorCopies
        {

            get
            {
                return _ObjectLocatorCopies;
            }

            set
            {
                _ObjectLocatorCopies = value;
                isDirty              = true;
            }

        }

        #endregion

        #region ObjectLocatorPositions

        List<ExtendedPosition> _ObjectLocatorPositions;

        /// <summary>
        /// The ExtendedPositions within the filesystem, where to find
        /// the ObjectLocator.
        /// </summary>
        public List<ExtendedPosition> ObjectLocatorPositions
        {

            get
            {
                return _ObjectLocatorPositions;
            }

            set
            {
                _ObjectLocatorPositions  = value;
                isDirty                  = true;
            }

        }

        #endregion

        #endregion

        #region ObjectLocatorStates

        private ObjectLocatorStates _ObjectLocatorStates;

        /// <summary>
        /// The INode must not be deleted before all revisions are deleted
        /// </summary>
        public ObjectLocatorStates ObjectLocatorStates
        {
            get
            {
                return _ObjectLocatorStates;
            }
            set
            {
                _ObjectLocatorStates = value;
            }
        }

        #endregion


        #region CopyTo

        public void CopyTo(INode myINode)
        {

            myINode._StructureVersion   = _StructureVersion;
            myINode.ObjectUUID          = ObjectUUID;

            #region Copy INode-specific Data

            #region Copy Common attributes

            myINode._CreationTime = _CreationTime;
            myINode._LastAccessTime = _LastAccessTime;
            myINode._LastModificationTime = _LastModificationTime;
            myINode._DeletionTime = _DeletionTime;
            myINode._ObjectSize = _ObjectSize;

            #endregion

            #region Object Safety and Security

            myINode._IntegrityCheckAlgorithm = _IntegrityCheckAlgorithm;
            myINode._EncryptionAlgorithm = _EncryptionAlgorithm;

            #endregion

            #region Copy list of ObjectLocatorPositions

            myINode._ObjectLocatorLength = _ObjectLocatorLength;
            myINode._ObjectLocatorCopies = _ObjectLocatorCopies;
            myINode._ObjectLocatorPositions = _ObjectLocatorPositions;

            #endregion

            #region Clone list of INodePositions

            myINode._INodePositions = _INodePositions;

            #endregion

            #endregion

            myINode._ObjectLocatorReference = _ObjectLocatorReference;
        }

        #endregion

        #region Clone()

        public override AFSStructure Clone()
        {

            var newINode = new INode();
            newINode._StructureVersion  = _StructureVersion;
            newINode.ObjectUUID         = ObjectUUID;

            #region Clone INode-specific Data

            #region Clone Common attributes

            newINode._CreationTime = _CreationTime;
            newINode._LastAccessTime = _LastAccessTime;
            newINode._LastModificationTime = _LastModificationTime;
            newINode._DeletionTime = _DeletionTime;
            newINode._ObjectSize = _ObjectSize;

            #endregion

            #region Object Safety and Security

            newINode._IntegrityCheckAlgorithm = _IntegrityCheckAlgorithm;
            newINode._EncryptionAlgorithm = _EncryptionAlgorithm;

            #endregion

            #region Clone list of ObjectLocatorPositions

            newINode._ObjectLocatorLength = _ObjectLocatorLength;
            newINode._ObjectLocatorCopies = _ObjectLocatorCopies;

            newINode._ObjectLocatorPositions = new List<ExtendedPosition>();

            foreach (var _ExtendedPosition in _ObjectLocatorPositions)
                newINode._ObjectLocatorPositions.Add(new ExtendedPosition(_ExtendedPosition.StorageUUID, _ExtendedPosition.Position));

            #endregion

            #region Clone list of INodePositions

            newINode._INodePositions = new List<ExtendedPosition>();

            foreach (var _ExtendedPosition in _INodePositions)
                newINode._INodePositions.Add(new ExtendedPosition(_ExtendedPosition.StorageUUID, _ExtendedPosition.Position));

            #endregion

            #region State

            newINode._ObjectLocatorStates = _ObjectLocatorStates;

            #endregion

            #endregion

            newINode._ObjectLocatorReference = _ObjectLocatorReference;

            return newINode;

        }

        #endregion

        #region ToString()

        public override String ToString()
        {
            return String.Concat(IntegrityCheckAlgorithm.ToString(), ", ", EncryptionAlgorithm.ToString());
        }

        #endregion


        #region IFastSerialize Member

        #region isDirty - Overwrite AGraphStructure property

        [NotIFastSerialized]
        public new Boolean isDirty
        {

            get
            {

                if (_ModificationTime > DateTime.MinValue) return true;
                    else return false;

            }

            set
            {

                if (value)    // isDirty = true
                    _ModificationTime       = TimestampNonce.Now;

                else          // isDirty = false
                    _ModificationTime       = DateTime.MinValue;

            }

        }


        #endregion

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {

            try
            {

                #region Write Common attributes

                mySerializationWriter.WriteUInt64(_CreationTime);
                mySerializationWriter.WriteUInt64(_LastAccessTime);
                mySerializationWriter.WriteUInt64(_LastModificationTime);
                mySerializationWriter.WriteUInt64(_DeletionTime);
                mySerializationWriter.WriteUInt64(_ObjectSize);

                #endregion

                #region Object Safety and Security

                mySerializationWriter.WriteByte((Byte)_IntegrityCheckAlgorithm);
                mySerializationWriter.WriteByte((Byte)_EncryptionAlgorithm);

                #endregion

                #region Write list of ObjectLocatorPositions

                mySerializationWriter.WriteUInt64(_ObjectLocatorLength);
                mySerializationWriter.WriteUInt64(_ObjectLocatorCopies);

                mySerializationWriter.WriteUInt16((UInt16)Math.Min(_ObjectLocatorPositions.Count, MaxNumberOfObjectLocatorPositions));

                for (int i=0; i < Math.Min(_ObjectLocatorPositions.Count, MaxNumberOfObjectLocatorPositions - 1); i++)
                {
                    _ObjectLocatorPositions[i].StorageUUID.Serialize(ref mySerializationWriter);
                    mySerializationWriter.WriteUInt64(_ObjectLocatorPositions[i].Position);
                }

                #endregion

                #region Write list of INodePositions

                mySerializationWriter.WriteUInt16((UInt16)Math.Min(_INodePositions.Count, MaxNumberOfINodePositions));

                for (int i=0; i < Math.Min(_INodePositions.Count, MaxNumberOfINodePositions - 1); i++)
                {
                    _INodePositions[i].StorageUUID.Serialize(ref mySerializationWriter);
                    mySerializationWriter.WriteUInt64(_INodePositions[i].Position);
                }

                #endregion

                #region Write State

                mySerializationWriter.WriteByte((Byte)_ObjectLocatorStates);

                #endregion


            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }

        }

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {

            UInt16 NumberOfObjectLocatorPositions;
            UInt16 NumberOfINodePositions;

            try
            {

                #region Read Common attributes

                _CreationTime                   =           mySerializationReader.ReadUInt64();
                _LastModificationTime           =           mySerializationReader.ReadUInt64();
                _LastAccessTime                 =           mySerializationReader.ReadUInt64();
                _DeletionTime                   =           mySerializationReader.ReadUInt64();
                _ObjectSize                     =           mySerializationReader.ReadUInt64();

                #endregion

                #region Object Safety and Security

                _IntegrityCheckAlgorithm        =           (IntegrityCheckTypes)mySerializationReader.ReadOptimizedByte();
                _EncryptionAlgorithm            =           (SymmetricEncryptionTypes)mySerializationReader.ReadOptimizedByte();

                #endregion

                #region Read list of ObjectLocatorPositions

                _ObjectLocatorLength            =           mySerializationReader.ReadUInt64();
                _ObjectLocatorCopies            =           mySerializationReader.ReadUInt64();

                NumberOfObjectLocatorPositions  =           mySerializationReader.ReadUInt16();
                _ObjectLocatorPositions         =           new List<ExtendedPosition>();

                for (int i = 1; i <= NumberOfObjectLocatorPositions; i++)
                    _ObjectLocatorPositions.Add(new ExtendedPosition(new StorageUUID(mySerializationReader.ReadByteArray()), mySerializationReader.ReadUInt64()));

                #endregion

                #region Read list of INodePositions

                NumberOfINodePositions          =           mySerializationReader.ReadUInt16();
                _INodePositions                 =           new List<ExtendedPosition>();

                for (int i = 1; i <= NumberOfINodePositions; i++)
                    _INodePositions.Add(new ExtendedPosition(new StorageUUID(mySerializationReader.ReadByteArray()), mySerializationReader.ReadUInt64()));

                #endregion

                #region Read State

                _ObjectLocatorStates = (ObjectLocatorStates)mySerializationReader.ReadOptimizedByte();

                #endregion

            }
            
            catch (Exception e)
            {
                throw new GraphFSException_INodeCouldNotBeDeserialized("INode could not be deserialized!\n\n" + e);
            }

            //isDirty = true;       // this is not useful!

        }

        #endregion

    }

}

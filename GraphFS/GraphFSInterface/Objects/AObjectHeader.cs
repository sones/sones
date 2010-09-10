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
 * AObjectHeader
 * (c) Achim Friedland, 2008 - 2010
 * 
 * The abstract class for all PandoraFS file system structures
 * 
 *  * Layout of the APandoraStructure Header:
 * -------------------------------------------------
 * IFastSerialize Header            - 4 bytes
 * 
 * Header Version                   - 1 byte 
 * Length of IntegrityCheckValue    - 1 byte  >> 3
 * Length of EncryptionParameters   - 1 byte  >> 3
 * Length of Data Padding           - 1 bytes
 * Length of Additional Padding     - 1 bytes >> 3
 * Reserved                         - 2 bytes
 * IntegrityCheckValue              - n bytes
 * EncryptionParameters             - m bytes
 * 
 * StructureVersion                 - 2 bytes
 * ObjectUUID                       - o bytes
 * SerializedObject                 - p bytes
 * 
 * Data Padding                     - q bytes
 * Additional Padding               - r bytes
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.Lib.Serializer;
using sones.Lib.DataStructures.Timestamp;
using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.Lib.Session;
using sones.Lib.DataStructures.WeakReference;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The abstract class for all GraphFS structures
    /// </summary>

    public abstract class AObjectHeader : IObjectHeader
    {

        #region Data 

        protected readonly Byte  HeaderVersion  = 1;
        protected readonly Byte  HeaderLength   = 8;

        #endregion

        #region Properties

        #region isNew - indicates that this APandoraStructure is new

        [NonSerialized]
        protected Boolean _isNew = false;

        /// <summary>
        /// Indicates that this APandoraStructure was newly created.
        /// This is e.g. evaluated within StoreAPandoraObject_private(...) and will
        /// lead to a new revision of the ParentDirectoyObject.
        /// </summary>
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


        #region StructureVersion - Documents the version of the structure used

        protected UInt16 _StructureVersion;

        /// <summary>
        /// Documents the version of the structure used.
        /// Purpose: Backward compatibility with older (on-disc) formats of this
        ///          file system structure.
        /// </summary>
        public UInt16 StructureVersion
        {

            get
            {
                return _StructureVersion;
            }

        }

        #endregion

        #region IntegrityCheckValue - An array of bytes for saving the integrity of this structure

        protected Byte[] _IntegrityCheckValue;

        /// <summary>
        /// An array of bytes for saving the integrity of this structure on disc.
        /// Purpose: File system safety and integrity even in the presence of
        /// biterrors on disc. If there are several copies of this structure on
        /// disc, the file system can load a version which integrity check doesn't
        /// fail.
        /// </summary>
        public Byte[] IntegrityCheckValue
        {

            get
            {
                return _IntegrityCheckValue;
            }

        }

        #endregion

        #region EncryptionParameters - An array of bytes for saving the parameters of the encryption algorithm

        protected Byte[] _EncryptionParameters;

        /// <summary>
        /// An array of bytes for saving the parameters for the encryption of this
        /// structure (e.g. the initialisation vector).
        /// Purpose: File system security by encrypting the structures and objects
        /// of this file system.
        /// </summary>
        [NotIFastSerialized]
        public Byte[] EncryptionParameters
        {

            get
            {
                return _EncryptionParameters;
            }

            set
            {
                _EncryptionParameters  = value;
                isDirty                = true;
            }

        }

        #endregion


        #region ObjectUUID - An unique but structured identifier of an object

        /// <summary>
        /// An unique identifier for this structure or object. The first bytes of
        /// this UUID should be common within a file system object group. Such a
        /// group can e.g. consist of an INode, an ObjectLocator, an DirectoryObject,
        /// an MetadataObject and a PreviewObject.
        /// [UUID][Type][ObjectStream][ObjectVersion]
        /// Purpose: (1) Identify unambiguously an object and it's object group
        /// especcially within a file system cluster. (2) File system safety after
        /// a crash. It should be possible to find INodes, ObjectLocators and
        /// ObjectStreams belonging together by low-level scans or even by hand
        /// after the file system structures are fundamentally broken.
        /// </summary>
        public ObjectUUID ObjectUUID { get; protected set; }

        #endregion

        #region INodeReference - A reference to the INode of this PandoraStructure

        [NonSerialized]
        [NotIFastSerialized]
        protected INode _INodeReference;

        /// <summary>
        /// A reference to the INode of this PandoraStructure.
        /// Purpose: Give fast access to the information stored within the INode
        /// </summary>
        [NotIFastSerialized]
        public INode INodeReference
        {

            get
            {
                return _INodeReference;
            }

            set
            {
                _INodeReference  = value;
                isDirty          = true;
            }

        }

        #endregion

        #region ObjectLocatorReference - A reference to the ObjectLocator of this PandoraStructure

        [NonSerialized]
        [NotIFastSerialized]
        protected ObjectLocator _ObjectLocatorReference;

        /// <summary>
        /// A reference to the ObjectLocator of this PandoraStructure.
        /// Purpose: Give fast access to the information stored within the ObjectLocator
        /// </summary>
        [NotIFastSerialized]
        public ObjectLocator ObjectLocatorReference
        {

            get
            {
                return _ObjectLocatorReference;
            }

            set
            {
                _ObjectLocatorReference  = value;
                isDirty                  = true;
            }

        }

        #endregion

        #region SessionToken

        [NonSerialized]
        protected SessionToken _SessionToken;
        
        /// <summary>
        /// SessionToken
        /// </summary>
        public SessionToken SessionToken
        {

            get
            {
                return _SessionToken;
            }

            set
            {
                _SessionToken = value;
            }

        }

        #endregion

        #region IGraphFSReference

        [NonSerialized]
        protected WeakReference<IGraphFS> _IGraphFSReference;

        /// <summary>
        /// IPandoraFSReference
        /// </summary>
        public WeakReference<IGraphFS> IGraphFSReference
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

        #region IGraphFSSessionReference

        [NonSerialized]
        protected WeakReference<IGraphFSSession> _IGraphFSSessionReference;

        /// <summary>
        /// FileSystemReference
        /// </summary>
        public WeakReference<IGraphFSSession> IGraphFSSessionReference
        {

            get
            {
                return _IGraphFSSessionReference;
            }

            set
            {
                _IGraphFSSessionReference = value;
            }

        }

        #endregion


        #region SerializedAPandoraStructure - in-memory only

        [NonSerialized]
        [NotIFastSerialized]
        protected Byte[] _SerializedAPandoraStructure;

        /// <summary>
        /// The APandoraStructure serialized into an array of bytes
        /// </summary>
        [NotIFastSerialized]
        public Byte[] SerializedAPandoraStructure
        {

            get
            {
                return _SerializedAPandoraStructure;
            }

            set
            {
                _SerializedAPandoraStructure  = value;
            }

        }

        #endregion

        #region EstimatedSize - The estimated size for preallocation - in-memory only

        [NonSerialized]
        protected UInt64 _EstimatedSize;

        /// <summary>
        /// The estimated size of this APandoraStructure for preallocation
        /// </summary>
        public UInt64 EstimatedSize
        {
            get
            {
                return _EstimatedSize;
            }
            set
            {
                _EstimatedSize = value;
            }
        }

        #endregion

        #region PreallocationTickets - The tickets of AllocationMap preallocate

        /// <summary>
        /// For each copy we have a preallocation ticket.
        /// As long as it is not null, the object was not flushed.
        /// </summary>
        public List<Byte[]> PreallocationTickets;

        #endregion

        #region ReservedSize - The reserved number of bytes for this object on disc

        [NonSerialized]
        protected UInt64 _ReservedSize;

        /// <summary>
        /// The reserved number of bytes for this object on disc
        /// </summary>
        public UInt64 ReservedSize
        {

            get
            {
                return _ReservedSize;
            }

            set
            {
                _ReservedSize = value;
            }

        }

        #endregion

        #region Blocksize - The blocksize for this object on disc

        [NonSerialized]
        protected UInt64 _Blocksize;

        /// <summary>
        /// The reserved number of bytes for this object on disc
        /// </summary>
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


        #region isDirty - in-memory only

        /// <summary>
        /// This property will be true if the inMemory structure or object
        /// was changed since loading or since the last write on disk.
        /// Purpose: Minimize the number of unnecessary writings
        /// </summary>
        [NotIFastSerialized]
        public Boolean isDirty
        {

            get
            {

                if (_ModificationTime > DateTime.MinValue)
                    return true;

                return false;

            }

            set
            {
                if (value)    // isDirty = true
                    _ModificationTime = TimestampNonce.Now;

                else          // isDirty = false
                    _ModificationTime = DateTime.MinValue;
            }

        }

        #endregion

        #region ModificationTime - in-memory only

        [NonSerialized]
        protected DateTime _ModificationTime;

        /// <summary>
        /// The timestamp of the last structure or object modification.
        /// This will later be used to set the LastModificationTime when
        /// stored object does not implement this feature.
        /// </summary>
        [NotIFastSerialized]
        public DateTime ModificationTime
        {

            get
            {

                if (INodeReference == null)
                    return _ModificationTime;

                return
                    new DateTime((Int64)INodeReference.LastModificationTime);

            }

        }

        #endregion

        #endregion

        #region Constructors

        #region APandoraHeader()

        /// <summary>
        /// This will set all important variables within this APandoraStructure.
        /// This will especially create a new ObjectUUID and mark the
        /// APandoraStructure as "new" and "dirty".
        /// </summary>
        public AObjectHeader()
        {

            // Members of APandoraStructure
            _isNew                   = true;
            _StructureVersion        = 1;
            _IntegrityCheckValue     = null;
            _EncryptionParameters    = null;
            ObjectUUID               = new ObjectUUID(false);
            _INodeReference          = null;
            _ObjectLocatorReference  = null;

            // Members of IFastSerialize
            isDirty                  = true;

        }

        #endregion

        #region APandoraHeader(myObjectUUID)

        /// <summary>
        /// This will set all important variables within this APandoraStructure.
        /// Additionally it sets the ObjectUUID to the given value and marks
        /// the APandoraStructure as "new" and "dirty".
        /// </summary>
        public AObjectHeader(ObjectUUID myObjectUUID)
            : this()
        {
            ObjectUUID              = myObjectUUID;
        }

        #endregion

        #endregion

    }

}
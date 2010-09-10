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

/* PandoraFS
 * (c) Daniel Kirstenpfad, 2007-2008
 * (c) Achim Friedland, 2008
 * 
 * This data structure holds a QueueEntry that is used by the
 * Read- and WriteQueueManagers
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.Lib;
using sones.Lib.Cryptography;
using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;

#endregion

namespace sones.StorageEngines.Caches
{

    /// <summary>
    /// This data structure holds a QueueEntry that is used by the
    /// Read- and WriteQueueManagers
    /// </summary>

    public class QueueEntry
    {


        #region Properties

        #region Data - the serialized data of the object

        private Byte[] _Data;

        public Byte[] Data
        {

            get
            {
                return (_Data);
            }

            set
            {
                _Data        = value;
                _DataLength = _Data.ULongLength();
            }

        }

        #endregion

        #region DataLength - the length of the data to store

        private UInt64 _DataLength;

        public UInt64 DataLength
        {
            get
            {
                return _DataLength;
            }
        }

        #endregion

        #region Streams - the locations where to read or store the data

        private List<ObjectExtent> _RWQueueStreams;

        public List<ObjectExtent> RWQueueStreams
        {

            get
            {
                return _RWQueueStreams;
            }

            set
            {
                _RWQueueStreams = value;
            }

        }

        #endregion

        #region FlushAfterWrite - Do a physical flush of the WriteStream after writing this QueueEntry

        private Boolean _FlushAfterWrite;

        public Boolean FlushAfterWrite
        {

            get
            {
                return _FlushAfterWrite;
            }

            set
            {
                _FlushAfterWrite = value;
            }

        }

        #endregion

        #endregion


        #region Empty Constructor - QueueEntry()

        public QueueEntry()
        {
            _Data           = null;
            _DataLength     = 0;
            _RWQueueStreams = new List<ObjectExtent>();
        }

        #endregion


        #region Constructors for reading

        #region QueueEntry(myExtendedPositions, myLength)

        /// <summary>
        /// Creates an QueueEntry for reading especially INodes and ObjectLocators.
        /// </summary>
        /// <param name="myExtendedPositions">the position of the ObjectExtents where to start reading</param>
        /// <param name="myLengthToRead">the number of bytes to read</param>
        public QueueEntry(IEnumerable<ExtendedPosition> myExtendedPositions, UInt64 myLength)
        {

            ObjectExtent _ObjectExtent;

            _DataLength       = myLength;
            _Data             = null;
            _RWQueueStreams   = new List<ObjectExtent>();
            _FlushAfterWrite  = false;


            // Convert the list of ExtendedPositions into a list of ObjectExtents
            foreach (var _ExtendedPosition in myExtendedPositions)
            {

                _ObjectExtent                   = new ObjectExtent();
                _ObjectExtent.LogicalPosition   = 0;
                _ObjectExtent.StorageUUID       = _ExtendedPosition.StorageUUID;
                _ObjectExtent.PhysicalPosition  = _ExtendedPosition.Position;
                _ObjectExtent.Length            = myLength;

                _RWQueueStreams.Add(_ObjectExtent);

            }

        }

        #endregion

        #region QueueEntry(myPosition, myLength)

        /// <summary>
        /// Creates an QueueEntry for reading a single extent
        /// </summary>
        /// <param name="myExtentPosition">the position of the extent where to start reading</param>
        /// <param name="myExtentLength">the number of bytes to read</param>
        public QueueEntry(UInt64 myPosition, UInt64 myLength)
            : this(new ObjectExtent(0, myLength, myPosition, new StorageUUID(1UL)))
        {
        }

        #endregion

        #region QueueEntry(myObjectExtent)

        /// <summary>
        /// Creates an QueueEntry for reading a single ObjectExtent
        /// </summary>
        /// <param name="myObjectExtent">the ObjectExtent to read</param>
        public QueueEntry(ObjectExtent myObjectExtent)
        {
            _Data               = null;
            _RWQueueStreams     = new List<ObjectExtent>{ myObjectExtent };
            _DataLength         = myObjectExtent.Length;
            _FlushAfterWrite    = false;
        }

        #endregion

        #region QueueEntry(myObjectExtents)

        /// <summary>
        /// Creates an QueueEntry for reading a multiple ObjectExtents
        /// </summary>
        /// <param name="myObjectExtents">the ObjectExtents to read</param>
        public QueueEntry(IEnumerable<ObjectExtent> myObjectExtents)
        {
            _Data               = null;
            _RWQueueStreams     = new List<ObjectExtent>(myObjectExtents);
            _DataLength         = (from _ObjectExtent in myObjectExtents select _ObjectExtent.Length).Sum();
            _FlushAfterWrite    = false;
        }

        #endregion

        #endregion


        #region Constructors for writing

        #region QueueEntry(mySerializedObject, myExtentsList) - Store nearly everything

        /// <summary>
        /// Creates an QueueEntry for storing all kinds of filesystem structures and objects.
        /// </summary>
        /// <param name="myObjectLocator">The object locator</param>
        /// <param name="myObjectStream">The ObjectStream (stream ID and Type) of the object (will be used to find the right streams)</param>
        /// <param name="myObjectEdition">the Name of the object edition</param>
        /// <param name="myObjectRevision">the requested metatdata object revision timestamp</param>
        /// <param name="mySerializedObject">the serialized data of the object</param>
        public QueueEntry(Byte[] mySerializedObject, ObjectExtentsList myExtentsList)
        {

            _DataLength       = mySerializedObject.ULongLength();
            _Data             = mySerializedObject;
            _RWQueueStreams   = new List<ObjectExtent>();
            _FlushAfterWrite  = false;

            if (_DataLength > myExtentsList.StreamLength)
                throw new Exception("[QueueEntry] The ObjectSize given in the ObjectStream is smaller than the size of the given data!");

            // Add this ObjectStream to the list of streams
            foreach (var _ObjectExtents in myExtentsList)
                _RWQueueStreams.Add(_ObjectExtents);

        }

        #endregion

        #region QueueEntry(mySerializedObject, myExtendedPosition, myDataLength) - Store nearly everything

        public QueueEntry(Byte[] mySerializedObject, IEnumerable<ExtendedPosition> myExtendedPosition, UInt64 myDataLength)
            : this(mySerializedObject, new ObjectExtentsList(myExtendedPosition, myDataLength))
        {
        }

        #endregion

        #region QueueEntry(myData, myPosition, myPaddedLength) - Store some bytes at a single position

        /// <summary>
        /// Creates an QueueEntry for storing some bytes at a single position
        /// </summary>
        /// <param name="myPosition">a single position of the data wihtin the filesystem</param>
        /// <param name="myPaddedLength">some bytes for padding</param>
        /// <param name="mySerializedData">the data</param>
        public QueueEntry(Byte[] myData, UInt64 myPosition, UInt64 myPaddedLength)
        {

            //ObjectDatastream myObjectStream;
            //ObjectExtent _ObjectExtent;

            _Data                           = myData;
            _DataLength                     = myData.ULongLength();
            _RWQueueStreams                 = new List<ObjectExtent>();
            _FlushAfterWrite                = false;

            var _ObjectExtent               = new ObjectExtent();
            _ObjectExtent.LogicalPosition   = 0;
            _ObjectExtent.StorageUUID       = new StorageUUID(1UL);
            _ObjectExtent.PhysicalPosition  = myPosition;
            _ObjectExtent.Length            = _DataLength;

            //myObjectStream                   = new ObjectDatastream();
            //myObjectStream.StreamLength      = _DataLength;
            //myObjectStream.ReservedLength    = myPaddedLength - _DataLength;
            //myObjectStream.Add(myObjectExtent);

            _RWQueueStreams.Add(_ObjectExtent);

        }

        #endregion

        #endregion


    }

}

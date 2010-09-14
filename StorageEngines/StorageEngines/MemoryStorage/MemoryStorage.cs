/* 
 * MemoryStorage
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

using sones.Lib;
using sones.Notifications;
using sones.StorageEngines;
using sones.Lib.Caches;
using sones.StorageEngines.Caches;

#endregion

namespace sones.StorageEngines.MemoryStorage
{

    /// <summary>
    /// A in-memory implementation of a sones StorageEngine
    /// </summary>
    public class MemoryStorage : AStorageEngine, IStorageEngine
    {

        #region Data

        private MemoryStream _MemoryStream;

        #endregion

        
        #region Constructors

        #region MemoryStorage()

        /// <summary>
        /// Main constructor
        /// </summary>
        public MemoryStorage()
        { }

        #endregion

        #region MemoryStorage(myStorage)

        /// <summary>
        /// Constructor for mounting a MemoryStorage
        /// </summary>
        /// <param name="myStorage">An identification of the MemoryStorage, e.g. memory://myMemoryStorage</param>
        public MemoryStorage(String myStorage) 
        {
            AttachStorage(myStorage);
        }

        #endregion

        #region MemoryStorage(myStorageLocation, myNumberOfBytes, myBufferSize, myOverwriteExistingFilesystem, myAction)

        /// <summary>
        /// Constructor for creating a new MemoryStorage
        /// </summary>
        /// <param name="myStorageLocation">An identification of the MemoryStorage, e.g. memory://myMemoryStorage</param>
        /// <param name="myNumberOfBytes">The initial size of the memory stream in byte.</param>
        /// <param name="myBufferSize">The size of the internal buffer during formating the memory stream.</param>
        /// <param name="myOverwriteExistingFilesystem">Delete an existing memory stream?</param>
        /// <param name="myAction">An action called to indicate to progress on formating the memory stream.</param>
        public MemoryStorage(String myStorageLocation, UInt64 myNumberOfBytes, UInt32 myBufferSize, Boolean myOverwriteExistingFilesystem, Action<Double> myAction)
        {
            _StorageLocation = myStorageLocation;
            FormatStorage(myStorageLocation, myNumberOfBytes, myBufferSize, myOverwriteExistingFilesystem, myAction);
            AttachStorage(myStorageLocation);
        }

        #endregion

        #endregion


        #region IStorageEngine Members

        #region URIPrefix

        public override String URIPrefix
        {
            get
            {
                return "memory";
            }
        }

        #endregion

        #region StorageSize

        public override UInt64 Size
        {
            get
            {
                return (UInt64) _MemoryStream.Capacity;// .ULength();
            }
        }

        #endregion

        #region IsAttached

        public override Boolean IsAttached
        {
            get
            {
                return (_MemoryStream != null);
            }
        }

        #endregion

        #region IsMemoryOnly

        public override Boolean IsMemoryOnly
        {
            get
            {
                return true;
            }
        }

        #endregion


        #region FormatStorage(myStorageLocation, myNumberOfBytes, myBufferSize, myAllowOverwrite, myAction)

        public override Boolean FormatStorage(String myStorageLocation, UInt64 myNumberOfBytes, UInt32 myBufferSize, Boolean myAllowOverwrite, Action<Double> myAction)
        {
            _MemoryStream = new MemoryStream((Int32)myNumberOfBytes);
            return true;
        }

        #endregion

        #region AttachStorage(myStorageLocation)

        public override Boolean AttachStorage(String myStorageLocation)
        {            
            return true;
        }

        #endregion

        #region DetachStorage()

        public override Boolean DetachStorage()
        {
            return true;
        }

        #endregion

        #region SealStorage()

        public override Boolean SealStorage()
        {
            //_MemoryStream = null;
            return true;
        }

        #endregion

        #region GrowStorage(myNumberOfBytesToAdd)

        /// <summary>
        /// Increases the size of the memorystream
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">The additional size of the memorystream</param>
        public override Boolean GrowStorage(UInt64 myNumberOfBytesToAdd)
        {

            lock (this)
            {

                try
                {

                    _MemoryStream.Capacity += (Int32) myNumberOfBytesToAdd;

                    return true;

                }
                catch
                {
                    return false;
                }

            }

        }

        #endregion

        #region ShrinkStorage(myNumberOfBytesToShrink)

        /// <summary>
        /// Decreases the size of the memorystream
        /// </summary>
        /// <param name="myNumberOfBytesToShrink">The number of bytes to remove</param>
        public override Boolean ShrinkStorage(UInt64 myNumberOfBytesToShrink)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region ReadPosition(myPhysicalPosition, myLength)

        public override Byte[] ReadPosition(UInt64 myPhysicalPosition, UInt64 myLength)
        {
            
            var _ByteArray = new Byte[myLength];

            _MemoryStream.Seek((Int64) myPhysicalPosition, SeekOrigin.Begin);
            _MemoryStream.Read(_ByteArray, 0, (Int32) myLength);

            return _ByteArray;

        }

        #endregion

        #region ReadExtent(myObjectExtentsList)

        public override Byte[] ReadExtents(ObjectExtentsList myObjectExtentsList)
        {

            try
            {

                Byte[] _ByteArray = null;
                var _OutputStream = new MemoryStream();                

                foreach (var _ObjectExtent in myObjectExtentsList)
                {
                    _ByteArray = new Byte[_ObjectExtent.Length];
                    _MemoryStream.Seek((Int64)_ObjectExtent.PhysicalPosition, SeekOrigin.Begin);
                    _OutputStream.Seek((Int64)_ObjectExtent.LogicalPosition, SeekOrigin.Begin);
                    _MemoryStream.Read(_ByteArray, 0, (Int32)_ObjectExtent.Length);
                    _OutputStream.Write(_ByteArray, 0, (Int32)_ObjectExtent.Length);
                }

                return _OutputStream.ToArray();

            }
            
            catch
            {
                return new Byte[0];
            }

        }

        #endregion


        #region WritePosition(myByteArray, myPhysicalPosition)

        public override Boolean WritePosition(Byte[] myByteArray, UInt64 myPhysicalPosition)
        {

            try
            {

                _MemoryStream.Seek((Int64) myPhysicalPosition, SeekOrigin.Begin);
                _MemoryStream.Write(myByteArray, 0, (Int32) myByteArray.Length);
                return true;

            }

            catch
            {
                return false;
            }

        }

        #endregion

        #region WritePositions(myByteArray, myPhysicalPositions)

        public override Boolean WritePositions(Byte[] myByteArray, IEnumerable<UInt64> myPhysicalPositions)
        {

            try
            {

                foreach (var _PhysicalPosition in myPhysicalPositions)
                {
                    _MemoryStream.Seek((Int64)_PhysicalPosition, SeekOrigin.Begin);
                    _MemoryStream.Write(myByteArray, 0, (Int32)myByteArray.Length);
                }

                return true;

            }

            catch
            {
                return false;
            }

        }

        #endregion

        #region WriteExtents(myByteArray, myObjectExtentsList)

        public override Boolean WriteExtents(Byte[] myByteArray, ObjectExtentsList myObjectExtentsList)
        {

            try
            {

                UInt64 _CopyLength;
                Byte[] _ByteArray = null;
                UInt64 _myByteArrayLength = myByteArray.ULongLength();

                foreach (var _ObjectExtent in myObjectExtentsList)
                {

                    _ByteArray = new Byte[_ObjectExtent.Length];

                    _CopyLength = _ObjectExtent.Length;
                    if (_ObjectExtent.LogicalPosition + _CopyLength > _myByteArrayLength)
                        _CopyLength = (_myByteArrayLength - _ObjectExtent.LogicalPosition);

                    Array.Copy(myByteArray, (Int32)_ObjectExtent.LogicalPosition, _ByteArray, 0, (Int32)_CopyLength);

                    _MemoryStream.Seek((Int64)_ObjectExtent.PhysicalPosition, SeekOrigin.Begin);
                    _MemoryStream.Write(_ByteArray, 0, (Int32) _ObjectExtent.Length);

                }

                return true;

            }

            catch
            {
                return false;
            }

        }

        #endregion

        #endregion

    }

}

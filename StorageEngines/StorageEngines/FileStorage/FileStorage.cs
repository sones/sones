/*
 * FileStorage
 * (c) Achim Friedland, 2008 - 2010
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

namespace sones.StorageEngines.FileStorage
{

    /// <summary>
    /// This class maintains a GraphFS filesystem within a normal file of the underlying filesystem
    /// </summary>
    public class FileStorage : AStorageEngine, IStorageEngine
    {

        #region Data

        private ReadQueueManager    _ReadQueue;
        private WriteQueueManager   _WriteQueue;
        private FileStream          _FileStream;
        private WriteQueueLock      _WriteQueueLock;
        private ByteCache           _ByteCache;

        #endregion

        #region Properties

        #region ByteCacheSettings

        private CacheSettings _ByteCacheSettings;

        private CacheSettings ByteCacheSettings
        {

            get
            {
                return _ByteCacheSettings;
            }

            set
            {
                _ByteCacheSettings = value;
            }

        }

        #endregion

        #endregion

        #region Constructors

        #region FileStorage()

        /// <summary>
        /// Main constructor
        /// </summary>
        public FileStorage()
        { }

        #endregion

        #region FileStorage(myStorage)

        /// <summary>
        /// Constructor for mounting a FileStorage
        /// </summary>
        /// <param name="myStorage">The location of the image file, e.g. file://myFileStorage.fs</param>
        public FileStorage(String myStorage) 
        {
            AttachStorage(myStorage);
        }

        #endregion

        #region FileStorage(myStorage, myNumberOfBytes, myBufferSize, myOverwriteExistingFilesystem, myAction)

        /// <summary>
        /// Constructor for creating a new MemoryMappedFileStorage
        /// </summary>
        /// <param name="myStorage">The location of the image file, e.g. file://myFileStorage.fs</param>
        /// <param name="myNumberOfBytes">The initial size of the image file in byte.</param>
        /// <param name="myBufferSize">The size of the internal buffer during formating the image file.</param>
        /// <param name="myOverwriteExistingFilesystem">Delete an existing image file?</param>
        /// <param name="myAction">An action called to indicate to progress on formating the image file.</param>
        public FileStorage(String myStorage, UInt64 myNumberOfBytes, UInt32 myBufferSize, Boolean myOverwriteExistingFilesystem, Action<Double> myAction)
        {
            FormatStorage(myStorage, myNumberOfBytes, myBufferSize, myOverwriteExistingFilesystem, myAction);
            AttachStorage(myStorage);
        }

        #endregion

        #endregion

        
        #region Private Helpers

        #region (private) WriteQueue_OnFlushSucceeded

        private void WriteQueue_OnFlushSucceeded(object sender, FlushSucceededEventArgs args)
        {
            _ByteCache.Flushed(args.PhysicalPostion);
        }

        #endregion

        #region (private) GetImageFileLocation

        private String GetImageFileLocation(String myStorageLocation)
        {

            var retVal = myStorageLocation;

            if (retVal.IndexOf("://") >= 0)
                retVal = retVal.Substring(retVal.IndexOf("://") + 3);

            return retVal;

        }

        #endregion

        #endregion


        #region IStorageEngine Members

        #region URIPrefix

        public override String URIPrefix
        {
            get
            {
                return "file";
            }
        }

        #endregion

        #region StorageSize

        public override UInt64 Size
        {
            get
            {

                var retVal = 0UL;

                if (_FileStream != null && _FileStream.CanRead)
                    retVal = _FileStream.ULength();

                else if (File.Exists(GetImageFileLocation(_StorageLocation)))
                {
                    var fileStream = new FileStream(GetImageFileLocation(_StorageLocation), FileMode.Open);
                    retVal = fileStream.ULength();
                    fileStream.Close();
                }

                return retVal;

            }
        }

        #endregion

        #region IsAttached

        public override Boolean IsAttached
        {
            get
            {
                return (_FileStream != null);
            }
        }

        #endregion

        #region IsMemoryOnly

        public override Boolean IsMemoryOnly
        {
            get
            {
                return false;
            }
        }

        #endregion


        #region FormatStorage(myStorageLocation, myNumberOfBytes, myBufferSize, myAllowOverwrite, myAction)

        /// <summary>
        /// Creates a new FileStorage
        /// </summary>
        /// <param name="myStorage">The location of the image file, e.g. file://myFileStorage.fs</param>
        /// <param name="myNumberOfBytes">The initial size of the image file in byte.</param>
        /// <param name="myBufferSize">The size of the internal buffer during formating the image file.</param>
        /// <param name="myOverwriteExistingFilesystem">Delete an existing image file?</param>
        /// <param name="myAction">An action called to indicate to progress on formating the image file.</param>
        /// <returns>true for success</returns>
        public override Boolean FormatStorage(String myStorageLocation, UInt64 myNumberOfBytes, UInt32 myBufferSize, Boolean myAllowOverwrite, Action<Double> myAction)
        {

            lock (this)
            {

                if (_FileStream == null)
                {

                    #region Sanity Checks

                        _StorageLocation   = myStorageLocation;
                    var _ImageFileLocation = GetImageFileLocation(myStorageLocation);

                    // Check if the image file already exists!
                    if (File.Exists(_ImageFileLocation))
                    {

                        if (!myAllowOverwrite)
                            throw new StorageEngineException("FileStorage \"" + _StorageLocation + "\" already exists!");

                        File.Delete(_ImageFileLocation);

                    }

                    #endregion

                    #region Open image file

                    try
                    {

                        _FileStream = new FileStream(_ImageFileLocation, FileMode.Create);
                        
                        // Set the overall size of the file
                        //newFile.SetLength((Int64)myNumberOfBytes);

                    }
                    catch (Exception e)
                    {
                        throw new StorageEngineException(e.Message);
                    }

                    #endregion

                    #region Format image file

                    // Initialize the buffer with 0xAA
                    var _buffer = new Byte[myBufferSize];

                    for (var i = 0UL; i < _buffer.ULongLength(); i++)
                    {
                        _buffer[i] = 0xAA;
                    }

                    var _NumberOfBlocks = myNumberOfBytes / (UInt64) myBufferSize;
                    var _RemainingBytes = myNumberOfBytes % (UInt64) myBufferSize;

                    try
                    {

                        for (var i = 0UL; i < _NumberOfBlocks; i++)
                        {

                            _FileStream.Write(_buffer, 0, (Int32)myBufferSize);

                            if (myAction != null)
                                myAction(100 * i / _NumberOfBlocks);

                        }

                        _FileStream.Write(_buffer, 0, (Int32) _RemainingBytes);

                    }
                    catch (Exception e)
                    {
                        throw new StorageEngineException(e.Message);
                    }

                    #endregion

                    #region Close image file

                    _FileStream.Close();
                    _FileStream = null;

                    #endregion

                    return true;

                }

                return false;

            }

        }

        #endregion

        #region AttachStorage(myStorageLocation)

        public override Boolean AttachStorage(String myStorageLocation)
        {

            lock (this)
            {

                if (_FileStream == null)
                {

                        _StorageLocation    = myStorageLocation;
                    var _ImageFileLocation  = GetImageFileLocation(myStorageLocation);

                    _WriteQueueLock         = new WriteQueueLock();
                    _FileStream             = new FileStream(_ImageFileLocation, FileMode.Open);
                    _ReadQueue              = new ReadQueueManager(_WriteQueueLock);
                    _WriteQueue             = new WriteQueueManager(_WriteQueueLock);

                    _ReadQueue.SetFileStream(_FileStream);
                    _WriteQueue.SetFileStream(_FileStream);

                    // Configure the ByteCache
                    if (_ByteCacheSettings == null)
                        _ByteCacheSettings = new CacheSettings()
                            {
                                TimerDueTime                 = TimeSpan.FromSeconds(3),
                                TimerPeriod                  = TimeSpan.FromSeconds(120),
                                AbsoluteExpirationTimeSpan   = TimeSpan.FromSeconds(120),
                                ExpirationType               = ExpirationTypes.Absolute
                            };

                    _ByteCache = new ByteCache("<FileStorage> " + _StorageLocation, _ByteCacheSettings);

                    _WriteQueue.NotificationDispatcher = _NotificationDispatcher;
                    _WriteQueue.OnFlushSucceeded += new FlushSucceededHandler(WriteQueue_OnFlushSucceeded);

                    return true;

                }

                return false;

            }

        }

        #endregion

        #region DetachStorage()

        public override Boolean DetachStorage()
        {

            lock (this)
            {

                try
                {

                    while (_WriteQueue != null && !_WriteQueue.isOffline)
                    {
                        _WriteQueue.ShutdownQueueManagerThread();
                        Thread.Sleep(10);
                    }

                    while (_ReadQueue != null && !_ReadQueue.isOffline)
                    {
                        _ReadQueue.ShutdownQueueManagerThread();
                        Thread.Sleep(10);
                    }

                    if (_FileStream != null)
                    {
                        _FileStream.Close();
                        _FileStream = null;
                    }

                    if (_ByteCache != null)
                    {
                        _ByteCache.Close();
                    }

                    return true;

                }

                catch (Exception e)
                {
                    throw new StorageEngineException("Could not detach the StorageEngine! " + e.Message);
                }

            }

        }

        #endregion

        #region SealStorage()

        public override Boolean SealStorage()
        {
            return true;
        }

        #endregion

        #region GrowStorage(myNumberOfBytesToAdd)

        /// <summary>
        /// Increases the size of the image file
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">The additional size of the image file</param>
        public override Boolean GrowStorage(UInt64 myNumberOfBytesToAdd)
        {

            lock (this)
            {

                #region Checks

                if (_FileStream == null)
                    throw new FileStorageException_GrowFileSystemFailed("File stream is invalid!");

                if (!_FileStream.CanWrite)
                    throw new FileStorageException_GrowFileSystemFailed("Can not write to the file stream!");

                #endregion

                #region Data

                Byte[] _buffer = new Byte[65536];

                // Initialize the buffer with 0xAA
                for (UInt32 counter = 0; counter < 65536; counter++)
                {
                    _buffer[counter] = 0xAA;
                }

                #endregion

                #region Format additional bytes of the file system

                UInt64 NumberOfBlocks = myNumberOfBytesToAdd / 65536;
                UInt64 RemainingBytes = myNumberOfBytesToAdd % 65536;

                _FileStream.Seek(_FileStream.Length, SeekOrigin.Begin);

                try
                {

                    for (var counter = 0UL; counter < NumberOfBlocks; counter++)
                        _FileStream.Write(_buffer, 0, 65536);

                    _FileStream.Write(_buffer, 0, (Int32)RemainingBytes);

                }

                catch (Exception e)
                {
                    throw new FileStorageException(e.Message);
                }

                #endregion

                return true;

            }

        }

        #endregion

        #region ShrinkStorage(myNumberOfBytesToShrink)

        /// <summary>
        /// Decreases the size of the image file
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

            try
            {

                var missingCachePositions = new Dictionary<UInt64, UInt64>();

                var cachedBytes = _ByteCache.Get(myPhysicalPosition, myLength, missingCachePositions);

                if (cachedBytes == null)
                    return _ReadQueue.Read(myPhysicalPosition, myLength);

                if (missingCachePositions.Count > 0)
                {

                    Int64 cachedBytesPos = 0;

                    foreach (var keyValPair in missingCachePositions)
                    {

                        var readBytes = _ReadQueue.Read(keyValPair.Key, keyValPair.Value);
                        Array.Copy(readBytes, 0, cachedBytes, (Int64)(keyValPair.Key - myPhysicalPosition), (Int64)readBytes.Length);

                        cachedBytesPos += (Int64)keyValPair.Key;

                    }

                }

                return cachedBytes;

            }

            catch
            {
                return new Byte[0];
            }

        }

        #endregion

        #region ReadExtent(myObjectExtentsList)

        public override Byte[] ReadExtents(ObjectExtentsList myObjectExtentsList)
        {

            try
            {

                Byte[] _ByteArray = null;
                var _OutputStream = new MemoryStream((Int32) myObjectExtentsList.StreamLength);

                foreach (var _ObjectExtent in myObjectExtentsList)
                {
                    _ByteArray = ReadPosition(_ObjectExtent.PhysicalPosition, _ObjectExtent.Length);
                    _OutputStream.Seek((Int64) _ObjectExtent.LogicalPosition, SeekOrigin.Begin);
                    _OutputStream.Write(_ByteArray, 0, (Int32) _ObjectExtent.Length);
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

                _ByteCache.Cache(myPhysicalPosition, myByteArray);
                _WriteQueue.Write(new QueueEntry(myByteArray, new List<ExtendedPosition> { new ExtendedPosition(myPhysicalPosition) }, myByteArray.ULongLength()));

            }
            catch (Exception e)
            {
                throw new FileStorageException("Error writing " + myByteArray.ULongLength() + " bytes at position " + myPhysicalPosition + "! " + e.Message);
            }

            return true;

        }

        #endregion

        #region WritePositions(myByteArray, myPhysicalPositions)

        public override Boolean WritePositions(Byte[] myByteArray, IEnumerable<UInt64> myPhysicalPositions)
        {

            try
            {

                var _ListOfExtendedPositions = new List<ExtendedPosition>();

                foreach (var _PhysicalPosition in myPhysicalPositions)
                {
                    _ByteCache.Cache(_PhysicalPosition, myByteArray);
                    _ListOfExtendedPositions.Add(new ExtendedPosition(_PhysicalPosition));
                }

                _WriteQueue.Write(new QueueEntry(myByteArray, _ListOfExtendedPositions, myByteArray.ULongLength()));

            }
            catch (Exception e)
            {

                var _Positions = "";

                foreach (var _PhysicalPosition in myPhysicalPositions)
                    _Positions = _PhysicalPosition + ", ";

                _Positions = _Positions.Remove(_Positions.Length - 2, 2);

                throw new FileStorageException("Error writing " + myByteArray.ULongLength() + " bytes at positions " + _Positions + "! " + e.Message);

            }

            return true;

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

                    _ByteCache.Cache(_ObjectExtent.PhysicalPosition, _ByteArray);
                    _WriteQueue.Write(new QueueEntry(myByteArray, myObjectExtentsList));

                }

                return true;

            }

            catch (Exception e)
            {
                throw new FileStorageException("Error writing " + myByteArray.ULongLength() + " bytes using extents " + myObjectExtentsList.ToString() + "! " + e.Message);
            }

        }

        #endregion

        #endregion

    }

}

/*
 * MemoryMappedFileStorage
 * (c) Achim Friedland, 2008 - 2010
 * (c) Martin Junghanns, 2010
 */

#region Usings

using System;
using System.IO;
using System.Collections.Generic;

using sones.Lib;
using System.IO.MemoryMappedFiles;
using System.Threading;


#endregion

namespace sones.StorageEngines.MemoryMappedFileStorage
{

    /// <summary>
    /// This class maintains a GraphFS filesystem within a normal file of the underlying filesystem
    /// </summary>
    public class MemoryMappedFileStorage : AStorageEngine, IStorageEngine
    {

        #region Data

        /// <summary>
        /// this is where the magic happens
        /// </summary>
        private MemoryMappedFile _MemoryMappedFile;

        /// <summary>
        /// the maximum size of the mmf, needs to be static because of the
        /// attach method which need to know the size of the mmf
        /// </summary>
        private static UInt64 _MemoryMappedFileSize;

        /// <summary>
        /// the window to operate on the mmf
        /// </summary>
        private MemoryMappedViewAccessor _Window;

        /// <summary>
        /// this is where the window starts
        /// </summary>
        private UInt64 _WindowOffset = 0x0; //0MB;

        /// <summary>
        /// this is where the window ends (if this is greater then
        /// the mmf-size, the whole mmf will be inside the window)
        /// </summary>
        private UInt64 _WindowSize = 0x20000000; //512MB;
        //private UInt64 _WindowSize = 0xA00000; //10MB;

        private static String _MemoryID;

        /// <summary>
        /// used for some thread-safety
        /// </summary>
        private Object _LockObj = new Object();

        /// <summary>
        /// used when sliding the window
        /// </summary>
        private Object _SlideLock = new Object();

        #endregion
        
        #region Constructors

        #region MemoryMappedFileStorage()

        /// <summary>
        /// Main constructor
        /// </summary>
        public MemoryMappedFileStorage()
        {}

        #endregion

        #region MemoryMappedFileStorage(myStorage)

        /// <summary>
        /// Constructor for mounting a MemoryMappedFileStorage
        /// </summary>
        /// <param name="myStorage">An identification of the MemoryMappedFileStorage, e.g. mmf://myMemoryStorage</param>
        public MemoryMappedFileStorage(String myStorage) 
        {
            AttachStorage(myStorage);
        }

        #endregion

        #region MemoryMappedFileStorage(myStorageLocation, myNumberOfBytes, myBufferSize, myOverwriteExistingFilesystem, myAction)

        /// <summary>
        /// Constructor for creating a new MemoryMappedFileStorage
        /// </summary>
        /// <param name="myStorageLocation">An identification of the MemoryMappedFileStorage, e.g. mmf://MemoryMappedFileStorage</param>
        /// <param name="myNumberOfBytes">The initial size of the memory mapped file in byte.</param>
        /// <param name="myBufferSize">The size of the internal buffer during formating the memory stream. (not used)</param>
        /// <param name="myOverwriteExistingFilesystem">Delete an existing memory stream?</param>
        /// <param name="myAction">An action called to indicate to progress on formating the memory stream.</param>
        public MemoryMappedFileStorage(String myStorageLocation, UInt64 myNumberOfBytes, UInt32 myBufferSize, Boolean myOverwriteExistingFilesystem, Action<Double> myAction)
            : this(myStorageLocation, myNumberOfBytes, myBufferSize, myOverwriteExistingFilesystem, myAction, 0x0, 0x20000000)
        {}

        #endregion

        #region MemoryMappedFileStorage(myStorageLocation, myNumberOfBytes, myBufferSize, myOverwriteExistingFilesystem, myAction, UInt64 myWindowOffset, UInt64 myWindowSize)

        /// <summary>
        /// Constructor for creating a new MemoryMappedFileStorage
        /// </summary>
        /// <param name="myStorageLocation">An identification of the MemoryMappedFileStorage, e.g. mmf://MemoryMappedFileStorage</param>
        /// <param name="myNumberOfBytes">The initial size of the memory mapped file in byte.</param>
        /// <param name="myBufferSize">The size of the internal buffer during formating the memory stream. (not used)</param>
        /// <param name="myOverwriteExistingFilesystem">Delete an existing memory stream?</param>
        /// <param name="myAction">An action called to indicate to progress on formating the memory stream.</param>
        /// <param name="myWindowOffset">Position in bytes where the window (view on the mmf) starts.</param>
        /// <param name="myWindowSize">Size of the window.</param>
        public MemoryMappedFileStorage(String myStorageLocation, UInt64 myNumberOfBytes, UInt32 myBufferSize, Boolean myOverwriteExistingFilesystem, Action<Double> myAction, UInt64 myWindowOffset, UInt64 myWindowSize)
        {

            if (_WindowOffset >= myNumberOfBytes || _WindowOffset < 0x0)
            {
                throw new ArgumentException(String.Format("window offset was larger than or equal to total storage size or less than zero"));
            }

            _StorageLocation        = myStorageLocation;
            _MemoryMappedFileSize   = myNumberOfBytes;
            _WindowOffset           = myWindowOffset;
            _WindowSize             = myWindowSize;

            //corrects the size if it's more than the available number of bytes
            _WindowSize             = GetCorrectWindowSize();

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
                return "mmf";
            }
        }

        #endregion

        #region StorageSize

        public override UInt64 Size
        {
            get
            {
                return (UInt64)_MemoryMappedFileSize;
            }
        }

        #endregion

        #region IsAttached

        public override Boolean IsAttached
        {
            get
            {
                return (_MemoryMappedFile != null);
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

        public override Boolean FormatStorage(String myStorageLocation, UInt64 myNumberOfBytes, UInt32 myBufferSize, Boolean myAllowOverwrite, Action<Double> myAction)
        {
            lock (_LockObj)
            {
                if (_MemoryMappedFile == null)
                {
                    #region Sanity Checks

                    _StorageLocation        = myStorageLocation;
                    var _ImageFileLocation  = GetImageFileLocation(myStorageLocation);
                    _MemoryID               = Guid.NewGuid().ToString();

                    // Check if the image file already exists!
                    if (File.Exists(_ImageFileLocation))
                    {
                        if (!myAllowOverwrite)
                        {
                            throw new StorageEngineException(String.Format("MemoryMappedFileStorage {0} already exists.", myStorageLocation));
                        }
                        File.Delete(_ImageFileLocation);
                    }

                    #endregion

                    #region Create memory mapped file and window

                    _MemoryMappedFile = MemoryMappedFile.CreateFromFile(
                                        _ImageFileLocation,
                                        FileMode.Create,
                                        _MemoryID, //also name in memory
                                        (long)_MemoryMappedFileSize //size
                                        );

                    //create view accessor
                    _WindowSize = GetCorrectWindowSize();
                    _Window = _MemoryMappedFile.CreateViewAccessor((long)_WindowOffset, (long)_WindowSize);

                    #endregion
                }
            }

            return true;
        }

        #endregion

        #region AttachStorage(myStorageLocation)

        public override Boolean AttachStorage(String myStorageLocation)
        {
            lock (_LockObj)
            {
                if (_MemoryMappedFile == null)
                {
                    _StorageLocation        = myStorageLocation;
                    var imageFileLocation   = GetImageFileLocation(myStorageLocation);

                    //check if file exists
                    if (!File.Exists(imageFileLocation))
                    {
                        throw new StorageEngineException(String.Format("MemoryMappedFile {0} doesn't exist on disk", myStorageLocation));
                    }

                    MemoryMappedFile mmf = null;

                    //check if still in memory
                    if (_MemoryID != null)
                    {
                        try
                        {
                            mmf = MemoryMappedFile.OpenExisting(_MemoryID);
                        }
                        catch (FileNotFoundException)
                        {
                            //not found in memory...continue
                        }
                    }
                    else
                    {
                        _MemoryID = Guid.NewGuid().ToString();
                    }


                    if (mmf == null)
                    {
                        //load from disk
                        try
                        {
                            _MemoryMappedFile = MemoryMappedFile.CreateFromFile(
                                                    imageFileLocation,
                                                    FileMode.Open,
                                                    _MemoryID, //name in memory
                                                    (long)_MemoryMappedFileSize, //size
                                                    MemoryMappedFileAccess.ReadWrite);
                        }
                        catch
                        {
                            throw new StorageEngineException(String.Format("MemoryMappedFile {0} is in use by another process", myStorageLocation));
                        }
                    }
                    else
                    {
                        _MemoryMappedFile = mmf;
                    }

                    //open window
                    _WindowSize = GetCorrectWindowSize();
                    _Window     = _MemoryMappedFile.CreateViewAccessor((long)_WindowOffset, (long)_WindowSize);

                    return true;

                }

                return false;

            }

        }

        #endregion

        #region DetachStorage()

        public override Boolean DetachStorage()
        {
            if (_MemoryMappedFile != null)
            {
                try
                {
                    //first close the open handles
                    _Window.SafeMemoryMappedViewHandle.Close();
                    _MemoryMappedFile.SafeMemoryMappedFileHandle.Close();
                    //and then dispose the window and the mmf
                    _Window.Dispose();
                    _MemoryMappedFile.Dispose();
                }
                catch
                {
                    throw new StorageEngineException(String.Format("Disposing MemoryMappedFileStorage {0} failed", _StorageLocation));
                }
                finally
                { 
                    _Window = null;
                    _MemoryMappedFile = null;
                }
                    
                return true;
            }

            return false;
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
        /// Increases the size of the memorystream
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">The additional size of the memorystream</param>
        public override Boolean GrowStorage(UInt64 myNumberOfBytesToAdd)
        {
            lock (_SlideLock)
            {
                try
                {
                    _MemoryMappedFileSize += myNumberOfBytesToAdd;
                    _MemoryMappedFile = null;

                    AttachStorage(_StorageLocation);

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
            #region prepare read access

            PrepareAccess(myPhysicalPosition, myLength);

            #endregion

            #region read

            try
            {
                lock (_SlideLock)
                {
                    var logicalPosition = (int)GetLogicalPositionInWindow(myPhysicalPosition);

                    var byteArray = new Byte[myLength];

                    _Window.ReadArray(logicalPosition, byteArray, 0, (int)myLength);

                    return byteArray;
                }
            }
            catch
            {
                return new Byte[myLength];
            }

            #endregion

        }

        #endregion

        #region ReadExtent(myObjectExtentsList)

        public override Byte[] ReadExtents(ObjectExtentsList myObjectExtentsList)
        {
            try
            {
                Byte[] byteArray = null;
                var _OutputStream = new MemoryStream();

                foreach (var _ObjectExtent in myObjectExtentsList)
                {
                    byteArray = ReadPosition(_ObjectExtent.PhysicalPosition, _ObjectExtent.Length);

                    _OutputStream.Seek((Int64)_ObjectExtent.LogicalPosition, SeekOrigin.Begin);
                    _OutputStream.Write(byteArray, 0, (Int32)_ObjectExtent.Length);
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
            PrepareAccess(myPhysicalPosition, myByteArray.ULongCount());

            #region write at logical position

            try
            {
                lock (_SlideLock)
                {
                    var logicalPosition = GetLogicalPositionInWindow(myPhysicalPosition);

                    _Window.WriteArray((int)logicalPosition, myByteArray, 0, myByteArray.Length);
                }
            }
            catch
            {
                throw new StorageEngineException(String.Format("Writing to MemoryMappedFileStorage {0} failed", _StorageLocation));
            }

            #endregion

            return true;
        }

        #endregion

        #region WritePositions(myByteArray, myPhysicalPositions)

        public override Boolean WritePositions(Byte[] myByteArray, IEnumerable<UInt64> myPhysicalPositions)
        {
            try
            {
                foreach (var physicalPosition in myPhysicalPositions)
                {
                    WritePosition(myByteArray, physicalPosition);
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
                UInt64 copyLength;
                Byte[] byteArray = null;
                UInt64 myByteArrayLength = myByteArray.ULongLength();

                foreach (var _ObjectExtent in myObjectExtentsList)
                {
                    byteArray = new Byte[_ObjectExtent.Length];

                    copyLength = _ObjectExtent.Length;
                    if (_ObjectExtent.LogicalPosition + copyLength > myByteArrayLength)
                    {
                        copyLength = (myByteArrayLength - _ObjectExtent.LogicalPosition);
                    }

                    Array.Copy(myByteArray, (Int32)_ObjectExtent.LogicalPosition, byteArray, 0, (Int32)copyLength);

                    //do the writing
                    WritePosition(byteArray, _ObjectExtent.PhysicalPosition);
                    
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

        #region Private Helpers

        #region (private) GetImageFileLocation

        private String GetImageFileLocation(String myStorageLocation)
        {

            var retVal = myStorageLocation;

            if (retVal.IndexOf("://") >= 0)
                retVal = retVal.Substring(retVal.IndexOf("://") + 3);

            return retVal;

        }

        #endregion

        #region (private) IsPhysicalPositionInWindow

        private Boolean IsPhysicalPositionInWindow(UInt64 myPhysicalPosition)
        {
            return _WindowOffset <= myPhysicalPosition && myPhysicalPosition <= (_WindowOffset + _WindowSize);
        }

        #endregion

        #region (private) IsEnoughSpaceInWindow

        private Boolean IsEnoughSpaceInWindow(UInt64 myLogicalPosition, UInt64 myDataLength)
        {
            return _WindowSize - myLogicalPosition >= myDataLength;
        }

        #endregion

        #region (private) SlideWindow

        /// <summary>
        /// slides the window upon the memory mapped file
        /// </summary>
        /// <param name="mySlideBytes">less than zero slides n bytes left, greater than zero n bytes right</param>
        private void SlideWindow(Int64 mySlideBytes)
        {
            #region checks

            if (mySlideBytes < 0)
            {
                if ((long)_WindowOffset + mySlideBytes < 0)
                {
                    throw new StorageEngineException(String.Format("Tried to slide window on MemoryMappedFileStorage {0} out of lower bound", _StorageLocation));
                }
            }
            else if (mySlideBytes > 0)
            {
                if (_WindowOffset + (ulong)mySlideBytes > _MemoryMappedFileSize)
                {
                    throw new StorageEngineException(String.Format("Tried to slide window on MemoryMappedFileStorage {0} out of upper bound", _StorageLocation));
                }
            }

            #endregion

            #region slide

            lock (_SlideLock)
            {
                try
                {
                    //free ressources
                    _Window.SafeMemoryMappedViewHandle.Close();
                    _Window.SafeMemoryMappedViewHandle.Dispose();

                    //update offset
                    _WindowOffset += (ulong)mySlideBytes;

                    //using a temporary size because the window size will stay minimized even if the window slides back
                    //TODO: maybe find a better way
                    var tmpWindowSize = GetCorrectWindowSize();

                    //create an new window
                    _Window = _MemoryMappedFile.CreateViewAccessor((long)_WindowOffset, (long)tmpWindowSize);
                }
                catch
                {
                    throw new StorageEngineException(String.Format("Error while slinding window on memory mapped storage {0}", _StorageLocation));
                }
            }

            #endregion
        }

        #endregion

        #region (private) GetLogicalPositionInWindow

        private UInt64 GetLogicalPositionInWindow(UInt64 myPhysicalPosition)
        {
            return (myPhysicalPosition - _WindowOffset);
        }

        #endregion

        #region (private) GetCorrectWindowSize

        private UInt64 GetCorrectWindowSize()
        {
            if ((_WindowOffset + _WindowSize) > _MemoryMappedFileSize)
            {
                return _MemoryMappedFileSize - _WindowOffset;
            }
            else
            {
                return _WindowSize;
            }
        }

        #endregion

        #region (private) PrepareAccess

        /// <summary>
        /// Method checks if physical position is accessible. If yes it also checks if the
        /// position is inside the current window position. If not it slides to the appropriate
        /// position on the memory mapped file. The method also checks if there are enough bytes
        /// left in the window to read / write data.
        /// </summary>
        /// <param name="myPhysicalPosition">Physical Location of the data to be read or written.</param>
        /// <param name="myByteLength">The byte-length of the data to be read or written.</param>
        private void PrepareAccess(UInt64 myPhysicalPosition, UInt64 myByteLength)
        {

            #region checks

            if (myPhysicalPosition < 0 || myPhysicalPosition > _MemoryMappedFileSize)
            {
                throw new StorageEngineException(String.Format("Writing to MemoryMappedFileStorage {0} failed because physical position was out of range", _StorageLocation));
            }

            #endregion

            #region check if physicalpostion is in window. if not, slide around

            if (!IsPhysicalPositionInWindow(myPhysicalPosition))
            {
                #region Sliding to physical position

                var slideBytes = 0L;

                if (myPhysicalPosition < _WindowOffset)
                {
                    //left slide
                    slideBytes = -(long)(_WindowOffset - myPhysicalPosition);
                }
                else
                {
                    //right slide
                    slideBytes = (long)(myPhysicalPosition - _WindowOffset);
                }

                //slide
                SlideWindow(slideBytes);

                #endregion
            }

            #endregion

            #region check if there is enough space left to read the bytes

            var logicalPosition = GetLogicalPositionInWindow(myPhysicalPosition);

            if (!IsEnoughSpaceInWindow(logicalPosition, myByteLength))
            {
                if ((_WindowOffset + _WindowSize) == _MemoryMappedFileSize)
                {
                    throw new StorageEngineException(String.Format("Reading from MemoryMappedFileStorage {0} failed. Not enough space left.", _StorageLocation));
                }
                else
                {
                    SlideWindow((long)myByteLength);
                }
            }

            #endregion
        
        }

        #endregion

        #endregion
    
    }
       
}

/* 
 * AStorageEngine
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;

using sones.StorageEngines;
using sones.Notifications;

#endregion

namespace sones.StorageEngines
{

    /// <summary>
    /// The abstract base class for most sones StorageEngines
    /// </summary>

    public abstract class AStorageEngine : IStorageEngine
    {
        
        #region Constructors

        #region AStorageEngine()

        public AStorageEngine()
        {
            _StorageUUID = StorageUUID.NewUUID;
            _Description = "";
        }

        #endregion

        #endregion

        #region AStorageEngine Members

        #region NotificationDispatcher

        protected NotificationDispatcher _NotificationDispatcher;

        public NotificationDispatcher NotificationDispatcher
        {

            get
            {
                return _NotificationDispatcher;
            }

            set
            {
                _NotificationDispatcher = value;
            }

        }

        #endregion

        #region StorageLocation

        protected String _StorageLocation;

        public String StorageLocation
        {
            get
            {
                return _StorageLocation;
            }
        }

        #endregion

        #region StorageUUID

        protected StorageUUID _StorageUUID;

        /// <summary>
        /// A unique identifier for this StorageEngine
        /// </summary>
        public StorageUUID StorageUUID
        {
            get
            {
                return _StorageUUID;
            }
        }

        #endregion

        #region URIPrefix

        public abstract String URIPrefix { get; }

        #endregion

        #region Description

        protected String _Description;

        public String Description
        {

            get
            {
                return _Description;
            }

            set
            {
                if (value != null && value != "")
                    _Description = value;
            }

        }

        #endregion

        #region IsAttached

        public abstract Boolean IsAttached { get; }

        #endregion

        #region StorageSize

        public abstract UInt64 Size { get; }

        #endregion

        #region Resizeable

        protected Boolean _Resizeable = true;

        public Boolean IsResizeable
        {

            get
            {
                return _Resizeable;
            }

            set
            {
                _Resizeable = value;
            }

        }

        #endregion

        #region Autogrow

        protected Boolean _Autogrow;

        public Boolean Autogrow
        {

            get
            {
                if (_Resizeable)
                    return _Autogrow;

                return false;
            }

            set
            {
                if (_Resizeable)
                    _Autogrow = value;
            }

        }

        #endregion

        #region IsMemoryOnly

        public abstract Boolean IsMemoryOnly { get; }

        #endregion


        #region Methods

        public abstract Boolean FormatStorage(String myStorageLocation, UInt64 myNumberOfBytes, UInt32 myBufferSize, Boolean myAllowOverwrite, Action<Double> myAction);
        public abstract Boolean AttachStorage(String myStorageLocation);
        public abstract Boolean DetachStorage();
        public abstract Boolean SealStorage();
        public abstract Boolean GrowStorage(UInt64 myNumberOfBytesToAdd);
        public abstract Boolean ShrinkStorage(UInt64 myNumberOfBytesToAdd);

        public abstract Byte[]  ReadPosition(UInt64 myPhysicalPosition, UInt64 myLength);
        public abstract Byte[]  ReadExtents(ObjectExtentsList myObjectExtentsList);

        public abstract Boolean WritePosition(Byte[] myByteArray, UInt64 myPhysicalPosition);
        public abstract Boolean WritePositions(Byte[] myByteArray, IEnumerable<UInt64> myPhysicalPositions);
        public abstract Boolean WriteExtents(Byte[] myByteArray, ObjectExtentsList myObjectExtentsList);

        #endregion

        #endregion

    }

}

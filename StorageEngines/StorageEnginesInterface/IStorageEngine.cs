/* 
 * IStorageEngine
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;
using System.ServiceModel;

#endregion

namespace sones.StorageEngines
{

    /// <summary>
    /// The Interface for all sones StorageEngines
    /// </summary>

    [ServiceContract]
    public interface IStorageEngine
    {

        #region Properties

        /// <summary>
        /// Returns the location of the StorageEngine
        /// </summary>
        String      StorageLocation     { get; }

        /// <summary>
        /// A unique identifier for this StorageEngine
        /// </summary>
        StorageUUID StorageUUID         { get; }

        /// <summary>
        /// Returns the prefix of the URI of the StorageEngine
        /// </summary>
        String      URIPrefix           { get; }
        
        /// <summary>
        /// Returns a description of the StorageEngine
        /// </summary>
        String      Description         { get; set; }

        /// <summary>
        /// Is the StorageEngine attached?
        /// </summary>
        Boolean     IsAttached          { get; }

        /// <summary>
        /// Returns the current size of the StorageEngine
        /// </summary>
        UInt64      Size                { get; }

        /// <summary>
        /// Returns true if the size of the StorageEngine can be in- or decreased.
        /// </summary>
        Boolean     IsResizeable        { get; }

        /// <summary>
        /// Returns true if the size of the StorageEngine will increase automatically.
        /// </summary>
        Boolean     Autogrow            { get; set; }

        /// <summary>
        /// Returns true if the StorageEngine is just a in-memory engine.
        /// </summary>
        Boolean     IsMemoryOnly        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new IStorageEngine
        /// </summary>
        /// <param name="myNumberOfBytes">The initial size of the storage engine in byte.</param>
        /// <param name="myBufferSize">The size of the internal buffer during formating the storage engine.</param>
        /// <param name="myOverwriteExistingFilesystem">Delete an existing storage engine?</param>
        /// <param name="myAction">An action called to indicate to progress on formating the storage engine.</param>
        /// <returns>true for success</returns>
        [OperationContract]
        Boolean FormatStorage(String myStorageLocation, UInt64 myNumberOfBytes, UInt32 myBufferSize, Boolean myAllowOverwrite, Action<Double> myAction);

        /// <summary>
        /// Attach an existing medium to this StorageEngine.
        /// This means opening the medium and instantiating the byte cache and read and write queues.
        /// </summary>
        /// <param name="myStorageLocation"></param>
        [OperationContract]
        Boolean AttachStorage(String myStorageLocation);

        /// <summary>
        /// Increases the size of the attached medium
        /// </summary>
        /// <param name="myNumberOfBytesToAdd"></param>
        [OperationContract]
        Boolean GrowStorage(UInt64 myNumberOfBytesToAdd); 

        /// <summary>
        /// Decreases the size of the attached medium
        /// </summary>
        /// <param name="myNumberOfBytesToShrink"></param>
        [OperationContract]
        Boolean ShrinkStorage(UInt64 myNumberOfBytesToShrink); 

        /// <summary>
        /// Detach this medium
        /// </summary>
        [OperationContract]
        Boolean DetachStorage();

        /// <summary>
        /// This will seal the whole storage (or just a part of it)
        /// </summary>
        [OperationContract]
        Boolean SealStorage();

        #endregion

        #region Read

        [OperationContract]
        Byte[] ReadPosition(UInt64 myPhysicalPosition, UInt64 myLength);

        [OperationContract]
        Byte[] ReadExtents(ObjectExtentsList myObjectExtentsList);

        #endregion

        #region Write

        [OperationContract]
        Boolean WritePosition(Byte[] myByteArray, UInt64 myPhysicalPosition);

        [OperationContract]
        Boolean WritePositions(Byte[] myByteArray, IEnumerable<UInt64> myPhysicalPositions);

        [OperationContract]
        Boolean WriteExtents(Byte[] myByteArray, ObjectExtentsList myListOfExtents);

        #endregion

    }

}
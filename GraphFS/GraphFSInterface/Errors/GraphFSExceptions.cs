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
 * GraphFSException
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Text;

#endregion

namespace sones.GraphFS.Exceptions
{

    /// <summary>
    /// This is a class for all GraphFSExceptions
    /// </summary>

    #region GraphFSException Superclass

    public class GraphFSException : ApplicationException
    {
        public GraphFSException(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region Generic Exceptions

    public class GraphFSException_ActivationError : GraphFSException
    {
        public GraphFSException_ActivationError(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_UnkownIGraphFSImplementation : GraphFSException
    {
        public GraphFSException_UnkownIGraphFSImplementation(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_OperationNotAllowed : GraphFSException
    {
        public GraphFSException_OperationNotAllowed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_InvalidObjectLocation : GraphFSException
    {
        public GraphFSException_InvalidObjectLocation(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion



    #region MakeFileSystem

    public class GraphFSException_MakeFilesystemFailedAlreadyMounted : GraphFSException
    {
        public GraphFSException_MakeFilesystemFailedAlreadyMounted(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region ForestDirectory

    public class GraphFSException_ForestDirectory_UUIDCouldNotBeRead : GraphFSException
    {
        public GraphFSException_ForestDirectory_UUIDCouldNotBeRead(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_ForestDirectory_AllocationMapCouldNotBeRead : GraphFSException
    {
        public GraphFSException_ForestDirectory_AllocationMapCouldNotBeRead(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_ForestDirectory_defaultFSCouldNotBeRead : GraphFSException
    {
        public GraphFSException_ForestDirectory_defaultFSCouldNotBeRead(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_RootDirectory_UUIDCouldNotBeRead : GraphFSException
    {
        public GraphFSException_RootDirectory_UUIDCouldNotBeRead(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_RootDirectory_EntityObjectCouldNotBeLoaded : GraphFSException
    {
        public GraphFSException_RootDirectory_EntityObjectCouldNotBeLoaded(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_RootDirectory_RightsObjectCouldNotBeLoaded : GraphFSException
    {
        public GraphFSException_RootDirectory_RightsObjectCouldNotBeLoaded(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_RootDirectory_RightsIndexObjectCouldNotBeLoaded : GraphFSException
    {
        public GraphFSException_RootDirectory_RightsIndexObjectCouldNotBeLoaded(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_RootDirectoryNotFoundWithinTheObjectCache : GraphFSException
    {
        public GraphFSException_RootDirectoryNotFoundWithinTheObjectCache(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region MountFileSystem

    public class GraphFSException_FileSystemNotMounted : GraphFSException
    {
        public GraphFSException_FileSystemNotMounted(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_MountFileSystemFailed : GraphFSException
    {
        public GraphFSException_MountFileSystemFailed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_MountFileSystemFailed_DirectoryIsAMountpoint : GraphFSException
    {
        public GraphFSException_MountFileSystemFailed_DirectoryIsAMountpoint(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_UnmountFileSystemFailed : GraphFSException
    {
        public GraphFSException_UnmountFileSystemFailed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion


    #region AllocationMapObject

    public class GraphFSAllocationMapException : GraphFSException
    {
        public GraphFSAllocationMapException(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_AllocationFailed : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_AllocationFailed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_DeallocationFailed : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_DeallocationFailed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_NumberTooLarge : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_NumberTooLarge(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_NoFreeBytesFound : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_NoFreeBytesFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_NotEnoughFreeBytes : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_NotEnoughFreeBytes(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_NumberOfBytesMustBeGreaterThanZero : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_NumberOfBytesMustBeGreaterThanZero(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_CouldNotAllocateAsSingleExtent : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_CouldNotAllocateAsSingleExtent(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_NoPreAllocationFound : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_NoPreAllocationFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_PreAllocatedSizeOverflow : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_PreAllocatedSizeOverflow(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_AddToFreeExtents : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_AddToFreeExtents(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_ExtentNotFoundWithinFreeExtentsByStartPosition : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_ExtentNotFoundWithinFreeExtentsByStartPosition(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    public class GraphFSAllocationMapException_ExtentNotFoundWithinFreeExtentsByLength : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_ExtentNotFoundWithinFreeExtentsByLength(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    public class GraphFSAllocationMapException_AddToListOfStartPositions : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_AddToListOfStartPositions(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    public class GraphFSAllocationMapException_RemoveFromListOfStartPositions : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_RemoveFromListOfStartPositions(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    public class GraphFSAllocationMapException_CouldNotAllocate : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_CouldNotAllocate(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_AllocationMapCouldNotBeDeserialized : GraphFSAllocationMapException
    {
        public GraphFSException_AllocationMapCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSAllocationMapException_FreeExtentsListsInconsistency : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_FreeExtentsListsInconsistency(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSAllocationMapException_CouldNotShrinkAllocationMap : GraphFSAllocationMapException
    {
        public GraphFSAllocationMapException_CouldNotShrinkAllocationMap(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion


    #region INode and ObjectLocator Exceptions

    public class GraphFSException_INodeCouldNotBeDeserialized : GraphFSException
    {
        public GraphFSException_INodeCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_InvalidINodePositions : GraphFSException
    {
        public GraphFSException_InvalidINodePositions(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_InvalidIndex : GraphFSException
    {
        public GraphFSException_InvalidIndex(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_ObjectLocatorCouldNotBeDeserialized : GraphFSException
    {
        public GraphFSException_ObjectLocatorCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion


    #region Symlink and ObjectStream Exceptions

    #region Object/Symlink not found

    public class GraphFSException_ObjectNotFound : GraphFSException
    {
        public GraphFSException_ObjectNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_SymlinkNotFound : GraphFSException_ObjectNotFound
    {
        public GraphFSException_SymlinkNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_FileObjectNotFound : GraphFSException_ObjectNotFound
    {
        public GraphFSException_FileObjectNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region ObjectSTREAMs_NotFound

    public class GraphFSException_DIRECTORYSTREAM_NotFound : GraphFSException_ObjectNotFound
    {
        public GraphFSException_DIRECTORYSTREAM_NotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_VIRTUALDIRECTORY_NotFound : GraphFSException_ObjectNotFound
    {
        public GraphFSException_VIRTUALDIRECTORY_NotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_FILESTREAM_NotFound : GraphFSException_ObjectNotFound
    {
        public GraphFSException_FILESTREAM_NotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_SYSTEMMETADATASTREAM_NotFound : GraphFSException_ObjectNotFound
    {
        public GraphFSException_SYSTEMMETADATASTREAM_NotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_USERMETADATASTREAM_NotFound : GraphFSException_ObjectNotFound
    {
        public GraphFSException_USERMETADATASTREAM_NotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_PreviewObjectNotFound : GraphFSException_ObjectNotFound
    {
        public GraphFSException_PreviewObjectNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_BlockIntegrityObjectNotFound : GraphFSException_ObjectNotFound
    {
        public GraphFSException_BlockIntegrityObjectNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_AccessControllObjectNotFound : GraphFSException_ObjectNotFound
    {
        public GraphFSException_AccessControllObjectNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_ObjectHasNoInlineData : GraphFSException_ObjectNotFound
    {
        public GraphFSException_ObjectHasNoInlineData(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region ObjectAlreadyExists

    public class GraphFSException_ObjectAlreadyExists : GraphFSException
    {
        public GraphFSException_ObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_ObjectStreamAlreadyExists : GraphFSException_ObjectAlreadyExists
    {
        public GraphFSException_ObjectStreamAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_SymlinkAlreadyExists : GraphFSException_ObjectAlreadyExists
    {
        public GraphFSException_SymlinkAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_DirectoryObjectAlreadyExists : GraphFSException_ObjectAlreadyExists
    {
        public GraphFSException_DirectoryObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_DeviceBusy : GraphFSException
    {
        public GraphFSException_DeviceBusy(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_FileObjectAlreadyExists : GraphFSException_ObjectAlreadyExists
    {
        public GraphFSException_FileObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_UserMetadataObjectAlreadyExists : GraphFSException_ObjectAlreadyExists
    {
        public GraphFSException_UserMetadataObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_SystemMetadataObjectAlreadyExists : GraphFSException_ObjectAlreadyExists
    {
        public GraphFSException_SystemMetadataObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_BlockIntegrityObjectAlreadyExists : GraphFSException_ObjectAlreadyExists
    {
        public GraphFSException_BlockIntegrityObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_AccessControllObjectAlreadyExists : GraphFSException_ObjectAlreadyExists
    {
        public GraphFSException_AccessControllObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region ObjectStreamNotAllowed

    public class GraphFSException_ObjectStreamNotAllowed : GraphFSException
    {
        public GraphFSException_ObjectStreamNotAllowed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region UnknownObjectStream

    public class GraphFSException_UnknownObjectStream : GraphFSException
    {
        public GraphFSException_UnknownObjectStream(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region GraphFSException_ObjectIsNotASymlink

    public class GraphFSException_ObjectIsNotASymlink : GraphFSException
    {
        public GraphFSException_ObjectIsNotASymlink(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region GraphFSException_NoPositionsForReading

    public class GraphFSException_NoPositionsForReading : GraphFSException
    {
        public GraphFSException_NoPositionsForReading(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #endregion

    #region SuperblockObject Exceptions

    public class GraphFSException_SuperblockCouldNotBeDeserialized : GraphFSException
    {
        public GraphFSException_SuperblockCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region ObjectSafety and Security

    #region GraphFS Information Header Exception

    public class GraphFSInformationHeaderException : GraphFSException
    {
        public GraphFSInformationHeaderException(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #region Integrity Check

    public class GraphFSException_IntegrityCheckFailed : GraphFSInformationHeaderException
    {
        public GraphFSException_IntegrityCheckFailed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_InvalidInformationHeader : GraphFSInformationHeaderException
    {
        public GraphFSException_InvalidInformationHeader(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_InvalidIntegrityCheckLengthField : GraphFSInformationHeaderException
    {
        public GraphFSException_InvalidIntegrityCheckLengthField(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region Encryption

    public class GraphFSException_InvalidEncryptionParametersLengthField : GraphFSInformationHeaderException
    {
        public GraphFSException_InvalidEncryptionParametersLengthField(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_InvalidDataPaddingLengthField : GraphFSInformationHeaderException
    {
        public GraphFSException_InvalidDataPaddingLengthField(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_InvalidAdditionalPaddingLengthField : GraphFSInformationHeaderException
    {
        public GraphFSException_InvalidAdditionalPaddingLengthField(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_InformationHeaderPaddingTooSmall : GraphFSInformationHeaderException
    {
        public GraphFSException_InformationHeaderPaddingTooSmall(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_InformationHeaderPaddingTooLarge : GraphFSInformationHeaderException
    {
        public GraphFSException_InformationHeaderPaddingTooLarge(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #endregion

    #region GraphFSException_IllegalRevisionTime

    public class GraphFSException_IllegalRevisionTime : GraphFSException
    {
        public GraphFSException_IllegalRevisionTime(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #endregion

    #region EntitiesObject Exceptions

    public class GraphFSException_EntityObjectCouldNotBeDeserialized : GraphFSException
    {
        public GraphFSException_EntityObjectCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_EntityCouldNotBeDeserialized : GraphFSException
    {
        public GraphFSException_EntityCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region AccessControlObject Exceptions

    public class GraphFSException_AccessControlObjectCouldNotBeDeserialized : GraphFSException
    {
        public GraphFSException_AccessControlObjectCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_RightsIncexObjectHasNotBeenLoaded : GraphFSException
    {
        public GraphFSException_RightsIncexObjectHasNotBeenLoaded(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion


    public class GraphFSException_AGraphStructureCouldNotBeDeserialized : GraphFSException
    {
        public GraphFSException_AGraphStructureCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_RevisionIDCouldNotBeDeserialized : GraphFSException
    {
        public GraphFSException_RevisionIDCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_ObjectStreamMappingCouldNotBeDeserialized : GraphFSException
    {
        public GraphFSException_ObjectStreamMappingCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_EntityNotFound : GraphFSException
    {
        public GraphFSException_EntityNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_EntityPasswordInvalid : GraphFSException
    {
        public GraphFSException_EntityPasswordInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_EntityPublicKeyInvalid : GraphFSException
    {
        public GraphFSException_EntityPublicKeyInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_RightNotFound : GraphFSException
    {
        public GraphFSException_RightNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    #region Object Caching

    public class GraphFSException_ObjectAlreadyCached : GraphFSException
    {
        public GraphFSException_ObjectAlreadyCached(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_ObjectDoesNotImplementIFastSerialize : GraphFSException
    {
        public GraphFSException_ObjectDoesNotImplementIFastSerialize(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region WebDAV

    public class GraphFSException_NoMountedFS : GraphFSException
    {
        public GraphFSException_NoMountedFS(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion




    public class GraphFSException_SymlinkCouldNotBeDeleted : GraphFSException
    {
        public GraphFSException_SymlinkCouldNotBeDeleted(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_SymlinkCouldNotBeRemoved : GraphFSException
    {
        public GraphFSException_SymlinkCouldNotBeRemoved(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_ObjectCouldNotBeDeserialized : GraphFSException
    {
        public GraphFSException_ObjectCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_FileObjectCouldNotBeDeserialized : GraphFSException_ObjectCouldNotBeDeserialized
    {
        public GraphFSException_FileObjectCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    public class GraphFSException_DirectoryObjectIsNotEmpty : GraphFSException
    {
        public GraphFSException_DirectoryObjectIsNotEmpty(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_ObjectStreamNotFound : GraphFSException
    {
        public GraphFSException_ObjectStreamNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_CircularSymlinksDetected : GraphFSException
    {
        public GraphFSException_CircularSymlinksDetected(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    #region Parser Exceptions

    public class GraphFSException_ParseError : GraphFSException
    {
        public GraphFSException_ParseError(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_ParsedSizeIsInvalid : GraphFSException_ParseError
    {
        public GraphFSException_ParsedSizeIsInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_ParsedCreationTimeIsInvalid : GraphFSException_ParseError
    {
        public GraphFSException_ParsedCreationTimeIsInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_ParsedLastAccessTimeIsInvalid : GraphFSException_ParseError
    {
        public GraphFSException_ParsedLastAccessTimeIsInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_ParsedLastModificationTimeIsInvalid : GraphFSException_ParseError
    {
        public GraphFSException_ParsedLastModificationTimeIsInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_ParsedDeletionTimeIsInvalid : GraphFSException_ParseError
    {
        public GraphFSException_ParsedDeletionTimeIsInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion


    //public class GraphFSException_InlineDataAndINodePositionsAtTheSameTimeAreInvalid : GraphFSException
    //{
    //    public GraphFSException_InlineDataAndINodePositionsAtTheSameTimeAreInvalid(String message)
    //        : base(message)
    //    {
    //        // do nothing extra
    //    }
    //}


    //public class GraphFSException_ObjectEditionAlreadyExists : GraphFSException
    //{
    //    public GraphFSException_ObjectEditionAlreadyExists(String message)
    //        : base(message)
    //    {
    //        // do nothing extra
    //    }
    //}


    //public class GraphFSException_ObjectStreamTypeFactoryError : GraphFSException
    //{
    //    public GraphFSException_ObjectStreamTypeFactoryError(String message)
    //        : base(message)
    //    {
    //        // do nothing extra
    //    }
    //}


    //public class GraphFSException_ObjectStreamTypeFactory_DuplicateObjectStream : GraphFSException_ObjectStreamTypeFactoryError
    //{
    //    public GraphFSException_ObjectStreamTypeFactory_DuplicateObjectStream(String message)
    //        : base(message)
    //    {
    //        // do nothing extra
    //    }
    //}



    public class GraphFSException_TypeParametersDiffer : GraphFSException
    {
        public GraphFSException_TypeParametersDiffer(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class GraphFSException_NoTransactionFound : GraphFSException
    {
        public GraphFSException_NoTransactionFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_RevisionAlreadyHoldTransaction : GraphFSException
    {
        public GraphFSException_RevisionAlreadyHoldTransaction(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_LocationAlreadyHoldTransaction : GraphFSException
    {
        public GraphFSException_LocationAlreadyHoldTransaction(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_IndexKeyAlreadyExist : GraphFSException
    {
        public GraphFSException_IndexKeyAlreadyExist(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class GraphFSException_CouldNotOpenStream : GraphFSException
    {
        public GraphFSException_CouldNotOpenStream(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #region FastSerializeTypeSurrogate


    public class GraphFSException_FastSerializeSurrogateTypeCodeExist : GraphFSException
    {
        public GraphFSException_FastSerializeSurrogateTypeCodeExist(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

}

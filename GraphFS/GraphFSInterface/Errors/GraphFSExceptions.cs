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
 * GraphFSException
 * Achim Friedland, 2008 - 2010
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

    public class PandoraFSException_MakeFilesystemFailedAlreadyMounted : GraphFSException
    {
        public PandoraFSException_MakeFilesystemFailedAlreadyMounted(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region ForestDirectory

    public class PandoraFSException_ForestDirectory_UUIDCouldNotBeRead : GraphFSException
    {
        public PandoraFSException_ForestDirectory_UUIDCouldNotBeRead(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_ForestDirectory_AllocationMapCouldNotBeRead : GraphFSException
    {
        public PandoraFSException_ForestDirectory_AllocationMapCouldNotBeRead(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_ForestDirectory_defaultFSCouldNotBeRead : GraphFSException
    {
        public PandoraFSException_ForestDirectory_defaultFSCouldNotBeRead(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_RootDirectory_UUIDCouldNotBeRead : GraphFSException
    {
        public PandoraFSException_RootDirectory_UUIDCouldNotBeRead(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_RootDirectory_EntityObjectCouldNotBeLoaded : GraphFSException
    {
        public PandoraFSException_RootDirectory_EntityObjectCouldNotBeLoaded(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_RootDirectory_RightsObjectCouldNotBeLoaded : GraphFSException
    {
        public PandoraFSException_RootDirectory_RightsObjectCouldNotBeLoaded(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_RootDirectory_RightsIndexObjectCouldNotBeLoaded : GraphFSException
    {
        public PandoraFSException_RootDirectory_RightsIndexObjectCouldNotBeLoaded(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_RootDirectoryNotFoundWithinTheObjectCache : GraphFSException
    {
        public PandoraFSException_RootDirectoryNotFoundWithinTheObjectCache(String message)
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


    public class PandoraFSException_MountFileSystemFailed : GraphFSException
    {
        public PandoraFSException_MountFileSystemFailed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_MountFileSystemFailed_DirectoryIsAMountpoint : GraphFSException
    {
        public PandoraFSException_MountFileSystemFailed_DirectoryIsAMountpoint(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_UnmountFileSystemFailed : GraphFSException
    {
        public PandoraFSException_UnmountFileSystemFailed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion


    #region AllocationMapObject

    public class PandoraFSAllocationMapException : GraphFSException
    {
        public PandoraFSAllocationMapException(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_AllocationFailed : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_AllocationFailed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_DeallocationFailed : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_DeallocationFailed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_NumberTooLarge : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_NumberTooLarge(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_NoFreeBytesFound : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_NoFreeBytesFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_NotEnoughFreeBytes : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_NotEnoughFreeBytes(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_NumberOfBytesMustBeGreaterThanZero : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_NumberOfBytesMustBeGreaterThanZero(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_CouldNotAllocateAsSingleExtent : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_CouldNotAllocateAsSingleExtent(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_NoPreAllocationFound : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_NoPreAllocationFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_PreAllocatedSizeOverflow : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_PreAllocatedSizeOverflow(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_AddToFreeExtents : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_AddToFreeExtents(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_ExtentNotFoundWithinFreeExtentsByStartPosition : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_ExtentNotFoundWithinFreeExtentsByStartPosition(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    public class PandoraFSAllocationMapException_ExtentNotFoundWithinFreeExtentsByLength : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_ExtentNotFoundWithinFreeExtentsByLength(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    public class PandoraFSAllocationMapException_AddToListOfStartPositions : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_AddToListOfStartPositions(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    public class PandoraFSAllocationMapException_RemoveFromListOfStartPositions : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_RemoveFromListOfStartPositions(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    public class PandoraFSAllocationMapException_CouldNotAllocate : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_CouldNotAllocate(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_AllocationMapCouldNotBeDeserialized : PandoraFSAllocationMapException
    {
        public PandoraFSException_AllocationMapCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSAllocationMapException_FreeExtentsListsInconsistency : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_FreeExtentsListsInconsistency(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSAllocationMapException_CouldNotShrinkAllocationMap : PandoraFSAllocationMapException
    {
        public PandoraFSAllocationMapException_CouldNotShrinkAllocationMap(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion


    #region INode and ObjectLocator Exceptions

    public class PandoraFSException_INodeCouldNotBeDeserialized : GraphFSException
    {
        public PandoraFSException_INodeCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_InvalidINodePositions : GraphFSException
    {
        public PandoraFSException_InvalidINodePositions(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_InvalidIndex : GraphFSException
    {
        public PandoraFSException_InvalidIndex(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_ObjectLocatorCouldNotBeDeserialized : GraphFSException
    {
        public PandoraFSException_ObjectLocatorCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion


    #region Symlink and ObjectStream Exceptions

    #region Object/Symlink not found

    public class PandoraFSException_ObjectNotFound : GraphFSException
    {
        public PandoraFSException_ObjectNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_SymlinkNotFound : PandoraFSException_ObjectNotFound
    {
        public PandoraFSException_SymlinkNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_FileObjectNotFound : PandoraFSException_ObjectNotFound
    {
        public PandoraFSException_FileObjectNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region ObjectSTREAMs_NotFound

    public class PandoraFSException_DIRECTORYSTREAM_NotFound : PandoraFSException_ObjectNotFound
    {
        public PandoraFSException_DIRECTORYSTREAM_NotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_VIRTUALDIRECTORY_NotFound : PandoraFSException_ObjectNotFound
    {
        public PandoraFSException_VIRTUALDIRECTORY_NotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_FILESTREAM_NotFound : PandoraFSException_ObjectNotFound
    {
        public PandoraFSException_FILESTREAM_NotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_SYSTEMMETADATASTREAM_NotFound : PandoraFSException_ObjectNotFound
    {
        public PandoraFSException_SYSTEMMETADATASTREAM_NotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_USERMETADATASTREAM_NotFound : PandoraFSException_ObjectNotFound
    {
        public PandoraFSException_USERMETADATASTREAM_NotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_PreviewObjectNotFound : PandoraFSException_ObjectNotFound
    {
        public PandoraFSException_PreviewObjectNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_BlockIntegrityObjectNotFound : PandoraFSException_ObjectNotFound
    {
        public PandoraFSException_BlockIntegrityObjectNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_AccessControllObjectNotFound : PandoraFSException_ObjectNotFound
    {
        public PandoraFSException_AccessControllObjectNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_ObjectHasNoInlineData : PandoraFSException_ObjectNotFound
    {
        public PandoraFSException_ObjectHasNoInlineData(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region ObjectAlreadyExists

    public class PandoraFSException_ObjectAlreadyExists : GraphFSException
    {
        public PandoraFSException_ObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_ObjectStreamAlreadyExists : PandoraFSException_ObjectAlreadyExists
    {
        public PandoraFSException_ObjectStreamAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_SymlinkAlreadyExists : PandoraFSException_ObjectAlreadyExists
    {
        public PandoraFSException_SymlinkAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_DirectoryObjectAlreadyExists : PandoraFSException_ObjectAlreadyExists
    {
        public PandoraFSException_DirectoryObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_DeviceBusy : GraphFSException
    {
        public PandoraFSException_DeviceBusy(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_FileObjectAlreadyExists : PandoraFSException_ObjectAlreadyExists
    {
        public PandoraFSException_FileObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_UserMetadataObjectAlreadyExists : PandoraFSException_ObjectAlreadyExists
    {
        public PandoraFSException_UserMetadataObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_SystemMetadataObjectAlreadyExists : PandoraFSException_ObjectAlreadyExists
    {
        public PandoraFSException_SystemMetadataObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_BlockIntegrityObjectAlreadyExists : PandoraFSException_ObjectAlreadyExists
    {
        public PandoraFSException_BlockIntegrityObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_AccessControllObjectAlreadyExists : PandoraFSException_ObjectAlreadyExists
    {
        public PandoraFSException_AccessControllObjectAlreadyExists(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region ObjectStreamNotAllowed

    public class PandoraFSException_ObjectStreamNotAllowed : GraphFSException
    {
        public PandoraFSException_ObjectStreamNotAllowed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region UnknownObjectStream

    public class PandoraFSException_UnknownObjectStream : GraphFSException
    {
        public PandoraFSException_UnknownObjectStream(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region PandoraFSException_ObjectIsNotASymlink

    public class PandoraFSException_ObjectIsNotASymlink : GraphFSException
    {
        public PandoraFSException_ObjectIsNotASymlink(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region PandoraFSException_NoPositionsForReading

    public class PandoraFSException_NoPositionsForReading : GraphFSException
    {
        public PandoraFSException_NoPositionsForReading(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #endregion

    #region SuperblockObject Exceptions

    public class PandoraFSException_SuperblockCouldNotBeDeserialized : GraphFSException
    {
        public PandoraFSException_SuperblockCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region ObjectSafety and Security

    #region PandoraFS Information Header Exception

    public class PandoraFSInformationHeaderException : GraphFSException
    {
        public PandoraFSInformationHeaderException(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #region Integrity Check

    public class PandoraFSException_IntegrityCheckFailed : PandoraFSInformationHeaderException
    {
        public PandoraFSException_IntegrityCheckFailed(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_InvalidInformationHeader : PandoraFSInformationHeaderException
    {
        public PandoraFSException_InvalidInformationHeader(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_InvalidIntegrityCheckLengthField : PandoraFSInformationHeaderException
    {
        public PandoraFSException_InvalidIntegrityCheckLengthField(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region Encryption

    public class PandoraFSException_InvalidEncryptionParametersLengthField : PandoraFSInformationHeaderException
    {
        public PandoraFSException_InvalidEncryptionParametersLengthField(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_InvalidDataPaddingLengthField : PandoraFSInformationHeaderException
    {
        public PandoraFSException_InvalidDataPaddingLengthField(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_InvalidAdditionalPaddingLengthField : PandoraFSInformationHeaderException
    {
        public PandoraFSException_InvalidAdditionalPaddingLengthField(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_InformationHeaderPaddingTooSmall : PandoraFSInformationHeaderException
    {
        public PandoraFSException_InformationHeaderPaddingTooSmall(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_InformationHeaderPaddingTooLarge : PandoraFSInformationHeaderException
    {
        public PandoraFSException_InformationHeaderPaddingTooLarge(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #endregion

    #region PandoraFSException_IllegalRevisionTime

    public class PandoraFSException_IllegalRevisionTime : GraphFSException
    {
        public PandoraFSException_IllegalRevisionTime(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #endregion

    #region EntitiesObject Exceptions

    public class PandoraFSException_EntityObjectCouldNotBeDeserialized : GraphFSException
    {
        public PandoraFSException_EntityObjectCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_EntityCouldNotBeDeserialized : GraphFSException
    {
        public PandoraFSException_EntityCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region AccessControlObject Exceptions

    public class PandoraFSException_AccessControlObjectCouldNotBeDeserialized : GraphFSException
    {
        public PandoraFSException_AccessControlObjectCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_RightsIncexObjectHasNotBeenLoaded : GraphFSException
    {
        public PandoraFSException_RightsIncexObjectHasNotBeenLoaded(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion


    public class PandoraFSException_APandoraStructureCouldNotBeDeserialized : GraphFSException
    {
        public PandoraFSException_APandoraStructureCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_RevisionIDCouldNotBeDeserialized : GraphFSException
    {
        public PandoraFSException_RevisionIDCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_ObjectStreamMappingCouldNotBeDeserialized : GraphFSException
    {
        public PandoraFSException_ObjectStreamMappingCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_EntityNotFound : GraphFSException
    {
        public PandoraFSException_EntityNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_EntityPasswordInvalid : GraphFSException
    {
        public PandoraFSException_EntityPasswordInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_EntityPublicKeyInvalid : GraphFSException
    {
        public PandoraFSException_EntityPublicKeyInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_RightNotFound : GraphFSException
    {
        public PandoraFSException_RightNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    #region Object Caching

    public class PandoraFSException_ObjectAlreadyCached : GraphFSException
    {
        public PandoraFSException_ObjectAlreadyCached(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_ObjectDoesNotImplementIFastSerialize : GraphFSException
    {
        public PandoraFSException_ObjectDoesNotImplementIFastSerialize(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region WebDAV

    public class PandoraFSException_NoMountedFS : GraphFSException
    {
        public PandoraFSException_NoMountedFS(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion




    public class PandoraFSException_SymlinkCouldNotBeDeleted : GraphFSException
    {
        public PandoraFSException_SymlinkCouldNotBeDeleted(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_SymlinkCouldNotBeRemoved : GraphFSException
    {
        public PandoraFSException_SymlinkCouldNotBeRemoved(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_ObjectCouldNotBeDeserialized : GraphFSException
    {
        public PandoraFSException_ObjectCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_FileObjectCouldNotBeDeserialized : PandoraFSException_ObjectCouldNotBeDeserialized
    {
        public PandoraFSException_FileObjectCouldNotBeDeserialized(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    public class PandoraFSException_DirectoryObjectIsNotEmpty : GraphFSException
    {
        public PandoraFSException_DirectoryObjectIsNotEmpty(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_ObjectStreamNotFound : GraphFSException
    {
        public PandoraFSException_ObjectStreamNotFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_CircularSymlinksDetected : GraphFSException
    {
        public PandoraFSException_CircularSymlinksDetected(String message)
            : base(message)
        {
            // do nothing extra
        }
    }



    #region Parser Exceptions

    public class PandoraFSException_ParseError : GraphFSException
    {
        public PandoraFSException_ParseError(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_ParsedSizeIsInvalid : PandoraFSException_ParseError
    {
        public PandoraFSException_ParsedSizeIsInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_ParsedCreationTimeIsInvalid : PandoraFSException_ParseError
    {
        public PandoraFSException_ParsedCreationTimeIsInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_ParsedLastAccessTimeIsInvalid : PandoraFSException_ParseError
    {
        public PandoraFSException_ParsedLastAccessTimeIsInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_ParsedLastModificationTimeIsInvalid : PandoraFSException_ParseError
    {
        public PandoraFSException_ParsedLastModificationTimeIsInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_ParsedDeletionTimeIsInvalid : PandoraFSException_ParseError
    {
        public PandoraFSException_ParsedDeletionTimeIsInvalid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion


    //public class PandoraFSException_InlineDataAndINodePositionsAtTheSameTimeAreInvalid : PandoraFSException
    //{
    //    public PandoraFSException_InlineDataAndINodePositionsAtTheSameTimeAreInvalid(String message)
    //        : base(message)
    //    {
    //        // do nothing extra
    //    }
    //}


    //public class PandoraFSException_ObjectEditionAlreadyExists : PandoraFSException
    //{
    //    public PandoraFSException_ObjectEditionAlreadyExists(String message)
    //        : base(message)
    //    {
    //        // do nothing extra
    //    }
    //}


    //public class PandoraFSException_ObjectStreamTypeFactoryError : PandoraFSException
    //{
    //    public PandoraFSException_ObjectStreamTypeFactoryError(String message)
    //        : base(message)
    //    {
    //        // do nothing extra
    //    }
    //}


    //public class PandoraFSException_ObjectStreamTypeFactory_DuplicateObjectStream : PandoraFSException_ObjectStreamTypeFactoryError
    //{
    //    public PandoraFSException_ObjectStreamTypeFactory_DuplicateObjectStream(String message)
    //        : base(message)
    //    {
    //        // do nothing extra
    //    }
    //}



    public class PandoraFSException_TypeParametersDiffer : GraphFSException
    {
        public PandoraFSException_TypeParametersDiffer(String message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class PandoraFSException_NoTransactionFound : GraphFSException
    {
        public PandoraFSException_NoTransactionFound(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_RevisionAlreadyHoldTransaction : GraphFSException
    {
        public PandoraFSException_RevisionAlreadyHoldTransaction(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_LocationAlreadyHoldTransaction : GraphFSException
    {
        public PandoraFSException_LocationAlreadyHoldTransaction(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class PandoraFSException_IndexKeyAlreadyExist : GraphFSException
    {
        public PandoraFSException_IndexKeyAlreadyExist(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #region FastSerializeTypeSurrogate


    public class PandoraFSException_FastSerializeSurrogateTypeCodeExist : GraphFSException
    {
        public PandoraFSException_FastSerializeSurrogateTypeCodeExist(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

}

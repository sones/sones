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
 * FSConstants
 * (c) sones Team, 2007 - 2010
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;
using sones.Lib.DataStructures;

#endregion

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    /// Here should all global file storage constants be declared and assigned
    /// </summary>
    public static class FSConstants
    {

        public const String ObjectStreamDelimiter               = ":";

        public const String DotLink                             = ".";
        public const String DotDotLink                          = "..";
        public const String DotFS                               = ".fs";
        public const String DotForest                           = ".forest";
        public const String DotMetadata                         = ".metadata";
        public const String DotSystem                           = ".system";
        public const String DotStreams                          = ".streams";
        public const String DotUUID                             = ".uuid";
        public const String DotDefaultFS                        = ".DefaultFS";
        public const String DefaultEdition                      = "DefaultEdition";
        public const String DotDefaultEditionSymlink            = ".DefaultEdition";
        public const String DotLatestRevisionSymlink            = ".LatestRevision";
        public const String EntitiesLocation                    = "Entities";
        public const String RightsLocation                      = "Rights";

        // OnDisc ObjectStream types
        public const String ACCESSCONTROLSTREAM                 = "ACCESSCONTROLSTREAM";
        public const String ALLOCATIONMAPSTREAM                 = "ALLOCATIONMAPSTREAM";
        public const String BLOCKINTEGRITYSTREAM                = "BLOCKINTEGRITYSTREAM";
        public const String DBINDEXSTREAM                       = "DBINDEXSTREAM";
        public const String DIRECTORYSTREAM                     = "DIRECTORYSTREAM";
        public const String ENTITIESSTREAM                      = "ENTITIESSTREAM";
        public const String FILESTREAM                          = "FILESTREAM";
        public const String OBJECTSTREAMMAPPINGSTREAM           = "OBJECTSTREAMMAPPINGSTREAM";
        public const String RIGHTSSTREAM                        = "RIGHTSSTREAM";
        public const String SYSTEMMETADATASTREAM                = "SYSTEMMETADATASTREAM";
        public const String USERMETADATASTREAM                  = "USERMETADATASTREAM";
        public const String SETTINGSSTREAM                      = "SETTINGSSTREAM";
        public const String QUEUESTREAM                         = "QUEUESTREAM";

        public const String INDEXSTREAM                         = "INDEXSTREAM";
        public const String DEFAULT_INDEXSTREAM                 = "DEFAULT_INDEXSTREAM";
        public const String INDEXSTREAM_STRING                  = "INDEXSTREAM_STRING";
        public const String INDEXSTREAM_INT64                   = "INDEXSTREAM_INT64";
        public const String INDEXSTREAM_DOUBLE                  = "INDEXSTREAM_DOUBLE";
        public const String INDEXSTREAM_DATETIME                = "INDEXSTREAM_DATETIME";

        public const String LISTOF_STRINGS                      = "LISTOF_STRINGS";
        public const String LISTOF_INT64                        = "LISTOF_INT64";
        public const String LISTOF_UINT64                       = "LISTOF_UINT64";

        // Virtual ObjectStream types
        public const String INLINEDATA                          = "INLINEDATA";
        public const String SYMLINK                             = "SYMLINK";
        public const String VIRTUALDIRECTORY                    = "VIRTUALDIRECTORY";

        public const String ObjectLocatorName_SuperBlock        = "Superblock";
        public const String ObjectLocatorUUID                   = "ObjectLocatorUUID";

        public const String ObjectCopies_MinNumberOfCopiesName  = "MinNumberOfCopies";
        public const String ObjectCopies_MaxNumberOfCopiesName  = "MaxNumberOfCopies";

        public const String Superblock                          = "Superblock";

        public const Int64  INodeCopies                         = 2;
        public const Int64  ObjectLocatorsCopies                = 2;
        public const Int64  NUMBER_OF_STREAMCOPIES              = 2;

        //public static String HistoryLocation                    = "/CliHistory";
        //public static int    HistoryEntryCount                  = 1000;
        //public static int    MaximumHistoryEntryLength          = 1000;
        //public static String CliDirectoryDefault                = "[NA]";


        public static UInt64 MIN_NUMBER_OF_REVISIONS            = 1;
        public static UInt64 MAX_NUMBER_OF_REVISIONS            = 3;
        public static UInt64 MIN_REVISION_DELTA                 = 0;
        public static UInt64 MAX_REVISION_AGE                   = 0;

        public static UInt64 MIN_NUMBER_OF_COPIES               = 2;
        public static UInt64 MAX_NUMBER_OF_COPIES               = 2;


    }

}

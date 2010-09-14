/* <id Name=”GraphFS – NHDirectoryObject” />
 * <copyright file=”NHDirectoryObject.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <summary>This implements the data structure for handling (access-)
 * rights on file system objects.<summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphFS.InternalObjects
{

    
    [Flags]
    public enum NHIDirectoryObject : long
    {

        ObjectStream_Created,
        ObjectStream_Removed,

        DirectoryEntry_Created,
        DirectoryEntry_Changed,
        DirectoryEntry_Removed,

        InlineData_Created,
        InlineData_Changed,
        InlineData_Removed,

        Symlink_Created,
        Symlink_Changed,
        Symlink_Removed,

        IDirectoryObject_Removed

    }

}

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

namespace sones.GraphFS.Objects
{

    
    [Flags]
    public enum NHIMetadataObject : long
    {

        Metadataum_Added,
        Metadataum_Changed,
        Metadataum_Removed,

        IMetadataObject_Removed

    }

}

/* <id name="GraphDSWebDAV – PropfindProperties" />
 * <copyright file="PropfindProperties.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>WebDAV specific propfind types</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDS.Connectors.WebDAV
{

    /// <summary>
    /// All valid PROPFIND properties
    /// </summary>
    [Flags]
    public enum PropfindProperties : uint
    {
        /// <summary>
        /// No special property requested, return ALL
        /// </summary>
        NONE = 0,
        Creationdate = 1,
        Displayname = 2,
        Getcontentlanguage = 4,
        Getcontentlength = 8,
        Getcontenttype = 16,
        /// <summary>
        /// Getetag: requests a unique identifier for the object
        /// </summary>
        Getetag = 32,
        Getlastmodified = 64,
        Lockdiscovery = 128,
        /// <summary>
        /// Resourcetype: if resourcetype contains &lt;collection/&gt; then it is treated as a directory
        /// </summary>
        Resourcetype = 256,
        Supportedlock = 512,

        #region SVN

        VersionControlledConfiguration = 1024,
        BaselineRelativePath = 2048,
        RepositoryUuid = 4096,
        CheckedIn = 4096 * 2,
        BaselineCollection = 4096 * 2 * 2,
        VersionName = 4096 * 2 * 2 * 2,
        AllProp = 4096 * 2 * 2 * 2 * 2,
        CreatorDisplayname = 4096 * 2 * 2 * 2 * 2 * 2,
        DeadpropCount = 4096 * 2 * 2 * 2 * 2 * 2 * 2,

        #endregion

    }
}

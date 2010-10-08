/* <id Name=”GraphFS – AccessControlObject” />
 * <copyright file=”AccessControlObject.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
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
    public enum NHAccessControlObject : ulong
    {

        AlertIfRightDoesNotExist,

        AlertIfEntityRemovedFromAllowACL,
        AlertIfEntityRemovedFromDenyACL,

        AlertIfEntityAddedToAllowACL,
        AlertIfEntityAddedToDenyACL,

        AlertIfAllowOverDenyChanged,

    }

}

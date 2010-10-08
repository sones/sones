/* GraphFS - ObjectUUIDSubtypes
 * (c) Achim Friedland, 2008 - 2009
 * 
 * The ObjectUUID is a common prefix for all UUIDs within the INodes,
 * ObjectLocators and ObjectStreams located at a specific ObjectLocation.
 * The ObjectUUIDSubtypes are an addition to this UUID prefix for
 * distinguishing the different types of objects and structures within
 * such a object group.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    /// The ObjectUUID is a common prefix for all UUIDs within the INodes,
    /// ObjectLocators and ObjectStreams located at a specific ObjectLocation.
    /// The ObjectUUIDSubtypes are an addition to this UUID prefix for
    /// distinguishing the different types of objects and structures within
    /// such a object group.
    /// </summary>

    [Flags]

    

    public enum ObjectUUIDSubtypes : byte
    {

        INODE                   = 3,       // 0000 0011
        OBJECTLOCATOR           = 15,      // 0000 1111
        OBJECTSTREAM            = 63,      // 0011 1111

    }

}

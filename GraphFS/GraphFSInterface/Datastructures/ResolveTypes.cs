/* GraphFS - ResolveTypes
 * (c) Achim Friedland, 2008 - 2009
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

    
    public enum ResolveTypes
    {
        UNDEF,
        OK,
        ABS_SYMLINK,
        REL_SYMLINK,
        REMOVE_MOUNTPOINT_DOTDOTLINK,
        OBJECT_NOT_FOUND
    }

}

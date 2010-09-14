/* Flags - Flags within the Graph filesystem
 * (c) Daniel Kirstenpfad, 2007-2008
 * (c) Achim Friedland, 2008
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
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

    [Flags]
    
    public enum FS_State
    {
        PFS_CLEAN                   = 0x434c454e,
        PFS_DIRTY                   = 0x44495254,
    }

}

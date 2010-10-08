/* GraphFS - EntityStatus
 * (c) Achim Friedland, 2009
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

namespace sones.GraphFS.InternalObjects
{

    [Flags]
    public enum EntityStatus : ushort
    {

        // This entity is disbaled and should act like an
        // entity without any rights.
        DISABLED,

        // This entity must not be included within the
        // membership hashset of any entity.
        SEALED

    }

}

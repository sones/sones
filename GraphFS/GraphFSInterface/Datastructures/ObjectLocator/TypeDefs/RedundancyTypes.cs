/* GraphFS - RedundancyTypes
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

    

    public enum RedundancyTypes : ushort
    {

        UNUSED,

        RAID5,
        RAID6,
        SecretSharing,

    }

}

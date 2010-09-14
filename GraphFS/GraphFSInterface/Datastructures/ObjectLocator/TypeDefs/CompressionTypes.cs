/* CompressionTypes
 * (c) Achim Friedland, 2008
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

    
    public enum CompressionTypes : ushort
    {

        UNUSED,

        RLE,
        LZW,
        GZIP,
        BZIP2,

    }

}

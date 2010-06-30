/*
 * AccessModeTypes
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.GraphFS
{

    /// <summary>
    /// The different access modes (read/write, read-only, ...)
    /// of a file system.
    /// </summary>

    

    public enum AccessModeTypes
    {
        //
        // The filesystem should be opened for reading and writing
        rw = 0,
        //
        // The filesystem will be opened only for appending data
        ap = 1,
        //
        // The filesystem will be opened only for reading
        ro = 2,
        //
        // ---------------------------------------------
        //
        // The filesystem should be opened only for reading and the metadata for reading and writing
        metarw = 3,
        //
        // The filesystem should be opened only for reading and the metadata only for appending
        metaap = 4,
    }

}

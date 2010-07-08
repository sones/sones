/* 
 * AGraphDBExport
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphDB.ImportExport
{

    /// <summary>
    /// Currently provided type of dump.
    /// </summary>
    [Flags]
    public enum DumpTypes
    {
        GDDL = 1,
        GDML = 2
    }

}

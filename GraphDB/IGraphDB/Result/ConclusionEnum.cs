using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Result
{
    /// <summary>
    /// This enum tells wether some action was successful or not
    /// </summary>
    public enum ConclusionEnum
    {
        /// <summary>
        /// Everything went fine
        /// </summary>
        Success,

        /// <summary>
        /// Something erreonous happend
        /// </summary>
        Fail
    }
}

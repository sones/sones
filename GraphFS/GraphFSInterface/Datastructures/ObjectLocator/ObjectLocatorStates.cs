using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    /// The INode must not be deleted before all revisions are deleted
    /// </summary>
    public enum ObjectLocatorStates : byte
    {
        Unknown,
        Exists,
        Erased
    }

}

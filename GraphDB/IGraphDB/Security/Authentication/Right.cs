using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Security
{
    /// <summary>
    /// The different rights for accessing a graph element
    /// </summary>
    public enum Right
    {
        /// <summary>
        /// Traversal
        /// </summary>
        Traverse,

        /// <summary>
        /// Write a graph element
        /// </summary>
        Write,
        
        /// <summary>
        /// Execution of sth
        /// </summary>
        Execute
    }
}

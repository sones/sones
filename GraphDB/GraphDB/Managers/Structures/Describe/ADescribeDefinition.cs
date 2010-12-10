/*
 * ADescribeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using sones.GraphDB.Result;
using sones.Lib.ErrorHandling;
using System.Collections.Generic;
using sones.GraphDB.NewAPI;


#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{
    /// <summary>
    /// The abstract base class for all describe commands
    /// </summary>
    public abstract class ADescribeDefinition
    {
        /// <summary>
        /// Return the result of a describe command
        /// </summary>
        /// <param name="myDBContext">The db context</param>
        /// <returns>An exceptional that contains an enumerable of vertices</returns>
        public abstract Exceptional<IEnumerable<Vertex>> GetResult(DBContext myDBContext);
    }

}

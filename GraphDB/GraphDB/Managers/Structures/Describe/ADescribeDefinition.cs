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

    public abstract class ADescribeDefinition
    {
        public abstract Exceptional<IEnumerable<Vertex>> GetResult(DBContext myDBContext);
    }

}

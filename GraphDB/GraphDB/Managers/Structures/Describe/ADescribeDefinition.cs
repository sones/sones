/*
 * ADescribeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System.Collections.Generic;
using sones.GraphDB.Structures.Result;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{

    public abstract class ADescribeDefinition
    {
        public abstract Exceptional<List<SelectionResultSet>> GetResult(DBContext myDBContext);
    }

}

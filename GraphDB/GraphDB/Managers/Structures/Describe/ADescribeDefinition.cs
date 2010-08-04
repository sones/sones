/*
 * ADescribeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{
    public abstract class ADescribeDefinition
    {

        public abstract Exceptional<List<SelectionResultSet>> GetResult(DBContext myDBContext);

    }
}

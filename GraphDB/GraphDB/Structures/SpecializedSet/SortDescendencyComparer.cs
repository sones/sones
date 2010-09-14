using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.BasicTypes;

namespace sones.GraphDB.Structures
{

    
    public class SortDescendencyComparer : IComparer<DBNumber>
    {
        #region IComparer<double> Members

        public int Compare(DBNumber x, DBNumber y)
        {
            return x.CompareTo(y) * -1;
        }

        #endregion
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Indices;

namespace sones.GraphDB.Errors
{
    public class Error_IndexIsNotPersistent : GraphDBIndexError
    {
        public IIndexObject<IndexKey, ObjectUUID> Index { get; private set; }

        public Error_IndexIsNotPersistent(IIndexObject<IndexKey, ObjectUUID> index)
        {
            Index = index;
        }

        public override string ToString()
        {
            return Index.IndexName;
        }
    }
}

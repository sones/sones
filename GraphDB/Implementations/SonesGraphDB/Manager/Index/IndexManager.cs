using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Manager.Index
{
    /// <summary>
    /// This class represents an index manager.
    /// </summary>
    /// The responsibilities of the index manager are creating, removing und retrieving of indices.
    /// Each database has one type manager.
    public class IndexManager : IIndexManager
    {
        public void CreateIndex(IIndexDefinition myIndexDefinition) 
        {
            throw new System.NotImplementedException();
        }
    }
}

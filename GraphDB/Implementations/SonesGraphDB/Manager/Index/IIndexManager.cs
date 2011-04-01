using System;

namespace sones.GraphDB.Manager.Index
{
    interface IIndexManager
    {
        void CreateIndex(sones.GraphDB.TypeSystem.IIndexDefinition myIndexDefinition);
    }
}

using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Manager.Index
{
    public interface IIndexManager
    {
        void CreateIndex(IIndexDefinition myIndexDefinition);
    }
}

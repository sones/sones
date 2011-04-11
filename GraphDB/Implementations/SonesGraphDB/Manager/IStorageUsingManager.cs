using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Manager.Index;
using sones.Library.VertexStore;

namespace sones.GraphDB.Manager.TypeManagement
{
    /// <summary>
    /// An interface that represents an type manager.
    /// </summary>
    /// The responsibilities of the type manager are creating, removing und retrieving of types.
    /// Each database has one type manager.
    public interface IStorageUsingManager 
    {
        /// <summary>
        /// Loads data from the underlying parentVertex store
        /// </summary>
        void Load(MetaManager myMetaManager);

        /// <summary>
        /// Creates the basic parentVertex type definitions.
        /// </summary>
        //TODO: here we get a VertexStore(no security, no transaction) and an IndexManager, so we can create the five base parentVertex types, that are used to store the type manager knowlegde.
        void Create(IIndexManager myIndexMgr, IVertexStore myVertexStore);
    }
}
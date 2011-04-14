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
        void Load(IMetaManager myMetaManager);

        /// <summary>
        /// Creates the basic parentVertex type definitions.
        /// </summary>
        void Create(IMetaManager myMetaManager);
    }
}
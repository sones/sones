using System;
using sones.Library.VersionedPluginManager;
using sones.Library.VertexStore;

namespace sones.Library.Transaction
{
    #region ITransactionManagerFSVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible ITransactionManager plugin versions. 
    /// Defines the min and max version for all ITransactionManager implementations which will be activated
    /// </summary>
    public static class ITransactionManagerVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
    }

    #endregion


    /// <summary>
    /// The interface for all transaction managers
    /// 
    /// ITransactionable : Begin/Commit/... transaction
    /// IVertexHandler   : Handle vertex interaction
    /// </summary>
    public interface ITransactionManager : ITransactionable, IVertexStore, IPluginable
    {
        //hab keine ahnung was hier rein muss
    }
}
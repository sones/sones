using System;
using sones.Library.ErrorHandling;
using sones.Library.VersionedPluginManager.ErrorHandling;

namespace sones.Library.VersionedPluginManager.ErrorHandling
{
    /// <summary>
    /// This exception occurs if a assembly could not be loaded due to a incompatible platform etc.
    /// </summary>
    public sealed class CouldNotLoadAssemblyException : APluginManagerException
    {
        #region data

        /// <summary>
        /// The path of the assembly file.
        /// </summary>
        public readonly String AssemblyFile;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new CouldNotLoadAssemblyException exception
        /// </summary>
        /// <param name="myAssemblyFile">The path of the assembly file</param>
        public CouldNotLoadAssemblyException(String myAssemblyFile)
        {
            AssemblyFile = myAssemblyFile;
        }

        #endregion
                        
    }
}
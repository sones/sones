using System;
using sones.Library.ErrorHandling;

namespace sones.VersionedPluginManager.ErrorHandling
{
    /// <summary>
    /// This exception occurs if a assembly could not be loaded due to a incompatible platform etc.
    /// </summary>
    public sealed class CouldNotLoadAssemblyException : ASonesException
    {
        #region data

        /// <summary>
        /// The path of the assembly file.
        /// </summary>
        public readonly String AssemblyFile;

        #endregion

        #region constructor

        public CouldNotLoadAssemblyException(String myAssemblyFile)
        {
            AssemblyFile = myAssemblyFile;
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.CouldNotLoadAssembly; }
        }

        #endregion
    }
}
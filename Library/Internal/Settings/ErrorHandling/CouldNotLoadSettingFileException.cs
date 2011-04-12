using System;

namespace sones.Library.Settings.ErrorHandling
{
    /// <summary>
    /// Could not load the setting file due to a FileNotFound or mailformed XML
    /// </summary>
    public sealed class CouldNotLoadSettingFileException : ASettingsException
    {
        #region Data

        /// <summary>
        /// An information about what went wrong
        /// </summary>
        public readonly String Info;

        #endregion

        #region constructor

        /// <summary>
        /// creates a new CouldNotLoadSettingFileException
        /// </summary>
        /// <param name="myInformation">An information about what went wrong</param>
        public CouldNotLoadSettingFileException(String myInformation)
        {
            Info = myInformation;
        }

        #endregion

        public override string ToString()
        {
            return Info;
        }
    }
}

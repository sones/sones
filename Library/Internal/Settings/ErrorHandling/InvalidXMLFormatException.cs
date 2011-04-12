using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Settings.ErrorHandling
{
    /// <summary>
    /// The XML of the setting is not well formed.
    /// </summary>
    public sealed class InvalidXMLFormatException : ASettingsException
    {
        #region Data

        /// <summary>
        /// An information about whats not well formed
        /// </summary>
        public readonly String Info;

        #endregion

        #region constructor

        /// <summary>
        /// creates a new InvalidXMLFormatException
        /// </summary>
        /// <param name="myInformation">An information about whats not well formed</param>
        public InvalidXMLFormatException(String myInformation)
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

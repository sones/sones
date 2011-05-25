using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace sones.Plugins.SonesGQL.Statements
{
    #region IGQLStatementPluginVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGQLStatementPlugin plugin versions. 
    /// Defines the min and max version for all IGQLStatementPlugin implementations which will be activated used this IGQLStatementPlugin.
    /// </summary>
    public static class IGQLStatementPluginVersionCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("2.0.0.0"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("2.0.0.0"); }
        }
    }

    #endregion

    /// <summary>
    /// The interface for all IGQLStatementPlugins
    /// </summary>
    public interface IGQLStatementPlugin
    {
        /// <summary>
        /// The name of the statement
        /// </summary>
        String StatementName { get; }

        /// <summary>
        /// The description of the statement
        /// </summary>
        String Description { get; }

        /// <summary>
        /// The grammar of the statement
        /// </summary>
        Grammar Grammar { get; }
    }
}

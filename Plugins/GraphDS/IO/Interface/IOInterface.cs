using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mime;
using sones.GraphQL.Result;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.GraphDS.IOInterface
{
    #region IOInterfaceCompatibility

    /// <summary>
    /// A static implementation of the compatible IOInterface plugin versions. 
    /// Defines the min and max version for all IOInterface implementations which will be activated used this IOInterface.
    /// </summary>
    public static class IOInterfaceCompatibility
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
    /// This is the interface for all IO methods, which can be used in GraphDS to generate an output result.
    /// </summary>
    public interface IOInterface : IPluginable
    {
        #region Content Type

        /// <summary>
        /// Returns the content type of the special format.
        /// </summary>
        ContentType ContentType { get; }

        #endregion

        #region Output Result

        /// <summary>
        /// Generates the representation of an query result in a special format.
        /// </summary>
        /// <param name="myQueryResult">The result of an query.</param>
        /// <returns>The representation of the result as string.</returns>
        String GenerateOutputResult(QueryResult myQueryResult);

        #endregion
        
        #region Query Result
        
        /// <summary>
        /// Generates an query result from a special respresentation.
        /// </summary>
        /// <param name="myResult">The query result as string in a special format.</param>
        /// <returns>An query result.</returns>
        QueryResult GenerateQueryResult(String myResult);

        #endregion
    }
}

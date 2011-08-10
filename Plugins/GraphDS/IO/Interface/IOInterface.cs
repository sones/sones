/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mime;
using sones.GraphQL.Result;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.GraphDS.IO
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

        #region Set Output Format Parameters

        /// <summary>
        /// Set parameters that influence output generation
        /// </summary>
        /// <param name="myResult">The query result as string in a special format.</param>
        void SetOutputFormatParameters(Dictionary<string, string> parameters);

        #endregion

    }
}

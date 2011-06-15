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
using sones.Library.VersionedPluginManager;
using sones.GraphDS;
using sones.GraphQL.Result;

namespace sones.Plugins.GraphDS
{
    #region IDrainPipeCompatibility

    /// <summary>
    /// A static implementation of the compatible IDrainPipe plugin versions. 
    /// Defines the min and max version for all IDrainPipe implementations which will be activated used this IDrainPipe.
    /// </summary>
    public static class IDrainPipeCompatibility
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

    public interface IDrainPipe : IGraphDS,IPluginable
    {
        /// <summary>
        /// this method is called AFTER the query was completed.
        /// </summary>
        /// <param name="result">the result of the query</param>
        void DrainQueryResult(QueryResult result);
    }
}

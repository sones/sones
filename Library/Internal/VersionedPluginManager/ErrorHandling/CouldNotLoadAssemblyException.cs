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

namespace sones.Library.VersionedPluginManager.ErrorHandling
{
    /// <summary>
    /// This exception occurs if an assembly could not be loaded due to an incompatible platform etc.
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
        public CouldNotLoadAssemblyException(String myAssemblyFile, Exception innerException = null) : base(innerException)
        {
            AssemblyFile = myAssemblyFile;
            _msg = "An assembly could not be loaded due to an incompatible platform ";
        }

        #endregion
                        
    }
}

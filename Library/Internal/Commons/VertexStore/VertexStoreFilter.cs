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
using sones.Library.PropertyHyperGraph;

namespace sones.Library.Commons.VertexStore
{
    /// <summary>
    /// Static filter class
    /// </summary>
    public static class VertexStoreFilter
    {
        /// <summary>
        /// A delegate to filter editions
        /// </summary>
        /// <param name="myEdition">The to be filtered edition</param>
        /// <returns>True or false</returns>
        public delegate bool EditionFilter(String myEdition);
        
        /// <summary>
        /// A delegate to filter revisions
        /// </summary>
        /// <param name="myRevisionID">The to be filtered revisions</param>
        /// <returns>True or false</returns>
        public delegate bool RevisionFilter(Int64 myRevisionID);
    }
}

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
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.Library.DataStructures;

namespace sones.GraphQL
{
    /// <summary>
    /// Marks a grammar as dump able
    /// </summary>
    public interface IDumpable
    {
        /// <summary>
        /// Export as GDDL (data definition language)
        /// </summary>
        /// <param name="myVertexTypesToDump">The vertex types to dump</param>
        /// <param name="myEdgeTypesToDump">The edge types to dump</param>
        /// <returns>A list of strings, containing the GDDL statements</returns>        
        IEnumerable<String> ExportGraphDDL(DumpFormats myDumpFormat, 
                                            IEnumerable<IVertexType> myVertexTypesToDump,
                                            IEnumerable<IEdgeType> myEdgeTypesToDump);

        /// <summary>
        /// Exports as GDML (data manipulation language)
        /// </summary>
        /// <param name="myVertexTypesToDump">The vertex types to dump</param>
        /// <returns>A list of strings, containing the GDML statments</returns>
        IEnumerable<String> ExportGraphDML(DumpFormats myDumpFormat,
                                            IEnumerable<IVertexType> myVertexTypesToDump,
                                            SecurityToken mySecurityToken, 
                                            Int64 myTransactionToken);
    }
}
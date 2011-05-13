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

using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Manager.TypeManagement
{
    public interface IEdgeTypeHandler: IManager
    {
        /// <summary>
        /// Gets an edge type by type id.
        /// </summary>
        /// <param name="myTypeId">The id of the edge type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An instance of IEdgeType, that represents the e type.</returns>
        IEdgeType GetEdgeType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Gets an edge type by name.
        /// </summary>
        /// <param name="myTypeName">The name of the e type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An instance of IEdgeType, that represents the e type.</returns>
        IEdgeType GetEdgeType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Gets all edge types.
        /// </summary>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An instance of IEdgeType, that represents the e type.</returns>
        IEnumerable<IEdgeType> GetAllEdgeTypes(TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Adds a bunch of new edge types to the edge type manager.
        /// </summary>
        /// <param name="myEdgeTypeDefinitions">The definitions of the new edge types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        IEdgeType AddEdgeType(IEnumerable<EdgeTypePredefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Removes a bunch of edge types from the edge type manager.
        /// </summary>
        /// <param name="myEdgeTypes">The e types that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// All types will be removed unless there are no edges that point to at least one of the given types.
        /// If there is such an IncomingEdge, remove the IncomingEdge by altering the type that holds it or remove this type too.
        /// All types are removed simultaneously. This means that edges between the types are not need to be removed before.
        void RemoveEdgeTypes(IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Updates existing edge types.
        /// </summary>
        /// <param name="myEdgeTypeDefinitions">TODO: for update use EdgeTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        void UpdateEdgeType(IEnumerable<EdgeTypePredefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity);
    }
}
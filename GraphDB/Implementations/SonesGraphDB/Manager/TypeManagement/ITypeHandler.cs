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
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Manager.TypeManagement
{
    public interface ITypeHandler<T>: IManager 
        where T : IBaseType 
    {
        /// <summary>
        /// Is there a type to a given name
        /// </summary>
        /// <param name="myTypeName">The name of the interesting  type</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <returns>True or false</returns>
        bool HasType(string myTypeName, 
                        SecurityToken mySecurityToken, 
                        TransactionToken myTransactionToken);

        /// <summary>
        /// Gets a type by id.
        /// </summary>
        /// <param name="myTypeId">The id of the  type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An instance of IType, that represents the  type.</returns>
        T GetType(long myTypeId, 
                    TransactionToken myTransaction, 
                    SecurityToken mySecurity);

        /// <summary>
        /// Gets a type by name.
        /// </summary>
        /// <param name="myTypeName">The name of the  type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An instance of IType, that represents the  type.</returns>
        T GetType(string myTypeName, 
                    TransactionToken myTransaction, 
                    SecurityToken mySecurity);

        /// <summary>
        /// Gets all  types.
        /// </summary>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An instance of IType, that represents the  type.</returns>
        IEnumerable<T> GetAllTypes(TransactionToken myTransaction, 
                                        SecurityToken mySecurity);

        /// <summary>
        /// Adds a bunch of  types to the  type manager.
        /// </summary>
        /// <param name="myTypeDefinitions">The definitions of the new  types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        IEnumerable<T> AddTypes(IEnumerable<ATypePredefinition> myTypeDefinitions, 
                                    TransactionToken myTransaction, 
                                    SecurityToken mySecurity);

        /// <summary>
        /// Removes a bunch of  types from the  type manager.
        /// </summary>
        /// <param name="myTypes">The  types that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// All types will be removed unless there are no edges that point to at least one of the given types.
        /// If there is such an IncomingEdge, remove the IncomingEdge by altering the type that holds it or remove this type too.
        /// All types are removed simultaneously. This means that edges between the types are not need to be removed before.
        Dictionary<Int64, String> RemoveTypes(IEnumerable<T> myTypes, 
                                                TransactionToken myTransaction, 
                                                SecurityToken mySecurity, 
                                                bool myIgnoreReprimands = false);

        /// <summary>
        /// Clears the graphDB and removes all user defined types.
        /// </summary>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// All types will be removed.
        /// All types are removed simultaneously. This means that edges between the types are not need to be removed before.
        IEnumerable<long> ClearTypes(TransactionToken myTransaction, 
                                        SecurityToken mySecurity);

        /// <summary>
        /// Removes all vertices of a given  type
        /// </summary>
        /// <param name="myTypeID">The  type id that should be truncated</param>
        /// <param name="myTransactionToken">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        void TruncateType(long myTypeID, 
                            TransactionToken myTransactionToken, 
                            SecurityToken mySecurityToken);

        /// <summary>
        /// Removes all vertices of a given  type
        /// </summary>
        /// <param name="myTypeName">The  type name that should be truncated</param>
        /// <param name="myTransactionToken">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        void TruncateType(string myTypeName, 
                            TransactionToken TransactionToken, 
                            SecurityToken SecurityToken);

        ///// <summary>
        ///// Alteres a certain  type
        ///// </summary>
        ///// <param name="myAlterTypeRequest">The alter  type request</param>
        ///// <param name="mySecurityToken">The current security token</param>
        ///// <param name="myTransactionToken">The current transaction token</param>
        //IBaseType AlterType(RequestAlterVertexType myAlterTypeRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        void CleanUpTypes();
    }
}

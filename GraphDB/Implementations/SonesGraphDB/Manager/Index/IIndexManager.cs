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

using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System.Collections;
using System;
using sones.Plugins.Index.Interfaces;
using System.Collections.Generic;
using sones.GraphDB.Request.CreateVertexTypes;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.Request;

namespace sones.GraphDB.Manager.Index
{
    /// <summary>
    /// The interface for all index manager.
    /// </summary>
    public interface IIndexManager : IManager
    {
        /// <summary>
        /// Removes the index instance from the index manager, but not the index representing vertex in the FS.
        /// </summary>
        /// <param name="myIndexID">The ID of the index.</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        void RemoveIndexInstance(long myIndexID, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Creates an index corresponding to a definition
        /// </summary>
        /// <param name="myIndexDefinition">The definition for the index</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        IIndexDefinition CreateIndex(IndexPredefinition myIndexDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken, bool myIsUserDefined = true);

        /// <summary>
        /// Determines if there are one or more indices for a given property
        /// </summary>
        /// <param name="myPropertyDefinition">The interesting property</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <returns>True or false</returns>
        bool HasIndex(IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myVertexType"></param>
        /// <param name="myPropertyDefinition"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        /// <returns></returns>
        IEnumerable<IIndex<IComparable, Int64>> GetIndices(IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myVertexType"></param>
        /// <param name="myPropertyDefinition"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        /// <returns></returns>
        IEnumerable<IIndex<IComparable, Int64>> GetIndices(IVertexType myVertexType, IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        /// <summary>
        /// Returns all indices
        /// </summary>
        /// <param name="myVertexType"></param>
        /// <param name="myPropertyDefinition"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        /// <returns></returns>
        IEnumerable<IIndex<IComparable, long>> GetIndices(IVertexType myVertexType, IList<IPropertyDefinition> myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        /// <summary>
        /// Returns the name of the index type, that matches the requirements.
        /// </summary>
        /// <param name="myIsSingleValue">If true, the index type must be a single value index otherwise a multi value index.</param>
        /// <param name="myIsRange">If true, the index type must support range queries otherwise not.</param>
        /// <param name="myIsVersioned">If true, the index type must support versioning otherwise not.</param>
        /// <returns></returns>
        String GetBestMatchingIndexName(bool myIsSingleValue, bool myIsRange, bool myIsVersioned);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myTypeName"></param>
        /// <param name="myIndexName"></param>
        /// <param name="myEdition"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        /// <returns></returns>
        IEnumerable<IIndexDefinition> DescribeIndex(String myTypeName, String myIndexName, String myEdition, TransactionToken myTransactionToken, SecurityToken mySecurityToken);

        /// <summary>
        /// Return all indices.
        /// </summary>
        /// <param name="myTransactionToken">The transaction token.</param>
        /// <param name="mySecurityToken">The security token.</param>
        /// <returns></returns>
        IEnumerable<IIndexDefinition> DescribeIndices(TransactionToken myTransactionToken, SecurityToken mySecurityToken);

        /// <summary>
        /// Rebuild the indices
        /// </summary>
        /// <param name="myVertexTypeID">The corresponding vertex type id</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        void RebuildIndices(long myVertexTypeID, TransactionToken myTransactionToken, SecurityToken mySecurityToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myIndexID"></param>
        ISingleValueIndex<IComparable, Int64> GetIndex(BaseUniqueIndex myIndex);

        /// <summary>
        /// Drops indices
        /// </summary>
        /// <param name="myDropIndexRequest">The drop index request</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        void DropIndex(RequestDropIndex myDropIndexRequest, TransactionToken myTransactionToken, SecurityToken mySecurityToken);

        IIndex<IComparable, Int64> GetIndex(string myIndexName, SecurityToken mySecurity, TransactionToken myTransaction);
    }
}

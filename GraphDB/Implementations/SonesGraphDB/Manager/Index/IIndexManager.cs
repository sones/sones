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
using sones.GraphDB.Request;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Plugins.Index;

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
        void RemoveIndexInstance(long myIndexID, Int64 myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Creates an index corresponding to a definition
        /// </summary>
        /// <param name="myIndexDefinition">The definition for the index</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        IIndexDefinition CreateIndex(IndexPredefinition myIndexDefinition, SecurityToken mySecurityToken, Int64 myTransactionToken, bool myIsUserDefined = true);

        /// <summary>
        /// Determines if there are one or more indices for a given property
        /// </summary>
        /// <param name="myPropertyDefinition">The interesting property</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <returns>True or false</returns>
        bool HasIndex(IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, Int64 myTransactionToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myVertexType"></param>
        /// <param name="myPropertyDefinition"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        /// <returns></returns>
        IEnumerable<ISonesIndex> GetIndices(IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, Int64 myTransactionToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myVertexType"></param>
        /// <param name="myPropertyDefinition"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        /// <returns></returns>
        IEnumerable<ISonesIndex> GetIndices(IVertexType myVertexType, IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, Int64 myTransactionToken);

        /// <summary>
        /// Returns all indices
        /// </summary>
        /// <param name="myVertexType"></param>
        /// <param name="myPropertyDefinition"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        /// <returns></returns>
        IEnumerable<ISonesIndex> GetIndices(IVertexType myVertexType, IList<IPropertyDefinition> myPropertyDefinition, SecurityToken mySecurityToken, Int64 myTransactionToken);

        /// <summary>
        /// Returns the name of the index type, that matches the requirements.
        /// </summary>
        /// <param name="myIsRange">If true, the index type must support range queries otherwise not.</param>
        /// <param name="myIsVersioned">If true, the index type must support versioning otherwise not.</param>
        /// <returns></returns>
        String GetBestMatchingIndexName(bool myIsRange, bool myIsVersioned);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myTypeName"></param>
        /// <param name="myIndexName"></param>
        /// <param name="myEdition"></param>
        /// <param name="myTransactionToken"></param>
        /// <param name="mySecurityToken"></param>
        /// <returns></returns>
        IEnumerable<IIndexDefinition> DescribeIndex(String myTypeName, String myIndexName, String myEdition, Int64 myTransactionToken, SecurityToken mySecurityToken);

        /// <summary>
        /// Return all indices.
        /// </summary>
        /// <param name="myTransactionToken">The transaction token.</param>
        /// <param name="mySecurityToken">The security token.</param>
        /// <returns></returns>
        IEnumerable<IIndexDefinition> DescribeIndices(Int64 myTransactionToken, SecurityToken mySecurityToken);

        /// <summary>
        /// Rebuild the indices
        /// </summary>
        /// <param name="myVertexTypeID">The corresponding vertex type id</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        void RebuildIndices(long myVertexTypeID, Int64 myTransactionToken, SecurityToken mySecurityToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myIndexID"></param>
        ISonesIndex GetIndex(BaseUniqueIndex myIndex);

        /// <summary>
        /// Drops indices
        /// </summary>
        /// <param name="myDropIndexRequest">The drop index request</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <param name="mySecurityToken">The current security token</param>
        void DropIndex(RequestDropIndex myDropIndexRequest, Int64 myTransactionToken, SecurityToken mySecurityToken);

        ISonesIndex GetIndex(string myIndexName, SecurityToken mySecurity, Int64 myTransaction);
    }
}

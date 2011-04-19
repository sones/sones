using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System.Collections;
using System;
using sones.Plugins.Index.Interfaces;
using System.Collections.Generic;

namespace sones.GraphDB.Manager.Index
{
    /// <summary>
    /// The interface for all index manager.
    /// </summary>
    public interface IIndexManager
    {
        /// <summary>
        /// Creates an index corresponding to a definition
        /// </summary>
        /// <param name="myIndexDefinition">The definition for the index</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        void CreateIndex(IIndexDefinition myIndexDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        /// <summary>
        /// Determines if there are one or more indices for a given property
        /// </summary>
        /// <param name="myVertexType">The vertex type that corresponds to the property</param>
        /// <param name="myPropertyDefinition">The interesting property</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        /// <returns>True or false</returns>
        bool HasIndex(IVertexType myVertexType, IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myVertexType"></param>
        /// <param name="myPropertyDefinition"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        /// <returns></returns>
        IEnumerable<IIndex<IComparable, Int64>> GetIndices(IVertexType myVertexType, IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken);
    }
}

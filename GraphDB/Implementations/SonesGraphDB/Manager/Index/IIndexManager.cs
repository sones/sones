using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System.Collections;
using System;
using sones.Plugins.Index.Interfaces;
using System.Collections.Generic;
using sones.GraphDB.Request.CreateVertexTypes;

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
        /// Returns all indices
        /// </summary>
        /// <param name="myPropertyDefinition"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        /// <returns></returns>
        IIndex<IComparable, Int64> GetIndex(IVertexType myVertexType, IList<IPropertyDefinition> myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

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

    }
}

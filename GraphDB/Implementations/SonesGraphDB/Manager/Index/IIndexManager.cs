using sones.GraphDB.TypeSystem;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDB.Manager.Index
{
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
    }
}

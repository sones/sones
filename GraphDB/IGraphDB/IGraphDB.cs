using sones.GraphDB.Request;
using sones.GraphDB.Security;
using sones.GraphDB.Transaction;

namespace sones.GraphDB
{
    /// <summary>
    /// The interface for all graphdb implementations
    /// </summary>
    public interface IGraphDB : ITransactionable
    {
        /// <summary>
        /// Creates a new type of vertex
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestCreateVertexType">The create vertex type request</param>
        /// <returns>A generic result</returns>
        TResult CreateVertexType<TResult>(  SecurityToken mySecurityToken,
                                            TransactionToken myTransactionToken,
                                            RequestCreateVertexType<TResult> myRequestCreateVertexType);

        /// <summary>
        /// Clears the graphdb entirely
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestClear">The clear request
        /// <returns>A generic Result</returns>
        TResult Clear<TResult>(             SecurityToken mySecurityToken,
                                            TransactionToken myTransactionToken,
                                            RequestClear<TResult> myRequestClear);

    }
}

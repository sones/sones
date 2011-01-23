using System;
using sones.GraphDB.Request;
using sones.Library.Internal.Token;

namespace sones.GraphDB.Session
{
    /// <summary>
    /// The interface for all sessions for the graphdb implementations
    /// </summary>
    public interface IGraphDBSession
    {
        /// <summary>
        /// Creates a new type of vertex
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="myRequestCreateVertexType">The create vertex type request</param>
        /// <param name="myTransactionToken">An optional transaction token</param>
        /// <returns>A generic result</returns>
        TResult CreateVertexType<TResult>(RequestCreateVertexType<TResult> myRequestCreateVertexType, TransactionToken myTransactionToken = null);

        /// <summary>
        /// Clears the graphdb entirely
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="myRequestClear">The clear request
        /// <param name="myTransactionToken">An optional transaction token</param>
        /// <returns>A Generic Result</returns>
        TResult Clear<TResult>(RequestClear<TResult> myRequestClear, TransactionToken myTransactionToken = null);
    }
}

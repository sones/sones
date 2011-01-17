using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Internal.Token;
using sones.GraphDB.Request;
using sones.GraphDB.Context;
using sones.Library.Internal.Security;

namespace sones.GraphDB
{
    /// <summary>
    /// The interface for all graphdb implementations
    /// </summary>
    public interface IGraphDB
    {
        #region internal methods
        //Those methods are internal. Please use the session

        /// <summary>
        /// Creates a new type of vertex
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySessionToken">The current session token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestCreateVertexType">The create vertex type request</param>
        /// <returns>A generic result</returns>
        TResult CreateVertexType<TResult>(  SessionToken mySessionToken,
                                            TransactionToken myTransactionToken,
                                            RequestCreateVertexType<TResult> myRequestCreateVertexType);

        /// <summary>
        /// Clears the graphdb entirely
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySessionToken">The current session token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestClear">The clear request
        /// <returns>A generic Result</returns>
        TResult Clear<TResult>(             SessionToken mySessionToken,
                                            TransactionToken myTransactionToken,
                                            RequestClear<TResult> myRequestClear);

        #endregion

        /// <summary>
        /// Returns a session for the graphdb
        /// </summary>
        /// <param name="myCredentials">The credentials that are going to be authorized</param>
        /// <returns>An IGraphDBSession</returns>
        IGraphDBSession GetSession(Credentials myCredentials);
    }
}

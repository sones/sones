using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.Request.Delete;

namespace sones.GraphDB
{
    /// <summary>
    /// The interface for all read-write requests for the GraphDB
    /// </summary>
    public interface IReadWriteGraphDB : IReadOnlyGraphDB
    {
        #region requests

        /// <summary>
        /// Creates new vertex types
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestCreateVertexTypes">The create vertex types request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult CreateVertexTypes<TResult>(SecurityToken mySecurityToken,
                                          TransactionToken myTransactionToken,
                                          RequestCreateVertexTypes myRequestCreateVertexTypes,
                                          Converter.CreateVertexTypesResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Creates a new vertex type
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestCreateVertexType">The create vertex type request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult CreateVertexType<TResult>(SecurityToken mySecurityToken,
                                          TransactionToken myTransactionToken,
                                          RequestCreateVertexType myRequestCreateVertexType,
                                          Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Clears the graphdb entirely
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestClear">The clear request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic Result</returns>
        TResult Clear<TResult>(SecurityToken mySecurityToken,
                               TransactionToken myTransactionToken,
                               RequestClear myRequestClear,
                               Converter.ClearResultConverter<TResult> myOutputconverter);

        TResult Delete<TResult>(SecurityToken mySecurityToken,
                               TransactionToken myTransactionToken,
                               RequestDelete myRequestClear,
                               Converter.DeleteResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Inserts a new vertex
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestInsert">The insert vertex request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult Insert<TResult>(SecurityToken mySecurityToken,
                                TransactionToken myTransactionToken,
                                RequestInsertVertex myRequestInsert,
                                Converter.InsertResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestInsert">The insert vertex request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult Truncate<TResult>(SecurityToken mySecurityToken,
                                    TransactionToken myTransactionToken,
                                    RequestTruncate myRequestTruncate,
                                    Converter.TruncateResultConverter<TResult> myOutputconverter);

        #endregion
    }
}

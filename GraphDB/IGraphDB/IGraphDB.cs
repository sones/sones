using System;
using sones.GraphDB.Request;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDB
{
    #region IGraphDBVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphDB plugin versions. 
    /// Defines the min and max version for all IGraphDB implementations which will be activated used this IGraphDB.
    /// </summary>
    internal static class IGraphDBVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
    }

    #endregion

    /// <summary>
    /// The interface for all graphdb implementations
    /// </summary>
    public interface IGraphDB : ITransactionable, IUserAuthentication
    {
        #region requests

        /// <summary>
        /// Creates a new type of vertex
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestCreateVertexType">The create vertex type request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult CreateVertexType<TResult>(SecurityToken mySecurityToken,
                                          TransactionToken myTransactionToken,
                                          RequestCreateVertexTypes myRequestCreateVertexType,
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
        /// Gets vertices from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestGetVertices">The get vertices request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetVertices<TResult>(SecurityToken mySecurityToken,
                                     TransactionToken myTransactionToken,
                                     RequestGetVertices myRequestGetVertices,
                                     Converter.GetVerticesResultConverter<TResult> myOutputconverter);

        #endregion

        #region misc

        /// <summary>
        /// The id of the graph database
        /// </summary>
        Guid ID { get; }

        #endregion
    }
}
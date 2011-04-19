using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Request;

namespace sones.GraphDB
{
    /// <summary>
    /// The interface for all read-only requests for the GraphDB
    /// </summary>
    public interface IReadOnlyGraphDB
    {
        #region requests

        /// <summary>
        /// Gets vertices from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestGetVertices">The get vertices request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetVertices<TResult>(   SecurityToken mySecurityToken,
                                        TransactionToken myTransactionToken,
                                        RequestGetVertices myRequestGetVertices,
                                        Converter.GetVerticesResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Traverses the graphdb an searches for verticies, wich fulfil the matching conditions
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestGetVertices">The traverse vertex request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult TraverseVertex<TResult>(SecurityToken mySecurity,
                                        TransactionToken myTransactionToken,
                                        RequestTraverseVertex myRequestTraverseVertex,
                                        Converter.TraverseVertexResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Get a vertex type from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestGetVertexType">The request to get a vertex type</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetVertexType<TResult>( SecurityToken mySecurityToken,
                                        TransactionToken myTransactionToken,
                                        RequestGetVertexType myRequestGetVertexType,
                                        Converter.GetVertexTypeResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Get a edge type from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestGetEdgeType">The request to get an edge type</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetEdgeType<TResult>(   SecurityToken mySecurityToken,
                                        TransactionToken myTransactionToken,
                                        RequestGetEdgeType myRequestGetEdgeType,
                                        Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Get a vertex from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestGetVertex">The request to get a vertex</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetVertex<TResult>(     SecurityToken mySecurityToken,
                                        TransactionToken myTransactionToken,
                                        RequestGetVertex myRequestGetVertex,
                                        Converter.GetVertexResultConverter<TResult> myOutputconverter);


        #endregion
    }
}

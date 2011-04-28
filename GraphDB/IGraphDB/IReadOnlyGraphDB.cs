using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Request;
using sones.GraphDB.Request.GetEdgeType;
using sones.GraphDB.Request.GetVertexType;
using sones.GraphDB.Request.GetIndex;

namespace sones.GraphDB
{
    /// <summary>
    /// The interface for all read-only requests for the GraphDB
    /// </summary>
    public interface IReadOnlyGraphDB
    {
        #region requests

        #region GetVertex

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

        #region GetVertices

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

        #endregion

        #region TraverseVertex

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

        #endregion

        #region GetVertexType

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
        /// Get a vertex type from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestGetAllVertexTypes">The request to get all vertex types</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetAllVertexTypes<TResult>(SecurityToken mySecurityToken,
                                            TransactionToken myTransactionToken,
                                            RequestGetAllVertexTypes myRequestGetAllVertexTypes,
                                            Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter);

        #endregion

        #region GetEdgeType

        /// <summary>
        /// Get a edge type from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestGetAllEdgeTypes">The request to get the edge types</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetEdgeType<TResult>(   SecurityToken mySecurityToken,
                                        TransactionToken myTransactionToken,
                                        RequestGetEdgeType myRequestGetEdgeType,
                                        Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Get a edge type from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestGetAllEdgeTypes">The request to get all edge types</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetAllEdgeTypes<TResult>(SecurityToken mySecurityToken,
                                            TransactionToken myTransactionToken,
                                            RequestGetAllEdgeTypes myRequestGetAllEdgeTypes,
                                            Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter);

        TResult DescribeIndex<TResult>(SecurityToken mySecurityToken,
                                            TransactionToken myTransactionToken,
                                            RequestDescribeIndex myRequestGetAllEdgeTypes,
                                            Converter.DescribeIndexResultConverter<TResult> myOutputconverter);
        
        #endregion

        #endregion
    }
}

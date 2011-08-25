/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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

        #region GetVertex

        /// <summary>
        /// Get a vertex from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myRequestGetVertex">The request to get a vertex</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetVertex<TResult>(     SecurityToken mySecurityToken,
                                        Int64 myTransactionID,
                                        RequestGetVertex myRequestGetVertex,
                                        Converter.GetVertexResultConverter<TResult> myOutputconverter);

        #endregion

        #region GetVertices

        /// <summary>
        /// Gets vertices from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myRequestGetVertices">The get vertices request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetVertices<TResult>(   SecurityToken mySecurityToken,
                                        Int64 myTransactionID,
                                        RequestGetVertices myRequestGetVertices,
                                        Converter.GetVerticesResultConverter<TResult> myOutputconverter);

        #endregion

        #region TraverseVertex

        /// <summary>
        /// Traverses the graphdb an searches for verticies, wich fulfil the matching conditions
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myRequestGetVertices">The traverse vertex request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult TraverseVertex<TResult>(SecurityToken mySecurity,
                                        Int64 myTransactionID,
                                        RequestTraverseVertex myRequestTraverseVertex,
                                        Converter.TraverseVertexResultConverter<TResult> myOutputconverter);

        #endregion

        #region GetVertexType

        /// <summary>
        /// Get a vertex type from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myRequestGetVertexType">The request to get a vertex type</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetVertexType<TResult>( SecurityToken mySecurityToken,
                                        Int64 myTransactionID,
                                        RequestGetVertexType myRequestGetVertexType,
                                        Converter.GetVertexTypeResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Get a vertex type from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myRequestGetAllVertexTypes">The request to get all vertex types</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetAllVertexTypes<TResult>(SecurityToken mySecurityToken,
                                            Int64 myTransactionID,
                                            RequestGetAllVertexTypes myRequestGetAllVertexTypes,
                                            Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter);

        #endregion

        #region GetEdgeType

        /// <summary>
        /// Get a edge type from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myRequestGetAllEdgeTypes">The request to get the edge types</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetEdgeType<TResult>(   SecurityToken mySecurityToken,
                                        Int64 myTransactionID,
                                        RequestGetEdgeType myRequestGetEdgeType,
                                        Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Get a edge type from the graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myRequestGetAllEdgeTypes">The request to get all edge types</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetAllEdgeTypes<TResult>(SecurityToken mySecurityToken,
                                            Int64 myTransactionID,
                                            RequestGetAllEdgeTypes myRequestGetAllEdgeTypes,
                                            Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Gets the index definition
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myRequestDescribeIndex">The request to get the index definitions</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult DescribeIndex<TResult>(     SecurityToken mySecurityToken,
                                            Int64 myTransactionID,
                                            RequestDescribeIndex myRequestDescribeIndex,
                                            Converter.DescribeIndexResultConverter<TResult> myOutputconverter);

        
        /// <summary>
        /// Gets the index definitions of all types.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myRequestDescribeIndex">The request to get the index definitions</param>
        /// <param name="myTransactionID">The current transaction id</param>        
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>        
        TResult DescribeIndices<TResult>(   SecurityToken mySecurityToken,
                                            Int64 myTransactionID,
                                            RequestDescribeIndex myRequestDescribeIndex,
                                            Converter.DescribeIndicesResultConverter<TResult> myOutputconverter);
        
        #endregion

        #region GetVertexCount

        /// <summary>
        /// Returns the count of vertices corresponding to a vertex type
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myRequestGetVertexCount">The request to get the vertex count</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult GetVertexCount<TResult>(SecurityToken mySecurityToken,
                                        Int64 myTransactionID,
                                        RequestGetVertexCount myRequestGetVertexCount,
                                        Converter.GetVertexCountResultConverter<TResult> myOutputconverter);

        #endregion

        #endregion
    }
}

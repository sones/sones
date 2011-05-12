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
using sones.GraphDB.Request;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;

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
        /// Alteres a vertex type
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestCreateVertexType">The alter vertex type request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult AlterVertexType<TResult>( SecurityToken mySecurityToken,
                                          TransactionToken myTransactionToken,
                                          RequestAlterVertexType myRequestAlterVertexType,
                                          Converter.AlterVertexTypeResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Creates a new edge type
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestCreateVertexType">The create edge type request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult CreateEdgeType<TResult>(  SecurityToken mySecurityToken,
                                          TransactionToken myTransactionToken,
                                          RequestCreateEdgeType myRequestCreateVertexType,
                                          Converter.CreateEdgeTypeResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Alteres a edge type
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestCreateVertexType">The alter edge type request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult AlterEdgeType<TResult>( SecurityToken mySecurityToken,
                                        TransactionToken myTransactionToken,
                                        RequestAlterEdgeType myRequestAlterEdgeType,
                                        Converter.AlterEdgeTypeResultConverter<TResult> myOutputconverter);


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
        /// Deletes the given type from graphdb
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestDelete">The delete request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic Result</returns>
        TResult Delete<TResult>(SecurityToken mySecurityToken,
                                TransactionToken myTransactionToken,
                                RequestDelete myRequestDelete,
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
        /// Truncate a given type
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestTruncate">The truncate vertex request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult Truncate<TResult>(SecurityToken mySecurityToken,
                                    TransactionToken myTransactionToken,
                                    RequestTruncate myRequestTruncate,
                                    Converter.TruncateResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Updates a given type
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestUpdate">The update request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult Update<TResult>(SecurityToken mySecurityToken,
                                TransactionToken myTransactionToken,
                                RequestUpdate myRequestUpdate,
                                Converter.UpdateResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Drops a type and all dbobjects of this type
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="RequestDropType">The drop vertex type request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult DropType<TResult>(  SecurityToken mySecurityToken,
                                    TransactionToken myTransactionToken,
                                    RequestDropVertexType myRequestDropType,
                                    Converter.DropVertexTypeResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Drops a index on type
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="RequestDropIndex">The drop index request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult DropIndex<TResult>(SecurityToken mySecurityToken,
                                    TransactionToken myTransactionToken,
                                    RequestDropIndex myRequestDropIndex,
                                    Converter.DropIndexResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Creates a index on type
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestCreateIndex">The create index request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult CreateIndex<TResult>(SecurityToken mySecurityToken,
                                        TransactionToken myTransactionToken,
                                        RequestCreateIndex myRequestCreateIndex,
                                        Converter.CreateIndexResultConverter<TResult> myOutputconverter);

        /// <summary>
        /// Rebuilds indeces of given types
        /// </summary>
        /// <typeparam name="TResult">The type of the result of this request</typeparam>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myRequestRebuildIndices">The create index request</param>
        /// <param name="myOutputconverter">A function to convert the output into the desired type</param>
        /// <returns>A generic result</returns>
        TResult RebuildIndices<TResult>(SecurityToken mySecurityToken,
                                        TransactionToken myTransactionToken,
                                        RequestRebuildIndices myRequestRebuildIndices,
                                        Converter.RebuildIndicesResultConverter<TResult> myOutputconverter);

        #endregion
    }
}

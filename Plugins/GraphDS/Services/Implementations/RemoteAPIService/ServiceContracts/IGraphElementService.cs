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
using System.ServiceModel;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;
using sones.Library.Commons.Security;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContracts
{
    /// <summary>
    /// The interface for graph elements like vertices or edges
    /// </summary>
    [ServiceContract(Namespace = sonesRPCServer.Namespace)]
    public interface IGraphElementService
    {
        #region Properties

        /// <summary>
        /// Returns the property of a graph element.
        /// </summary>
        /// <typeparam name="T">The type of the interesting property</typeparam>
        /// <param name="myPropertyID">The ID of the interesing property</param>
        /// <returns>A Property</returns>
        [OperationContract]
        object GetProperty(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex, Int64 myPropertyID);
                

        /// <summary>
        /// Checks whether the graph element is in possession of a certain property
        /// </summary>
        /// <param name="myPropertyID">The ID of the property</param>
        /// <returns>True if the property exists, otherwise false</returns>
        [OperationContract]
        bool HasProperty(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex, Int64 myPropertyID);

        /// <summary>
        /// Returns the count of the vertex properties
        /// </summary>
        /// <returns>An unsigned value</returns>
        [OperationContract]
        int GetCountOfProperties(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Returns all properties
        /// </summary>
        /// <param name="myFilter">A function to filter properties</param>
        /// <returns>An IEnumerable of Property/Value</returns>
        [OperationContract]
        List<Tuple<Int64, object>> GetAllProperties(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Returns a property as string
        /// </summary>
        /// <param name="myPropertyID">The id of the property</param>
        /// <returns>The string representation of the property</returns>
        [OperationContract]
        String GetPropertyAsString(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex, Int64 myPropertyID);

        #endregion

        #region Unstructured data/properties

        /// <summary>
        /// Gets unstructured data of the graph element
        /// </summary>
        /// <typeparam name="T">The type of the interesting property</typeparam>
        /// <param name="myPropertyName">The name of the interesting unstructured property</param>
        /// <returns>The value of an unstructured property</returns>
        [OperationContract]
        object GetUnstructuredProperty(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex, string myPropertyName);

        /// <summary>
        /// Checks whether the graph element is in possession of a certain unstructered property
        /// </summary>
        /// <param name="myPropertyName">The name of the unstructured property</param>
        /// <returns>True if the property exists, otherwise false</returns>
        [OperationContract]
        bool HasUnstructuredProperty(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex, String myPropertyName);

        /// <summary>
        /// Returns the count of the unstructured vertex properties
        /// </summary>
        /// <returns>An unsigned value</returns>
        [OperationContract]
        int GetCountOfUnstructuredProperties(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Returns all unstructured properties
        /// </summary>
        /// <param name="myFilter">A function to filter properties</param>
        /// <returns>An IEnumerable of NameOfProperty/Value</returns>
        [OperationContract]
        List<Tuple<String, Object>> GetAllUnstructuredProperties(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Returns an unstructured property as string
        /// </summary>
        /// <param name="myPropertyName">The name of the unstructured property</param>
        /// <returns>The string representation of the property</returns>
        [OperationContract]
        String GetUnstructuredPropertyAsString(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex, String myPropertyName);

        #endregion

        #region Comment

        /// <summary>
        /// Gets the comment of this graph element
        /// </summary>
        [OperationContract]
        String Comment(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex);

        #endregion

        #region Creation date

        /// <summary>
        /// The date the graph element has been created
        /// </summary>
        [OperationContract]
        long CreationDate(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex);

        #endregion

        #region Modification date

        /// <summary>
        /// The date the graph element has been modified the last time
        /// </summary>
        [OperationContract]
        long ModificationDate(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexInstance myVertex);

        #endregion
    }
}

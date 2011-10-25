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
using sones.GraphDS.Services.RemoteAPIService.DataContracts;
using System.IO;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using sones.Library.Commons.Security;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.PayloadObjects;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexInstanceService
{
    [ServiceContract(Namespace = sonesRPCServer.Namespace, Name = "VertexInstanceService")]
    public interface IVertexService
    {

        #region Edges

        #region Incoming

        /// <summary>
        /// Are there incoming vertices on this vertex?
        /// </summary>
        /// <param name="myVertexTypeID">The id of the vertex type that defines the edge</param>
        /// <param name="myEdgePropertyID">The property id of the interesting edge</param>
        /// <returns>True if there are incoming vertices, otherwise false</returns>
        [OperationContract]
        Boolean HasIncomingVertices(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex, Int64 myVertexTypeID, Int64 myEdgePropertyID);

        /// <summary>
        /// Returns all incoming vertices
        /// </summary>
        /// <param name="myFilter">A function to filter those incoming edges (VertexTypeID, EdgeID, ISingleEdges, Bool)</param>
        /// <returns>An IEnumerable of incoming edges</returns>
        [OperationContract]
        List<Tuple<Int64, Int64, List<ServiceVertexInstance>>> GetAllIncomingVertices(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Return all incoming vertices
        /// </summary>
        /// <param name="myVertexTypeID">The vertex type that points to this IVertex</param>
        /// <param name="myEdgePropertyID">The edge property id that points to this vertex</param>
        /// <returns>All incoming vertices</returns>
        [OperationContract]
        List<ServiceVertexInstance> GetIncomingVertices(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex, Int64 myVertexTypeID, Int64 myEdgePropertyID);

        #endregion

        #region Outgoing

        /// <summary>
        /// Is there a specified outgoing edge?
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the interesting edge</param>
        /// <returns>True if there is a specified edge, otherwise false</returns>
        [OperationContract(Name="HasOutgoingEdgeByVertexInstance")]
        Boolean HasOutgoingEdge(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex, Int64 myEdgePropertyID);

        /// <summary>
        /// Returns all outgoing edges
        /// </summary>
        /// <param name="myFilter">A function to filter those edges (EdgeID, IEdge, Bool)</param>
        /// <returns>An IEnumerable of all outgoing edges</returns>
        [OperationContract]
        List<ServiceEdgeInstance> GetAllOutgoingEdges(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Returns all outgoing hyper edges
        /// </summary>
        /// <param name="myFilter">A function to filter those edges (EdgeID, IHyperEdge, Bool)</param>
        /// <returns>An IEnumerable of propertyID/hyper edge KVP</returns>
        [OperationContract]
        List<ServiceHyperEdgeInstance> GetAllOutgoingHyperEdges(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Returns all outgoing single edges
        /// </summary>
        /// <param name="myFilter">A function to filter those edges (EdgeID, ISingleEdge, Bool)</param>
        /// <returns>An IEnumerable of all single edges</returns>
        [OperationContract]
        List<ServiceSingleEdgeInstance> GetAllOutgoingSingleEdges(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Returns a specified edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>An IEdge</returns>
        [OperationContract]
        ServiceEdgeInstance GetOutgoingEdge(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex, Int64 myEdgePropertyID);

        /// <summary>
        /// Returns a specified hyper edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>A hyper edge</returns>
        [OperationContract]
        ServiceHyperEdgeInstance GetOutgoingHyperEdge(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex, Int64 myEdgePropertyID);

        /// <summary>
        /// Get a specified single edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>A single edge</returns>
        [OperationContract]
        ServiceSingleEdgeInstance GetOutgoingSingleEdge(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex, Int64 myEdgePropertyID);

        #endregion

        #endregion


        #region Properties

        /// <summary>
        /// Returns the property of a graph element.
        /// </summary>
        /// <typeparam name="T">The type of the interesting property</typeparam>
        /// <param name="myPropertyID">The ID of the interesing property</param>
        /// <returns>A Property</returns>
        [OperationContract(Name = "GetPropertyByVertexInstance")]
        object GetProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myGraphElement, Int64 myPropertyID);


        /// <summary>
        /// Checks whether the graph element is in possession of a certain property
        /// </summary>
        /// <param name="myPropertyID">The ID of the property</param>
        /// <returns>True if the property exists, otherwise false</returns>
        [OperationContract(Name = "HasPropertyByVertexInstance")]
        bool HasProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex, long myPropertyID);

        /// <summary>
        /// Returns the count of the vertex properties
        /// </summary>
        /// <returns>An unsigned value</returns>
        [OperationContract(Name = "GetCountOfPropertiesByVertexInstance")]
        int GetCountOfProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myGraphElement);

        /// <summary>
        /// Returns all properties
        /// </summary>
        /// <param name="myFilter">A function to filter properties</param>
        /// <returns>An IEnumerable of Property/Value</returns>
        [OperationContract(Name = "GetAllPropertiesByVertexInstance")]
        List<Tuple<Int64, object>> GetAllProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myGraphElement);

        /// <summary>
        /// Returns a property as string
        /// </summary>
        /// <param name="myPropertyID">The id of the property</param>
        /// <returns>The string representation of the property</returns>
        [OperationContract(Name = "GetPropertyAsStringByVertexInstance")]
        String GetPropertyAsString(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myGraphElement, Int64 myPropertyID);

        #endregion

        #region Unstructured data/properties

        /// <summary>
        /// Gets unstructured data of the graph element
        /// </summary>
        /// <typeparam name="T">The type of the interesting property</typeparam>
        /// <param name="myPropertyName">The name of the interesting unstructured property</param>
        /// <returns>The value of an unstructured property</returns>
        [OperationContract(Name = "GetUnstructuredPropertyByVertexInstance")]
        object GetUnstructuredProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myGraphElement, string myPropertyName);

        /// <summary>
        /// Checks whether the graph element is in possession of a certain unstructered property
        /// </summary>
        /// <param name="myPropertyName">The name of the unstructured property</param>
        /// <returns>True if the property exists, otherwise false</returns>
        [OperationContract(Name = "HasUnstructuredPropertyByVertexInstance")]
        bool HasUnstructuredProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myGraphElement, String myPropertyName);

        /// <summary>
        /// Returns the count of the unstructured vertex properties
        /// </summary>
        /// <returns>An unsigned value</returns>
        [OperationContract(Name = "GetCountOfUnstructuredPropertiesByVertexInstance")]
        int GetCountOfUnstructuredProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myGraphElement);

        /// <summary>
        /// Returns all unstructured properties
        /// </summary>
        /// <param name="myFilter">A function to filter properties</param>
        /// <returns>An IEnumerable of NameOfProperty/Value</returns>
        [OperationContract(Name = "GetAllUnstructuredPropertiesByVertexInstance")]
        List<Tuple<String, Object>> GetAllUnstructuredProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myGraphElement);

        /// <summary>
        /// Returns an unstructured property as string
        /// </summary>
        /// <param name="myPropertyName">The name of the unstructured property</param>
        /// <returns>The string representation of the property</returns>
        [OperationContract(Name = "GetUnstructuredPropertyAsStringByVertexInstance")]
        String GetUnstructuredPropertyAsString(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myGraphElement, String myPropertyName);

        #endregion

        #region Comment

        /// <summary>
        /// Gets the comment of this graph element
        /// </summary>
        [OperationContract(Name="CommentByVertexInstance")]
        String Comment(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myGraphElement);

        #endregion

        #region Creation date

        /// <summary>
        /// The date the graph element has been created
        /// </summary>
        [OperationContract(Name="CreationDateByVertexInstance")]
        long CreationDate(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myGraphElement);

        #endregion

        #region Modification date

        /// <summary>
        /// The date the graph element has been modified the last time
        /// </summary>
        [OperationContract(Name="ModificationDateByVertexInstance")]
        long ModificationDate(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex);

        #endregion

        #region Statistics

        [OperationContract]
        ServiceVertexStatistics VertexStatistics(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex);

        #endregion

        #region PartitionID

        [OperationContract]
        Int64 PartitionID(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex);

        #endregion
    }
}

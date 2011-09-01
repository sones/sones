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

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexInstanceService
{
    [ServiceContract(Namespace = sonesRPCServer.Namespace)]
    public interface IVertexService : IGraphElementService
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
        Boolean HasIncomingVertices(SecurityToken mySecToken, ServiceTransactionToken myTransToken, Int64 myVertexTypeID, ServiceVertexInstance myVertex, Int64 myEdgePropertyID);

        /// <summary>
        /// Returns all incoming vertices
        /// </summary>
        /// <param name="myFilter">A function to filter those incoming edges (VertexTypeID, EdgeID, ISingleEdges, Bool)</param>
        /// <returns>An IEnumerable of incoming edges</returns>
        [OperationContract]
        List<Tuple<Int64, Int64, List<ServiceVertexInstance>>> GetAllIncomingVertices(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Return all incoming vertices
        /// </summary>
        /// <param name="myVertexTypeID">The vertex type that points to this IVertex</param>
        /// <param name="myEdgePropertyID">The edge property id that points to this vertex</param>
        /// <returns>All incoming vertices</returns>
        [OperationContract]
        List<ServiceVertexInstance> GetIncomingVertices(SecurityToken mySecToken, ServiceTransactionToken myTransToken, Int64 myVertexTypeID, ServiceVertexInstance myVertex, Int64 myEdgePropertyID);

        #endregion

        #region Outgoing

        /// <summary>
        /// Is there a specified outgoing edge?
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the interesting edge</param>
        /// <returns>True if there is a specified edge, otherwise false</returns>
        [OperationContract]
        Boolean HasOutgoingEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, Int64 myEdgePropertyID);

        /// <summary>
        /// Returns all outgoing edges
        /// </summary>
        /// <param name="myFilter">A function to filter those edges (EdgeID, IEdge, Bool)</param>
        /// <returns>An IEnumerable of all outgoing edges</returns>
        [OperationContract]
        List<Tuple<Int64, ServiceEdgeInstance>> GetAllOutgoingEdges(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Returns all outgoing hyper edges
        /// </summary>
        /// <param name="myFilter">A function to filter those edges (EdgeID, IHyperEdge, Bool)</param>
        /// <returns>An IEnumerable of propertyID/hyper edge KVP</returns>
        [OperationContract]
        List<Tuple<Int64, ServiceHyperEdgeInstance>> GetAllOutgoingHyperEdges(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Returns all outgoing single edges
        /// </summary>
        /// <param name="myFilter">A function to filter those edges (EdgeID, ISingleEdge, Bool)</param>
        /// <returns>An IEnumerable of all single edges</returns>
        [OperationContract]
        List<Tuple<Int64, ServiceSingleEdgeInstance>> GetAllOutgoingSingleEdges(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex);

        /// <summary>
        /// Returns a specified edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>An IEdge</returns>
        [OperationContract]
        ServiceEdgeInstance GetOutgoingEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, Int64 myEdgePropertyID);

        /// <summary>
        /// Returns a specified hyper edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>A hyper edge</returns>
        [OperationContract]
        ServiceHyperEdgeInstance GetOutgoingHyperEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, Int64 myEdgePropertyID);

        /// <summary>
        /// Get a specified single edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>A single edge</returns>
        [OperationContract]
        ServiceSingleEdgeInstance GetOutgoingSingleEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, Int64 myEdgePropertyID);

        #endregion

        #endregion

        #region Binary data

        /// <summary>
        /// Returns a specified binary property
        /// </summary>
        /// <param name="myPropertyID">The property id of the specified binary</param>
        /// <returns>A stream</returns>
        /// 
        /// <exception cref="sones.Library.PropertyHyperGraph.ErrorHandling.BinaryNotExistentException">
        /// The requested binary property does not exist on this vertex.
        /// </exception>
        [OperationContract]
        Stream GetBinaryProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, Int64 myPropertyID);

        /// <summary>
        /// Returns all binary properties
        /// </summary>
        /// <param name="myFilter">A function to filter the binary properties</param> 
        /// <returns>An IEnumerable of PropertyID/stream KVP</returns>
        [OperationContract]
        List<Tuple<Int64, Stream>> GetAllBinaryProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex);

        #endregion

    }
}

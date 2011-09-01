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
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexTypeServices;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;


namespace sones.GraphDS.Services.RemoteAPIService.EdgeTypeService
{
    [ServiceContract(Namespace = sonesRPCServer.Namespace, Name = "EdgeTypeService")]
    public interface IEdgeTypeService : IBaseTypeService
    {
        #region Inheritance

        /// <summary>
        /// Returns the descendant of this IEdgeType.
        /// </summary>
        /// <returns>An enumeration of ServiceEdgeType that are descendant of this IEdgeType.</returns>
        /// <seealso cref="IBaseType.GetDescendantTypes"/>
        [OperationContract]
        List<ServiceEdgeType> GetDescendantEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myEdgeType);

        /// <summary>
        /// Returns the descendant of this IEdgeType and this IEdgeType in one enumeration.
        /// </summary>
        /// <returns>An enumeration of IEdgeType that are descendant of this IEdgeType and this IEdgeType itself.</returns>
        /// <seealso cref="IBaseType.GetDescendantTypesAndSelf"/>
        [OperationContract]
        List<ServiceEdgeType> GetDescendantEdgeTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myEdgeType);

        /// <summary>
        /// Returns the ancestor of this IEdgeType.
        /// </summary>
        /// <returns>An enumeration of IEdgeType that are ancestors of this IEdgeType.</returns>
        /// <seealso cref="IBaseType.GetAncestorTypes"/>
        [OperationContract]
        List<ServiceEdgeType> GetAncestorEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myEdgeType);

        /// <summary>
        /// Returns the ancestor of this IEdgeType and this IEdgeType in one enumeration.
        /// </summary>
        /// <returns>An enumeration of IEdgeType that are ancestors of this IEdgeType and this IEdgeType itself.</returns>
        /// <seealso cref="IBaseType.GetAncestorTypesAndSelf"/>
        [OperationContract]
        List<ServiceEdgeType> GetAncestorEdgeTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myEdgeType);

        /// <summary>
        /// Returns all descendant and ancestors of this IEdgeType.
        /// </summary>
        /// <returns>An enumeration of all IEdgeType that are ancestors or descendant of this IEdgeType.</returns>
        /// <seealso cref="IBaseType.GetKinsmenTypes"/>
        [OperationContract]
        List<ServiceEdgeType> GetKinsmenEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myEdgeType);

        /// <summary>
        /// Returns all descendant and ancestors of this IEdgeType and this IEdgeType in one enumeration. 
        /// </summary>
        /// <returns>An enumeration of all IEdgeType that are ancestors or descendant of this IEdgeType and this IEdgeType itself.</returns>
        /// <seealso cref="IBaseType.GetKinsmenTypesAndSelf"/>
        [OperationContract]
        List<ServiceEdgeType> GetKinsmenEdgeTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myEdgeType);

        /// <summary>
        /// Returns the direct children of this IEdgeType.
        /// </summary>
        /// <seealso cref="IBaseType.ChildrenTypes"/>
        [OperationContract]
        List<ServiceEdgeType> ChildrenEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myEdgeType);

        /// <summary>
        /// Gets the parent of this IEdgeType.
        /// </summary>
        /// <seealso cref="IBaseType.ParentType"/>
        [OperationContract]
        ServiceEdgeType ParentEdgeType(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myEdgeType);

        #endregion
    }
}

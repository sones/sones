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
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using System.IO;
using sones.GraphDS.Services.RemoteAPIService.MessageContracts;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContracts.StreamedService
{
    [ServiceContract(Namespace = sonesRPCServer.Namespace, Name = "StreamedService")]
    public interface IStreamedService
    {
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
        Stream GetBinaryProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex, Int64 myPropertyID);

        /// <summary>
        /// Returns all binary properties
        /// </summary>
        /// <param name="myFilter">A function to filter the binary properties</param> 
        /// <returns>An IEnumerable of PropertyID/stream KVP</returns>
        [OperationContract]
        List<Tuple<Int64, Stream>> GetAllBinaryProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex);

        [OperationContract]
        void SetBinaryProperty(SetBinaryPropertyMessage myMessage);

        #endregion
    }
}

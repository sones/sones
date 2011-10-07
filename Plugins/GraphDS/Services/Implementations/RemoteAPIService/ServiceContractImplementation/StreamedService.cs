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
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using sones.Library.Commons.Security;
using System.IO;
using sones.GraphDS.Services.RemoteAPIService.ServiceConverter;
using sones.Library.PropertyHyperGraph;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.StreamedService;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    public partial class RPCServiceContract : IStreamedService
    {

        public Stream GetBinaryProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexInstance myVertex, Int64 myPropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetBinaryProperty(myPropertyID);
        }

        public List<Tuple<long, Stream>> GetAllBinaryProperties(SecurityToken mySecurityToken, long myTransToken, ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllBinaryProperties().ToList();
        }
    }
}

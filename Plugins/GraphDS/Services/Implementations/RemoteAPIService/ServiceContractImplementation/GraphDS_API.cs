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
using System.ServiceModel.Description;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphDS.Services.RemoteAPIService.API_Services;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.VertexType;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    
    public partial class RPCServiceContract : IGraphDS_API
    {


        public ServiceVertexType CreateVertexType(ServiceVertexTypePredefinition myVertexTypePreDef)
        {
            VertexTypePredefinition Predefinition = myVertexTypePreDef.ToVertexTypePredefinition();
            return new ServiceVertexType(GraphDS.CreateVertexType<IVertexType>(null, null, new RequestCreateVertexType(Predefinition), (Statistics, VertexType) => VertexType));
        }
    }
}

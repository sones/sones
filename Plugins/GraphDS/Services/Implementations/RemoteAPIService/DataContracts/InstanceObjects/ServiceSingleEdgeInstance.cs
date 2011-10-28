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
using System.Runtime.Serialization;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceSingleEdgeInstance : ServiceEdgeInstance
    {
        public ServiceSingleEdgeInstance(ISingleEdge mySingleEdge, Nullable<Int64> myEdgePropertyID, ServiceVertexInstance myHyperEdgeSourceVertex = null)
            : base(mySingleEdge as IEdge, myEdgePropertyID)
        {
            this.TargetVertex = new ServiceVertexInstance(mySingleEdge.GetTargetVertex());
            this.HyperEdgeSourceVertex = myHyperEdgeSourceVertex;
        }

        public ServiceSingleEdgeInstance() { }

        [DataMember]
        public ServiceVertexInstance TargetVertex;

        [DataMember]
        public ServiceVertexInstance HyperEdgeSourceVertex;
    }
}

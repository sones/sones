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
using sones.GraphQL.Result;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.QueryResult
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceVertexView
    {
        public ServiceVertexView(IVertexView myVertexView)
        {
            SingleEdges = myVertexView.GetAllSingleEdges().Select(x => new Tuple<string, ServiceSingleEdgeView>(x.Item1, new ServiceSingleEdgeView(x.Item2))).ToList();
            HyperEdges = myVertexView.GetAllHyperEdges().Select(x => new Tuple<string, ServiceHyperEdgeView>(x.Item1, new ServiceHyperEdgeView(x.Item2))).ToList();
            Properties = myVertexView.GetAllProperties().ToList();
        }

        [DataMember]
        public List<Tuple<string, ServiceSingleEdgeView>> SingleEdges;

        [DataMember]
        public List<Tuple<string, ServiceHyperEdgeView>> HyperEdges;

        [DataMember]
        public List<Tuple<string, object>> Properties;
    }
}

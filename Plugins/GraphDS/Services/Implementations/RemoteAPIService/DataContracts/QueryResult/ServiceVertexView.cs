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
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.Services.RemoteAPIService.ServiceConverter;
using sones.Library.CollectionWrapper;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.QueryResult
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    [KnownType(typeof(ServicePropertyMultiplicity))]
    [KnownType(typeof(ServiceEdgeView))]
    [KnownType(typeof(ServiceIndexDefinition))]
    [KnownType(typeof(ListCollectionWrapper))]
    public class ServiceVertexView
    {
        public ServiceVertexView(IVertexView myVertexView)
        {
            var singleEdges = myVertexView.GetAllSingleEdges();
            SingleEdges = (singleEdges == null || singleEdges.Count() == 0) ? null : singleEdges.ToDictionary(x => x.Item1, x => new ServiceSingleEdgeView(x.Item2));
            var hyperEdges = myVertexView.GetAllHyperEdges();
            HyperEdges = (hyperEdges == null || hyperEdges.Count() == 0) ? null : hyperEdges.ToDictionary(x => x.Item1, x => new ServiceHyperEdgeView(x.Item2));
            var properties = myVertexView.GetAllProperties();
            if (properties == null || properties.Count() == 0)
            {
                Properties = null;
            }
            else
            {
                Properties = properties.ToDictionary(x => x.Item1, x =>
                {
                    object value = ConvertHelper.ToServiceObject(x.Item2);
                    if (value != null)
                        return value;
                    else
                        return x.Item2;
                });
            }
        }

        [DataMember]
        public Dictionary<string, object> Properties;

        [DataMember]
        public Dictionary<string, ServiceSingleEdgeView> SingleEdges;

        [DataMember]
        public Dictionary<string, ServiceHyperEdgeView> HyperEdges;
    }
}

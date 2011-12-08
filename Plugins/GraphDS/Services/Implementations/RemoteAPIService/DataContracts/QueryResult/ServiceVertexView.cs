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
using System.Collections;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.QueryResult
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    [KnownType(typeof(ServicePropertyMultiplicity))]
    [KnownType(typeof(ServiceEdgeView))]
    [KnownType(typeof(ServiceIndexDefinition))]
    public class ServiceVertexView
    {
        public ServiceVertexView(IVertexView myVertexView)
        {
            var singleEdges = myVertexView.GetAllSingleEdges();
            SingleEdges = (singleEdges == null || singleEdges.Count() == 0) ? null : singleEdges.ToDictionary(x => x.Item1, x => new ServiceSingleEdgeView(x.Item2));
            var hyperEdges = myVertexView.GetAllHyperEdges();
            HyperEdges = (hyperEdges == null || hyperEdges.Count() == 0) ? null : hyperEdges.ToDictionary(x => x.Item1, x => new ServiceHyperEdgeView(x.Item2));
            var properties = myVertexView.GetAllProperties();

            Properties = null;
            ListProperties = null;
            if (properties != null && properties.Count() > 0)
            {
                foreach (var item in properties)
                {
                    if (item.Item2 != null && item.Item2.GetType().IsGenericType && item.Item2 is IEnumerable)
                    {
                        if (ListProperties == null)
                        {
                            ListProperties = new Dictionary<string, List<object>>();
                        }
                        ListProperties.Add(item.Item1, ConvertHelper.ToServiceObjectList(item.Item2 as IEnumerable));
                    }
                    else if (item.Item2 != null && item.Item2 is ListCollectionWrapper)
                    {
                        if (ListProperties == null)
                        {
                            ListProperties = new Dictionary<string, List<object>>();
                        }
                        ListProperties.Add(item.Item1, ConvertHelper.ToServiceObjectList(item.Item2 as ListCollectionWrapper));
                    }
                    else
                    {
                        if (Properties == null)
                        {
                            Properties = new Dictionary<string, object>();
                        }
                        object value = ConvertHelper.ToServiceObject(item.Item2);
                        if (value == null)
                        {
                            value = item.Item2;
                        }
                        Properties.Add(item.Item1, value);
                    }
                }
            }
        }

        /*
         * Properties are splitted in two dictionaries because current mono (2.10.6)
         * is unable to lookup object value of the first dictionary as a list.
         * Therefore a second dictionary is implemented to contain lists.
         */
        [DataMember]
        public Dictionary<string, object> Properties;

        [DataMember]
        public Dictionary<string, List<object>> ListProperties;

        [DataMember]
        public Dictionary<string, ServiceSingleEdgeView> SingleEdges;

        [DataMember]
        public Dictionary<string, ServiceHyperEdgeView> HyperEdges;
    }
}

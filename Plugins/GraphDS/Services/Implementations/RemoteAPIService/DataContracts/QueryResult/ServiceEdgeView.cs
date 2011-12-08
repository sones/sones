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
using System.ServiceModel;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.Services.RemoteAPIService.ServiceConverter;
using System.Collections;
using sones.Library.CollectionWrapper;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.QueryResult
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    [KnownType(typeof(ServicePropertyMultiplicity))]
    public class ServiceEdgeView
    {
        public ServiceEdgeView(IEnumerable<Tuple<string, object>> myPropertyList)
        {
            Properties = null;
            ListProperties = null;
            if (myPropertyList != null && myPropertyList.Count() > 0)
            {
                foreach (var item in myPropertyList)
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

        [DataMember]
        public Dictionary<string, object> Properties;

        [DataMember]
        public Dictionary<String, List<object>> ListProperties;
    }
}

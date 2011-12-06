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

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.QueryResult
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    [KnownType(typeof(ServicePropertyMultiplicity))]
    public class ServiceEdgeView
    {
        public ServiceEdgeView(IEnumerable<Tuple<string, object>> myPropertyList)
        {
            if (myPropertyList != null && myPropertyList.Count() > 0)
            {
                Properties = new Dictionary<string, object>();
                foreach (var item in myPropertyList)
                {
                    var value = ConvertHelper.ToServiceObject(item.Item2);
                    if (value != null)
                    {
                        Properties.Add(item.Item1, value);
                    }
                    else
                    {
                        Properties.Add(item.Item1, item.Item2);
                    }
                }
            }
        }

        [DataMember]
        public Dictionary<string, object> Properties;
    }
}

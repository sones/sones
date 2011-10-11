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
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    [KnownType(typeof(ServiceVertexType))]
    [KnownType(typeof(ServiceEdgeType))]
    public class ServiceBaseType
    {
        public ServiceBaseType(IBaseType myBaseType)
        {
            this.ID = myBaseType.ID;
            this.Comment = myBaseType.Comment;
            this.Name = myBaseType.Name;
            this.IsUserDefined = myBaseType.IsUserDefined;
        }

        [DataMember]
        public long ID;
        [DataMember]
        public String Name;
        [DataMember]
        public String Comment;
        [DataMember]
        public Boolean IsUserDefined;
        
    }
}

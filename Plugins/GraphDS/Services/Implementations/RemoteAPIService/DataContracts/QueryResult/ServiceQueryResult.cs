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
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.GraphDB.TypeSystem;
using sones.Library.ErrorHandling;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.QueryResult
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceQueryResult
    {
        public ServiceQueryResult(IQueryResult myQueryResult)
        {
            this.Duration = myQueryResult.Duration;
            if (myQueryResult.Error != null)
                this.Error = myQueryResult.Error.Message;
            else
                this.Error = null;
            this.NameOfQueryLanguage = myQueryResult.NameOfQuerylanguage;
            this.NumberOfAffectedVertices = myQueryResult.NumberOfAffectedVertices;
            this.Query = myQueryResult.Query;
            if (myQueryResult.TypeOfResult == ResultType.Successful)
                this.TypeOfResult = ServiceResultType.Successful;
            else if (myQueryResult.TypeOfResult == ResultType.Failed)
                this.TypeOfResult = ServiceResultType.Failed;
            this.Vertices = (myQueryResult.Vertices == null || myQueryResult.Vertices.Count() == 0) ? null : myQueryResult.Vertices.Select(x => new ServiceVertexView(x)).ToList();
        }

        [DataContract(Namespace = sonesRPCServer.Namespace)]
        public enum ServiceResultType : byte
        {
            [EnumMember]
            Successful,
            [EnumMember]
            Failed
        }

        [DataMember]
        public UInt64 Duration;

        [DataMember]
        public UInt64 NumberOfAffectedVertices;

        [DataMember]
        public String Error;

        [DataMember]
        public ServiceResultType TypeOfResult;

        [DataMember]
        public String Query;

        [DataMember]
        public String NameOfQueryLanguage;

        [DataMember]
        public List<ServiceVertexView> Vertices;
    }
}

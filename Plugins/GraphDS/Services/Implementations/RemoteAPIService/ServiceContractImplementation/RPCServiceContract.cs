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
using sones.Library.Commons.Transaction;
using sones.GraphDB;
using sones.GraphDS;
using sones.GraphQL.Result;
using System.Runtime.Serialization;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.GraphDB.TypeSystem;
using sones.Plugins.GraphDS.Services;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{


    [ServiceBehavior(Namespace = sonesRPCServer.Namespace, InstanceContextMode = InstanceContextMode.Single, MaxItemsInObjectGraph = 2147483646)]
    public partial class RPCServiceContract : IRPCServiceContract
    {
        #region Data

        private IGraphDS GraphDS;

        #endregion

        #region C'tor

        public RPCServiceContract(IGraphDS myGraphDS)
        {
            this.GraphDS = myGraphDS;
        }

        #endregion
    }
}

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
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.MEX;
using System.Reflection;
using System.IO;
using System.Xml;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    
    
    [ServiceBehavior(Namespace = sonesRPCServer.Namespace,  IncludeExceptionDetailInFaults = true)]
    public  class MonoMEX : IMonoMEX
    {
        public Stream GetWSDL()
        {

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("sones.GraphDS.Services.RemoteAPIService.ServiceContracts.MEX.WSDL.xml");
     
            OutgoingWebResponseContext context = WebOperationContext.Current.OutgoingResponse;
            
            // everything is OK, so send image 

            context.Headers.Add(System.Net.HttpResponseHeader.CacheControl, "public");
            context.ContentType = "application/xml";
            context.StatusCode = System.Net.HttpStatusCode.OK;
            return stream;
        }
    }
}

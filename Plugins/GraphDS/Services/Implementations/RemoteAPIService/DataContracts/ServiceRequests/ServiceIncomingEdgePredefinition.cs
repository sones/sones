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
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceIncomingEdgePredefinition : ServiceAttributePredefinition
    {
        

        public IncomingEdgePredefinition ToIncomingEdgePredefinition()
        {
            IncomingEdgePredefinition IncomingEdgePredef = new IncomingEdgePredefinition(this.AttributeName,"",""); //todo
            var VertexTypeName = this.AttributeType.Substring(0,this.AttributeType.IndexOf('.'));
            var OutgoingEdgeName = this.AttributeType.Substring(this.AttributeType.IndexOf('.'),this.AttributeType.Length - 1);

                      

            if(this.Comment != null)
                IncomingEdgePredef.SetComment(this.Comment);

            return IncomingEdgePredef;
        }

       
    }
}

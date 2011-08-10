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
            IncomingEdgePredefinition IncomingEdgePredef = new IncomingEdgePredefinition(this.AttributeName);
            var VertexTypeName = this.AttributeType.Substring(0,this.AttributeType.IndexOf('.'));
            var OutgoingEdgeName = this.AttributeType.Substring(this.AttributeType.IndexOf('.'),this.AttributeType.Length - 1);

            IncomingEdgePredef.SetOutgoingEdge(VertexTypeName,OutgoingEdgeName);
            

            if(this.Comment != null)
                IncomingEdgePredef.SetComment(this.Comment);

            return IncomingEdgePredef;
        }

       
    }
}

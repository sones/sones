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

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    
    [ServiceBehavior(Namespace = "http://www.sones.com", InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public partial class RPCServiceContract : IRPCServiceContract
    {
        #region Data

        private IGraphDS GraphDS;

        #endregion

        #region C'tor

        public RPCServiceContract(IGraphDS myGraphDSServer)
            : base(myGraphDSServer)
        {
            this.GraphDS = myGraphDSServer;
            
        }

        #endregion
   
    }
}

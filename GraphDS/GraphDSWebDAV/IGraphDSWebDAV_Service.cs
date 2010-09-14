/* <id name="GraphDSWebDAV – IGraphDSWebDAV_Service" />
 * <copyright file="IGraphDSWebDAV_Service.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The WebDAV interface</summary>
 */

using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using sones.Networking.HTTP;

namespace sones.GraphDS.Connectors.WebDAV
{
    [ServiceContract]
    interface IGraphDSWebDAV_Service
    {

        #region Tested WebDAV methods

        [OperationContract, ForceAuthentication]
        [WebGet(UriTemplate = "{destination}")]
        void DoGET(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "PROPFIND")]
        void DoPROPFIND(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "MKCOL")]
        void DoMKCOL(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "DELETE")]
        void DoDELETE(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "PUT")]
        void DoPUT(String destination);

        #endregion

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "OPTIONS")]
        void DoOPTIONS(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "PROPPATCH")]
        void DoPROPPATCH(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "HEAD")]
        void DoHEAD(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "POST")]
        void DoPOST(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "COPY")]
        void DoCOPY(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "MOVE")]
        void DoMOVE(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "LOCK")]
        void DoLOCK(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "UNLOCK")]
        void DoUNLOCK(String destination);

        [OperationContract, ForceAuthentication]
        [WebInvoke(UriTemplate = "{destination}", Method = "TRACE")]
        void DoTRACE(String destination);


    }
}

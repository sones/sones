/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* <id name="GraphDSWebDAV – IGraphDSWebDAV_Service" />
 * <copyright file="IGraphDSWebDAV_Service.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
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

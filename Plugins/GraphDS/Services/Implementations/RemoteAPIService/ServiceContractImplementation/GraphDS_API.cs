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
using System.ServiceModel.Description;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphDS.Services.RemoteAPIService.API_Services;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.QueryResult;
using sones.GraphDS.Services.RemoteAPIService.ServiceConverter;
using sones.Library.PropertyHyperGraph;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ChangesetObjects;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InsertPayload;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.PayloadObjects;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{

    public partial class RPCServiceContract : IGraphDS_API
    {

        public ServiceVertexType CreateVertexType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexTypePredefinition myVertexTypePreDef)
        {
            var Request = ServiceRequestFactory.MakeRequestCreateVertexType(myVertexTypePreDef);
            var Response = this.GraphDS.CreateVertexType<IVertexType>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response);
        }

        public ServiceVertexType AlterVertexType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType, ServiceAlterVertexChangeset myChangeset)
        {
            var Request = ServiceRequestFactory.MakeRequestAlterVertexType(myVertexType, myChangeset);
            var Response = this.GraphDS.AlterVertexType<IVertexType>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response);
        }

        public List<Int64> Clear(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken)
        {
            var Request = ServiceRequestFactory.MakeRequestClear();
            var Response = this.GraphDS.Clear(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyVertexTypeIDs);
            return Response.ToList();
        }

        public ServiceIndexDefinition CreateIndex(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceIndexPredefinition myVertexTypePreDef)
        {
            var Request = ServiceRequestFactory.MakeRequestCreateIndex(myVertexTypePreDef);
            var Response = this.GraphDS.CreateIndex<IIndexDefinition>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConverteOnlyIndexDefinition);
            return new ServiceIndexDefinition(Response);
        }

        public List<ServiceVertexType> CreateVertexTypes(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            List<ServiceVertexTypePredefinition> myVertexTypePreDef)
        {
            var Request = ServiceRequestFactory.MakeRequestCreateVertexTypes(myVertexTypePreDef);
            var Response = this.GraphDS.CreateVertexTypes<IEnumerable<IVertexType>>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyVertexTypes);
            return Response.Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<Int64> Delete(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs = null, ServiceDeletePayload myDeletePayload = null)
        {
            var Request = ServiceRequestFactory.MakeRequestDelete(myVertexType, myVertexIDs, myDeletePayload);
            var Result = this.GraphDS.Delete<List<Int64>>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConverterOnlyRelevantList);
            return Result;
        }

        public ServiceIndexDefinition DescribeIndex(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType, String myIndexName)
        {
            var Request = ServiceRequestFactory.MakeRequestDescribeIndex(myVertexType, myIndexName);
            var Response = this.GraphDS.DescribeIndex<IEnumerable<IIndexDefinition>>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConverteOnlyIndexDefinitions);
            return new ServiceIndexDefinition(Response.FirstOrDefault()); //should only returns one index definition
        }

        public void DropIndex(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType, String myIndexName, String myEdition)
        {
            var Request = ServiceRequestFactory.MakeRequestDropIndex(myVertexType, myIndexName, myEdition);
            var Response = this.GraphDS.DropIndex(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConverteToVoid);
        }

        public Dictionary<Int64, String> DropVertexType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestDropVertexType(myVertexType);
            var Response = this.GraphDS.DropVertexType<Dictionary<Int64, String>>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConverteOnlyDeletedTypeIDs);
            return Response;
        }

        public List<ServiceEdgeType> GetAllEdgeTypes(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            String myEdition)
        {
            var Request = ServiceRequestFactory.MakeRequestGetAllEdgeTypes(myEdition);
            var Response = this.GraphDS.GetAllEdgeTypes<IEnumerable<IEdgeType>>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyEdgeTypes);
            return Response.Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceVertexType> GetAllVertexTypes(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            String myEdition)
        {
            var Request = ServiceRequestFactory.MakeRequestGetAllVertexTypes(myEdition);
            var Response = this.GraphDS.GetAllVertexTypes<IEnumerable<IVertexType>>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyVertexTypes);
            return Response.Select(x => new ServiceVertexType(x)).ToList();
        }

        public ServiceEdgeType GetEdgeType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            String myEdgeTypeName, String myEdition = null)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myEdgeTypeName, myEdition);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response);
        }

        public ServiceVertexInstance GetVertex(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType, Int64 myVertexID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertexType, myVertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyVertexInstance);
            return new ServiceVertexInstance(Response);
        }

        public UInt64 GetVertexCount(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexType myVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexCount(myVertexType);
            var Response = this.GraphDS.GetVertexCount<UInt64>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyCount);
            return Response;
        }

        public List<ServiceVertexInstance> GetVertices(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexType myVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertices(myVertexType);
            var Response = this.GraphDS.GetVertices<IEnumerable<IVertex>>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyVertices);
            return Response.Select(x => new ServiceVertexInstance(x)).ToList();
        }

        public ServiceVertexInstance Insert(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, String myVertexTypeName,
            ServiceInsertPayload myPayload)
        {
            var Request = ServiceRequestFactory.MakeRequestInsertVertex(myVertexTypeName, myPayload);
            var Response = this.GraphDS.Insert<IVertex>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyVertexInstance);
            return new ServiceVertexInstance(Response);
        }

        public ServiceEdgeType AlterEdgeType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceEdgeType myEdgeType, ServiceAlterEdgeChangeset myChangeset)
        {
            var Request = ServiceRequestFactory.MakeRequestAlterEdgeType(myEdgeType, myChangeset);
            var Response = this.GraphDS.AlterEdgeType<IEdgeType>(mySecurityToken, myTransactionToken.TransactionID, Request,
               ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response);
        }

        public ServiceEdgeType CreateEdgeType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceEdgeTypePredefinition myEdgeTypePreDef)
        {
            var Request = ServiceRequestFactory.MakeRequestCreateEdgeType(myEdgeTypePreDef);
            var Response = this.GraphDS.CreateEdgeType<IEdgeType>(mySecurityToken, myTransactionToken.TransactionID, Request,
               ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response);
        }

        public List<ServiceEdgeType> CreateEdgeTypes(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            IEnumerable<ServiceEdgeTypePredefinition> myEdgeTypePreDef)
        {
            var Request = ServiceRequestFactory.MakeRequestCreateEdgeTypes(myEdgeTypePreDef);
            var Response = this.GraphDS.CreateEdgeTypes<IEnumerable<IEdgeType>>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyEdgeTypes);
            return Response.Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceIndexDefinition> DescribeIndices(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestDescribeIndex(myVertexType, ""); // todo prove the capability of this method call
            var Response = this.GraphDS.DescribeIndices<IEnumerable<IIndexDefinition>>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConverteOnlyIndexDefinitions);
            return Response.Select(x => new ServiceIndexDefinition(x)).ToList();
        }

        public Dictionary<Int64, String> DropEdgeType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceEdgeType myEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestDropEdgeType(myEdgeType);
            var Response = this.GraphDS.DropEdgeType<Dictionary<Int64, String>>(mySecurityToken, myTransactionToken.TransactionID, Request,
                ServiceReturnConverter.ConverteOnlyDeletedTypeIDs);
            return Response;
        }

        public void LogOff(SecurityToken mySecurityToken)
        {
            this.GraphDS.LogOff(mySecurityToken);
        }

        public SecurityToken LogOn(String myLogin, String myPassword)
        {
            var SecToken = this.GraphDS.LogOn(new UserPasswordCredentials(myLogin, myPassword));
            return SecToken;
        }

        public ServiceTransactionToken BeginTransaction(SecurityToken mySecurityToken)
        {
            return new ServiceTransactionToken(this.GraphDS.BeginTransaction(mySecurityToken));
        }

        public ServiceQueryResult Query(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, string myQueryString, string myLanguage)
        {
            return new ServiceQueryResult(this.GraphDS.Query(mySecurityToken, myTransactionToken.TransactionID, myQueryString, myLanguage));
        }

        public List<ServiceVertexInstance> Update(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myVertexType, IEnumerable<long> myVertexIDs, ServiceUpdateChangeset myUpdateChangeset)
        {
            var Request = ServiceRequestFactory.MakeRequestUpdate(myVertexType, myVertexIDs, myUpdateChangeset);
            var Response = this.GraphDS.Update<IEnumerable<IVertex>>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertices);
            return Response.Select(x => new ServiceVertexInstance(x)).ToList();
        }


        public ServiceVertexType GetVertexType(SecurityToken mySecToken, ServiceTransactionToken myTransToken, string myVertexTypeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myVertexTypeName);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request,
                ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response);
        }


        public void CommitTransaction(SecurityToken mySecToken, ServiceTransactionToken myTransToken)
        {
            this.GraphDS.CommitTransaction(mySecToken, myTransToken.TransactionID);
        }

        public void RollbackTransaction(SecurityToken mySecToken, ServiceTransactionToken myTransToken)
        {
            this.GraphDS.RollbackTransaction(mySecToken, myTransToken.TransactionID);
        }

        public void Shutdown(SecurityToken mySecurityToken)
        {
            this.GraphDS.Shutdown(mySecurityToken);
        }
    }
}

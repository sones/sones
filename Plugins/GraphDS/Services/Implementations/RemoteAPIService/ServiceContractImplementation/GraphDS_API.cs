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
using sones.GraphDS.Services.RemoteAPIService.ErrorHandling;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{

    public partial class RPCServiceContract : IGraphDS_API
    {

        public ServiceVertexType CreateVertexType(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexTypePredefinition myVertexTypePreDef)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken,out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestCreateVertexType(myVertexTypePreDef);
            var Response = this.GraphDS.CreateVertexType<IVertexType>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response);
        }

        public ServiceVertexType AlterVertexType(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, ServiceAlterVertexChangeset myChangeset)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestAlterVertexType(myVertexType, myChangeset);
            var Response = this.GraphDS.AlterVertexType<IVertexType>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response);
        }

        public List<Int64> Clear(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestClear();
            var Response = this.GraphDS.Clear(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexTypeIDs);
            return Response.ToList();
        }

        public ServiceIndexDefinition CreateIndex(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken, ServiceIndexPredefinition myVertexTypePreDef)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestCreateIndex(myVertexTypePreDef);
            var Response = this.GraphDS.CreateIndex<IIndexDefinition>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteOnlyIndexDefinition);
            return new ServiceIndexDefinition(Response);
        }

        public List<ServiceVertexType> CreateVertexTypes(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            List<ServiceVertexTypePredefinition> myVertexTypePreDef)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestCreateVertexTypes(myVertexTypePreDef);
            var Response = this.GraphDS.CreateVertexTypes<IEnumerable<IVertexType>>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexTypes);
            return Response.Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<Int64> Delete(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs = null, ServiceDeletePayload myDeletePayload = null)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestDelete(myVertexType, myVertexIDs, myDeletePayload);
            var Result = this.GraphDS.Delete<List<Int64>>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverterOnlyRelevantList);
            return Result;
        }

        public ServiceIndexDefinition DescribeIndex(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, String myIndexName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestDescribeIndex(myVertexType, myIndexName);
            var Response = this.GraphDS.DescribeIndex<IEnumerable<IIndexDefinition>>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteOnlyIndexDefinitions);
            return new ServiceIndexDefinition(Response.FirstOrDefault()); //should only returns one index definition
        }

        public void DropIndex(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, String myIndexName, String myEdition)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestDropIndex(myVertexType, myIndexName, myEdition);
            var Response = this.GraphDS.DropIndex(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteToVoid);
        }

        public Dictionary<Int64, String> DropVertexType(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestDropVertexType(myVertexType);
            var Response = this.GraphDS.DropVertexType<Dictionary<Int64, String>>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteOnlyDeletedTypeIDs);
            return Response;
        }

        public List<ServiceEdgeType> GetAllEdgeTypes(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            String myEdition)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetAllEdgeTypes(myEdition);
            var Response = this.GraphDS.GetAllEdgeTypes<IEnumerable<IEdgeType>>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyEdgeTypes);
            return Response.Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceVertexType> GetAllVertexTypes(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            String myEdition)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetAllVertexTypes(myEdition);
            var Response = this.GraphDS.GetAllVertexTypes<IEnumerable<IVertexType>>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexTypes);
            return Response.Select(x => new ServiceVertexType(x)).ToList();
        }

        public ServiceEdgeType GetEdgeType(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            String myEdgeTypeName, String myEdition = null)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myEdgeTypeName, myEdition);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response);
        }

        public ServiceVertexInstance GetVertex(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, Int64 myVertexID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertexType, myVertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexInstance);
            return new ServiceVertexInstance(Response);
        }

        public UInt64 GetVertexCount(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken, ServiceVertexType myVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexCount(myVertexType);
            var Response = this.GraphDS.GetVertexCount<UInt64>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyCount);
            return Response;
        }

        public List<ServiceVertexInstance> GetVertices(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken, ServiceVertexType myVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertices(myVertexType);
            var Response = this.GraphDS.GetVertices<IEnumerable<IVertex>>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertices);
            return Response.Select(x => new ServiceVertexInstance(x)).ToList();
        }

        public ServiceVertexInstance Insert(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken, String myVertexTypeName,
            ServiceInsertPayload myPayload)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestInsertVertex(myVertexTypeName, myPayload);
            var Response = this.GraphDS.Insert<IVertex>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexInstance);
            return new ServiceVertexInstance(Response);
        }

        public ServiceEdgeType AlterEdgeType(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceEdgeType myEdgeType, ServiceAlterEdgeChangeset myChangeset)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestAlterEdgeType(myEdgeType, myChangeset);
            var Response = this.GraphDS.AlterEdgeType<IEdgeType>(myDBSecToken, myTransactionToken, Request,
               ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response);
        }

        public ServiceEdgeType CreateEdgeType(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceEdgeTypePredefinition myEdgeTypePreDef)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestCreateEdgeType(myEdgeTypePreDef);
            var Response = this.GraphDS.CreateEdgeType<IEdgeType>(myDBSecToken, myTransactionToken, Request,
               ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response);
        }

        public List<ServiceEdgeType> CreateEdgeTypes(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            IEnumerable<ServiceEdgeTypePredefinition> myEdgeTypePreDef)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestCreateEdgeTypes(myEdgeTypePreDef);
            var Response = this.GraphDS.CreateEdgeTypes<IEnumerable<IEdgeType>>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyEdgeTypes);
            return Response.Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceIndexDefinition> DescribeIndices(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestDescribeIndex(myVertexType, ""); // todo prove the capability of this method call
            var Response = this.GraphDS.DescribeIndices<IEnumerable<IIndexDefinition>>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteOnlyIndexDefinitions);
            return Response.Select(x => new ServiceIndexDefinition(x)).ToList();
        }

        public Dictionary<Int64, String> DropEdgeType(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceEdgeType myEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestDropEdgeType(myEdgeType);
            var Response = this.GraphDS.DropEdgeType<Dictionary<Int64, String>>(myDBSecToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteOnlyDeletedTypeIDs);
            return Response;
        }

        public void LogOff(ServiceSecurityToken mySecurityToken)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            this.GraphDS.LogOff(myDBSecToken);
        }

        public ServiceSecurityToken LogOn(String myLogin, String myPassword)
        {
            var SecToken = this.GraphDS.LogOn(new UserPasswordCredentials(myLogin, myPassword));
            ServiceSecurityToken ServiceSecToken = new ServiceSecurityToken(SecToken.GetID());
            SecurityTokenMap.Add(ServiceSecToken, SecToken);
            return ServiceSecToken;
        }

        public Int64 BeginTransaction(ServiceSecurityToken mySecurityToken)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            return this.GraphDS.BeginTransaction(myDBSecToken);
        }

        public ServiceQueryResult Query(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken, string myQueryString, string myLanguage)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            return new ServiceQueryResult(this.GraphDS.Query(myDBSecToken, myTransactionToken, myQueryString, myLanguage));
        }

        public List<ServiceVertexInstance> Update(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myVertexType, IEnumerable<long> myVertexIDs, ServiceUpdateChangeset myUpdateChangeset)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestUpdate(myVertexType, myVertexIDs, myUpdateChangeset);
            var Response = this.GraphDS.Update<IEnumerable<IVertex>>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertices);
            return Response.Select(x => new ServiceVertexInstance(x)).ToList();
        }


        public ServiceVertexType GetVertexType(ServiceSecurityToken mySecurityToken, Int64 myTransToken, string myVertexTypeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myVertexTypeName);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response);
        }


        public void CommitTransaction(ServiceSecurityToken mySecurityToken, Int64 myTransToken)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            this.GraphDS.CommitTransaction(myDBSecToken, myTransToken);
        }

        public void RollbackTransaction(ServiceSecurityToken mySecurityToken, Int64 myTransToken)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            this.GraphDS.RollbackTransaction(myDBSecToken, myTransToken);
        }

        public void Shutdown(ServiceSecurityToken mySecurityToken)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            this.GraphDS.Shutdown(myDBSecToken);
        }


        public List<ServiceVertexInstance> GetVertices(ServiceSecurityToken mySecToken, long myTransToken, DataContracts.ServiceRequests.Expression.ServiceBaseExpression myVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertices(myVertexType);
            var Response = this.GraphDS.GetVertices<IEnumerable<IVertex>>(myDBSecToken, myTransToken, Request,
                ServiceReturnConverter.ConvertOnlyVertices);
            return Response.Select(x => new ServiceVertexInstance(x)).ToList();
        }


        public void RebuildIndices(ServiceSecurityToken mySecurityToken, long myTransToken, IEnumerable<string> myVertexTypeNames)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityTokenException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestRebuildIndices(myVertexTypeNames);
            var Response = this.GraphDS.RebuildIndices(myDBSecToken, myTransToken, Request, (Statistics) => Statistics);
            return;
        }
    }
}

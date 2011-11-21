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
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests.Expression;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    public partial class RPCServiceContract : IGraphDS_API
    {
        public ServiceVertexType CreateVertexType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexTypePredefinition myVertexTypePreDef)
        {
            var Request = ServiceRequestFactory.MakeRequestCreateVertexType(myVertexTypePreDef);
            var Response = this.GraphDS.CreateVertexType<IVertexType>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response);
        }

        public ServiceVertexType AlterVertexType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, ServiceAlterVertexChangeset myChangeset)
        {
            var Request = ServiceRequestFactory.MakeRequestAlterVertexType(myVertexType, myChangeset);
            var Response = this.GraphDS.AlterVertexType<IVertexType>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response);
        }

        public List<Int64> Clear(SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            var Request = ServiceRequestFactory.MakeRequestClear();
            var Response = this.GraphDS.Clear(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexTypeIDs);
            return Response.ToList();
        }

        public ServiceIndexDefinition CreateIndex(SecurityToken mySecurityToken, Int64 myTransactionToken, ServiceIndexPredefinition myVertexTypePreDef)
        {
            var Request = ServiceRequestFactory.MakeRequestCreateIndex(myVertexTypePreDef);
            var Response = this.GraphDS.CreateIndex<IIndexDefinition>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteOnlyIndexDefinition);
            return new ServiceIndexDefinition(Response);
        }

        public List<ServiceVertexType> CreateVertexTypes(SecurityToken mySecurityToken, Int64 myTransactionToken,
            List<ServiceVertexTypePredefinition> myVertexTypePreDef)
        {
            var Request = ServiceRequestFactory.MakeRequestCreateVertexTypes(myVertexTypePreDef);
            var Response = this.GraphDS.CreateVertexTypes<IEnumerable<IVertexType>>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexTypes);
            return Response.Select(x => new ServiceVertexType(x)).ToList();
        }

        public Tuple<IEnumerable<IComparable>, IEnumerable<IComparable>> Delete(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs = null, ServiceDeletePayload myDeletePayload = null)
        {
            var Request = ServiceRequestFactory.MakeRequestDelete(myVertexType, myVertexIDs, myDeletePayload);
            var Result = this.GraphDS.Delete<Tuple<IEnumerable<IComparable>, IEnumerable<IComparable>>>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteAllLists);
            return Result;
        }

        public ServiceIndexDefinition DescribeIndex(SecurityToken mySecurityToken, Int64 myTransactionToken,
            String myVertexTypeName, String myIndexName)
        {
            var Request = ServiceRequestFactory.MakeRequestDescribeIndex(myVertexTypeName, myIndexName);
            var Response = this.GraphDS.DescribeIndex<IEnumerable<IIndexDefinition>>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteOnlyIndexDefinitions);
            return new ServiceIndexDefinition(Response.FirstOrDefault()); //should only returns one index definition
        }

        public void DropIndex(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, String myIndexName, String myEdition)
        {
            var Request = ServiceRequestFactory.MakeRequestDropIndex(myVertexType, myIndexName, myEdition);
            var Response = this.GraphDS.DropIndex(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteToVoid);
        }

        public Dictionary<Int64, String> DropVertexType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestDropVertexType(myVertexType);
            var Response = this.GraphDS.DropVertexType<Dictionary<Int64, String>>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteOnlyDeletedTypeIDs);
            return Response;
        }

        public List<ServiceEdgeType> GetAllEdgeTypes(SecurityToken mySecurityToken, Int64 myTransactionToken,
            String myEdition)
        {
            var Request = ServiceRequestFactory.MakeRequestGetAllEdgeTypes(myEdition);
            var Response = this.GraphDS.GetAllEdgeTypes<IEnumerable<IEdgeType>>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyEdgeTypes);
            return Response.Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceVertexType> GetAllVertexTypes(SecurityToken mySecurityToken, Int64 myTransactionToken,
            String myEdition)
        {
            var Request = ServiceRequestFactory.MakeRequestGetAllVertexTypes(myEdition);
            var Response = this.GraphDS.GetAllVertexTypes<IEnumerable<IVertexType>>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexTypes);
            return Response.Select(x => new ServiceVertexType(x)).ToList();
        }

        public ServiceEdgeType GetEdgeType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            String myEdgeTypeName, String myEdition = null)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myEdgeTypeName, myEdition);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response);
        }

        public ServiceEdgeType GetEdgeType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            Int64 myEdgeTypeID, String myEdition = null)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myEdgeTypeID, myEdition);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response);
        }

        public ServiceVertexInstance GetVertex(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, Int64 myVertexID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertexType, myVertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexInstance);
            return new ServiceVertexInstance(Response);
        }

        public UInt64 GetVertexCount(SecurityToken mySecurityToken, Int64 myTransactionToken, ServiceVertexType myVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexCount(myVertexType);
            var Response = this.GraphDS.GetVertexCount<UInt64>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyCount);
            return Response;
        }

        public List<ServiceVertexInstance> GetVertices(SecurityToken mySecurityToken, Int64 myTransactionToken, ServiceVertexType myVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertices(myVertexType);
            var Response = this.GraphDS.GetVertices<IEnumerable<IVertex>>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertices);
            return Response.Select(x => new ServiceVertexInstance(x)).ToList();
        }

        public List<ServiceVertexInstance> GetVertices(SecurityToken mySecurityToken, long myTransToken, ServiceBaseExpression myExpression)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertices(myExpression);
            var Response = this.GraphDS.GetVertices<IEnumerable<IVertex>>(mySecurityToken, myTransToken, Request,
                ServiceReturnConverter.ConvertOnlyVertices);
            return Response.Select(x => new ServiceVertexInstance(x)).ToList();
        }

        public ServiceVertexInstance Insert(SecurityToken mySecurityToken, Int64 myTransactionToken, String myVertexTypeName,
            ServiceInsertPayload myPayload)
        {
            var Request = ServiceRequestFactory.MakeRequestInsertVertex(myVertexTypeName, myPayload);
            var Response = this.GraphDS.Insert<IVertex>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexInstance);
            return new ServiceVertexInstance(Response);
        }

        public ServiceEdgeType AlterEdgeType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceEdgeType myEdgeType, ServiceAlterEdgeChangeset myChangeset)
        {
            var Request = ServiceRequestFactory.MakeRequestAlterEdgeType(myEdgeType, myChangeset);
            var Response = this.GraphDS.AlterEdgeType<IEdgeType>(mySecurityToken, myTransactionToken, Request,
               ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response);
        }

        public ServiceEdgeType CreateEdgeType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceEdgeTypePredefinition myEdgeTypePreDef)
        {
            var Request = ServiceRequestFactory.MakeRequestCreateEdgeType(myEdgeTypePreDef);
            var Response = this.GraphDS.CreateEdgeType<IEdgeType>(mySecurityToken, myTransactionToken, Request,
               ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response);
        }

        public List<ServiceEdgeType> CreateEdgeTypes(SecurityToken mySecurityToken, Int64 myTransactionToken,
            IEnumerable<ServiceEdgeTypePredefinition> myEdgeTypePreDef)
        {
            var Request = ServiceRequestFactory.MakeRequestCreateEdgeTypes(myEdgeTypePreDef);
            var Response = this.GraphDS.CreateEdgeTypes<IEnumerable<IEdgeType>>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConvertOnlyEdgeTypes);
            return Response.Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceIndexDefinition> DescribeIndices(SecurityToken mySecurityToken, Int64 myTransactionToken,
            String myVertexTypeName)
        {
            var Request = ServiceRequestFactory.MakeRequestDescribeIndex(myVertexTypeName, ""); // todo prove the capability of this method call
            var Response = this.GraphDS.DescribeIndices<IEnumerable<IIndexDefinition>>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteOnlyIndexDefinitions);
            return Response.Select(x => new ServiceIndexDefinition(x)).ToList();
        }

        public List<ServiceIndexDefinition> DescribeIndicesByNames(SecurityToken mySecurityToken, Int64 myTransToken,
            String myVertexTypeName, List<String> myIndexNames)
        {
            var ResponseList = new List<ServiceIndexDefinition>();
            foreach (var item in myIndexNames)
            {
                var Request = ServiceRequestFactory.MakeRequestDescribeIndex(myVertexTypeName, item);
                var Response = this.GraphDS.DescribeIndex<IEnumerable<IIndexDefinition>>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConverteOnlyIndexDefinitions);
                foreach (var index in Response)
                {
                    ResponseList.Add(new ServiceIndexDefinition(index));
                }
            }
            return ResponseList;
        }

        public Dictionary<Int64, String> DropEdgeType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceEdgeType myEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestDropEdgeType(myEdgeType);
            var Response = this.GraphDS.DropEdgeType<Dictionary<Int64, String>>(mySecurityToken, myTransactionToken, Request,
                ServiceReturnConverter.ConverteOnlyDeletedTypeIDs);
            return Response;
        }

        public void LogOff(SecurityToken mySecurityToken)
        {
            this.GraphDS.LogOff(mySecurityToken);
        }

        public SecurityToken LogOn(ServiceUserPasswordCredentials myUserCredentials)
        {
            return this.GraphDS.LogOn(myUserCredentials);
        }

        public Int64 BeginTransaction(SecurityToken mySecurityToken)
        {
            return this.GraphDS.BeginTransaction(mySecurityToken);
        }

        public ServiceQueryResult Query(SecurityToken mySecurityToken, Int64 myTransactionToken, string myQueryString, string myLanguage)
        {
            return new ServiceQueryResult(this.GraphDS.Query(mySecurityToken, myTransactionToken, myQueryString, myLanguage));
        }

        public List<ServiceVertexInstance> Update(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myVertexType, IEnumerable<long> myVertexIDs, ServiceUpdateChangeset myUpdateChangeset)
        {
            var Request = ServiceRequestFactory.MakeRequestUpdate(myVertexType, myVertexIDs, myUpdateChangeset);
            var Response = this.GraphDS.Update<IEnumerable<IVertex>>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertices);
            return Response.Select(x => new ServiceVertexInstance(x)).ToList();
        }

        public ServiceVertexType GetVertexType(SecurityToken mySecurityToken, Int64 myTransToken, string myVertexTypeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myVertexTypeName);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response);
        }

        public ServiceVertexType GetVertexType(SecurityToken mySecurityToken, Int64 myTransToken, Int64 myVertexTypeID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myVertexTypeID);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request,
                ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response);
        }

        public void CommitTransaction(SecurityToken mySecurityToken, Int64 myTransToken)
        {
            this.GraphDS.CommitTransaction(mySecurityToken, myTransToken);
        }

        public void RollbackTransaction(SecurityToken mySecurityToken, Int64 myTransToken)
        {
            this.GraphDS.RollbackTransaction(mySecurityToken, myTransToken);
        }

        public void Shutdown(SecurityToken mySecurityToken)
        {
            this.GraphDS.Shutdown(mySecurityToken);
        }

        public void RebuildIndices(SecurityToken mySecurityToken, long myTransToken, IEnumerable<string> myVertexTypeNames)
        {
            var Request = ServiceRequestFactory.MakeRequestRebuildIndices(myVertexTypeNames);
            var Response = this.GraphDS.RebuildIndices(mySecurityToken, myTransToken, Request, (Statistics) => Statistics);
            return;
        }


        public void TruncateVertexType(SecurityToken mySecurityToken, long myTransToken, string myVertexTypeName)
        {
            var Request = ServiceRequestFactory.MakeRequestTruncate(myVertexTypeName);
            this.GraphDS.Truncate(mySecurityToken, myTransToken, Request, (x) => x);
        }
    }
}

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
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexTypeServices;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.GraphDS.Services.RemoteAPIService.ServiceConverter;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    
    public partial class RPCServiceContract :  IVertexTypeService
    {
        #region IBaseTypeServices

        #region Inheritance

        public List<ServiceVertexType> GetDescendantVertexTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetDescendantVertexTypes().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetDescendantVertexTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetDescendantVertexTypesAndSelf().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetAncestorVertexTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetAncestorVertexTypes().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetAncestorVertexTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetAncestorVertexTypesAndSelf().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetKinsmenVertexTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetKinsmenVertexTypes().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetKinsmenVertexTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetKinsmenVertexTypesAndSelf().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> ChildrenVertexTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.ChildrenVertexTypes.Select(x => new ServiceVertexType(x)).ToList();
        }

        public ServiceVertexType ParentVertexType(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response.ParentVertexType);
        }

        #endregion

        #region Inheritance

        public bool IsSealed(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsSealed;
        }

        public bool HasParentType(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasParentType;
        }

        public bool HasChildTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasChildTypes;
        }

        public bool IsAncestor(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsAncestor(BaseType);
        }

        public bool IsAncestorOrSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsAncestorOrSelf(BaseType);
        }

        public bool IsDescendant(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsDescendant(BaseType);
        }

        public bool IsDescendantOrSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsDescendantOrSelf(BaseType);
        }

        #endregion

        #region Attributes

        public bool HasAttribute(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasAttribute(myAttributeName);
        }

        public ServiceAttributeDefinition GetAttributeDefinition(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceAttributeDefinition(Response.GetAttributeDefinition(myAttributeName));
        }

        public ServiceAttributeDefinition GetAttributeDefinitionByID(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, long myAttributeID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceAttributeDefinition(Response.GetAttributeDefinition(myAttributeID));
        }

        public bool HasAttributes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasAttributes(myIncludeAncestorDefinitions);
        }

        public List<ServiceAttributeDefinition> GetAttributeDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetAttributeDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceAttributeDefinition(x)).ToList();
        }

        #endregion

        #region Properties

        public bool HasProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasProperty(myAttributeName);
        }

        public ServicePropertyDefinition GetPropertyDefinition(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, string myPropertyName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServicePropertyDefinition(Response.GetPropertyDefinition(myPropertyName));
        }

        public ServicePropertyDefinition GetPropertyDefinitionByID(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, long myPropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServicePropertyDefinition(Response.GetPropertyDefinition(myPropertyID));
        }

        public bool HasProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasProperties(myIncludeAncestorDefinitions);
        }

        public List<ServicePropertyDefinition> GetPropertyDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetPropertyDefinitions(myIncludeAncestorDefinitions).Select(x => new ServicePropertyDefinition(x)).ToList();
        }

        public List<ServicePropertyDefinition> GetPropertyDefinitionsByNameList(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, List<string> myPropertyNames)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetPropertyDefinitions(myPropertyNames).Select(x => new ServicePropertyDefinition(x)).ToList();
        }

        #endregion

        #endregion

        #region Incoming

        public bool HasBinaryProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasBinaryProperty(myEdgeName);
        }

        public ServiceBinaryPropertyDefinition GetBinaryPropertyDefinition(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceBinaryPropertyDefinition(Response.GetBinaryPropertyDefinition(myEdgeName));
        }

        public bool HasBinaryProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasBinaryProperties(myIncludeAncestorDefinitions);
        }

        public List<ServiceBinaryPropertyDefinition> GetBinaryProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetBinaryProperties(myIncludeAncestorDefinitions).Select(x => new ServiceBinaryPropertyDefinition(x)).ToList();
        }

        #endregion

        #region Edges

        #region Incoming

        public bool HasIncomingEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasIncomingEdge(myEdgeName);
        }

        public ServiceIncomingEdgeDefinition GetIncomingEdgeDefinition(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceIncomingEdgeDefinition(Response.GetIncomingEdgeDefinition(myEdgeName));
        }

        public bool HasIncomingEdges(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasIncomingEdges(myIncludeAncestorDefinitions);
        }

        public List<ServiceIncomingEdgeDefinition> GetIncomingEdgeDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetIncomingEdgeDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceIncomingEdgeDefinition(x)).ToList();
        }

        #endregion

        #region Outgoing

        public bool HasOutgoingEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasOutgoingEdge(myEdgeName);
        }

        public ServiceOutgoingEdgeDefinition GetOutgoingEdgeDefinition(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceOutgoingEdgeDefinition(Response.GetOutgoingEdgeDefinition(myEdgeName));
        }

        public bool HasOutgoingEdges(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasOutgoingEdges(myIncludeAncestorDefinitions);
        }

        public List<ServiceOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetOutgoingEdgeDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceOutgoingEdgeDefinition(x)).ToList();
        }

        #endregion

        #endregion

        #region Uniques

        public bool HasUniqueDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasUniqueDefinitions(myIncludeAncestorDefinitions);
        }

        public List<ServiceUniqueDefinition> GetUniqueDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetUniqueDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceUniqueDefinition(x)).ToList();
        }

        #endregion

        #region Indices
        
        public bool HasIndexDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasIndexDefinitions(myIncludeAncestorDefinitions);
        }

        public List<ServiceIndexDefinition> GetIndexDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetIndexDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceIndexDefinition(x)).ToList();
        }

        #endregion
    }
}

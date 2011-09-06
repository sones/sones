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
using sones.GraphDS.Services.RemoteAPIService.EdgeTypeService;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    public partial class RPCServiceContract : IEdgeTypeService
    {
        #region Inheritance

        public List<ServiceEdgeType> GetDescendantEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetDescendantEdgeTypes().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> GetDescendantEdgeTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetDescendantEdgeTypesAndSelf().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> GetAncestorEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetAncestorEdgeTypes().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> GetAncestorEdgeTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetAncestorEdgeTypesAndSelf().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> GetKinsmenEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetKinsmenEdgeTypes().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> GetKinsmenEdgeTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetKinsmenEdgeTypesAndSelf().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> ChildrenEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.ChildrenEdgeTypes.Select(x => new ServiceEdgeType(x)).ToList();
        }

        public ServiceEdgeType ParentEdgeType(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response.ParentEdgeType);
        }

        #endregion

        #region Inheritance

        public bool IsSealed(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.IsSealed;
        }

        public bool HasParentType(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasParentType;
        }

        public bool HasChildTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasChildTypes;
        }

        public bool IsAncestor(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myOtherType.Name);
            var BaseType = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.IsAncestor(BaseType);
        }

        public bool IsAncestorOrSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myOtherType.Name);
            var BaseType = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.IsAncestorOrSelf(BaseType);
        }

        public bool IsDescendant(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myOtherType.Name);
            var BaseType = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.IsDescendant(BaseType);
        }

        public bool IsDescendantOrSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myOtherType.Name);
            var BaseType = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.IsDescendantOrSelf(BaseType);
        }

        #endregion

        #region Attributes
        
        public bool HasAttribute(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, string myAttributeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasAttribute(myAttributeName);
        }

        public ServiceAttributeDefinition GetAttributeDefinition(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, string myAttributeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceAttributeDefinition(Response.GetAttributeDefinition(myAttributeName));
        }

        public ServiceAttributeDefinition GetAttributeDefinitionByID(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, long myAttributeID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceAttributeDefinition(Response.GetAttributeDefinition(myAttributeID));
        }

        public bool HasAttributes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasAttributes(myIncludeAncestorDefinitions);
        }

        public List<ServiceAttributeDefinition> GetAttributeDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetAttributeDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceAttributeDefinition(x)).ToList();
        }

        #endregion

        #region Properties

        public bool HasProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, string myAttributeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasProperty(myAttributeName);
        }

        public ServicePropertyDefinition GetPropertyDefinition(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, string myPropertyName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServicePropertyDefinition(Response.GetPropertyDefinition(myPropertyName));
        }

        public ServicePropertyDefinition GetPropertyDefinitionByID(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, long myPropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServicePropertyDefinition(Response.GetPropertyDefinition(myPropertyID));
        }

        public bool HasProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasProperties(myIncludeAncestorDefinitions);
        }

        public List<ServicePropertyDefinition> GetPropertyDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetPropertyDefinitions(myIncludeAncestorDefinitions).Select(x => new ServicePropertyDefinition(x)).ToList();
        }

        public List<ServicePropertyDefinition> GetPropertyDefinitionsByNameList(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, List<string> myPropertyNames)
        {
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetPropertyDefinitions(myPropertyNames).Select(x => new ServicePropertyDefinition(x)).ToList();
        }

        #endregion
    }
}

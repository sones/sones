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
using sones.GraphDS.Services.RemoteAPIService.ErrorHandling;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    public partial class RPCServiceContract : IEdgeTypeService
    {
        #region Inheritance

        public List<ServiceEdgeType> GetDescendantEdgeTypes(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetDescendantEdgeTypes().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> GetDescendantEdgeTypesAndSelf(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetDescendantEdgeTypesAndSelf().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> GetAncestorEdgeTypes(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetAncestorEdgeTypes().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> GetAncestorEdgeTypesAndSelf(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetAncestorEdgeTypesAndSelf().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> GetKinsmenEdgeTypes(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetKinsmenEdgeTypes().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> GetKinsmenEdgeTypesAndSelf(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetKinsmenEdgeTypesAndSelf().Select(x => new ServiceEdgeType(x)).ToList();
        }

        public List<ServiceEdgeType> ChildrenEdgeTypes(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.ChildrenEdgeTypes.Select(x => new ServiceEdgeType(x)).ToList();
        }

        public ServiceEdgeType ParentEdgeType(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceEdgeType(Response.ParentEdgeType);
        }

        #endregion

        #region Inheritance

        public bool IsSealed(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.IsSealed;
        }

        public bool HasParentType(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasParentType;
        }

        public bool HasChildTypes(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasChildTypes;
        }

        public bool IsAncestor(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myOtherType.Name);
            var BaseType = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.IsAncestor(BaseType);
        }

        public bool IsAncestorOrSelf(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myOtherType.Name);
            var BaseType = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.IsAncestorOrSelf(BaseType);
        }

        public bool IsDescendant(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myOtherType.Name);
            var BaseType = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.IsDescendant(BaseType);
        }

        public bool IsDescendantOrSelf(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myOtherType.Name);
            var BaseType = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.IsDescendantOrSelf(BaseType);
        }

        #endregion

        #region Attributes

        public bool HasAttribute(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, string myAttributeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasAttribute(myAttributeName);
        }

        public ServiceAttributeDefinition GetAttributeDefinition(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, string myAttributeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceAttributeDefinition(Response.GetAttributeDefinition(myAttributeName));
        }

        public ServiceAttributeDefinition GetAttributeDefinitionByID(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, long myAttributeID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServiceAttributeDefinition(Response.GetAttributeDefinition(myAttributeID));
        }

        public bool HasAttributes(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasAttributes(myIncludeAncestorDefinitions);
        }

        public List<ServiceAttributeDefinition> GetAttributeDefinitions(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetAttributeDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceAttributeDefinition(x)).ToList();
        }

        #endregion

        #region Properties

        public bool HasProperty(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, string myAttributeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasProperty(myAttributeName);
        }

        public ServicePropertyDefinition GetPropertyDefinition(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, string myPropertyName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServicePropertyDefinition(Response.GetPropertyDefinition(myPropertyName));
        }

        public ServicePropertyDefinition GetPropertyDefinitionByID(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, long myPropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return new ServicePropertyDefinition(Response.GetPropertyDefinition(myPropertyID));
        }

        public bool HasProperties(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.HasProperties(myIncludeAncestorDefinitions);
        }

        public List<ServicePropertyDefinition> GetPropertyDefinitions(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetPropertyDefinitions(myIncludeAncestorDefinitions).Select(x => new ServicePropertyDefinition(x)).ToList();
        }

        public List<ServicePropertyDefinition> GetPropertyDefinitionsByNameList(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeType myServiceEdgeType, List<string> myPropertyNames)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecurityToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetEdgeType(myServiceEdgeType.Name);
            var Response = this.GraphDS.GetEdgeType<IEdgeType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyEdgeType);
            return Response.GetPropertyDefinitions(myPropertyNames).Select(x => new ServicePropertyDefinition(x)).ToList();
        }

        #endregion
    }
}

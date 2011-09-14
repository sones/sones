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
using sones.GraphDS.Services.RemoteAPIService.ErrorHandling;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    
    public partial class RPCServiceContract :  IVertexTypeService
    {
        #region IBaseTypeServices

        #region Inheritance

        public List<ServiceVertexType> GetDescendantVertexTypes(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetDescendantVertexTypes().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetDescendantVertexTypesAndSelf(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetDescendantVertexTypesAndSelf().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetAncestorVertexTypes(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetAncestorVertexTypes().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetAncestorVertexTypesAndSelf(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetAncestorVertexTypesAndSelf().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetKinsmenVertexTypes(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetKinsmenVertexTypes().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetKinsmenVertexTypesAndSelf(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetKinsmenVertexTypesAndSelf().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> ChildrenVertexTypes(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.ChildrenVertexTypes.Select(x => new ServiceVertexType(x)).ToList();
        }

        public ServiceVertexType ParentVertexType(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response.ParentVertexType);
        }

        #endregion

        #region Inheritance

        public bool IsSealed(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsSealed;
        }

        public bool HasParentType(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasParentType;
        }

        public bool HasChildTypes(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasChildTypes;
        }

        public bool IsAncestor(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsAncestor(BaseType);
        }

        public bool IsAncestorOrSelf(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsAncestorOrSelf(BaseType);
        }

        public bool IsDescendant(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsDescendant(BaseType);
        }

        public bool IsDescendantOrSelf(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsDescendantOrSelf(BaseType);
        }

        #endregion

        #region Attributes

        public bool HasAttribute(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasAttribute(myAttributeName);
        }

        public ServiceAttributeDefinition GetAttributeDefinition(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceAttributeDefinition(Response.GetAttributeDefinition(myAttributeName));
        }

        public ServiceAttributeDefinition GetAttributeDefinitionByID(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, long myAttributeID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceAttributeDefinition(Response.GetAttributeDefinition(myAttributeID));
        }

        public bool HasAttributes(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasAttributes(myIncludeAncestorDefinitions);
        }

        public List<ServiceAttributeDefinition> GetAttributeDefinitions(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetAttributeDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceAttributeDefinition(x)).ToList();
        }

        #endregion

        #region Properties

        public bool HasProperty(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasProperty(myAttributeName);
        }

        public ServicePropertyDefinition GetPropertyDefinition(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myPropertyName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServicePropertyDefinition(Response.GetPropertyDefinition(myPropertyName));
        }

        public ServicePropertyDefinition GetPropertyDefinitionByID(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, long myPropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServicePropertyDefinition(Response.GetPropertyDefinition(myPropertyID));
        }

        public bool HasProperties(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasProperties(myIncludeAncestorDefinitions);
        }

        public List<ServicePropertyDefinition> GetPropertyDefinitions(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetPropertyDefinitions(myIncludeAncestorDefinitions).Select(x => new ServicePropertyDefinition(x)).ToList();
        }

        public List<ServicePropertyDefinition> GetPropertyDefinitionsByNameList(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, List<string> myPropertyNames)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetPropertyDefinitions(myPropertyNames).Select(x => new ServicePropertyDefinition(x)).ToList();
        }

        #endregion

        #endregion

        #region Incoming

        public bool HasBinaryProperty(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasBinaryProperty(myEdgeName);
        }

        public ServiceBinaryPropertyDefinition GetBinaryPropertyDefinition(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceBinaryPropertyDefinition(Response.GetBinaryPropertyDefinition(myEdgeName));
        }

        public bool HasBinaryProperties(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasBinaryProperties(myIncludeAncestorDefinitions);
        }

        public List<ServiceBinaryPropertyDefinition> GetBinaryProperties(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetBinaryProperties(myIncludeAncestorDefinitions).Select(x => new ServiceBinaryPropertyDefinition(x)).ToList();
        }

        #endregion

        #region Edges

        #region Incoming

        public bool HasIncomingEdge(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasIncomingEdge(myEdgeName);
        }

        public ServiceIncomingEdgeDefinition GetIncomingEdgeDefinition(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceIncomingEdgeDefinition(Response.GetIncomingEdgeDefinition(myEdgeName));
        }

        public bool HasIncomingEdges(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasIncomingEdges(myIncludeAncestorDefinitions);
        }

        public List<ServiceIncomingEdgeDefinition> GetIncomingEdgeDefinitions(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetIncomingEdgeDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceIncomingEdgeDefinition(x)).ToList();
        }

        #endregion

        #region Outgoing

        public bool HasOutgoingEdge(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasOutgoingEdge(myEdgeName);
        }

        public ServiceOutgoingEdgeDefinition GetOutgoingEdgeDefinition(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            var value = Response.GetOutgoingEdgeDefinition(myEdgeName);
            if (value != null)
                return new ServiceOutgoingEdgeDefinition(value);
            return null;
        }

        public bool HasOutgoingEdges(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasOutgoingEdges(myIncludeAncestorDefinitions);
        }

        public List<ServiceOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            var value = Response.GetOutgoingEdgeDefinitions(myIncludeAncestorDefinitions);
            if (value != null)
                return Response.GetOutgoingEdgeDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceOutgoingEdgeDefinition(x)).ToList();
            return null;
        }
        #endregion

        #endregion

        #region Uniques

        public bool HasUniqueDefinitions(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasUniqueDefinitions(myIncludeAncestorDefinitions);
        }

        public List<ServiceUniqueDefinition> GetUniqueDefinitions(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetUniqueDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceUniqueDefinition(x)).ToList();
        }

        #endregion

        #region Indices
        
        public bool HasIndexDefinitions(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasIndexDefinitions(myIncludeAncestorDefinitions);
        }

        public List<ServiceIndexDefinition> GetIndexDefinitions(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetIndexDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceIndexDefinition(x)).ToList();
        }

        #endregion
    }
}

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

        public List<ServiceVertexType> GetDescendantVertexTypes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetDescendantVertexTypes().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetDescendantVertexTypesAndSelf(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetDescendantVertexTypesAndSelf().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetAncestorVertexTypes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetAncestorVertexTypes().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetAncestorVertexTypesAndSelf(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetAncestorVertexTypesAndSelf().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetKinsmenVertexTypes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetKinsmenVertexTypes().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> GetKinsmenVertexTypesAndSelf(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetKinsmenVertexTypesAndSelf().Select(x => new ServiceVertexType(x)).ToList();
        }

        public List<ServiceVertexType> ChildrenVertexTypes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.ChildrenVertexTypes.Select(x => new ServiceVertexType(x)).ToList();
        }

        public ServiceVertexType ParentVertexType(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceVertexType(Response.ParentVertexType);
        }

        #endregion

        #region Inheritance

        public bool IsSealed(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsSealed;
        }

        public bool HasParentType(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasParentType;
        }

        public bool HasChildTypes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasChildTypes;
        }

        public bool IsAncestor(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsAncestor(BaseType);
        }

        public bool IsAncestorOrSelf(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsAncestorOrSelf(BaseType);
        }

        public bool IsDescendant(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsDescendant(BaseType);
        }

        public bool IsDescendantOrSelf(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myOtherType.Name);
            var BaseType = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.IsDescendantOrSelf(BaseType);
        }

        #endregion

        #region Attributes

        public bool HasAttribute(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasAttribute(myAttributeName);
        }

        public ServiceAttributeDefinition GetAttributeDefinition(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return ConvertHelper.ToServiceAttributeDefinition(Response.GetAttributeDefinition(myAttributeName));
        }

        public ServiceAttributeDefinition GetAttributeDefinitionByID(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, long myAttributeID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return ConvertHelper.ToServiceAttributeDefinition(Response.GetAttributeDefinition(myAttributeID));
        }

        public bool HasAttributes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasAttributes(myIncludeAncestorDefinitions);
        }

        public List<ServiceAttributeDefinition> GetAttributeDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetAttributeDefinitions(myIncludeAncestorDefinitions).Select(x => ConvertHelper.ToServiceAttributeDefinition(x)).ToList();
        }

        #endregion

        #region Properties

        public bool HasProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myPropertyName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasProperty(myPropertyName);
        }

        public ServicePropertyDefinition GetPropertyDefinitionByVertexType(SecurityToken mySecurityToken, Int64 myTransToken, String myServiceVertexTypeName, string myPropertyName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexTypeName);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            var blub = new ServicePropertyDefinition(Response.GetPropertyDefinition(myPropertyName));
            return blub;
        }

        public ServicePropertyDefinition GetPropertyDefinitionByIDByVertexType(SecurityToken mySecurityToken, Int64 myTransToken, String myServiceVertexTypeName, long myPropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexTypeName);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServicePropertyDefinition(Response.GetPropertyDefinition(myPropertyID));
        }

        public bool HasProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasProperties(myIncludeAncestorDefinitions);
        }

        public List<ServicePropertyDefinition> GetPropertyDefinitionsByVertexType(SecurityToken mySecurityToken, Int64 myTransToken, String myServiceVertexTypeName, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexTypeName);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetPropertyDefinitions(myIncludeAncestorDefinitions).Select(x => new ServicePropertyDefinition(x)).ToList();
        }

        public List<ServicePropertyDefinition> GetPropertyDefinitionsByNameListByVertexType(SecurityToken mySecurityToken, Int64 myTransToken, String myServiceVertexTypeName, List<string> myPropertyNames)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexTypeName);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetPropertyDefinitions(myPropertyNames).Select(x => new ServicePropertyDefinition(x)).ToList();
        }

        #endregion

        #endregion

        #region Incoming

        public bool HasBinaryProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myPropertyName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasBinaryProperty(myPropertyName);
        }

        public ServiceBinaryPropertyDefinition GetBinaryPropertyDefinition(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myPropertyName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceBinaryPropertyDefinition(Response.GetBinaryPropertyDefinition(myPropertyName));
        }

        public bool HasBinaryProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasBinaryProperties(myIncludeAncestorDefinitions);
        }

        public List<ServiceBinaryPropertyDefinition> GetBinaryPropertyDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetBinaryProperties(myIncludeAncestorDefinitions).Select(x => new ServiceBinaryPropertyDefinition(x)).ToList();
        }

        #endregion

        #region Edges

        #region Incoming

        public bool HasIncomingEdge(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasIncomingEdge(myEdgeName);
        }

        public ServiceIncomingEdgeDefinition GetIncomingEdgeDefinition(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return new ServiceIncomingEdgeDefinition(Response.GetIncomingEdgeDefinition(myEdgeName));
        }

        public bool HasIncomingEdges(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasIncomingEdges(myIncludeAncestorDefinitions);
        }

        public List<ServiceIncomingEdgeDefinition> GetIncomingEdgeDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetIncomingEdgeDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceIncomingEdgeDefinition(x)).ToList();
        }

        #endregion

        #region Outgoing

        public bool HasOutgoingEdge(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasOutgoingEdge(myEdgeName);
        }

        public ServiceOutgoingEdgeDefinition GetOutgoingEdgeDefinition(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            var value = Response.GetOutgoingEdgeDefinition(myEdgeName);
            if (value != null)
                return new ServiceOutgoingEdgeDefinition(value);
            return null;
        }

        public bool HasOutgoingEdges(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasOutgoingEdges(myIncludeAncestorDefinitions);
        }

        public List<ServiceOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            var value = Response.GetOutgoingEdgeDefinitions(myIncludeAncestorDefinitions);
            if (value != null)
                return Response.GetOutgoingEdgeDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceOutgoingEdgeDefinition(x)).ToList();
            return null;
        }
        #endregion

        #endregion

        #region Uniques

        public bool HasUniqueDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasUniqueDefinitions(myIncludeAncestorDefinitions);
        }

        public List<ServiceUniqueDefinition> GetUniqueDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetUniqueDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceUniqueDefinition(x)).ToList();
        }

        #endregion

        #region Indices
        
        public bool HasIndexDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.HasIndexDefinitions(myIncludeAncestorDefinitions);
        }

        public List<ServiceIndexDefinition> GetIndexDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertexType(myServiceVertexType.Name);
            var Response = this.GraphDS.GetVertexType<IVertexType>(mySecurityToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexType);
            return Response.GetIndexDefinitions(myIncludeAncestorDefinitions).Select(x => new ServiceIndexDefinition(x)).ToList();
        }

        #endregion
    }
}

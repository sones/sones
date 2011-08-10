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
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using System.ServiceModel;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexTypeServices;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.VertexType;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    [ServiceBehavior(Namespace = "http://www.sones.com")]
    public abstract class AbstractBaseTypeService : IBaseTypeService
    {
        private IGraphDS GraphDS;

        public AbstractBaseTypeService(IGraphDS myGraphDS)
        {
            this.GraphDS = myGraphDS;
        }

        public bool HasAttribute(ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            return GraphDS.GetVertexType<IVertexType>(null, null, new RequestGetVertexType(myServiceVertexType.ID), (Statistics, Type) => Type).HasAttribute(myAttributeName);
        }

        public ServiceAttributeDefinition GetAttributeDefinition(ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            return new ServiceAttributeDefinition(GraphDS.GetVertexType<IVertexType>
                (null, null, new RequestGetVertexType(myServiceVertexType.ID),
                (Statistics, Type) => Type).GetAttributeDefinition(myAttributeName));
        }

        public ServiceAttributeDefinition GetAttributeDefinitionByID(ServiceVertexType myServiceVertexType, long myAttributeID)
        {
            return new ServiceAttributeDefinition(GraphDS.GetVertexType<IVertexType>
                (null, null, new RequestGetVertexType(myServiceVertexType.ID),
                (Statistics, Type) => Type).GetAttributeDefinition(myAttributeID));
        }

        public bool HasAttributes(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            return GraphDS.GetVertexType<IVertexType>
                (null, null, new RequestGetVertexType(myServiceVertexType.ID),
                (Statistics, Type) => Type).HasAttributes(myIncludeAncestorDefinitions);
        }

        public IEnumerable<ServiceAttributeDefinition> GetAttributeDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasProperty(ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            throw new NotImplementedException();
        }

        public ServicePropertyDefinition GetPropertyDefinition(ServiceVertexType myServiceVertexType, string myPropertyName)
        {
            return new ServicePropertyDefinition(GraphDS.GetVertexType<IVertexType>
                (null, null, new RequestGetVertexType(myServiceVertexType.ID),
                (Statistics, Type) => Type).GetPropertyDefinition(myPropertyName));
          
        }

        public ServicePropertyDefinition GetPropertyDefinitionByID(ServiceVertexType myServiceVertexType, long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasProperties(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            return GraphDS.GetVertexType<IVertexType>
                (null, null, new RequestGetVertexType(myServiceVertexType.ID),
                (Statistics, Type) => Type).HasProperties(myIncludeAncestorDefinitions);
        }

        public IEnumerable<ServicePropertyDefinition> GetPropertyDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            return GraphDS.GetVertexType<IVertexType>
               (null, null, new RequestGetVertexType(myServiceVertexType.ID),
               (Statistics, Type) => Type).GetPropertyDefinitions(myIncludeAncestorDefinitions).Select(_=> new ServicePropertyDefinition(_));
        }

        public IEnumerable<ServicePropertyDefinition> GetPropertyDefinitionsByNameList(ServiceVertexType myServiceVertexType, IEnumerable<string> myPropertyNames)
        {
            throw new NotImplementedException();
        }
    }
}

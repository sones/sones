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
            throw new NotImplementedException();
        }

        public ServiceAttributeDefinition GetAttributeDefinition(ServiceVertexType myServiceVertexType, string myAttributeName)
        {
            throw new NotImplementedException();
        }

        public ServiceAttributeDefinition GetAttributeDefinitionByID(ServiceVertexType myServiceVertexType, long myAttributeID)
        {
            throw new NotImplementedException();
        }

        public bool HasAttributes(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
          
        }

        public ServicePropertyDefinition GetPropertyDefinitionByID(ServiceVertexType myServiceVertexType, long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasProperties(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServicePropertyDefinition> GetPropertyDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServicePropertyDefinition> GetPropertyDefinitionsByNameList(ServiceVertexType myServiceVertexType, IEnumerable<string> myPropertyNames)
        {
            throw new NotImplementedException();
        }
    }
}

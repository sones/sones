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
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexTypeServices
{
    [ServiceContract(Namespace = "http://www.sones.com")]
    interface IBaseTypeService
    {

        #region Attributes

        /// <summary>
        /// Has this vertex type a certain attribute?
        /// </summary>
        /// <returns>True or false</returns>
        [OperationContract]
        bool HasAttribute(ServiceVertexType myServiceVertexType, String myAttributeName);

        /// <summary>
        /// Gets a certain attribute definition
        /// </summary>
        /// <param name="myAttributeName">The name of the interesting attribute</param>
        /// <returns>A attribute definition</returns>
        [OperationContract]
        ServiceAttributeDefinition GetAttributeDefinition(ServiceVertexType myServiceVertexType, String myAttributeName);

        /// <summary>
        /// Gets a certain attribute definition
        /// </summary>
        /// <param name="myAttributeID">The id of the interesting attribute</param>
        /// <returns>A attribute definition</returns>
        [OperationContract]
        ServiceAttributeDefinition GetAttributeDefinitionByID(ServiceVertexType myServiceVertexType, Int64 myAttributeID);

        /// <summary>
        /// Has this vertex type any attributes?
        /// </summary>
        /// <returns>True or false</returns>
        [OperationContract]
        bool HasAttributes(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Gets all attributes defined on this vertex type.
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent vertex type(s)</param>
        /// <returns>An enumerable of attribute definitions</returns>
        [OperationContract]
        IEnumerable<ServiceAttributeDefinition> GetAttributeDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

        #endregion
        
        #region Properties

        /// <summary>
        /// Has this vertex type a certain property?
        /// </summary>
        /// <returns>True or false</returns>
        [OperationContract]
        bool HasProperty(ServiceVertexType myServiceVertexType, String myAttributeName);

        /// <summary>
        /// Gets a certain attribute definition
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <returns>A property definition</returns>
        [OperationContract]
        ServicePropertyDefinition GetPropertyDefinition(ServiceVertexType myServiceVertexType, String myPropertyName);

        /// <summary>
        /// Gets a certain attribute definition
        /// </summary>
        /// <param name="myPropertyID">The id of the property</param>
        /// <returns>A property definition</returns>
        [OperationContract]
        ServicePropertyDefinition GetPropertyDefinitionByID(ServiceVertexType myServiceVertexType, Int64 myPropertyID);

        /// <summary>
        /// Has this vertex type any properties?
        /// </summary>
        /// <returns>True or false</returns>
        [OperationContract]
        bool HasProperties(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Gets all properties defined on this vertex type.
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent vertex type(s)</param>
        /// <returns>An enumerable of property definitions</returns>
        [OperationContract]
        IEnumerable<ServicePropertyDefinition> GetPropertyDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Gets the properties with the given name.
        /// </summary>
        /// <param name="myPropertyNames">A list of peroperty names.</param>
        /// <returns>An enumerable of property definitions</returns>
        [OperationContract]
        IEnumerable<ServicePropertyDefinition> GetPropertyDefinitionsByNameList(ServiceVertexType myServiceVertexType, IEnumerable<string> myPropertyNames);

        #endregion

        
    }
}

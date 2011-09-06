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
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexTypeServices;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts;


namespace sones.GraphDS.Services.RemoteAPIService.EdgeTypeService
{
    [ServiceContract(Namespace = sonesRPCServer.Namespace, Name = "EdgeTypeService")]
    
    public interface IEdgeTypeService
    {
        #region IBaseTypeServices

        #region Inheritance

        /// <summary>
        /// Returns the descendant of this IEdgeType.
        /// </summary>
        /// <returns>An enumeration of ServiceEdgeType that are descendant of this IEdgeType.</returns>
        /// <seealso cref="IBaseType.GetDescendantTypes"/>
        [OperationContract]
        List<ServiceEdgeType> GetDescendantEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType);

        /// <summary>
        /// Returns the descendant of this IEdgeType and this IEdgeType in one enumeration.
        /// </summary>
        /// <returns>An enumeration of IEdgeType that are descendant of this IEdgeType and this IEdgeType itself.</returns>
        /// <seealso cref="IBaseType.GetDescendantTypesAndSelf"/>
        [OperationContract]
        List<ServiceEdgeType> GetDescendantEdgeTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType);

        /// <summary>
        /// Returns the ancestor of this IEdgeType.
        /// </summary>
        /// <returns>An enumeration of IEdgeType that are ancestors of this IEdgeType.</returns>
        /// <seealso cref="IBaseType.GetAncestorTypes"/>
        [OperationContract]
        List<ServiceEdgeType> GetAncestorEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType);

        /// <summary>
        /// Returns the ancestor of this IEdgeType and this IEdgeType in one enumeration.
        /// </summary>
        /// <returns>An enumeration of IEdgeType that are ancestors of this IEdgeType and this IEdgeType itself.</returns>
        /// <seealso cref="IBaseType.GetAncestorTypesAndSelf"/>
        [OperationContract]
        List<ServiceEdgeType> GetAncestorEdgeTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType);

        /// <summary>
        /// Returns all descendant and ancestors of this IEdgeType.
        /// </summary>
        /// <returns>An enumeration of all IEdgeType that are ancestors or descendant of this IEdgeType.</returns>
        /// <seealso cref="IBaseType.GetKinsmenTypes"/>
        [OperationContract]
        List<ServiceEdgeType> GetKinsmenEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType);

        /// <summary>
        /// Returns all descendant and ancestors of this IEdgeType and this IEdgeType in one enumeration. 
        /// </summary>
        /// <returns>An enumeration of all IEdgeType that are ancestors or descendant of this IEdgeType and this IEdgeType itself.</returns>
        /// <seealso cref="IBaseType.GetKinsmenTypesAndSelf"/>
        [OperationContract]
        List<ServiceEdgeType> GetKinsmenEdgeTypesAndSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType);

        /// <summary>
        /// Returns the direct children of this IEdgeType.
        /// </summary>
        /// <seealso cref="IBaseType.ChildrenTypes"/>
        [OperationContract]
        List<ServiceEdgeType> ChildrenEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType);

        /// <summary>
        /// Gets the parent of this IEdgeType.
        /// </summary>
        /// <seealso cref="IBaseType.ParentType"/>
        [OperationContract]
        ServiceEdgeType ParentEdgeType(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType);

        #endregion

        #region Inheritance

        /// <summary>
        /// Defines whether this type can be used as parent type.
        /// </summary>
        /// <value>
        /// If true, this edge type must not be used as a parent edge type.
        /// </value>
        [OperationContract(Name="IsSealedByEdgeType")]
        Boolean IsSealed(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType);

        /// <summary>
        /// Has this type a parent type?
        /// </summary>
        /// <returns>True, if this type has a parent type, otherwise false.</returns>
        [OperationContract(Name="HasParentTypeByEdgeType")]
        bool HasParentType(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType);

        /// <summary>
        /// Has this type child types?
        /// </summary>
        /// <returns>False, if this type has no child types, otherwise true.</returns>
        [OperationContract(Name = "HasChildTypesByEdgeType")]
        bool HasChildTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType);

        /// <summary>
        /// Returns if the given type is an ancestor of the current type.
        /// </summary>
        /// <param name="myOtherType">The given type.</param>
        /// <returns>True, if the given type is an ancestor of the current type, otherwise false.</returns>
        [OperationContract(Name = "IsAncestorByEdgeType")]
        bool IsAncestor(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType);

        /// <summary>
        /// Returns if the given type is an ancestor of or the current itself.
        /// </summary>
        /// <param name="myOtherType">The given type.</param>
        /// <returns>True, if the given type is an ancestor of the current type or the current type itself, otherwise false.</returns>
        [OperationContract(Name = "IsAncestorOrSelfByEdgeType")]
        bool IsAncestorOrSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType);

        /// <summary>
        /// Returns if the given type is a descendant of the current type.
        /// </summary>
        /// <param name="myOtherType">The given type.</param>
        /// <returns>True, if the given type is a descendant of the current type, otherwise false.</returns>
        [OperationContract(Name = "IsDescendantByEdgeType")]
        bool IsDescendant(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType);

        /// <summary>
        /// Returns if the given type is a descendant of or the current type itself.
        /// </summary>
        /// <param name="myOtherType">The given type.</param>
        /// <returns>True, if the given type is a descendant of the current type or the current type itself, otherwise false.</returns>
        [OperationContract(Name = "IsDescendantOrSelfByEdgeType")]
        bool IsDescendantOrSelf(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, ServiceEdgeType myOtherType);

        #endregion

        #region Attributes

        /// <summary>
        /// Has this edge type a certain attribute?
        /// </summary>
        /// <returns>True or false</returns>
        [OperationContract(Name = "HasAttributeByEdgeType")]
        bool HasAttribute(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, String myAttributeName);

        /// <summary>
        /// Gets a certain attribute definition
        /// </summary>
        /// <param name="myAttributeName">The name of the interesting attribute</param>
        /// <returns>A attribute definition</returns>
        [OperationContract(Name = "GetAttributeDefinitionByEdgeType")]
        ServiceAttributeDefinition GetAttributeDefinition(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, String myAttributeName);

        /// <summary>
        /// Gets a certain attribute definition
        /// </summary>
        /// <param name="myAttributeID">The id of the interesting attribute</param>
        /// <returns>A attribute definition</returns>
        [OperationContract(Name = "GetAttributeDefinitionByIDByEdgeType")]
        ServiceAttributeDefinition GetAttributeDefinitionByID(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, Int64 myAttributeID);

        /// <summary>
        /// Has this edge type any attributes?
        /// </summary>
        /// <returns>True or false</returns>
        [OperationContract(Name = "HasAttributesByEdgeType")]
        bool HasAttributes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Gets all attributes defined on this edge type.
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent edge type(s)</param>
        /// <returns>An enumerable of attribute definitions</returns>
        [OperationContract(Name = "GetAttributeDefinitionsByEdgeType")]
        List<ServiceAttributeDefinition> GetAttributeDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions);

        #endregion

        #region Properties

        /// <summary>
        /// Has this edge type a certain property?
        /// </summary>
        /// <returns>True or false</returns>
        [OperationContract(Name = "HasProprtyByEdgeType")]
        bool HasProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, String myAttributeName);

        /// <summary>
        /// Gets a certain attribute definition
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <returns>A property definition</returns>
        [OperationContract(Name = "GetPropertyDefinitionByEdgeType")]
        ServicePropertyDefinition GetPropertyDefinition(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, String myPropertyName);

        /// <summary>
        /// Gets a certain attribute definition
        /// </summary>
        /// <param name="myPropertyID">The id of the property</param>
        /// <returns>A property definition</returns>
        [OperationContract(Name = "GetPropertyDefinitionByIDByEdgeType")]
        ServicePropertyDefinition GetPropertyDefinitionByID(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, Int64 myPropertyID);

        /// <summary>
        /// Has this edge type any properties?
        /// </summary>
        /// <returns>True or false</returns>
        [OperationContract(Name = "HasPropertiesByEdgeType")]
        bool HasProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Gets all properties defined on this edge type.
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent edge type(s)</param>
        /// <returns>An enumerable of property definitions</returns>
        [OperationContract(Name = "GetPropertyDefinitionsByEdgeType")]
        List<ServicePropertyDefinition> GetPropertyDefinitions(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Gets the properties with the given name.
        /// </summary>
        /// <param name="myPropertyNames">A list of peroperty names.</param>
        /// <returns>An enumerable of property definitions</returns>
        [OperationContract(Name = "GetPropertyDefinitionsByNameListByEdgeType")]
        List<ServicePropertyDefinition> GetPropertyDefinitionsByNameList(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeType myServiceEdgeType, List<string> myPropertyNames);

        #endregion

        #endregion
    }
}

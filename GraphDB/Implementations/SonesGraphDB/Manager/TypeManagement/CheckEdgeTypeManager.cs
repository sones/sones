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
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.LanguageExtensions;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Request;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class CheckEdgeTypeManager: ACheckTypeManager<IEdgeType>
    {
        #region ACheckTypeManager member

        public override IEdgeType AlterType(IRequestAlterType myAlterTypeRequest,
                                            Int64 myTransactionToken,
                                            SecurityToken mySecurityToken,
                                            out RequestUpdate myUpdateRequest)
        {
            CheckRequestType(myAlterTypeRequest);

            RequestAlterEdgeType myRequest = myAlterTypeRequest as RequestAlterEdgeType;

            var edgeType = _TypeManager.GetType(myRequest.TypeName,
                                                    myTransactionToken,
                                                    mySecurityToken);

            #region check to be added

            if (myRequest.ToBeAddedUnknownAttributes != null)
            {
                var toBeConverted = myRequest.ToBeAddedUnknownAttributes.ToArray();

                foreach (var unknown in toBeConverted)
                {
                    if (_baseTypeManager.IsBaseType(unknown.AttributeType))
                    {
                        var prop = ConvertUnknownToProperty(unknown);

                        myRequest.AddProperty(prop);
                    }
                    else
                        throw new InvalidAttributeTypeException(unknown.AttributeName, 
                                                                unknown.AttributeType, 
                                                                "This type is an invalid attribute type for an edge type!");
                }

                myRequest.ResetUnknown();
            }

            #endregion

            #region check to be removed

            if (myRequest.ToBeRemovedUnknownAttributes != null)
            {
                foreach (var unknownProp in myRequest.ToBeRemovedUnknownAttributes)
                {
                    var attrDef = edgeType.GetAttributeDefinition(unknownProp);

                    if (attrDef == null)
                        throw new AttributeDoesNotExistException(unknownProp, edgeType.Name);

                    switch (attrDef.Kind)
                    {
                        case AttributeType.Property:
                            myRequest.RemoveProperty(unknownProp);
                            break;

                        case AttributeType.OutgoingEdge:
                            throw new Exception("Invalid AttributeType [OutgoingEdge] in alter type request!");

                        case AttributeType.IncomingEdge:
                            throw new Exception("Invalid AttributeType [IncomingEdge] in alter type request!");

                        case AttributeType.BinaryProperty:
                            throw new Exception("Invalid AttributeType [BinaryProperty] in alter type request!");

                        default:
                            throw new Exception("The enumeration AttributeType was changed, but not this switch statement.");
                    }
                }

                myRequest.ClearToBeRemovedUnknownAttributes();
            }

            #endregion

            #region check attributes to be defined
            if (myRequest.ToBeDefinedAttributes != null)
            {
                foreach (var unknownProp in myRequest.ToBeDefinedAttributes)
                {
                    var toBeDefined = myRequest.ToBeDefinedAttributes.ToArray();

                    foreach (var unknown in toBeDefined)
                    {
                        if (BinaryPropertyPredefinition.TypeName.Equals(unknown.AttributeType))
                        {
                            throw new InvalidDefineAttributeTypeException(BinaryPropertyPredefinition.TypeName, edgeType.Name);
                        }
                        else if (unknown.AttributeType.Contains(IncomingEdgePredefinition.TypeSeparator))
                        {
                            throw new InvalidDefineAttributeTypeException("incoming edge", edgeType.Name);
                        }
                        else if (!_baseTypeManager.IsBaseType(unknown.AttributeType))
                        {
                            throw new InvalidDefineAttributeTypeException("user defined", edgeType.Name);
                        }
                    }
                }
            }
            #endregion

            #region check attributes to be undefined

            if (myRequest.ToBeUndefinedAttributes != null)
            {
                foreach (var attr in myRequest.ToBeUndefinedAttributes)
                {
                    var attrDef = edgeType.GetAttributeDefinition(attr);

                    if (attrDef == null)
                        throw new AttributeDoesNotExistException(attr);

                    switch (attrDef.Kind)
                    {
                        case AttributeType.Property:
                            break;

                        case AttributeType.OutgoingEdge:
                            throw new InvalidUndefineAttributeTypeException("Outgoing Edge", edgeType.Name);

                        case AttributeType.IncomingEdge:
                            throw new InvalidUndefineAttributeTypeException("Incoming Edge", edgeType.Name);

                        case AttributeType.BinaryProperty:
                            throw new InvalidUndefineAttributeTypeException(BinaryPropertyPredefinition.TypeName, edgeType.Name);

                        default:
                            throw new Exception("The enumeration AttributeType was changed, but not this switch statement.");
                    }
                }
            }

            #endregion

            #region checks

            CallCheckFunctions(myAlterTypeRequest, edgeType, myTransactionToken, mySecurityToken);

            #endregion

            myUpdateRequest = new RequestUpdate();

            return null;
        }

        public override void Initialize(IMetaManager myMetaManager)
        {
            _TypeManager        = myMetaManager.EdgeTypeManager.ExecuteManager;
            _baseTypeManager    = myMetaManager.BaseTypeManager;
            _baseStorageManager = myMetaManager.BaseGraphStorageManager;
        }

        public override void Load(Int64 myTransaction, 
                                    SecurityToken mySecurity)
        { }

        #endregion

        #region private helper

        #region private abstract helper

        /// <summary>
        /// Checks if the given type is a base type
        /// </summary>
        protected override bool IsTypeBaseType(long myTypeID)
        {
            return ((long)BaseTypes.Edge).Equals(myTypeID) ||
                        ((long)BaseTypes.Orderable).Equals(myTypeID) ||
                        ((long)BaseTypes.Weighted).Equals(myTypeID);
        }

        /// <summary>
        /// Checks if the given type is a base type
        /// </summary>
        protected override bool IsTypeBaseType(String myTypeName)
        {
            BaseTypes type;
            if (!Enum.TryParse(myTypeName, out type))
                return false;

            return true;
        }

        /// <summary>
        /// Converts the properties which marked as unique into unique predefinitions.
        /// </summary>
        /// <param name="myTypePredefinition">The type predefinition.</param>
        protected override void ConvertPropertyUniques(ATypePredefinition myTypePredefinition)
        {
            if (myTypePredefinition.Properties != null)
                foreach (var uniqueProp in myTypePredefinition.Properties.Where(_ => _.IsUnique))
                {
                    (myTypePredefinition as VertexTypePredefinition)
                        .AddUnique(new UniquePredefinition(uniqueProp.AttributeName));
                }
        }

        /// <summary>
        /// Checks if the given paramter type is valid.
        /// </summary>
        /// <param name="myTypePredefinitions">The parameter to be checked.</param>
        protected override void CheckPredefinitionsType(IEnumerable<ATypePredefinition> myTypePredefinitions)
        {
            if (!(myTypePredefinitions is IEnumerable<EdgeTypePredefinition>))
                if (!myTypePredefinitions.All(_ => _ is EdgeTypePredefinition))
                    throw new InvalidParameterTypeException("TypePredefinitions",
                                                            myTypePredefinitions.GetType().Name,
                                                            typeof(IEnumerable<EdgeTypePredefinition>).GetType().Name,
                                                            "");
        }

        protected override void ConvertUnknownAttributes(ATypePredefinition myTypePredefinition)
        {
            if (myTypePredefinition.UnknownAttributes == null)
                return;

            var toBeConverted = myTypePredefinition.UnknownAttributes.ToArray();

            foreach (var unknown in toBeConverted)
            {
                if (_baseTypeManager.IsBaseType(unknown.AttributeType))
                {
                    var prop = ConvertUnknownToProperty(unknown);

                    (myTypePredefinition as EdgeTypePredefinition).AddProperty(prop);
                }
                else
                    throw new PropertyHasWrongTypeException(myTypePredefinition.TypeName, unknown.AttributeName, unknown.Multiplicity, "a base type");
            }

            myTypePredefinition.ResetUnknown();
        }

        /// <summary>
        /// Checks whether a given type name is not a basix vertex type.
        /// </summary>
        /// <param name="myTypeName">The type name to be checked.</param>
        /// <returns>True, if the type name is the name of a base vertex type (but Vertex), otherwise false.</returns>
        protected override bool CanBaseTypeBeParentType(string myTypeName)
        {
            BaseTypes type;
            if (!Enum.TryParse(myTypeName, out type))
                return false;

            return type == BaseTypes.Edge;
        }

        /// <summary>
        /// Checks the uniqueness of attribute names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        protected override void CheckAttributes(ATypePredefinition myTypePredefinition)
        {
            var uniqueNameSet = new HashSet<string>();

            CheckPropertiesUniqueName(myTypePredefinition, uniqueNameSet);
        }

        /// <summary>
        /// Checks if tje given types can be removed.
        /// </summary>
        /// <param name="myTypes">The to be removed types.</param>
        /// <param name="myTransaction">Int64</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <param name="myIgnoreReprimands">Marks if reprimands are ignored on the to be removed types.</param>
        protected override void CanRemove(IEnumerable<IEdgeType> myTypes,
                                            Int64 myTransaction,
                                            SecurityToken mySecurity,
                                            bool myIgnoreReprimands)
        {
            #region check if specified types can be removed

            //get child vertex types and check if they are specified by user
            foreach (var delType in myTypes)
            {
                var temp = GetType(delType.ID, myTransaction, mySecurity);

                #region check that the remove type is no base type

                if (delType == null)
                    throw new TypeRemoveException<IEdgeType>("null", "Edge Type is null.");

                if (!delType.HasParentType)
                    //type must be base type because there is no parent type, Exception that base type cannot be deleted
                    throw new TypeRemoveException<IEdgeType>(delType.Name, "A BaseType connot be removed.");

                if (IsTypeBaseType(delType.ID))
                    //Exception that base type cannot be deleted
                    throw new TypeRemoveException<IEdgeType>(delType.Name, "A BaseType connot be removed.");

                #endregion

                if (!myIgnoreReprimands)
                {
                    #region check that existing child types are specified

                    if (!delType.GetDescendantEdgeTypes().All(child => myTypes.Contains(child)))
                        throw new TypeRemoveException<IEdgeType>(delType.Name, "The given type has child types and cannot be removed.");

                    #endregion
                }
            }

            #endregion
        }

        /// <summary>
        /// Calls various check function which check the specified member inside the RequestAlterEdgeType
        /// </summary>
        /// <param name="myAlterTypeRequest">The request which contains the to be checked member.</param>
        /// <param name="myType">The Type which is returned.</param>
        /// <param name="myTransactionToken">The Int64.</param>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        protected override void CallCheckFunctions(IRequestAlterType myAlterTypeRequest,
                                                    IEdgeType myType,
                                                    Int64 myTransactionToken,
                                                    SecurityToken mySecurityToken)
        {
            CheckAttributesNameAndType(myAlterTypeRequest);

            CheckToBeAddedAttributes(myAlterTypeRequest, myType);
            CheckToBeDefinedAttributes(myAlterTypeRequest, myType);
            CheckToBeRemovedAttributes(myAlterTypeRequest, myType);
            CheckToBeRenamedAttributes(myAlterTypeRequest, myType);
            CheckNewTypeName(myAlterTypeRequest.AlteredTypeName, myTransactionToken, mySecurityToken);
        }

        /// <summary>
        /// Calls a Check function foreach IEnumerable of definied attributes.
        /// </summary>
        /// <param name="myRequest">The alter type request which contains the attributes.</param>
        protected override void CheckAttributesNameAndType(IRequestAlterType myRequest)
        {
            CheckNameAndTypeOfAttributePredefinitions(myRequest.ToBeAddedProperties);
            CheckNameAndTypeOfAttributePredefinitions(myRequest.ToBeAddedUnknownAttributes);

            CheckNameOfAttributeList(myRequest.ToBeRemovedProperties);
            CheckNameOfAttributeList(myRequest.ToBeRemovedUnknownAttributes);
        }

        /// <summary>
        /// Checks if the new vertex type name already exists
        /// </summary>
        /// <param name="myAlteredTypeName">The new name.</param>
        /// <param name="mySecurityToken">Int64.</param>
        /// <param name="myTransactionToken">SecurityToken.</param>
        protected override void CheckNewTypeName(String myAlteredTypeName, 
                                                    Int64 myTransactionToken, 
                                                    SecurityToken mySecurityToken)
        {
            if (myAlteredTypeName != null && _TypeManager.HasType(myAlteredTypeName, myTransactionToken, mySecurityToken))
            {
                throw new EdgeTypeAlreadyExistException(myAlteredTypeName);
            }
        }

        /// <summary>
        /// Checks if the to be added attributes exist in the given vertex type or derived oness
        /// </summary>
        /// <param name="myAlterVertexTypeRequest"></param>
        /// <param name="vertexType"></param>
        protected override void CheckToBeAddedAttributes(IRequestAlterType myAlterTypeRequest, 
                                                            IEdgeType myType)
        {
            foreach (var aType in myType.GetDescendantEdgeTypesAndSelf())
            {
                var attributesOfCurrentVertexType = aType.GetAttributeDefinitions(false).ToList();

                #region property

                if (myAlterTypeRequest.ToBeAddedProperties != null)
                {
                    foreach (var aToBeAddedAttribute in myAlterTypeRequest.ToBeAddedProperties)
                    {
                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                            throw new AttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                    }
                }

                #endregion

                #region unknown attributes

                if (myAlterTypeRequest.ToBeAddedUnknownAttributes != null)
                {
                    foreach (var aToBeAddedAttribute in myAlterTypeRequest.ToBeAddedUnknownAttributes)
                    {
                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                        {
                            throw new AttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                        }
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// Checks the attributes that should be defined.
        /// </summary>
        /// <param name="myAlterTypeRequest">The alter type request.</param>
        /// <param name="myType">The type.</param>
        protected override void CheckToBeDefinedAttributes(IRequestAlterType myAlterTypeRequest,
                                                            IEdgeType myType)
        {
            var request = myAlterTypeRequest as RequestAlterEdgeType;

            foreach (var aEdgeType in myType.GetDescendantEdgeTypesAndSelf())
            {
                var attributesOfCurrentEdgeType = aEdgeType.GetAttributeDefinitions(false).ToList();

                if (request.ToBeDefinedAttributes != null)
                {
                    foreach (var aToBeDefinedAttribute in request.ToBeDefinedAttributes)
                    {
                        if (attributesOfCurrentEdgeType.Any(_ => _.Name == aToBeDefinedAttribute.AttributeName))
                            throw new AttributeAlreadyExistsException(aToBeDefinedAttribute.AttributeName);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the to be removed attributes exists on this type
        /// </summary>
        /// <param name="myAlterTypeRequest">The request.</param>
        /// <param name="myType">The type.</param>
        protected override void CheckToBeRemovedAttributes(IRequestAlterType myAlterTypeRequest, 
                                                            IEdgeType myType)
        {
            #region properties

            var attributesOfCurrentVertexType = myType.GetAttributeDefinitions(false).ToList();

            if (myAlterTypeRequest.ToBeRemovedProperties != null)
            {
                foreach (var aToBeDeletedAttribute in myAlterTypeRequest.ToBeRemovedProperties)
                {
                    if (!attributesOfCurrentVertexType.Any(_ => _.Name == aToBeDeletedAttribute))
                    {
                        throw new AttributeDoesNotExistException(aToBeDeletedAttribute, myType.Name);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Checks if the given parameter type is valid.
        /// </summary>
        /// <param name="myRequest">The parameter to be checked.</param>
        protected override void CheckRequestType(IRequestAlterType myRequest)
        {
            if (!(myRequest is RequestAlterEdgeType))
                throw new InvalidParameterTypeException("AlterTypeRequest", myRequest.GetType().Name, typeof(RequestAlterEdgeType).Name);
        }

        /// <summary>
        /// Checks whether a vertex type predefinition is not sealed and abstract.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The vertex type predefinition to be checked.</param>
        protected override void CheckSealedAndAbstract(ATypePredefinition myTypePredefinition)
        {
            if (myTypePredefinition.IsSealed)
                throw new UselessTypeException(myTypePredefinition);
        }

        #endregion

        #endregion
    }
}

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
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.Library.LanguageExtensions;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.Request;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class CheckVertexTypeManager : ACheckTypeManager<IVertexType>
    {
        #region ACheckTypeManager member

        public override IVertexType AlterType(IRequestAlterType myAlterTypeRequest,
                                                TransactionToken myTransactionToken,
                                                SecurityToken mySecurityToken)
        {
            CheckRequestType(myAlterTypeRequest);

            RequestAlterVertexType myRequest = myAlterTypeRequest as RequestAlterVertexType;

            var vertexType = _TypeManager.GetType(myRequest.TypeName,
                                                    myTransactionToken,
                                                    mySecurityToken);

            #region check to be added

            if (myRequest.ToBeAddedUnknownAttributes != null)
            {
                var toBeConverted = myRequest.ToBeAddedUnknownAttributes.ToArray();

                foreach (var unknown in toBeConverted)
                {
                    if (BinaryPropertyPredefinition.TypeName.Equals(unknown.AttributeType))
                    {
                        var prop = ConvertUnknownToBinaryProperty(unknown);

                        myRequest.AddBinaryProperty(prop);
                    }
                    else if (_baseTypeManager.IsBaseType(unknown.AttributeType))
                    {
                        var prop = ConvertUnknownToProperty(unknown);

                        myRequest.AddProperty(prop);
                    }
                    else if (unknown.AttributeType.Contains(IncomingEdgePredefinition.TypeSeparator))
                    {
                        var prop = ConvertUnknownToIncomingEdge(unknown);
                        myRequest.AddIncomingEdge(prop);
                    }
                    else
                    {
                        var prop = ConvertUnknownToOutgoingEdge(unknown);
                        myRequest.AddOutgoingEdge(prop);
                    }
                }

                myRequest.ResetUnknown();
            }

            #endregion

            #region check to be removed

            if (myRequest.ToBeRemovedUnknownAttributes != null)
            {
                foreach (var unknownProp in myRequest.ToBeRemovedUnknownAttributes)
                {
                    var attrDef = vertexType.GetAttributeDefinition(unknownProp);

                    if (attrDef == null)
                        throw new AttributeDoesNotExistException(unknownProp);

                    switch (attrDef.Kind)
                    {
                        case AttributeType.Property:
                            myRequest.RemoveProperty(unknownProp);
                            break;

                        case AttributeType.OutgoingEdge:
                            myRequest.RemoveOutgoingEdge(unknownProp);
                            break;

                        case AttributeType.IncomingEdge:
                            myRequest.RemoveIncomingEdge(unknownProp);
                            break;

                        case AttributeType.BinaryProperty:
                            myRequest.RemoveBinaryProperty(unknownProp);
                            break;

                        default:
                            throw new Exception("The enumeration AttributeType was changed, but not this switch statement.");
                    }
                }

                myRequest.ClearToBeRemovedUnknownAttributes();
            }

            #endregion

            #region checks

            CallCheckFunctions(myAlterTypeRequest, vertexType, myTransactionToken, mySecurityToken);

            #endregion

            return null;
        }

        public override void Initialize(IMetaManager myMetaManager)
        {
            _TypeManager = myMetaManager.VertexTypeManager.ExecuteManager;
            _baseTypeManager = myMetaManager.BaseTypeManager;
            _baseStorageManager = myMetaManager.BaseGraphStorageManager;
        }

        public override void Load(TransactionToken myTransaction, SecurityToken mySecurity)
        { }

        #endregion

        #region private helper

        #region private abstract helper

        /// <summary>
        /// Checks if the given type is a base type
        /// </summary>
        protected override bool IsTypeBaseType(long myTypeID)
        {
            return ((long)BaseTypes.Attribute).Equals(myTypeID) ||
                        ((long)BaseTypes.BaseType).Equals(myTypeID) ||
                        ((long)BaseTypes.BinaryProperty).Equals(myTypeID) ||
                        ((long)BaseTypes.EdgeType).Equals(myTypeID) ||
                        ((long)BaseTypes.IncomingEdge).Equals(myTypeID) ||
                        ((long)BaseTypes.Index).Equals(myTypeID) ||
                        ((long)BaseTypes.OutgoingEdge).Equals(myTypeID) ||
                        ((long)BaseTypes.Property).Equals(myTypeID) ||
                        ((long)BaseTypes.Vertex).Equals(myTypeID) ||
                        ((long)BaseTypes.VertexType).Equals(myTypeID);
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
            if (!(myTypePredefinitions is IEnumerable<VertexTypePredefinition>))
                if (!myTypePredefinitions.All(_ => _ is VertexTypePredefinition))
                    throw new InvalidParameterTypeException("TypePredefinitions",
                                                            myTypePredefinitions.GetType().Name,
                                                            typeof(IEnumerable<VertexTypePredefinition>).GetType().Name,
                                                            "");
        }

        protected override void ConvertUnknownAttributes(ATypePredefinition myTypePredefinitions)
        {
            if (myTypePredefinitions.UnknownAttributes == null)
                return;

            var toBeConverted = myTypePredefinitions.UnknownAttributes.ToArray();

            foreach (var unknown in toBeConverted)
            {
                if (BinaryPropertyPredefinition.TypeName.Equals(unknown.AttributeType))
                {
                    var prop = ConvertUnknownToBinaryProperty(unknown);

                    (myTypePredefinitions as VertexTypePredefinition).AddBinaryProperty(prop);
                }
                else if (_baseTypeManager.IsBaseType(unknown.AttributeType))
                {
                    var prop = ConvertUnknownToProperty(unknown);

                    (myTypePredefinitions as VertexTypePredefinition).AddProperty(prop);
                }
                else if (unknown.AttributeType.Contains(IncomingEdgePredefinition.TypeSeparator))
                {
                    var prop = ConvertUnknownToIncomingEdge(unknown);
                    (myTypePredefinitions as VertexTypePredefinition).AddIncomingEdge(prop);
                }
                else
                {
                    var prop = ConvertUnknownToOutgoingEdge(unknown);
                    (myTypePredefinitions as VertexTypePredefinition).AddOutgoingEdge(prop);
                }
            }

            myTypePredefinitions.ResetUnknown();
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

            return type == BaseTypes.Vertex;
        }

        /// <summary>
        /// Checks the uniqueness of attribute names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        protected override void CheckAttributes(ATypePredefinition myTypePredefinitions)
        {
            var uniqueNameSet = new HashSet<string>();

            CheckIncomingEdgesUniqueName((myTypePredefinitions as VertexTypePredefinition), uniqueNameSet);
            CheckOutgoingEdgesUniqueName((myTypePredefinitions as VertexTypePredefinition), uniqueNameSet);
            CheckPropertiesUniqueName((myTypePredefinitions as VertexTypePredefinition), uniqueNameSet);
            CheckBinaryPropertiesUniqueName((myTypePredefinitions as VertexTypePredefinition), uniqueNameSet);
        }

        protected override void CanRemove(IEnumerable<IVertexType> myTypes,
                                            TransactionToken myTransaction,
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
                    throw new TypeRemoveException<IVertexType>("null", "Vertex Type is null.");

                if (!delType.HasParentType)
                    //type must be base type because there is no parent type, Exception that base type cannot be deleted
                    throw new TypeRemoveException<IVertexType>(delType.Name, "A BaseType connot be removed.");

                if (IsTypeBaseType(delType.ID))
                    //Exception that base type cannot be deleted
                    throw new TypeRemoveException<IVertexType>(delType.Name, "A BaseType connot be removed.");

                #endregion

                if (!myIgnoreReprimands)
                {
                    #region check that existing child types are specified

                    if (!delType.GetDescendantVertexTypes().All(child => myTypes.Contains(child)))
                        throw new TypeRemoveException<IVertexType>(delType.Name, "The given type has child types and cannot be removed.");

                    #endregion

                    #region check that the delete type has no incoming edges, just when reprimands should not be ignored
                    if (delType.HasIncomingEdges(false))
                        if (!delType.GetIncomingEdgeDefinitions(false).All(edge => myTypes.Contains(edge.RelatedEdgeDefinition.RelatedType) == true))
                            throw new TypeRemoveException<IVertexType>(delType.Name, "The given type has incoming edges and cannot be removed.");

                    #region check if there are incoming edges of target vertices of outgoing edges of the deleting type

                    foreach (var outEdge in delType.GetOutgoingEdgeDefinitions(false))
                    {
                        if (outEdge.TargetVertexType
                                    .GetIncomingEdgeDefinitions(true)
                                    .Any(inEdge => inEdge.RelatedEdgeDefinition.ID.Equals(outEdge.ID) &&
                                            inEdge.RelatedType.ID != delType.ID) && !myIgnoreReprimands)
                            throw new VertexTypeRemoveException(delType.Name,
                                        @"There are other types which have incoming edges, 
                                        whose related type is a outgoing edge of the type which should be removed.");
                    }

                    #endregion
                }

                    #endregion
            }

            #endregion
        }

        protected override void CallCheckFunctions(IRequestAlterType myAlterTypeRequest,
                                                    IVertexType myType,
                                                    TransactionToken myTransactionToken,
                                                    SecurityToken mySecurityToken)
        {
            var request = myAlterTypeRequest as RequestAlterVertexType;

            CheckAttributesNameAndType(request);

            CheckToBeAddedAttributes(request, myType);
            CheckToBeRemovedAttributes(request, myType);
            CheckToBeRenamedAttributes(request, myType);
            CheckNewTypeName(request.AlteredTypeName, myTransactionToken, mySecurityToken);
            CheckToBeAddedMandatoryAndUnique(request, myType);
            CheckToBeRemovedMandatoryAndUnique(request.ToBeRemovedMandatories, request.ToBeRemovedUniques, myType);
            CheckToBeAddedIndices(request.ToBeAddedIndices, myType);
            CheckToBeRemovedIndices(request.ToBeRemovedIndices, myType);
        }

        /// <summary>
        /// Checks if the new vertex type name already exists
        /// </summary>
        /// <param name="myAlteredTypeName">The new name.</param>
        /// <param name="mySecurityToken">TransactionToken.</param>
        /// <param name="myTransactionToken">SecurityToken.</param>
        protected override void CheckNewTypeName(String myAlteredTypeName,
                                                    TransactionToken myTransactionToken,
                                                    SecurityToken mySecurityToken)
        {
            if (myAlteredTypeName != null &&
                _TypeManager.HasType(myAlteredTypeName, myTransactionToken, mySecurityToken))
            {
                throw new EdgeTypeAlreadyExistException(myAlteredTypeName);
            }
        }

        /// <summary>
        /// Calls a Check function foreach IEnumerable of definied attributes.
        /// </summary>
        /// <param name="myRequest">The alter type request which contains the attributes.</param>
        protected override void CheckAttributesNameAndType(IRequestAlterType myRequest)
        {
            var request = myRequest as RequestAlterVertexType;

            CheckNameAndTypeOfAttributePredefinitions(request.ToBeAddedProperties);
            CheckNameAndTypeOfAttributePredefinitions(request.ToBeAddedUnknownAttributes);
            CheckNameAndTypeOfAttributePredefinitions(request.ToBeAddedBinaryProperties);
            CheckNameAndTypeOfAttributePredefinitions(request.ToBeAddedIncomingEdges);
            CheckNameAndTypeOfAttributePredefinitions(request.ToBeAddedOutgoingEdges);
            CheckNameAndTypeOfAttributePredefinitions(request.ToBeAddedMandatories);
            CheckNameAndTypeOfAttributePredefinitions(request.ToBeAddedUniques);
            CheckNameAndTypeOfAttributePredefinitions(request.ToBeAddedIndices);

            CheckNameOfAttributeList(request.ToBeRemovedProperties);
            CheckNameOfAttributeList(request.ToBeRemovedUnknownAttributes);
            CheckNameOfAttributeList(request.ToBeRemovedMandatories);
            CheckNameOfAttributeList(request.ToBeRemovedUniques);
            CheckNameOfAttributeList(request.ToBeRemovedBinaryProperties);
            CheckNameOfAttributeList(request.ToBeRemovedIncomingEdges);
            CheckNameOfAttributeList(request.ToBeRemovedOutgoingEdges);
        }

        /// <summary>
        /// Checks if the to be added attributes exist in the given type or derived ones.
        /// </summary>
        /// <param name="myAlterTypeRequest">The request.</param>
        /// <param name="myType">The type.</param>
        protected override void CheckToBeAddedAttributes(IRequestAlterType myAlterTypeRequest,
                                                            IVertexType myType)
        {
            var request = myAlterTypeRequest as RequestAlterVertexType;

            foreach (var aVertexType in myType.GetDescendantVertexTypesAndSelf())
            {
                var attributesOfCurrentVertexType = aVertexType.GetAttributeDefinitions(false).ToList();

                #region binary properties

                if (request.ToBeAddedBinaryProperties != null)
                {
                    foreach (var aToBeAddedAttribute in request.ToBeAddedBinaryProperties)
                    {
                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                            throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                    }
                }

                #endregion

                #region outgoing edges

                if (request.ToBeAddedOutgoingEdges != null)
                {
                    foreach (var aToBeAddedAttribute in request.ToBeAddedOutgoingEdges)
                    {
                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                            throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                    }
                }

                #endregion

                #region Incoming edges

                if (request.ToBeAddedIncomingEdges != null)
                {
                    foreach (var aToBeAddedAttribute in request.ToBeAddedIncomingEdges)
                    {
                        var parts = aToBeAddedAttribute.AttributeType.Split(IncomingEdgePredefinition.TypeSeparator);

                        //check if vertex type or outgoing edge name is null or empty
                        if (parts[0].IsNullOrEmpty() ||
                            parts[1].IsNullOrEmpty())
                            throw new EmptyAttributeTypeException(aToBeAddedAttribute.GetType());

                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                            throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                    }
                }

                #endregion

                #region property

                if (request.ToBeAddedProperties != null)
                {
                    foreach (var aToBeAddedAttribute in request.ToBeAddedProperties)
                    {
                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                            throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                    }
                }

                #endregion

                #region unknown attributes

                if (request.ToBeAddedUnknownAttributes != null)
                {
                    foreach (var aToBeAddedAttribute in request.ToBeAddedUnknownAttributes)
                    {
                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                            throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// Checks if the to be removed attributes exists on this type.
        /// </summary>
        /// <param name="myAlterTypeRequest">The request.</param>
        /// <param name="myType">The type.</param>
        protected override void CheckToBeRemovedAttributes(IRequestAlterType myAlterTypeRequest,
                                                            IVertexType myType)
        {
            var request = myAlterTypeRequest as RequestAlterVertexType;

            #region properties

            var attributesOfCurrentVertexType = myType.GetAttributeDefinitions(false).ToList();

            if (request.ToBeRemovedProperties != null)
            {
                foreach (var aToBeDeletedAttribute in request.ToBeRemovedProperties)
                {
                    if (!attributesOfCurrentVertexType.Any(_ => _.Name == aToBeDeletedAttribute))
                        throw new AttributeDoesNotExistException(aToBeDeletedAttribute, myType.Name);
                }
            }

            #endregion

            #region binary properties

            if (request.ToBeRemovedBinaryProperties != null)
            {
                foreach (var aToBeDeletedAttribute in request.ToBeRemovedBinaryProperties)
                {
                    if (!attributesOfCurrentVertexType.Any(_ => _.Name == aToBeDeletedAttribute))
                        throw new VertexAttributeIsNotDefinedException(aToBeDeletedAttribute, myType.Name);
                }
            }

            #endregion

            #region outgoing Edges

            if (request.ToBeRemovedOutgoingEdges != null)
            {
                foreach (var aToBeDeletedAttribute in request.ToBeRemovedOutgoingEdges)
                {
                    if (!attributesOfCurrentVertexType.Any(_ => _.Name == aToBeDeletedAttribute))
                        throw new VertexAttributeIsNotDefinedException(aToBeDeletedAttribute, myType.Name);
                }
            }

            #endregion

            #region incoming edges

            if (request.ToBeRemovedIncomingEdges != null)
            {
                foreach (var aToBeDeletedAttribute in request.ToBeRemovedIncomingEdges)
                {
                    if (!attributesOfCurrentVertexType.Any(_ => _.Name == aToBeDeletedAttribute))
                        throw new VertexAttributeIsNotDefinedException(aToBeDeletedAttribute, myType.Name);
                }
            }

            #endregion
        }

        #endregion

        /// <summary>
        /// Converts the given unknown attribute predefinition to a binary property predfeinition.
        /// </summary>
        /// <param name="unknown">The unknown attribute predegfinition.</param>
        /// <returns>The created binary property predefinition.</returns>
        private static BinaryPropertyPredefinition ConvertUnknownToBinaryProperty(UnknownAttributePredefinition unknown)
        {
            if (unknown.DefaultValue != null)
                throw new Exception("A default value is not allowed on a binary property.");

            if (unknown.EdgeType != null)
                throw new Exception("An edge type is not allowed on a binary property.");

            if (unknown.Multiplicity != null)
                throw new Exception("A multiplicity is not allowed on a binary property.");

            var prop = new BinaryPropertyPredefinition(unknown.AttributeName, unknown.AttributeType)
                           .SetComment(unknown.Comment);
            return prop;
        }

        /// <summary>
        /// Converts the given unknown attribute predefinition to an outgoing edge predfeinition.
        /// </summary>
        /// <param name="unknown">The unknown attribute predegfinition.</param>
        /// <returns>The created outgoing edge predefinition.</returns>
        private static OutgoingEdgePredefinition ConvertUnknownToOutgoingEdge(UnknownAttributePredefinition unknown)
        {
            if (unknown.DefaultValue != null)
                throw new Exception("A default value is not allowed on a unknown property.");

            var prop = new OutgoingEdgePredefinition(unknown.AttributeName, unknown.AttributeType)
                .SetEdgeType(unknown.EdgeType)
                .SetComment(unknown.Comment);

            if (unknown.Multiplicity != null)
                switch (unknown.Multiplicity)
                {
                    case UnknownAttributePredefinition.SETMultiplicity:
                        prop.SetMultiplicityAsMultiEdge(unknown.InnerEdgeType);
                        break;
                    default:
                        throw new Exception("Unknown multiplicity for edges.");
                }
            return prop;
        }

        /// <summary>
        /// Converts the given unknown attribute predefinition to an incoming edge predfeinition.
        /// </summary>
        /// <param name="unknown">The unknown attribute predegfinition.</param>
        /// <returns>The created incoming edge predefinition.</returns>
        private static IncomingEdgePredefinition ConvertUnknownToIncomingEdge(UnknownAttributePredefinition unknown)
        {
            if (unknown.DefaultValue != null)
                throw new Exception("A default value is not allowed on an incoming edge.");

            if (unknown.EdgeType != null)
                throw new Exception("An edge type is not allowed on an incoming edge.");

            if (unknown.Multiplicity != null)
                throw new Exception("A multiplicity is not allowed on an incoming edge.");

            var prop = new IncomingEdgePredefinition(unknown.AttributeType,
                                                        GetTargetVertexTypeFromAttributeType(unknown.AttributeType),
                                                        GetTargetEdgeNameFromAttributeType(unknown.AttributeType))
                            .SetComment(unknown.Comment);
            return prop;
        }

        /// <summary>
        /// Checks the uniqueness of property names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        /// <param name="myUniqueNameSet">A set of attribute names defined on this vertex type predefinition.</param>
        private static void CheckBinaryPropertiesUniqueName(VertexTypePredefinition myVertexTypeDefinition,
                                                            ISet<string> myUniqueNameSet)
        {
            if (myVertexTypeDefinition.BinaryProperties != null)
                foreach (var prop in myVertexTypeDefinition.BinaryProperties)
                {
                    prop.CheckNull("Binary Property in vertex type predefinition " + myVertexTypeDefinition.TypeName);
                    if (!myUniqueNameSet.Add(prop.AttributeName))
                        throw new DuplicatedAttributeNameException(myVertexTypeDefinition, prop.AttributeName);
                }
        }

        /// <summary>
        /// Checks if every to be added index is valid
        /// </summary>
        /// <param name="myToBeAddedIndices"></param>
        /// <param name="vertexType"></param>
        private static void CheckToBeAddedIndices(IEnumerable<IndexPredefinition> myToBeAddedIndices, IVertexType vertexType)
        {
            if (myToBeAddedIndices == null)
                return;

            var indexDefinitions = vertexType.GetIndexDefinitions(true).ToList();

            foreach (var aIndexPredefinition in myToBeAddedIndices)
            {
                #region check the properties

                foreach (var aProperty in aIndexPredefinition.Properties)
                {
                    if (!vertexType.HasProperty(aProperty))
                    {
                        throw new AttributeDoesNotExistException(aProperty, vertexType.Name);
                    }
                }

                #endregion

                #region check the idx name, etc

                if (indexDefinitions.Any(_ => _.Name == aIndexPredefinition.Name))
                {
                    throw new IndexCreationException(aIndexPredefinition, "This index definition is ambiguous.");
                }

                #endregion
            }
        }

        /// <summary>
        /// Checks if the desired index are removable
        /// </summary>
        /// <param name="myToBeRemovedIndices"></param>
        /// <param name="vertexType"></param>
        private static void CheckToBeRemovedIndices(Dictionary<string, string> myToBeRemovedIndices, IVertexType vertexType)
        {
            if (myToBeRemovedIndices == null)
                return;

            var indexDefinitions = vertexType.GetIndexDefinitions(true).ToList();

            foreach (var aKV in myToBeRemovedIndices)
            {
                if (!indexDefinitions.Any(_ => _.Name == aKV.Key && (aKV.Value == null || _.Edition == aKV.Value)))
                {
                    throw new IndexRemoveException(aKV.Key, aKV.Value, "The desired index does not exist.");
                }
            }
        }

        /// <summary>
        /// Checks if it possible to remove a mandatory or unique constraint
        /// </summary>
        /// <param name="myAlterTypeRequest">The request.</param>
        /// <param name="myType">The type.</param>
        protected static void CheckToBeAddedMandatoryAndUnique(IRequestAlterType myAlterTypeRequest,
                                                                IVertexType myType)
        {
            var request = myAlterTypeRequest as RequestAlterVertexType;

            CheckToBeAddedMandatory(request, myType);

            CheckToBeAddedUniques(request, myType);
        }

        /// <summary>
        /// Checks if the specified mandatory attribute exist on type.
        /// </summary>
        /// <param name="myAlterTypeRequest">The request.</param>
        /// <param name="myType">The type.</param>
        private static void CheckToBeAddedMandatory(RequestAlterVertexType myAlterTypeRequest,
                                                    IVertexType myType)
        {
            var mandatories = myAlterTypeRequest.ToBeAddedMandatories;
            var addProperties = myAlterTypeRequest.ToBeAddedProperties;

            if (mandatories == null)
                return;

            var attributes = myType.GetAttributeDefinitions(false).ToList();

            foreach (var aMandatory in mandatories)
            {
                if (!attributes.Any(_ => _.Name == aMandatory.MandatoryAttribute) &&
                    !addProperties.Any(x => x.AttributeName == aMandatory.MandatoryAttribute))
                {
                    throw new AttributeDoesNotExistException(aMandatory.MandatoryAttribute, myType.Name);
                }
            }
        }

        /// <summary>
        /// Checks if the specified unique attribute exist on type.
        /// </summary>
        /// <param name="myAlterTypeRequest">The request.</param>
        /// <param name="myType">The type.</param>
        private static void CheckToBeAddedUniques(RequestAlterVertexType myAlterTypeRequest,
                                                    IVertexType myType)
        {
            var uniques = myAlterTypeRequest.ToBeAddedUniques;
            var addProperties = myAlterTypeRequest.ToBeAddedProperties;

            if (uniques == null)
                return;

            var attributes = myType.GetAttributeDefinitions(false).ToList();

            foreach (var aUnique in uniques)
            {
                foreach (var aAttribute in aUnique.Properties)
                {
                    if (!attributes.Any(_ => _.Name == aAttribute) && !addProperties.Any(x => x.AttributeName == aAttribute))
                    {
                        throw new AttributeDoesNotExistException(aAttribute, myType.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the given parameter type is valid.
        /// </summary>
        /// <param name="myRequest">The parameter to be checked.</param>
        protected override void CheckRequestType(IRequestAlterType myRequest)
        {
            if (!(myRequest is RequestAlterVertexType))
                throw new InvalidParameterTypeException("AlterTypeRequest", myRequest.GetType().Name, typeof(RequestAlterVertexType).Name);
        }

        #endregion
    }
}

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
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.TypeManagement.Base;
using sones.Library.LanguageExtensions;
using sones.GraphDB.Request.CreateVertexTypes;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal sealed class CheckVertexTypeManager: AVertexTypeManager
    {
        #region data

        private IVertexTypeHandler _vertexTypeManager;


        #endregion


        #region IVertexTypeManager Members

        public override IVertexType GetVertexType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        public override IVertexType GetVertexType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            if (String.IsNullOrWhiteSpace(myTypeName))
            {
                throw new EmptyVertexTypeNameException();
            }
            return null;
        }

        public override bool HasVertexType(string myAlteredVertexTypeName, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (String.IsNullOrWhiteSpace(myAlteredVertexTypeName))
            {
                throw new EmptyVertexTypeNameException();
            }

            return true;
        }

        public override IEnumerable<IVertexType> GetAllVertexTypes(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            //always possible to get all vertices, no checks necessary
            return null;
        }

        public override IEnumerable<IVertexType> AddVertexTypes(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region check arguments

            myVertexTypeDefinitions.CheckNull("myVertexTypeDefinitions");

            #endregion

            CheckAdd(myVertexTypeDefinitions, myTransaction, mySecurity);
            return null;
        }

        public override Dictionary<Int64, String> RemoveVertexTypes(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region check arguments

            myVertexTypes.CheckNull("myVertexTypes");

            #endregion

            CanRemove(myVertexTypes, myTransaction, mySecurity);

            return null;
        }

        public override IEnumerable<long> ClearDB(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region check arguments
            #endregion

            return null;
        }

        public override void UpdateVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region check arguments

            myVertexTypeDefinitions.CheckNull("myVertexTypeDefinitions");

            #endregion

            CanUpdate(myVertexTypeDefinitions, myTransaction, mySecurity);
        }

        public override void TruncateVertexType(long myVertexTypeID, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            GetVertexType(myVertexTypeID, myTransactionToken, mySecurityToken);
        }

        public override void TruncateVertexType(String myVertexTypeName, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            GetVertexType(myVertexTypeName, myTransactionToken, mySecurityToken);
        }

        public override IVertexType AlterVertexType(RequestAlterVertexType myAlterVertexTypeRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var vertexType =  _vertexTypeManager.GetVertexType(myAlterVertexTypeRequest.VertexTypeName, myTransactionToken, mySecurityToken);

            if (myAlterVertexTypeRequest.ToBeAddedUnknownAttributes != null)
            {
                var toBeConverted = myAlterVertexTypeRequest.ToBeAddedUnknownAttributes.ToArray();
                foreach (var unknown in toBeConverted)
                {
                    if (BinaryPropertyPredefinition.TypeName.Equals(unknown.AttributeType))
                    {
                        var prop = ConvertUnknownToBinaryProperty(unknown);

                        myAlterVertexTypeRequest.AddBinaryProperty(prop);
                    }
                    else if (IsBaseType(unknown.AttributeType))
                    {
                        var prop = ConvertUnknownToProperty(unknown);

                        myAlterVertexTypeRequest.AddProperty(prop);
                    }
                    else if (unknown.AttributeType.Contains(IncomingEdgePredefinition.TypeSeparator))
                    {
                        var prop = ConvertUnknownToIncomingEdge(unknown);
                        myAlterVertexTypeRequest.AddIncomingEdge(prop);
                    }
                    else
                    {
                        var prop = ConvertUnknownToOutgoingEdge(unknown);
                        myAlterVertexTypeRequest.AddOutgoingEdge(prop);
                    }
                }
                myAlterVertexTypeRequest.ResetUnknown();
            }

            #region checks

            CheckToBeAddedAttributes(myAlterVertexTypeRequest, vertexType);
            CheckToBeRemovedAttributes(myAlterVertexTypeRequest, vertexType);
            CheckToBeRenamedAttributes(myAlterVertexTypeRequest, vertexType);
            CheckNewVertexTypeName(myAlterVertexTypeRequest.AlteredVertexTypeName, mySecurityToken, myTransactionToken);
            CheckToBeAddedMandatoryAndUniques(myAlterVertexTypeRequest.ToBeAddedMandatories, myAlterVertexTypeRequest.ToBeAddedUniques, vertexType);
            CheckToBeRemovedMandatoryAndUnique(myAlterVertexTypeRequest.ToBeRemovedMandatories, myAlterVertexTypeRequest.ToBeRemovedUniques, vertexType);
            CheckToBeAddedIndices(myAlterVertexTypeRequest.ToBeAddedIndices, vertexType, mySecurityToken, myTransactionToken);
            CheckToBeRemovedIndices(myAlterVertexTypeRequest.ToBeRemovedIndices, vertexType, mySecurityToken, myTransactionToken);

            #endregion

            return null;
        }

        #endregion

        #region alter vertex type

        /// <summary>
        /// Checks if the desired index are removable
        /// </summary>
        /// <param name="myToBeRemovedIndices"></param>
        /// <param name="vertexType"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        private void CheckToBeRemovedIndices(Dictionary<string, string> myToBeRemovedIndices, IVertexType vertexType, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myToBeRemovedIndices != null)
            {
                var indexDefinitions = vertexType.GetIndexDefinitions(true).ToList();

                foreach (var aKV in myToBeRemovedIndices)
                {
                    if (!indexDefinitions.Any(_ => _.Name == aKV.Key && _.Edition == aKV.Value))
                    {
                        throw new IndexRemoveException(aKV.Key, aKV.Value, "The desired index does not exist.");
                    }
                }
            }
        }

        /// <summary>
        /// Checks if every to be added index is valid
        /// </summary>
        /// <param name="myToBeAddedIndices"></param>
        /// <param name="vertexType"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        private void CheckToBeAddedIndices(IEnumerable<IndexPredefinition> myToBeAddedIndices, IVertexType vertexType, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myToBeAddedIndices != null)
            {
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
        }

        /// <summary>
        /// Checks if it possible to remove a mandatory or unique constraint
        /// </summary>
        /// <param name="myMandatories"></param>
        /// <param name="myUniques"></param>
        /// <param name="vertexType"></param>
        private void CheckToBeRemovedMandatoryAndUnique(IEnumerable<string> myMandatories, IEnumerable<string> myUniques, IVertexType vertexType)
        {
            if (myMandatories != null || myUniques != null)
            {
                List<IAttributeDefinition> attributes = vertexType.GetAttributeDefinitions(false).ToList();

                #region mandatories

                if (myMandatories != null)
                {
                    foreach (var aMandatory in myMandatories)
                    {
                        if (!attributes.Any(_ => _.Name == aMandatory))
                        {
                            throw new AttributeDoesNotExistException(aMandatory, vertexType.Name);
                        }
                    }
                }

                #endregion

                #region uniques

                if (myUniques != null)
                {
                    foreach (var aUnique in myUniques)
                    {
                        if (!attributes.Any(_ => _.Name == aUnique))
                        {
                            throw new AttributeDoesNotExistException(aUnique, vertexType.Name);
                        }
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// Checks if the mandatories can be added
        /// </summary>
        /// <param name="myMandatories"></param>
        /// <param name="myUniques"></param>
        /// <param name="vertexType"></param>
        private void CheckToBeAddedMandatoryAndUniques(IEnumerable<MandatoryPredefinition> myMandatories, IEnumerable<UniquePredefinition> myUniques, IVertexType vertexType)
        {
            if (myMandatories != null || myUniques != null)
            {
                List<IAttributeDefinition> attributes = vertexType.GetAttributeDefinitions(false).ToList();

                #region mandatories

                if (myMandatories != null)
                {
                    foreach (var aMandatory in myMandatories)
                    {
                        if (!attributes.Any(_ => _.Name == aMandatory.MandatoryAttribute))
                        {
                            throw new AttributeDoesNotExistException(aMandatory.MandatoryAttribute, vertexType.Name);
                        }
                    }
                }

                #endregion

                #region uniques

                if (myUniques != null)
                {
                    foreach (var aUnique in myUniques)
                    {
                        foreach (var aAttribute in aUnique.Properties)
                        {
                            if (!attributes.Any(_ => _.Name == aAttribute))
                            {
                                throw new AttributeDoesNotExistException(aAttribute, vertexType.Name);
                            }
                        }
                    }
                }

                #endregion

            }

        }

        /// <summary>
        /// Checks if the new vertex type name already exists
        /// </summary>
        /// <param name="myAlteredVertexTypeName"></param>
        /// <param name="mySecurityToken"></param>
        /// <param name="myTransactionToken"></param>
        private void CheckNewVertexTypeName(string myAlteredVertexTypeName, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if (myAlteredVertexTypeName != null && _vertexTypeManager.HasVertexType(myAlteredVertexTypeName, mySecurityToken, myTransactionToken))
            {
                throw new VertexTypeAlreadyExistException(myAlteredVertexTypeName);
            }
        }

        /// <summary>
        /// Checks the to be renamed attributes
        /// </summary>
        /// <param name="myAlterVertexTypeRequest"></param>
        /// <param name="vertexType"></param>
        private static void CheckToBeRenamedAttributes(RequestAlterVertexType myAlterVertexTypeRequest, IVertexType vertexType)
        {
            if (myAlterVertexTypeRequest.ToBeRenamedProperties != null)
            {
                foreach (var aToBeRenamedAttributes in myAlterVertexTypeRequest.ToBeRenamedProperties)
                {
                    if (!CheckOldName(aToBeRenamedAttributes.Key, vertexType))
                    {
                        throw new InvalidAlterVertexTypeException(String.Format("It is not possible to rename {0} into {1}. The to be renamed attribute does not exist."));
                    }

                    if (!CheckNewName(aToBeRenamedAttributes.Value, vertexType))
                    {
                        throw new InvalidAlterVertexTypeException(String.Format("It is not possible to rename {0} into {1}. The new attribute name already exists."));
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the new name of the attribute already exists
        /// </summary>
        /// <param name="myNewAttributeName"></param>
        /// <param name="vertexType"></param>
        /// <returns></returns>
        private static bool CheckNewName(string myNewAttributeName, IVertexType vertexType)
        {
            if (myNewAttributeName != null)
            {
                foreach (var aVertexType in vertexType.GetChildVertexTypes(true, true))
                {
                    var attributesOfCurrentVertexType = aVertexType.GetAttributeDefinitions(false).ToList();

                    if (attributesOfCurrentVertexType.Any(_ => _.Name == myNewAttributeName))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the old name exists on the given vertex type
        /// </summary>
        /// <param name="myOldAttributeName"></param>
        /// <param name="vertexType"></param>
        /// <returns></returns>
        private static bool CheckOldName(string myOldAttributeName, IVertexType vertexType)
        {
            if (myOldAttributeName != null)
            {
                var attributesOfCurrentVertexType = vertexType.GetAttributeDefinitions(false).ToList();

                return attributesOfCurrentVertexType.Any(_ => _.Name == myOldAttributeName);
            }

            return true;
        }

        /// <summary>
        /// Checks if the to be removed attributes exists on this type
        /// </summary>
        /// <param name="myAlterVertexTypeRequest"></param>
        /// <param name="vertexType"></param>
        private static void CheckToBeRemovedAttributes(RequestAlterVertexType myAlterVertexTypeRequest, IVertexType vertexType)
        {
            #region properties

            var attributesOfCurrentVertexType = vertexType.GetAttributeDefinitions(false).ToList();

            if (myAlterVertexTypeRequest.ToBeRemovedProperties != null)
            {
                foreach (var aToBeDeletedAttribute in myAlterVertexTypeRequest.ToBeRemovedProperties)
                {
                    if (!attributesOfCurrentVertexType.Any(_ => _.Name == aToBeDeletedAttribute))
                    {
                        throw new VertexAttributeIsNotDefinedException(aToBeDeletedAttribute);
                    }
                }
            }

            #endregion

            #region outgoing Edges

            if (myAlterVertexTypeRequest.ToBeRemovedOutgoingEdges != null)
            {
                foreach (var aToBeDeletedAttribute in myAlterVertexTypeRequest.ToBeRemovedOutgoingEdges)
                {
                    if (!attributesOfCurrentVertexType.Any(_ => _.Name == aToBeDeletedAttribute))
                    {
                        throw new VertexAttributeIsNotDefinedException(aToBeDeletedAttribute);
                    }
                }
            }

            #endregion

            #region incoming edges

            if (myAlterVertexTypeRequest.ToBeRemovedIncomingEdges != null)
            {
                foreach (var aToBeDeletedAttribute in myAlterVertexTypeRequest.ToBeRemovedIncomingEdges)
                {
                    if (!attributesOfCurrentVertexType.Any(_ => _.Name == aToBeDeletedAttribute))
                    {
                        throw new VertexAttributeIsNotDefinedException(aToBeDeletedAttribute);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Checks if the to be added attributes exist in the given vertex type or derived oness
        /// </summary>
        /// <param name="myAlterVertexTypeRequest"></param>
        /// <param name="vertexType"></param>
        private static void CheckToBeAddedAttributes(RequestAlterVertexType myAlterVertexTypeRequest, IVertexType vertexType)
        {
            foreach (var aVertexType in vertexType.GetChildVertexTypes(true, true))
            {
                var attributesOfCurrentVertexType = aVertexType.GetAttributeDefinitions(false).ToList();

                #region binary properties
                if (myAlterVertexTypeRequest.ToBeAddedBinaryProperties != null)
                {
                    foreach (var aToBeAddedAttribute in myAlterVertexTypeRequest.ToBeAddedBinaryProperties)
                    {
                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                        {
                            throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                        }
                    }
                }

                #endregion

                #region outgoing edges

                if (myAlterVertexTypeRequest.ToBeAddedOutgoingEdges != null)
                {
                    foreach (var aToBeAddedAttribute in myAlterVertexTypeRequest.ToBeAddedOutgoingEdges)
                    {
                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                        {
                            throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                        }
                    }
                }

                #endregion

                #region Incoming edges
                if (myAlterVertexTypeRequest.ToBeAddedIncomingEdges != null)
                {
                    foreach (var aToBeAddedAttribute in myAlterVertexTypeRequest.ToBeAddedIncomingEdges)
                    {
                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                        {
                            throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                        }
                    }
                }

                #endregion

                #region property

                if (myAlterVertexTypeRequest.ToBeAddedProperties != null)
                {
                    foreach (var aToBeAddedAttribute in myAlterVertexTypeRequest.ToBeAddedProperties)
                    {
                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                        {
                            throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                        }
                    }
                }

                #endregion

                #region unknown attributes

                if (myAlterVertexTypeRequest.ToBeAddedUnknownAttributes != null)
                {
                    foreach (var aToBeAddedAttribute in myAlterVertexTypeRequest.ToBeAddedUnknownAttributes)
                    {
                        if (attributesOfCurrentVertexType.Any(_ => _.Name == aToBeAddedAttribute.AttributeName))
                        {
                            throw new VertexAttributeAlreadyExistsException(aToBeAddedAttribute.AttributeName);
                        }
                    }
                }

                #endregion
            }
        }

        #endregion

        private bool CanRemove(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region check if specified types can be removed
            //get child vertex types and check if they are specified by user
            foreach (var delType in myVertexTypes)
            {
                #region check that the remove type is no base type
                if (delType == null)
                    throw new VertexTypeRemoveException("", "Vertex Type is null.");

                if (!delType.HasParentType)
                    continue;

                if (delType.ParentVertexType.ID.Equals((long)BaseTypes.BaseType) && IsTypeBaseType(delType.ID))
                    //Exception that base type cannot be deleted
                    throw new VertexTypeRemoveException(delType.Name, "A BaseType connot be removed.");

                #endregion

                #region check that existing child types are specified

                foreach (var child in delType.GetChildVertexTypes())
                    if (!myVertexTypes.Contains(child))
                        //all child types has to be specified by user
                        throw new VertexTypeRemoveException(delType.Name, "The given type has child types and cannot be removed.");

                #endregion

                #region check that the delete type has no incoming edges

                if (delType.HasIncomingEdges(false))
                {
                    foreach (var edge in delType.GetIncomingEdgeDefinitions(false))
                    {
                        //just throw Exception if incomingedge doesn't point to the type itself
                        if (edge.RelatedType.ID != delType.ID)
                            //all incoming edges has to be deletet by user
                            throw new VertexTypeRemoveException(delType.Name, "The given type has incoming edges and cannot be removed.");
                    }
                }

                #endregion

            }
            #endregion

            return true;
        }

        private bool CanUpdate(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the given vertex type predefinitions will succeed.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The list of vertex type predefinitions.<remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CheckAdd(
            IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, 
            TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region prolog
            // Basically first check the pre-definitions itself without asking the IVertexManager. 
            // If these checks are okay, proof everything concerning the types stored in the fs using the IVertexManager.

            // These are the necessary checks:
            //   OK - vertex type names are unique
            //   OK - attribute names are unique for each type pre-definition
            //   OK - parentPredef types are none of the base vertex types
            //   OK - check that no vertex type has the flags sealed and abstract at the same time
            //   OK - check if the derviation is circle free
            // ---- now with IVertexManager ---- (This means we can assume, that the vertex types are created, so we have a list of all vertex types containing the 'to-be-added-types'.)
            //   OK - check if the type names are unique
            //   OK - check if the attribute names are unique regarding the derivation
            //   OK - check if all parentPredef types exists and are not sealed
            //   OK - check if all outgoing edges have existing targets
            //   OK - check if all incoming edges have existing outgoing edges
            // TODO - check that unique constraints and indices definition contains existing myAttributes
            #endregion

            #region Checks without IVertexManager

            CanAddCheckBasics(myVertexTypeDefinitions);

            //Contains dictionary of vertex name to vertex predefinition.
            var defsByVertexName = CanAddCheckDuplicates(myVertexTypeDefinitions);
            
            //Contains dictionary of parent vertex name to list of vertex predefinitions.
            var defsByParentVertexName = myVertexTypeDefinitions
                .GroupBy(def=>def.SuperVertexTypeName)
                .ToDictionary(group => group.Key, group=>group.AsEnumerable());

            //Contains list of vertex predefinitions sorted topologically.
            var defsTopologically = CanAddSortTopolocically(defsByVertexName, defsByParentVertexName);
            
            #endregion

            #region Checks with IVertexManager 
            //Here we know that the VertexTypePredefinitions are syntactical correct.
            
            //Perf: We comment the FS checks out, to have a better performance
            //CanAddCheckWithFS(defsTopologically, defsByVertexName, myTransaction, mySecurity);

            #endregion
        }

        /// <summary>
        /// Checks for errors in a list of vertex type predefinitions without using the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The list of vertex type predefinitions to be checked.</param>
        private static void CanAddCheckBasics(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions)
        {
            foreach (var vertexTypeDefinition in myVertexTypeDefinitions)
            {
                vertexTypeDefinition.CheckNull("Element in myVertexTypeDefinitions");

                ConvertUnknownAttributes(vertexTypeDefinition);

                CheckSealedAndAbstract(vertexTypeDefinition);
                CheckVertexTypeName(vertexTypeDefinition);
                CheckParentTypeAreNoBaseTypes(vertexTypeDefinition);
                CheckAttributes(vertexTypeDefinition);
                CheckUniques(vertexTypeDefinition);
                CheckIndices(vertexTypeDefinition);
            }
        }

        /// <summary>
        /// Checks the uniqueness of attribute names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        private static void CheckAttributes(VertexTypePredefinition vertexTypeDefinition)
        {
            var uniqueNameSet = new HashSet<string>();

            CheckIncomingEdgesUniqueName(vertexTypeDefinition, uniqueNameSet);
            CheckOutgoingEdgesUniqueName(vertexTypeDefinition, uniqueNameSet);
            CheckPropertiesUniqueName(vertexTypeDefinition, uniqueNameSet);
            CheckBinaryPropertiesUniqueName(vertexTypeDefinition, uniqueNameSet);
        }

        private static void CheckBinaryPropertiesUniqueName(VertexTypePredefinition myVertexTypeDefinition, HashSet<string> myUniqueNameSet)
        {
            if (myVertexTypeDefinition.BinaryProperties != null)
                foreach (var prop in myVertexTypeDefinition.BinaryProperties)
                {
                    prop.CheckNull("Binary Property in vertex type predefinition " + myVertexTypeDefinition.VertexTypeName);
                    if (!myUniqueNameSet.Add(prop.AttributeName))
                        throw new DuplicatedAttributeNameException(myVertexTypeDefinition, prop.AttributeName);
                }
        }


        private static void CheckIndices(VertexTypePredefinition vertexTypeDefinition)
        {
            //TODO
        }

        private static void CheckUniques(VertexTypePredefinition vertexTypeDefinition)
        {
            //TODO
        }

        /// <summary>
        /// Checks whether a vertex type predefinition is not derived from a base vertex type.
        /// </summary>
        /// <param name="myVertexTypeDefinition"></param>
        private static void CheckParentTypeAreNoBaseTypes(VertexTypePredefinition myVertexTypeDefinition)
        {
            if (!CanBeParentType(myVertexTypeDefinition.SuperVertexTypeName))
            {
                throw new InvalidBaseVertexTypeException(myVertexTypeDefinition.VertexTypeName);
            }
        }

        /// <summary>
        /// Checks whether a given type name is not a basix vertex type.
        /// </summary>
        /// <param name="myTypeName">The type name to be checked.</param>
        /// <returns>True, if the type name is the name of a base vertex type (but Vertex), otherwise false.</returns>
        private static bool CanBeParentType(string myTypeName)
        {
            BaseTypes type;
            if (!Enum.TryParse(myTypeName, out type))
                return true;

            return type == BaseTypes.Vertex;
        }

        /// <summary>
        /// Checks whether a vertex type predefinition is not sealed and abstract.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The vertex type predefinition to be checked.</param>
        private static void CheckSealedAndAbstract(VertexTypePredefinition myVertexTypePredefinition)
        {
            if (myVertexTypePredefinition.IsSealed && myVertexTypePredefinition.IsAbstract)
            {
                throw new UselessVertexTypeException(myVertexTypePredefinition);
            }
        }

        /// <summary>
        /// Checks whether the vertex type property on an vertex type definition contains anything.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        private static void CheckVertexTypeName(VertexTypePredefinition myVertexTypeDefinition)
        {
            if (string.IsNullOrWhiteSpace(myVertexTypeDefinition.VertexTypeName))
            {
                throw new EmptyVertexTypeNameException();
            }
        }

        private static void ConvertUnknownAttributes(VertexTypePredefinition myVertexTypeDefinition)
        {
            if (myVertexTypeDefinition.UnknownAttributes == null)
                return;

            var toBeConverted = myVertexTypeDefinition.UnknownAttributes.ToArray();
            foreach (var unknown in toBeConverted)
            {
                if (BinaryPropertyPredefinition.TypeName.Equals(unknown.AttributeType))
                {
                    var prop = ConvertUnknownToBinaryProperty(unknown);

                    myVertexTypeDefinition.AddBinaryProperty(prop);
                }
                else if (IsBaseType(unknown.AttributeType))
                {
                    var prop = ConvertUnknownToProperty(unknown);

                    myVertexTypeDefinition.AddProperty(prop);
                }
                else if (unknown.AttributeType.Contains(IncomingEdgePredefinition.TypeSeparator))
                {
                    var prop = ConvertUnknownToIncomingEdge(unknown);
                    myVertexTypeDefinition.AddIncomingEdge(prop);
                }
                else
                {
                    var prop = ConvertUnknownToOutgoingEdge(unknown);
                    myVertexTypeDefinition.AddOutgoingEdge(prop);
                }
            }
            myVertexTypeDefinition.ResetUnknown();
        }

        private static BinaryPropertyPredefinition ConvertUnknownToBinaryProperty(UnknownAttributePredefinition unknown)
        {
            if (unknown.DefaultValue != null)
                throw new Exception("A default value is not allowed on a binary property.");

            if (unknown.EdgeType != null)
                throw new Exception("An edge type is not allowed on a binary property.");

            if (unknown.Multiplicity != null)
                throw new Exception("A multiplicity is not allowed on a binary property.");

            var prop = new BinaryPropertyPredefinition(unknown.AttributeName)
                           .SetComment(unknown.Comment);
            return prop;
        }

        private static OutgoingEdgePredefinition ConvertUnknownToOutgoingEdge(UnknownAttributePredefinition unknown)
        {
            if (unknown.DefaultValue != null)
                throw new Exception("A default value is not allowed on a binary property.");

            var prop = new OutgoingEdgePredefinition(unknown.AttributeName)
                .SetAttributeType(unknown.AttributeType)
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

        private static IncomingEdgePredefinition ConvertUnknownToIncomingEdge(UnknownAttributePredefinition unknown)
        {
            if (unknown.DefaultValue != null)
                throw new Exception("A default value is not allowed on an incoming edge.");

            if (unknown.EdgeType != null)
                throw new Exception("An edge type is not allowed on an incoming edge.");

            if (unknown.Multiplicity != null)
                throw new Exception("A multiplicity is not allowed on an incoming edge.");

            var prop = new IncomingEdgePredefinition(unknown.AttributeType)
                           .SetComment(unknown.Comment)
                           .SetOutgoingEdge(GetTargetVertexTypeFromAttributeType(unknown.AttributeType), GetTargetEdgeNameFromAttributeType(unknown.AttributeType));
            return prop;
        }

        private static PropertyPredefinition ConvertUnknownToProperty(UnknownAttributePredefinition unknown)
        {
            if (unknown.EdgeType != null)
                throw new Exception("An edge type is not allowed on a property.");

            var prop = new PropertyPredefinition(unknown.AttributeName)
                           .SetDefaultValue(unknown.DefaultValue)
                           .SetAttributeType(unknown.AttributeType)
                           .SetComment(unknown.Comment);

            if (unknown.Multiplicity != null)
                switch (unknown.Multiplicity)
                {
                    case UnknownAttributePredefinition.LISTMultiplicity:
                        prop.SetMultiplicityToList();
                        break;
                    case UnknownAttributePredefinition.SETMultiplicity:
                        prop.SetMultiplicityToSet();
                        break;
                    default:
                        throw new Exception("Unknown multiplicity for properties.");
                }
            return prop;
        }

        public override void Initialize(IMetaManager myMetaManager)
        {
            _vertexTypeManager = myMetaManager.VertexTypeManager.ExecuteManager;
        }

        public override void Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
        }

        public override void CleanUpTypes()
        {
        }
    }
}

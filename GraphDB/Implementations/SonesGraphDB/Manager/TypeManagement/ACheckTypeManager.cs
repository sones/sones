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
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.LanguageExtensions;
using sones.GraphDB.Manager.BaseGraph;
using sones.GraphDB.Manager.Vertex;
using sones.GraphDB.ErrorHandling;
using System.Collections;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.Request;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal abstract class ACheckTypeManager<T> : ATypeManager<T>
        where T : IBaseType
    {
        #region Data

        /// <summary>
        /// Holds the instance of the Edge- / VertexTypeManager of the MetaManager.
        /// </summary>
        protected ITypeHandler<T> _TypeManager;

        #endregion

        #region ATypeManager member

        public override T GetType(long myTypeId,
                                    TransactionToken myTransaction,
                                    SecurityToken mySecurity)
        {
            return default(T);
        }

        public override T GetType(string myTypeName,
                                    TransactionToken myTransaction,
                                    SecurityToken mySecurity)
        {
            if (String.IsNullOrWhiteSpace(myTypeName))
                throw new EmptyTypeNameException();

            return default(T);
        }

        public override IEnumerable<T> GetAllTypes(TransactionToken myTransaction,
                                                    SecurityToken mySecurity)
        {
            return default(IEnumerable<T>);
        }

        public override IEnumerable<T> AddTypes(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                                    TransactionToken myTransaction,
                                                    SecurityToken mySecurity)
        {
            #region check arguments

            myTypePredefinitions.CheckNull("myTypePredefinitions");

            #endregion

            CheckAdd(myTypePredefinitions, myTransaction, mySecurity);

            return null;
        }

        public override Dictionary<long, string> RemoveTypes(IEnumerable<T> myTypes,
                                                                TransactionToken myTransaction,
                                                                SecurityToken mySecurity,
                                                                bool myIgnoreReprimands = false)
        {
            #region check arguments

            myTypes.CheckNull("myVertexTypes");

            #endregion

            //check if types exist

            CanRemove(myTypes, myTransaction, mySecurity, myIgnoreReprimands);

            return null;
        }

        public override IEnumerable<long> ClearTypes(TransactionToken myTransaction,
                                                        SecurityToken mySecurity)
        {
            return null;
        }


        public override void TruncateType(long myTypeID,
                                            TransactionToken myTransactionToken,
                                            SecurityToken mySecurityToken)
        {
            GetType(myTypeID, myTransactionToken, mySecurityToken);

            if (IsTypeBaseType(myTypeID))
                throw new InvalidTypeException("[BaseType] " + myTypeID.ToString(), "userdefined type");
        }

        public override void TruncateType(string myTypeName,
                                            TransactionToken myTransactionToken,
                                            SecurityToken mySecurityToken)
        {
            GetType(myTypeName, myTransactionToken, mySecurityToken);
            
            if (IsTypeBaseType(myTypeName))
                throw new InvalidTypeException("[BaseType] " + myTypeName, "userdefined type");
        }

        public override bool HasType(string myTypeName,
                                        TransactionToken myTransactionToken,
                                        SecurityToken mySecurityToken)
        {
            GetType(myTypeName, myTransactionToken, mySecurityToken);

            return true;
        }

        public override void CleanUpTypes()
        { }

        public override abstract T AlterType(IRequestAlterType myAlterTypeRequest,
                                                TransactionToken myTransactionToken,
                                                SecurityToken mySecurityToken,
                                                out RequestUpdate myUpdateRequest);

        public override abstract void Initialize(IMetaManager myMetaManager);

        public override abstract void Load(TransactionToken myTransaction,
                                            SecurityToken mySecurity);

        /// <summary>
        /// Checks if the given type is a base type
        /// </summary>
        protected override abstract bool IsTypeBaseType(long myTypeID);

        /// <summary>
        /// Checks if the given type is a base type
        /// </summary>
        protected override abstract bool IsTypeBaseType(string myTypeName);

        #endregion

        #region helper methods

        #region abstract helber methods

        /// <summary>
        /// Checks if the given parameter type is valid.
        /// </summary>
        /// <param name="myTypePredefinitions">The parameter to be checked.</param>
        protected abstract void CheckPredefinitionsType(IEnumerable<ATypePredefinition> myTypePredefinitions);

        /// <summary>
        /// Checks if the given parameter type is valid.
        /// </summary>
        /// <param name="myRequest">The parameter to be checked.</param>
        protected override abstract void CheckRequestType(IRequestAlterType myRequest);

        /// <summary>
        /// Convertes Unknown attributes depending on the type of the predefinition.
        /// </summary>
        /// <param name="myTypePredefinition">The predefinitions which contains the unknown attributes.</param>
        protected abstract void ConvertUnknownAttributes(ATypePredefinition myTypePredefinition);

        /// <summary>
        /// Checks whether a given type name is not a basis type.
        /// </summary>
        /// <param name="myTypeName">The type name to be checked.</param>
        /// <returns>True, if the type name is the name of a base type, otherwise false.</returns>
        protected abstract bool CanBaseTypeBeParentType(string myTypeName);

        /// <summary>
        /// Checks the uniqueness of attribute names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        protected abstract void CheckAttributes(ATypePredefinition vertexTypeDefinition);

        /// <summary>
        /// Checks the properties which are marked as unique and add them to uniques.
        /// </summary>
        /// <param name="myTypePredefinition">The predefinition which contains the properties.</param>
        protected abstract void ConvertPropertyUniques(ATypePredefinition myTypePredefinition);

        /// <summary>
        /// Checks if the specified type can be removed.
        /// </summary>
        /// <param name="myTypes">The types which should be removed.</param>
        /// <param name="myTransaction">TransactionToken</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <param name="myIgnoreReprimands">Marks if reprimands are ignored on the types which should be removed.</param>
        protected abstract void CanRemove(IEnumerable<T> myTypes,
                                            TransactionToken myTransaction,
                                            SecurityToken mySecurity,
                                            bool myIgnoreReprimands);

        /// <summary>
        /// Calls a variable number of check functions.
        /// </summary>
        /// <param name="myAlterTypeRequest">The alter type request.</param>
        /// <param name="myType">The type which is going to be altered.</param>
        /// <param name="myTransactionToken">TransactionToken</param>
        /// <param name="mySecurityToken">SecurityToken</param>
        protected abstract void CallCheckFunctions(IRequestAlterType myAlterTypeRequest,
                                                    T myType,
                                                    TransactionToken myTransactionToken,
                                                    SecurityToken mySecurityToken);

        /// <summary>
        /// Checks the to be added attributes.
        /// </summary>
        /// <param name="myAlterTypeRequest">The alter type request.</param>
        /// <param name="myType">The type.</param>
        protected abstract void CheckToBeAddedAttributes(IRequestAlterType myAlterTypeRequest,
                                                            T myType);

        /// <summary>
        /// Checks the to be removed attributes.
        /// </summary>
        /// <param name="myAlterTypeRequest">The alter type request.</param>
        /// <param name="myType">The type.</param>
        protected abstract void CheckToBeRemovedAttributes(IRequestAlterType myAlterTypeRequest,
                                                            T myType);

        /// <summary>
        /// Checks if the new type name is valid.
        /// </summary>
        /// <param name="myAlteredTypeName">The new type name.</param>
        /// <param name="myTransactionToken">TransactionToken</param>
        /// <param name="mySecurityToken">SecurityToken</param>
        protected abstract void CheckNewTypeName(string myAlteredTypeName,
                                                    TransactionToken myTransactionToken,
                                                    SecurityToken mySecurityToken);

        /// <summary>
        /// Checks if the names and types of the attributes are valid.
        /// </summary>
        /// <param name="myRequest">The alter type request.</param>
        protected abstract void CheckAttributesNameAndType(IRequestAlterType myRequest);

        #endregion

        /// <summary>
        /// Checks the given type predefinitions and all contained members.
        /// </summary>
        /// <param name="myTypePredefinitions">The type predefinintions.</param>
        protected void CheckAdd(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                TransactionToken myTransactionToken,
                                SecurityToken mySecurityToken)
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

            CheckPredefinitionsType(myTypePredefinitions);

            CanAddCheckBasics(myTypePredefinitions, myTransactionToken, mySecurityToken);

            //Contains dictionary of vertex name to vertex predefinition.
            var defsByVertexName = CanAddCheckDuplicates(myTypePredefinitions);

            //Contains dictionary of parent vertex name to list of vertex predefinitions.
            var defsByParentVertexName = myTypePredefinitions
                .GroupBy(def => def.SuperTypeName)
                .ToDictionary(group => group.Key, group => group.AsEnumerable());

            //Contains list of vertex predefinitions sorted topologically.
            var defsTopologically = CanAddSortTopolocically(defsByVertexName, defsByParentVertexName);

            #endregion

            #region Checks with IVertexManager
            //Here we know that the VertexTypePredefinitions are syntactical correct.

            //Perf: We comment the FS checks out, to have a better performance
            //CanAddCheckWithFS(defsTopologically, defsByVertexName, myTransaction, mySecurity);

            foreach (var type in myTypePredefinitions)
                GetType(type.TypeName, myTransactionToken, mySecurityToken);
            #endregion
        }

        /// <summary>
        /// Checks for errors in a list of vertex type predefinitions without using the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The list of vertex type predefinitions to be checked.</param>
        protected void CanAddCheckBasics(IEnumerable<ATypePredefinition> myTypePredefinitions,
                                            TransactionToken myTransactionToken,
                                            SecurityToken mySecurityToken)
        {
            foreach (var typePredefinition in myTypePredefinitions)
            {
                myTypePredefinitions.CheckNull("Element in myTypePredefinitions");

                ConvertUnknownAttributes(typePredefinition);
                ConvertPropertyUniques(typePredefinition);
                CheckSealedAndAbstract(typePredefinition);
                CheckVertexTypeName(typePredefinition);

                if (IsTypeBaseType(typePredefinition.SuperTypeName))
                {
                    if (!CanBaseTypeBeParentType(typePredefinition.SuperTypeName))
                        throw new InvalidBaseTypeException(typePredefinition.SuperTypeName);
                }
                else if (!CheckParentTypeExistInPredefinitions(typePredefinition.SuperTypeName, myTypePredefinitions))
                    GetType(typePredefinition.SuperTypeName, myTransactionToken, mySecurityToken);

                CheckAttributes(typePredefinition);
                CheckDefaultValue(typePredefinition);
                CheckUniques(typePredefinition);
                CheckIndices(typePredefinition);
            }
        }

        /// <summary>
        /// Converts the unknown properties into properties / edges.
        /// </summary>
        /// <param name="myUnknown">The unknown property predefinition.</param>
        /// <returns>The property predefinition.</returns>
        protected static PropertyPredefinition ConvertUnknownToProperty(UnknownAttributePredefinition myUnknown)
        {
            if (myUnknown.EdgeType != null)
                throw new Exception("An edge type is not allowed on a property.");

            var prop = new PropertyPredefinition(myUnknown.AttributeName, myUnknown.AttributeType)
                           .SetDefaultValue(myUnknown.DefaultValue)
                           .SetComment(myUnknown.Comment);

            if (myUnknown.IsUnique)
                prop.SetAsUnique();

            if (myUnknown.IsMandatory)
                prop.SetAsMandatory();

            if (myUnknown.Multiplicity != null)
                switch (myUnknown.Multiplicity)
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

        /// <summary>
        /// Checks whether a vertex type predefinition is not sealed and abstract.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The vertex type predefinition to be checked.</param>
        protected static void CheckSealedAndAbstract(ATypePredefinition myTypePredefinition)
        {
            if (myTypePredefinition.IsSealed && myTypePredefinition.IsAbstract)
                throw new UselessTypeException(myTypePredefinition);
        }

        /// <summary>
        /// Checks whether the vertex type property on an vertex type definition contains anything.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        protected static void CheckVertexTypeName(ATypePredefinition myTypePredefinition)
        {
            if (string.IsNullOrWhiteSpace(myTypePredefinition.TypeName))
                throw new EmptyTypeNameException();
        }

        /// <summary>
        /// Checks if the specified parent type exists inside the given type predefinitions.
        /// </summary>
        /// <param name="myType">The type.</param>
        /// <param name="myTypePredefintions">The type predefinitions.</param>
        protected static bool CheckParentTypeExistInPredefinitions(String myType,
                                                            IEnumerable<ATypePredefinition> myTypePredefintions)
        {
            return myTypePredefintions.Any(_ => _.TypeName.Equals(myType));
        }

        /// <summary>
        /// Check for the correct default value.
        /// </summary>
        /// <param name="vertexTypeDefinition">The vertex type predefinition to be checked.</param>
        protected void CheckDefaultValue(ATypePredefinition typePredefinition)
        {
            if (typePredefinition.Properties != null)
            {
                foreach (var item in typePredefinition.Properties.Where(_ => _.DefaultValue != null))
                {
                    try
                    {
                        var baseType = _baseStorageManager.GetBaseType(item.AttributeType);
                        item.DefaultValue.ConvertToIComparable(baseType);
                    }
                    catch (Exception)
                    {
                        throw new InvalidTypeException(item.DefaultValue.GetType().Name, item.AttributeType);
                    }
                }
            }
        }

        /// <summary>
        /// Checks the unique predefinitions.
        /// </summary>
        /// <param name="myTypePredefinition">The to be checked unique predefinitions.</param>
        protected static void CheckUniques(ATypePredefinition myTypePredefinition)
        {
            //TODO
            //check that the properties in the uniques are existing in the properties of the predefinition or any parent type
        }

        /// <summary>
        /// Checks the index predefinitions.
        /// </summary>
        /// <param name="myTypePredefinition">The to be checked index predefinitions.</param>
        protected static void CheckIndices(ATypePredefinition myTypePredefinition)
        {
            //TODO
        }

        /// <summary>
        /// Checks that the attribute name and type of the given AAttributePredefinitions are not null or empty.
        /// </summary>
        /// <param name="myPredefinitions">The to be checked predefinitions.</param>
        protected static void CheckNameAndTypeOfAttributePredefinitions(IEnumerable<AAttributePredefinition> myPredefinitions)
        {
            if (myPredefinitions != null)
                foreach (var predef in myPredefinitions)
                {
                    if (predef.AttributeName.IsNullOrEmpty())
                        throw new EmptyAttributeNameException(predef.GetType());

                    if (predef.AttributeType.IsNullOrEmpty())
                        throw new EmptyAttributeTypeException(predef.GetType());
                }
        }

        /// <summary>
        /// Checks that the attribute name of the given MandatoryPredefinitions are not null or empty.
        /// </summary>
        /// <param name="myPredefinitions">The to be checked predefinitions.</param>
        protected static void CheckNameAndTypeOfAttributePredefinitions(IEnumerable<MandatoryPredefinition> myPredefinitions)
        {
            if (myPredefinitions != null)
                foreach (var predef in myPredefinitions)
                    if (predef.MandatoryAttribute.IsNullOrEmpty())
                        throw new EmptyAttributeNameException(predef.GetType());
        }

        /// <summary>
        /// Checks that the attribute name of the given MandatoryPredefinitions are not null or empty.
        /// </summary>
        /// <param name="myPredefinitions">The to be checked predefinitions.</param>
        protected static void CheckNameAndTypeOfAttributePredefinitions(IEnumerable<UniquePredefinition> myPredefinitions)
        {
            if (myPredefinitions != null)
                foreach (var predef in myPredefinitions)
                    foreach (var prop in predef.Properties)
                        if (prop.IsNullOrEmpty())
                            throw new EmptyAttributeNameException(predef.GetType(), "A property name inside a UniquePredefinition was null or empty.");
        }

        /// <summary>
        /// Checks that the attribute name of the given MandatoryPredefinitions are not null or empty.
        /// </summary>
        /// <param name="myPredefinitions">The to be checked predefinitions.</param>
        protected static void CheckNameAndTypeOfAttributePredefinitions(IEnumerable<IndexPredefinition> myPredefinitions)
        {
            if (myPredefinitions != null)
                foreach (var predef in myPredefinitions)
                {
                    if (predef.VertexTypeName.IsNullOrEmpty())
                        throw new IndexCreationException(predef, "Name of vertex type is null or empty.");

                    if (predef.Properties == null || predef.Properties.Count == 0)
                        throw new IndexCreationException(predef, "Indexed properties cannot be null or empty.");

                    foreach (var prop in predef.Properties)
                        if (prop.IsNullOrEmpty())
                            throw new EmptyAttributeNameException(predef.GetType(), "A property name inside a UniquePredefinition was null or empty.");
                }
        }

        /// <summary>
        /// Checks that the attribute names are not null or empty.
        /// </summary>
        /// <param name="myAttributes">The to be checked attributes.</param>
        protected static void CheckNameOfAttributeList(IEnumerable<String> myAttributes)
        {
            if (myAttributes != null)
                foreach (var attr in myAttributes)
                    if (attr.IsNullOrEmpty())
                        throw new EmptyAttributeNameException();
        }

        /// <summary>
        /// Checks the to be renamed attributes
        /// </summary>
        /// <param name="myAlterTypeRequest"></param>
        /// <param name="myType"></param>
        protected static void CheckToBeRenamedAttributes(IRequestAlterType myAlterTypeRequest, T myType)
        {
            if (myAlterTypeRequest.ToBeRenamedProperties == null)
                return;

            foreach (var aToBeRenamedAttributes in myAlterTypeRequest.ToBeRenamedProperties)
            {
                if (aToBeRenamedAttributes.Key.IsNullOrEmpty())
                    throw new EmptyAttributeNameException(
                                "The to be renamed attribute name is null or empty.");

                if (aToBeRenamedAttributes.Value.IsNullOrEmpty())
                    throw new EmptyAttributeNameException(
                                String.Format("The to be attribute name to which the attribute {0} should be renamed is null or empty.",
                                                aToBeRenamedAttributes.Key));

                if (!CheckOldName(aToBeRenamedAttributes.Key, myType))
                {
                    throw new AttributeDoesNotExistException(
                        String.Format("It is not possible to rename {0} into {1}. The to be renamed attribute does not exist.",
                                        aToBeRenamedAttributes.Key, aToBeRenamedAttributes.Value));
                }

                if (!CheckNewName(aToBeRenamedAttributes.Value, myType))
                {
                    throw new AttributeAlreadyExistsException(
                        String.Format("It is not possible to rename {0} into {1}. The new attribute name already exists.",
                                        aToBeRenamedAttributes.Key, aToBeRenamedAttributes.Value));
                }
            }
        }

        /// <summary>
        /// Checks if the old name exists on the given vertex type
        /// </summary>
        /// <param name="myOldAttributeName">The old attribute name.</param>
        /// <param name="myType">The type.</param>
        /// <returns></returns>
        protected static bool CheckOldName(string myOldAttributeName, T myType)
        {
            if (myOldAttributeName != null)
            {
                var attributesOfCurrentVertexType = myType.GetAttributeDefinitions(false).ToList();

                return attributesOfCurrentVertexType.Any(_ => _.Name == myOldAttributeName);
            }

            return true;
        }

        /// <summary>
        /// Checks if the new name of the attribute already exists
        /// </summary>
        /// <param name="myNewAttributeName">The new attribute name.</param>
        /// <param name="myType">The type.</param>
        /// <returns></returns>
        protected static bool CheckNewName(string myNewAttributeName, T myType)
        {
            if (myNewAttributeName != null)
            {
                return myType.GetKinsmenTypesAndSelf()
                    .Select(aVertexType => aVertexType.GetAttributeDefinitions(false).ToArray())
                    .All(attributesOfCurrentVertexType => !attributesOfCurrentVertexType.Any(_ => _.Name == myNewAttributeName));
            }

            return true;
        }

        #endregion
    }
}

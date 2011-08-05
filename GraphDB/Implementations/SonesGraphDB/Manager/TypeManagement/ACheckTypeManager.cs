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

namespace sones.GraphDB.Manager.TypeManagement
{
    internal abstract class ACheckTypeManager<T>: ATypeManager<T>
        where T: IBaseType
    {
        /// <summary>
        /// The expected count of vertex types to add.
        /// </summary>
        private const int ExpectedTypes = 100;

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

            CheckAdd(myTypePredefinitions);

            return null;
        }

        /// <summary>
        /// Checks if the specified types can be removed.
        /// </summary>
        /// <param name="myTypes">The to be removed types.</param>
        /// <param name="myIgnoreReprimands">Specifies if reprimands on any type in myTypes should be ignored.</param>
        /// <returns></returns>
        public override Dictionary<long, string> RemoveTypes(IEnumerable<T> myTypes,
                                                                TransactionToken myTransaction,
                                                                SecurityToken mySecurity,
                                                                bool myIgnoreReprimands = false)
        {
            #region check arguments

            myTypes.CheckNull("myVertexTypes");

            #endregion

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
        }

        public override void TruncateType(string myTypeName,
                                            TransactionToken myTransactionToken,
                                            SecurityToken mySecurityToken)
        {
            GetType(myTypeName, myTransactionToken, mySecurityToken);
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

        public override abstract void Initialize(IMetaManager myMetaManager);

        public override abstract void Load(TransactionToken myTransaction,
                                            SecurityToken mySecurity);

        #endregion

        #region helper methods

        #region abstract helber methods

        /// <summary>
        /// Checks if the given parameter type is valid.
        /// </summary>
        /// <param name="myTypePredefinitions">The parameter to be checked.</param>
        protected abstract void CheckPredefinitionsType(IEnumerable<ATypePredefinition> myTypePredefinitions);

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
        protected abstract bool CanBeParentType(string myTypeName);

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

        protected abstract void CanRemove(IEnumerable<T> myTypes, 
                                            TransactionToken myTransaction, 
                                            SecurityToken mySecurity,
                                            bool myIgnoreReprimands);

        #endregion

        protected void CheckAdd(IEnumerable<ATypePredefinition> myTypePredefinitions)
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

            CanAddCheckBasics(myTypePredefinitions);

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
            #endregion
        }

        /// <summary>
        /// Checks for errors in a list of vertex type predefinitions without using the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The list of vertex type predefinitions to be checked.</param>
        protected void CanAddCheckBasics(IEnumerable<ATypePredefinition> myTypePredefinitions)
        {
            foreach (var typePredefinition in myTypePredefinitions)
            {
                myTypePredefinitions.CheckNull("Element in myTypePredefinitions");

                ConvertUnknownAttributes(typePredefinition);
                ConvertPropertyUniques(typePredefinition);
                CheckSealedAndAbstract(typePredefinition);
                CheckVertexTypeName(typePredefinition);

                if(CheckParentTypeAreNoBaseTypes(typePredefinition))
                    CheckParentTypeExistInPredefinitions(typePredefinition.SuperTypeName, myTypePredefinitions);

                CheckAttributes(typePredefinition);
                CheckDefaultValue(typePredefinition);
                CheckUniques(typePredefinition);
                CheckIndices(typePredefinition);
            }
        }

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

        protected static string GetTargetEdgeNameFromAttributeType(string myAttributeType)
        {
            return myAttributeType.Split(IncomingEdgePredefinition.TypeSeparator)[1];
        }

        protected static string GetTargetVertexTypeFromAttributeType(string myAttributeType)
        {
            return myAttributeType.Split(IncomingEdgePredefinition.TypeSeparator)[0];
        }

        /// <summary>
        /// Checks whether a vertex type predefinition is not sealed and abstract.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The vertex type predefinition to be checked.</param>
        protected static void CheckSealedAndAbstract(ATypePredefinition myTypePredefinition)
        {
            if (myTypePredefinition.IsSealed && myTypePredefinition.IsAbstract)
            {
                throw new UselessTypeException(myTypePredefinition);
            }
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
        /// Checks whether a vertex type predefinition is not derived from a base vertex type.
        /// </summary>
        /// <param name="myVertexTypeDefinition"></param>
        protected bool CheckParentTypeAreNoBaseTypes(ATypePredefinition myTypePredefinition)
        {
            if (!CanBeParentType(myTypePredefinition.SuperTypeName))
                return true;

            return false;
        }

        protected void CheckParentTypeExistInPredefinitions(String myType, IEnumerable<ATypePredefinition> myTypePredefintions)
        {
            if (!myTypePredefintions.Any(_ => _.TypeName.Equals(myType)))
                throw new InvalidBaseTypeException(myType);
        }

        /// <summary>
        /// Checks the uniqueness of property names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        /// <param name="myUniqueNameSet">A set of attribute names defined on this vertex type predefinition.</param>
        protected void CheckPropertiesUniqueName(ATypePredefinition myTypePredefinition, ISet<string> myUniqueNameSet)
        {
            if (myTypePredefinition.Properties != null)
                foreach (var prop in myTypePredefinition.Properties)
                {
                    prop.CheckNull("Property in type predefinition " + myTypePredefinition.TypeName);

                    if (!myUniqueNameSet.Add(prop.AttributeName))
                        throw new DuplicatedAttributeNameException(myTypePredefinition, prop.AttributeName);

                    CheckPropertyType(myTypePredefinition, prop);
                }
        }

        /// <summary>
        /// Checks if a given property definition has a valid type.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition that defines the property.</param>
        /// <param name="myProperty">The property to be checked.</param>
        protected void CheckPropertyType(ATypePredefinition myTypePredefinition, PropertyPredefinition myProperty)
        {
            if (String.IsNullOrWhiteSpace(myProperty.AttributeType))
            {
                throw new EmptyPropertyTypeException(myTypePredefinition, myProperty.AttributeName);
            }

            if (!_baseTypeManager.IsBaseType(myProperty.AttributeType))
            {
                throw new UnknownPropertyTypeException(myTypePredefinition, myProperty.AttributeType);
            }
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

        protected static void CheckUniques(ATypePredefinition myTypePredefinition)
        {
            //TODO
            //check that the properties in the uniques are existing in the properties of the predefinition or any parent type
        }

        protected static void CheckIndices(ATypePredefinition myTypePredefinition)
        {
            //TODO
        }

        /// <summary>
        /// Checks a list of ATypePredefinitions for duplicate names.
        /// </summary>
        /// <param name="myTypeDefinitions">A list of type predefinitions.</param>
        /// <returns>A dictionary of name to ATypePredefinition.</returns>
        protected static Dictionary<String, ATypePredefinition> CanAddCheckDuplicates(
            IEnumerable<ATypePredefinition> myTypePredefinitions)
        {
            var result = (myTypePredefinitions is ICollection)
                ? new Dictionary<String, ATypePredefinition>((myTypePredefinitions as ICollection).Count)
                : new Dictionary<String, ATypePredefinition>(ExpectedTypes);

            foreach (var predef in myTypePredefinitions)
            {
                if (result.ContainsKey(predef.TypeName))
                    throw new DuplicatedVertexTypeNameException(predef.TypeName);

                result.Add(predef.TypeName, predef);
            }

            return result;
        }

        /// <summary>
        /// Sorts a list of type predefinitions topologically regarding their parentPredef type name.
        /// </summary>
        /// <param name="myDefsByVertexName"></param>
        /// <param name="myDefsByParentVertexName"></param>
        /// <returns> if the vertex type predefinition can be sorted topologically regarding their parentPredef type, otherwise false.</returns>
        protected static LinkedList<ATypePredefinition> CanAddSortTopolocically(
            Dictionary<String, ATypePredefinition> myDefsByName,
            Dictionary<String, IEnumerable<ATypePredefinition>> myDefsByParentName)
        {

            //The list of topolocically sorted vertex types
            //In this step, we assume that parent types, that are not in the list of predefinitons are correct.
            //Correct means: either they are in fs or they are not in fs but then they are not defined. (this will be detected later)
            var correctRoots = myDefsByParentName
                .Where(parent => !myDefsByName.ContainsKey(parent.Key))
                .SelectMany(x => x.Value);

            var result = new LinkedList<ATypePredefinition>(correctRoots);


            //Here we step throught the list of topolocically sorted predefinitions.
            //Each predefinition that is in this list, is a valid parent type for other predefinitions.
            //Thus we can add all predefinitions, that has parent predefinition in the list to the end of the list.
            for (var current = result.First; current != null; current = current.Next)
            {
                if (!myDefsByParentName.ContainsKey(current.Value.TypeName))
                    continue;

                //All predefinitions, that has the current predefintion as parent vertex type.
                var corrects = myDefsByParentName[current.Value.TypeName];

                foreach (var correct in corrects)
                {
                    result.AddLast(correct);
                }
            }


            if (myDefsByName.Count > result.Count)
                //There are some defintions that are not in the vertex...so they must contain a circle.
                throw new CircularTypeHierarchyException(myDefsByName.Values.Except(result));

            return result;
        }

        /// <summary>
        /// TODO find better check method
        /// Checks if the given type is a base type
        /// </summary>
        protected static bool IsTypeBaseType(long myTypeID)
        {
            return Enum.IsDefined(typeof(BaseTypes), myTypeID);

            //return ((long)BaseTypes.Attribute).Equals(myTypeID) ||
            //            ((long)BaseTypes.BaseType).Equals(myTypeID) ||
            //            ((long)BaseTypes.BinaryProperty).Equals(myTypeID) ||
            //            ((long)BaseTypes.Edge).Equals(myTypeID) ||
            //            ((long)BaseTypes.EdgeType).Equals(myTypeID) ||
            //            ((long)BaseTypes.IncomingEdge).Equals(myTypeID) ||
            //            ((long)BaseTypes.Index).Equals(myTypeID) ||
            //            ((long)BaseTypes.Orderable).Equals(myTypeID) ||
            //            ((long)BaseTypes.OutgoingEdge).Equals(myTypeID) ||
            //            ((long)BaseTypes.Property).Equals(myTypeID) ||
            //            ((long)BaseTypes.Vertex).Equals(myTypeID) ||
            //            ((long)BaseTypes.VertexType).Equals(myTypeID) ||
            //            ((long)BaseTypes.Weighted).Equals(myTypeID);
        }

        #endregion
    }
}

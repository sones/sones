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
using sones.GraphDB.Manager.Vertex;
using sones.GraphDB.Manager.BaseGraph;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Request;
using sones.GraphDB.ErrorHandling;
using System.Collections;
using sones.Library.LanguageExtensions;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal abstract class ATypeManager<T>: ITypeHandler<T>
        where T: IBaseType
    {
        #region Data

        protected BaseTypeManager               _baseTypeManager;
        protected BaseGraphStorageManager       _baseStorageManager;
        protected IManagerOf<IVertexHandler>    _vertexManager;

        /// <summary>
        /// The expected count of types to add.
        /// </summary>
        protected const int ExpectedTypes = 100;

        #endregion

        #region ITypeManager Members

        public abstract T GetType(long myTypeId, 
                                    TransactionToken myTransaction, 
                                    SecurityToken mySecurity);

        public abstract T GetType(string myTypeName, 
                                    TransactionToken myTransaction, 
                                    SecurityToken mySecurity);

        public abstract IEnumerable<T> GetAllTypes(TransactionToken myTransaction, 
                                                    SecurityToken mySecurity);
        
        public abstract IEnumerable<T> AddTypes(IEnumerable<ATypePredefinition> myTypePredefinitions, 
                                                    TransactionToken myTransaction, 
                                                    SecurityToken mySecurity);

        public abstract Dictionary<Int64, String> RemoveTypes(IEnumerable<T> myTypes, 
                                                                TransactionToken myTransaction, 
                                                                SecurityToken mySecurity, 
                                                                bool myIgnoreReprimands = false);

        public abstract IEnumerable<long> ClearTypes(TransactionToken myTransaction, 
                                                        SecurityToken mySecurity);

        public abstract void TruncateType(long myTypeID, 
                                            TransactionToken myTransactionToken, 
                                            SecurityToken mySecurityToken);

        public abstract void TruncateType(String myTypeName, 
                                            TransactionToken myTransactionToken, 
                                            SecurityToken mySecurityToken);

        public abstract T AlterType(IRequestAlterType myAlterTypeRequest,
                                        TransactionToken myTransactionToken,
                                        SecurityToken mySecurityToken);

        public abstract bool HasType(string myTypeName,
                                        TransactionToken myTransactionToken,
                                        SecurityToken mySecurityToken);

        public abstract void CleanUpTypes();

        #endregion

        #region IManager Members

        public abstract void Initialize(IMetaManager myMetaManager);

        public abstract void Load(TransactionToken myTransaction, 
                                    SecurityToken mySecurity);

        #endregion

        #region helper methods

        /// <summary>
        /// Checks if the given type is a base type
        /// </summary>
        protected abstract bool IsTypeBaseType(long myTypeID);

        /// <summary>
        /// Checks if the given type is a base type
        /// </summary>
        protected abstract bool IsTypeBaseType(string myTypeName);

        /// <summary>
        /// Checks if the given parameter type is valid.
        /// </summary>
        /// <param name="myRequest">The parameter to be checked.</param>
        protected abstract void CheckRequestType(IRequestAlterType myRequest);

        /// <summary>
        /// Gets the name of the outgoing edge to which the incoming edge references.
        /// </summary>
        /// <param name="myAttributeType">The attribute type.</param>
        /// <returns>A string which represents the name.</returns>
        protected static string GetTargetEdgeNameFromAttributeType(string myAttributeType)
        {
            return myAttributeType.Split(IncomingEdgePredefinition.TypeSeparator)[1];
        }

        /// <summary>
        /// Gets the name of the vertex type of the outgoing edge to which the incoming edge references.
        /// </summary>
        /// <param name="myAttributeType">The attribute type.</param>
        /// <returns>A string which represents the name.</returns>
        protected static string GetTargetVertexTypeFromAttributeType(string myAttributeType)
        {
            return myAttributeType.Split(IncomingEdgePredefinition.TypeSeparator)[0];
        }

        /// <summary>
        /// Checks if it possible to remove a mandatory or unique constraint
        /// </summary>
        /// <param name="myMandatories"></param>
        /// <param name="myUniques"></param>
        /// <param name="vertexType"></param>
        protected static void CheckToBeRemovedMandatoryAndUnique(IEnumerable<string> myMandatories,
                                                                    IEnumerable<string> myUniques,
                                                                    T myType)
        {
            CheckToBeRemovedMandatory(myMandatories, myType);
        }

        /// <summary>
        /// Checks that the mandatory attributes exists on type and are realy mandatory.
        /// </summary>
        /// <param name="myMandatories">The names of the mandatory attribtues.</param>
        /// <param name="myType">The type.</param>
        protected static void CheckToBeRemovedMandatory(IEnumerable<string> myMandatories,
                                                        T myType)
        {
            if (myMandatories == null)
                return;

            var attributes = myType.GetAttributeDefinitions(false).ToList();

            foreach (var aMandatory in myMandatories)
            {
                if (!attributes.Any(_ => _.Name == aMandatory && (_ as IPropertyDefinition).IsMandatory))
                    throw new AttributeDoesNotExistException(aMandatory, myType.Name);
            }
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
        /// Checks the uniqueness of property names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        /// <param name="myUniqueNameSet">A set of attribute names defined on this vertex type predefinition.</param>
        protected void CheckPropertiesUniqueName(ATypePredefinition myTypePredefinition,
                                                    ISet<string> myUniqueNameSet)
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
        protected void CheckPropertyType(ATypePredefinition myTypePredefinition,
                                            PropertyPredefinition myProperty)
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
        /// Checks whether the edge type property on an outgoing edge definition contains anything.
        /// </summary>
        /// <param name="myTypeDefinition">The type predefinition that defines the outgoing edge.</param>
        /// <param name="myEdge">The outgoing edge to be checked.</param>
        protected static void CheckEdgeType(ATypePredefinition myTypeDefinition,
                                            OutgoingEdgePredefinition myEdge)
        {
            if (string.IsNullOrWhiteSpace(myEdge.EdgeType))
            {
                throw new EmptyEdgeTypeException(myTypeDefinition, myEdge.AttributeName);
            }
        }

        /// <summary>
        /// Checks the uniqueness of incoming edge names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        /// <param name="myUniqueNameSet">A set of attribute names defined on this vertex type predefinition.</param>
        protected static void CheckIncomingEdgesUniqueName(VertexTypePredefinition myVertexTypeDefinition,
                                                            ISet<String> myUniqueNameSet)
        {
            if (myVertexTypeDefinition.IncomingEdges != null)
                foreach (var edge in myVertexTypeDefinition.IncomingEdges)
                {
                    edge.CheckNull("Incoming myEdge in vertex type predefinition " + myVertexTypeDefinition.TypeName);
                    if (!myUniqueNameSet.Add(edge.AttributeName))
                        throw new DuplicatedAttributeNameException(myVertexTypeDefinition, edge.AttributeName);
                }
        }

        /// <summary>
        /// Checks the uniqueness of outgoing edge names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        /// <param name="myUniqueNameSet">A set of attribute names defined on this vertex type predefinition.</param>
        protected static void CheckOutgoingEdgesUniqueName(VertexTypePredefinition myVertexTypeDefinition,
                                                            ISet<string> myUniqueNameSet)
        {
            if (myVertexTypeDefinition.OutgoingEdges != null)
                foreach (var edge in myVertexTypeDefinition.OutgoingEdges)
                {
                    edge.CheckNull("Outgoing myEdge in vertex type predefinition " + myVertexTypeDefinition.TypeName);
                    if (!myUniqueNameSet.Add(edge.AttributeName))
                        throw new DuplicatedAttributeNameException(myVertexTypeDefinition, edge.AttributeName);

                    CheckEdgeType(myVertexTypeDefinition, edge);
                }
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

        #endregion
    }
}

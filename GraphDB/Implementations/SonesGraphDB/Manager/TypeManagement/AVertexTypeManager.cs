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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Request;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.LanguageExtensions;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal abstract class AVertexTypeManager: IVertexTypeHandler
    {
        #region IVertexTypeManager Members

        public abstract IVertexType GetVertexType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity);
               
        public abstract IVertexType GetVertexType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity);

        public abstract IEnumerable<IVertexType> GetAllVertexTypes(TransactionToken myTransaction, SecurityToken mySecurity);

        public abstract IEnumerable<IVertexType> AddVertexTypes(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity);

        public abstract Dictionary<Int64, String> RemoveVertexTypes(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, bool myIgnoreReprimands = false);

        public abstract IEnumerable<long> ClearDB(TransactionToken myTransaction, SecurityToken mySecurity);

        public abstract void TruncateVertexType(long myVertexTypeID, TransactionToken myTransactionToken, SecurityToken mySecurityToken);
        
        public abstract void TruncateVertexType(String myVertexTypeName, TransactionToken myTransactionToken, SecurityToken mySecurityToken);

        public abstract IVertexType AlterVertexType(RequestAlterVertexType myAlterVertexTypeRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        public abstract bool HasVertexType(string myVertexTypeName, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        public abstract void CleanUpTypes();

        #endregion

        /// <summary>
        /// The expected count of vertex types to add.
        /// </summary>
        private const int ExpectedVertexTypes = 100;

        /// <summary>
        /// The base type manager
        /// </summary>
        protected BaseTypeManager _baseTypeManager;

        /// <summary>
        /// Checks a list of VertexTypePredefinitions for duplicate vertex names.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">A list of vertex type predefinitions.</param>
        /// <returns>A dictionary of vertex name to VertexTypePredefinition.</returns>
        protected static Dictionary<String, VertexTypePredefinition> CanAddCheckDuplicates(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions)
        {
            var result = (myVertexTypeDefinitions is ICollection)
                ? new Dictionary<String, VertexTypePredefinition>((myVertexTypeDefinitions as ICollection).Count)
                : new Dictionary<String, VertexTypePredefinition>(ExpectedVertexTypes);

            foreach (var predef in myVertexTypeDefinitions)
            {
                if (result.ContainsKey(predef.TypeName))
                    throw new DuplicatedVertexTypeNameException(predef.TypeName);

                result.Add(predef.TypeName, predef);
            }
            return result;
        }


        /// <summary>
        /// Sorts a list of vertex type predefinitions topologically regarding their parentPredef type name.
        /// </summary>
        /// <param name="myDefsByVertexName"></param>
        /// <param name="myDefsByParentVertexName"></param>
        /// <returns> if the vertex type predefinition can be sorted topologically regarding their parentPredef type, otherwise false.</returns>
        protected static LinkedList<VertexTypePredefinition> CanAddSortTopolocically(
            Dictionary<String, VertexTypePredefinition> myDefsByVertexName,
            Dictionary<String, IEnumerable<VertexTypePredefinition>> myDefsByParentVertexName)
        {

            //The list of topolocically sorted vertex types
            //In this step, we assume that parent types, that are not in the list of predefinitons are correct.
            //Correct means: either they are in fs or they are not in fs but then they are not defined. (this will be detected later)
            var correctRoots = myDefsByParentVertexName
                .Where(parent => !myDefsByVertexName.ContainsKey(parent.Key))
                .SelectMany(x => x.Value);
            var result = new LinkedList<VertexTypePredefinition>(correctRoots);


            //Here we step throught the list of topolocically sorted predefinitions.
            //Each predefinition that is in this list, is a valid parent type for other predefinitions.
            //Thus we can add all predefinitions, that has parent predefinition in the list to the end of the list.
            for (var current = result.First; current != null; current = current.Next)
            {
                if (!myDefsByParentVertexName.ContainsKey(current.Value.TypeName)) 
                    continue;

                //All predefinitions, that has the current predefintion as parent vertex type.
                var corrects = myDefsByParentVertexName[current.Value.TypeName];

                foreach (var correct in corrects)
                {
                    result.AddLast(correct);
                }
            }


            if (myDefsByVertexName.Count > result.Count)
                //There are some defintions that are not in the vertex...so they must contain a circle.
                throw new CircularTypeHierarchyException(myDefsByVertexName.Values.Except(result));

            return result;
        }

        /// <summary>
        /// Checks the uniqueness of incoming edge names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        /// <param name="myUniqueNameSet">A set of attribute names defined on this vertex type predefinition.</param>
        protected static void CheckIncomingEdgesUniqueName(VertexTypePredefinition myVertexTypeDefinition, ISet<String> myUniqueNameSet)
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
        /// Checks the uniqueness of property names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        /// <param name="myUniqueNameSet">A set of attribute names defined on this vertex type predefinition.</param>
        protected static void CheckOutgoingEdgesUniqueName(VertexTypePredefinition myVertexTypeDefinition, ISet<string> myUniqueNameSet)
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
        /// Checks whether the edge type property on an outgoing edge definition contains anything.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition that defines the outgoing edge.</param>
        /// <param name="myEdge">The outgoing edge to be checked.</param>
        protected static void CheckEdgeType(VertexTypePredefinition myVertexTypeDefinition, OutgoingEdgePredefinition myEdge)
        {
            if (string.IsNullOrWhiteSpace(myEdge.EdgeType))
            {
                throw new EmptyEdgeTypeException(myVertexTypeDefinition, myEdge.AttributeName);
            }
        }

        /// <summary>
        /// Checks the uniqueness of property names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        /// <param name="myUniqueNameSet">A set of attribute names defined on this vertex type predefinition.</param>
        protected void CheckPropertiesUniqueName(VertexTypePredefinition myVertexTypeDefinition, ISet<string> myUniqueNameSet)
        {
            if (myVertexTypeDefinition.Properties != null)
                foreach (var prop in myVertexTypeDefinition.Properties)
                {
                    prop.CheckNull("Property in vertex type predefinition " + myVertexTypeDefinition.TypeName);
                    if (!myUniqueNameSet.Add(prop.AttributeName))
                        throw new DuplicatedAttributeNameException(myVertexTypeDefinition, prop.AttributeName);

                    CheckPropertyType(myVertexTypeDefinition, prop);
                }
        }

        /// <summary>
        /// Checks if a given property definition has a valid type.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition that defines the property.</param>
        /// <param name="myProperty">The property to be checked.</param>
        protected void CheckPropertyType(VertexTypePredefinition myVertexTypeDefinition, PropertyPredefinition myProperty)
        {
            if (String.IsNullOrWhiteSpace(myProperty.AttributeType))
            {
                throw new EmptyPropertyTypeException(myVertexTypeDefinition, myProperty.AttributeName);
            }
                        
            if (!_baseTypeManager.IsBaseType(myProperty.AttributeType))
            {
                throw new UnknownPropertyTypeException(myVertexTypeDefinition, myProperty.AttributeType);
            }
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
        /// TODO find better check method
        /// Checks if the given type is a base type
        /// </summary>
        protected static bool IsTypeBaseType(long myTypeID)
        {
            return ((long)BaseTypes.Attribute).Equals(myTypeID) ||
                        ((long)BaseTypes.BaseType).Equals(myTypeID) ||
                        ((long)BaseTypes.BinaryProperty).Equals(myTypeID) ||
                        ((long)BaseTypes.Edge).Equals(myTypeID) ||
                        ((long)BaseTypes.EdgeType).Equals(myTypeID) ||
                        ((long)BaseTypes.IncomingEdge).Equals(myTypeID) ||
                        ((long)BaseTypes.Index).Equals(myTypeID) ||
                        ((long)BaseTypes.Orderable).Equals(myTypeID) ||
                        ((long)BaseTypes.OutgoingEdge).Equals(myTypeID) ||
                        ((long)BaseTypes.Property).Equals(myTypeID) ||
                        ((long)BaseTypes.Vertex).Equals(myTypeID) ||
                        ((long)BaseTypes.VertexType).Equals(myTypeID) ||
                        ((long)BaseTypes.Weighted).Equals(myTypeID);
        }

        #region IVertexTypeHandler Members

        #endregion

        #region IManager Members

        public abstract void Initialize(IMetaManager myMetaManager);
        
        public abstract void Load(TransactionToken myTransaction, SecurityToken mySecurity);
        

        #endregion
    }
}

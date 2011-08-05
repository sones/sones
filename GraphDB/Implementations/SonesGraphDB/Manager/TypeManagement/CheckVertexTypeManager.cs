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

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class CheckVertexTypeManager : ACheckTypeManager<IVertexType>
    {
        private ITypeHandler<IVertexType> _vertexTypeManager;

        #region ACheckTypeManager member

        public override void Initialize(IMetaManager myMetaManager)
        {
            _vertexTypeManager  = myMetaManager.VertexTypeManager.ExecuteManager;
            _baseTypeManager    = myMetaManager.BaseTypeManager;
            _baseStorageManager = myMetaManager.BaseGraphStorageManager;
        }

        public override void Load(TransactionToken myTransaction, SecurityToken mySecurity)
        { }

        #endregion

        #region private helper

        #region private abstract helper

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
                if(!myTypePredefinitions.All(_ => _ is VertexTypePredefinition))
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
        protected override bool CanBeParentType(string myTypeName)
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
                #region check that the remove type is no base type

                if (delType == null)
                    throw new TypeRemoveException<IVertexType>("null" , "Vertex Type is null.");

                if (!delType.HasParentType)
                    continue;

                if (delType.ParentVertexType.ID.Equals((long)BaseTypes.BaseType) && IsTypeBaseType(delType.ID))
                    //Exception that base type cannot be deleted
                    throw new TypeRemoveException<IVertexType>(delType.Name, "A BaseType connot be removed.");

                #endregion

                #region check that existing child types are specified

                if (delType.GetDescendantVertexTypes().Any(child => !myTypes.Contains(child)))
                    throw new TypeRemoveException<IVertexType>(delType.Name, "The given type has child types and cannot be removed.");

                #endregion

                #region check that the delete type has no incoming edges, just when reprimands should not be ignored

                if(!myIgnoreReprimands)
                    if (delType.HasIncomingEdges(false))
                        if (delType.GetIncomingEdgeDefinitions(false).Any(edge => edge.RelatedType.ID != delType.ID))
                            throw new TypeRemoveException<IVertexType>(delType.Name, "The given type has incoming edges and cannot be removed.");

                #endregion
            }

            #endregion
        }

        #endregion

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

        private static void CheckBinaryPropertiesUniqueName(VertexTypePredefinition myVertexTypeDefinition, ISet<string> myUniqueNameSet)
        {
            if (myVertexTypeDefinition.BinaryProperties != null)
                foreach (var prop in myVertexTypeDefinition.BinaryProperties)
                {
                    prop.CheckNull("Binary Property in vertex type predefinition " + myVertexTypeDefinition.TypeName);
                    if (!myUniqueNameSet.Add(prop.AttributeName))
                        throw new DuplicatedAttributeNameException(myVertexTypeDefinition, prop.AttributeName);
                }
        }

        #endregion
    }
}

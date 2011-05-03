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

namespace sones.GraphDB.Manager.TypeManagement
{
    internal sealed class CheckVertexTypeManager: AVertexTypeManager
    {

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

        public override void RemoveVertexTypes(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region check arguments

            myVertexTypes.CheckNull("myVertexTypes");

            #endregion

            CanRemove(myVertexTypes, myTransaction, mySecurity);
        }

        public override void UpdateVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region check arguments

            myVertexTypeDefinitions.CheckNull("myVertexTypeDefinitions");

            #endregion

            CanUpdate(myVertexTypeDefinitions, myTransaction, mySecurity);
        }


        #endregion

        private bool CanRemove(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
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
    }
}

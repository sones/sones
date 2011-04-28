using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Expression;
using sones.GraphDB.Request;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.LanguageExtensions;
using System.Collections;
using sones.GraphDB.Manager.Vertex;
using sones.GraphDB.Manager.Index;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.VertexStore.Definitions;
using System.Threading;
using sones.GraphDB.Manager.BaseGraph;
using sones.GraphDB.Index;
using sones.Library.Commons.VertexStore.Definitions.Update;
using sones.GraphDB.Request.CreateVertexTypes;

/*
 * edge cases:
 *   - if someone changes the super type of an vertex or edge type 
 *     - Henning, Timo 
 *       - that this isn't a required feature for version 2.0
 * 
 *   - undoability of the typemanager 
 *     - Henning, Timo 
 *       - the type manager is only responsible for converting type changing request into filesystem requests
 *       - the ability to undo an request should be implemented in the corresponding piplineable request
 * 
 *   - unique myAttributes
 *     - Henning, Timo
 *       - the type manager creates unique indices on attributes on the type that declares the uniqueness attribute and all deriving types
 * 
 *   - load 
 *     - Timo
 *       - will proove if the main vertex types are available
 *       - will load the main vertex types
 *       - looks for the maximum vertex type id
 * 
 *   - create
 *     - Timo
 *       - not part of the vertex type manager
 *       
 *   - get vertex type
 *     - if one of the base vertex types is requested, return a predefined vertex.
 * 
 *   - insert vertex type
 *     - no type can derive from the base types
 */

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed class VertexTypeManager : IVertexTypeManager
    {
        #region Data

        /// <summary>
        /// Used to communicate with the persistance layer.
        /// </summary>
        private IVertexManager _vertexManager;

        /// <summary>
        /// Used to get or create index instances.
        /// </summary>
        private IIndexManager _indexManager;

        /// <summary>
        /// Used to check outgoing edge definitions.
        /// </summary>
        private IEdgeTypeManager _edgeManager;

        /// <summary>
        /// Stores the last vertex type id.
        /// </summary>
        private long _LastTypeID;

        /// <summary>
        /// Stores the last attribute id.
        /// </summary>
        private long _LastAttrID;


        /// <summary>
        /// The unique ids for vertices indexed by their type id.
        /// </summary>
        private Dictionary<long, UniqueID> _vertexIDs = new Dictionary<long, UniqueID>();

        #endregion

        #region C'tor

        /// <summary>
        /// Create a new VertexTypeManager.
        /// </summary>
        public VertexTypeManager()
        {
        }

        #endregion
        
        #region Constants

        /// <summary>
        /// The expected count of vertex types to add.
        /// </summary>
        private const int ExpectedVertexTypes = 100;

        /// <summary>
        /// A property expression on VertexType.Name
        /// </summary>
        private readonly IExpression _vertexTypeNameExpression = new PropertyExpression(BaseTypes.VertexType.ToString(), AttributeDefinitions.Name.ToString());

        /// <summary>
        /// A property expression on VertexType.ID
        /// </summary>
        private readonly IExpression _vertexTypeIDExpression = new PropertyExpression(BaseTypes.VertexType.ToString(), AttributeDefinitions.ID.ToString());

        /// <summary>
        /// A property expression on OutgoingEdge.Name
        /// </summary>
        private readonly IExpression _attributeNameExpression = new PropertyExpression(BaseTypes.OutgoingEdge.ToString(), AttributeDefinitions.Name.ToString());
        
        /// <summary>
        /// stores the base vertex types indexed by name.
        /// </summary>
        private Dictionary<string, IVertexType> _baseTypes = new Dictionary<String, IVertexType>();

        #endregion

        #region IVertexTypeManager Members

        #region Retrieving

        public IVertexType GetVertexType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region get static types

            if (Enum.IsDefined(typeof(BaseTypes), myTypeId) && _baseTypes.ContainsKey(((BaseTypes)myTypeId).ToString()))
            {
                return _baseTypes[((BaseTypes)myTypeId).ToString()];
            }

            #endregion


            #region get from fs

            var vertex = Get(myTypeId, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeId));

            return new VertexType(vertex);
        
            #endregion
        }

        public IVertexType GetVertexType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            if (String.IsNullOrWhiteSpace(myTypeName))
                throw new ArgumentOutOfRangeException("myTypeName", "The type name must contain at least one character.");

            #region get static types

            if (_baseTypes.ContainsKey(myTypeName))
            {
                return _baseTypes[myTypeName];
            }

            #endregion

            #region get from fs

            var vertex = Get(myTypeName, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeName));

            return new VertexType(vertex);

            #endregion
        }

        public IEnumerable<IVertexType> GetAllVertexTypes(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertices = _vertexManager.GetVertices(BaseTypes.VertexType.ToString(), myTransaction, mySecurity);

            if (vertices == null)
                return Enumerable.Empty<IVertexType>();

            return vertices.Select(x => new VertexType(x));
        }

        #endregion

        #region Updates

        #region Add

        public void CanAddVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region check arguments

            myVertexTypeDefinitions.CheckNull("myVertexTypeDefinitions");
            
            #endregion

            CheckAdd(myVertexTypeDefinitions, myTransaction, mySecurity);
        }


        public IEnumerable<IVertexType> AddVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return Add(myVertexTypeDefinitions, myTransaction, mySecurity);
        }

        #endregion

        #region Remove

        public void CanRemoveVertexType(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region check arguments

            myVertexTypes.CheckNull("myVertexTypes");
            
            #endregion
            
            CanRemove(myVertexTypes, myTransaction, mySecurity);
        }

        public void RemoveVertexType(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            Remove(myVertexTypes, myTransaction, mySecurity);
        }

        #endregion

        #region Update

        public void CanUpdateVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region check arguments

            myVertexTypeDefinitions.CheckNull("myVertexTypeDefinitions");

            #endregion

            CanUpdate(myVertexTypeDefinitions, myTransaction, mySecurity);
        }

        public void UpdateVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            Update(myVertexTypeDefinitions, myTransaction, mySecurity);
        }

        #endregion

        #endregion
        
        #endregion

        #region private members

        #region Get

        /// <summary>
        /// Gets an IVertex representing the vertex type given by <paramref name="myTypeName"/>.
        /// </summary>
        /// <param name="myTypeName">The vertex type name.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An IVertex instance, that represents the vertex type with the given name or <c>NULL</c>, if not present.</returns>
        private IVertex Get(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region get the type from fs

            return _vertexManager.GetSingleVertex(new BinaryExpression(_vertexTypeNameExpression, BinaryOperator.Equals, new SingleLiteralExpression(myTypeName)), myTransaction, mySecurity);

            #endregion
        }

        /// <summary>
        /// Gets an IVertex representing the vertex type given by <paramref name="myTypeID"/>.
        /// </summary>
        /// <param name="myTypeName">The vertex type ID.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An IVertex instance, that represents the vertex type with the given ID or <c>NULL</c>, if not present.</returns>
        private IVertex Get(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region get the type from fs

            return _vertexManager.GetSingleVertex(new BinaryExpression(_vertexTypeIDExpression, BinaryOperator.Equals, new SingleLiteralExpression(myTypeId)), myTransaction, mySecurity);

            #endregion
        }


        #endregion

        #region Add

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
        /// Does the necessary checks for can add with the use of the FS.
        /// </summary>
        /// <param name="myDefsTopologically">A topologically sorted list of vertex type predefinitions. <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myDefsByName">The same vertex type predefinitions as in <paramref name="myDefsTpologically"/>, but indexed by their name. <remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <remarks><paramref name="myDefsTopologically"/> and <paramref name="myDefsByName"/> must contain the same vertex type predefinitions. This is never checked.</remarks>
        /// The predefinitions are checked one by one in topologically order. 
        private void CanAddCheckWithFS(
            LinkedList<VertexTypePredefinition> myDefsTopologically, 
            Dictionary<String, VertexTypePredefinition> myDefsByName,
            TransactionToken myTransaction, SecurityToken mySecurity)
        {

            //Contains the vertex type name to the attribute names of the vertex type.
            var attributes = new Dictionary<String, HashSet<String>>(myDefsTopologically.Count);

            for (var current = myDefsTopologically.First; current != null; current = current.Next)
            {
                 CanAddCheckVertexNameUniqueWithFS(current.Value, myTransaction, mySecurity);
                 CanAddCheckAttributeNameUniquenessWithFS(current, attributes, myTransaction, mySecurity);
                 CanAddCheckOutgoingEdgeTargets(current.Value, myDefsByName, myTransaction, mySecurity);
                 CanAddCheckIncomingEdgeSources(current.Value, myDefsByName, myTransaction, mySecurity);
             }
        }

        /// <summary>
        /// Checks if an incoming myEdge has a coresponding outgoing myEdge.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The vertex type definition of that the incoming edges will be checked,</param>
        /// <param name="myDefsByName">The vertex type predefinitions indexed by their name that are alse defined in this CanAdd operation.<remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CanAddCheckIncomingEdgeSources(VertexTypePredefinition myVertexTypePredefinition, IDictionary<string, VertexTypePredefinition> myDefsByName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var grouped = myVertexTypePredefinition.IncomingEdges.GroupBy(x => x.AttributeType.Substring(0, x.AttributeType.IndexOf('.') + 1));
            foreach (var group in grouped)
            {
                if (!myDefsByName.ContainsKey(group.Key))
                {
                    var vertex = Get(group.Key, myTransaction, mySecurity);
                    if (vertex == null)
                        throw new TargetVertexTypeNotFoundException(myVertexTypePredefinition, group.Key, group.Select(x=>x.AttributeName));

                    var attributes = vertex.GetIncomingVertices((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.DefiningType);
                    foreach (var edge in group)
                    {
                        if (!attributes.Any(outgoing => GetTargetVertexTypeFromAttributeType(edge.AttributeName).Equals(outgoing.GetPropertyAsString((long)AttributeDefinitions.Name))))
                            throw new OutgoingEdgeNotFoundException(myVertexTypePredefinition, edge);
                    }
                }
                else
                {
                    var target = myDefsByName[group.Key];

                    foreach (var edge in group)
                    {
                        if (!target.OutgoingEdges.Any(outgoing => GetTargetEdgeNameFromAttributeType(edge.AttributeName).Equals(outgoing.AttributeName)))
                            throw new OutgoingEdgeNotFoundException(myVertexTypePredefinition, edge);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Checks if outgoing edges have a valid target.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The vertex type definition of that the outgoing edges will be checked,</param>
        /// <param name="myDefsByName">The vertex type predefinitions indexed by their name that are alse defined in this CanAdd operation.<remarks><c>NULL</c> is not allowed, but not checked.</remarks></param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CanAddCheckOutgoingEdgeTargets(VertexTypePredefinition myVertexTypePredefinition, IDictionary<string, VertexTypePredefinition> myDefsByName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var grouped = myVertexTypePredefinition.OutgoingEdges.GroupBy(x => x.AttributeType);
            foreach (var group in grouped)
            {
                if (!myDefsByName.ContainsKey(group.Key))
                {
                    var vertex = Get(group.Key, myTransaction, mySecurity);
                    if (vertex == null)
                        throw new TargetVertexTypeNotFoundException(myVertexTypePredefinition, group.Key, group.Select(x => x.AttributeName));

                }
            }
        }

        /// <summary>
        /// Checks if the name of the given vertex type predefinition is not used in FS before.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The name of this vertex type definition will be checked.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CanAddCheckVertexNameUniqueWithFS(VertexTypePredefinition myVertexTypePredefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            if (Get(myVertexTypePredefinition.VertexTypeName, myTransaction, mySecurity) != null)
                throw new DuplicatedVertexTypeNameException(myVertexTypePredefinition.VertexTypeName);
        }

        /// <summary>
        /// Checks if the attribute names on vertex type definitions are unique, containing parent myAttributes.
        /// </summary>
        /// <param name="myTopologicallySortedPointer">A pointer to a vertex type predefinitions in a topologically sorted linked list.</param>
        /// <param name="myAttributes">A dictionary vertex type name to attribute names, that is build up during the process of CanAddCheckWithFS.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        private void CanAddCheckAttributeNameUniquenessWithFS(LinkedListNode<VertexTypePredefinition> myTopologicallySortedPointer, IDictionary<string, HashSet<string>> myAttributes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var parentPredef = GetParentPredefinitionOnTopologicallySortedList(myTopologicallySortedPointer);

            if (parentPredef == null)
            {
                //Get the parent type from FS.
                var parent = Get(myTopologicallySortedPointer.Value.SuperVertexTypeName, myTransaction, mySecurity);

                if (parent == null)
                    //No parent type was found.
                    throw new InvalidBaseVertexTypeException(myTopologicallySortedPointer.Value.SuperVertexTypeName);

                if (parent.GetProperty<bool>((long)AttributeDefinitions.IsSealed))
                    //The parent type is sealed.
                    throw new SealedBaseVertexTypeException(myTopologicallySortedPointer.Value.VertexTypeName, parent.GetPropertyAsString((long)AttributeDefinitions.Name));

                var attributeNames = parent.GetIncomingVertices(
                    (long)BaseTypes.Attribute,
                    (long)AttributeDefinitions.DefiningType).Select(vertex => vertex.GetPropertyAsString((long)AttributeDefinitions.Name));

                myAttributes[myTopologicallySortedPointer.Value.VertexTypeName] = new HashSet<string>(attributeNames);
            }
            else 
            {
                myAttributes[myTopologicallySortedPointer.Value.VertexTypeName] = new HashSet<string>(myAttributes[parentPredef.Value.VertexTypeName]);
            }

            var attributeNamesSet = myAttributes[myTopologicallySortedPointer.Value.VertexTypeName];

            CheckIncomingEdgesUniqueName(myTopologicallySortedPointer.Value, attributeNamesSet);
            CheckOutgoingEdgesUniqueName(myTopologicallySortedPointer.Value, attributeNamesSet);
            CheckPropertiesUniqueName(myTopologicallySortedPointer.Value, attributeNamesSet);
        }

        /// <summary>
        /// Gets the parent predefinition of the given predefinition.
        /// </summary>
        /// <param name="myCurrent">The predefinition of that the parent vertex predefinition is searched.</param>
        /// <returns>The link to the parent predefinition of the <paramref name="myCurrent"/> predefinition, otherwise <c>NULL</c>.</returns>
        private static LinkedListNode<VertexTypePredefinition> GetParentPredefinitionOnTopologicallySortedList(LinkedListNode<VertexTypePredefinition> myCurrent)
        {
            for (var parent = myCurrent.Previous; parent != null; parent = parent.Previous)
            {
                if (parent.Value.VertexTypeName.Equals(myCurrent.Value.SuperVertexTypeName))
                    return parent;
            }
            return null;
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

        private static void ConvertUnknownAttributes(VertexTypePredefinition myVertexTypeDefinition)
        {
            foreach (var unknown in myVertexTypeDefinition.UnknownAttributes)
            {
                if (BinaryPropertyPredefinition.TypeName.Equals(unknown.AttributeType))
                {
                    if (unknown.DefaultValue != null)
                        throw new Exception("A default value is not allowed on a binary property.");

                    if (unknown.EdgeType != null)
                        throw new Exception("An edge type is not allowed on a binary property.");

                    if (unknown.Multiplicity != null)
                        throw new Exception("A multiplicity is not allowed on a binary property.");

                    var prop = new BinaryPropertyPredefinition(unknown.AttributeName)
                                   .SetComment(unknown.Comment);

                    myVertexTypeDefinition.AddBinaryProperty(prop);
                }
                else if (IsBaseType(unknown.AttributeType))
                {
                    if (unknown.EdgeType != null)
                        throw new Exception("An edge type is not allowed on a property.");

                    var prop = new PropertyPredefinition(unknown.AttributeName)
                                   .SetDefaultValue(unknown.DefaultValue)
                                   .SetAttributeType(unknown.AttributeType)
                                   .SetComment(unknown.Comment);

                    switch (unknown.Multiplicity)
                    {
                        case UnknownAttributePredefinition.ListMultiplicity:
                            prop.SetMultiplicityToList();
                            break;
                        case UnknownAttributePredefinition.SetMultiplicity:
                            prop.SetMultiplicityToSet();
                            break;
                        default:
                            throw new Exception("Unknown multiplicity for properties.");
                    }

                    myVertexTypeDefinition.AddProperty(prop);
                }
                else if (unknown.AttributeType.Contains(IncomingEdgePredefinition.TypeSeparator))
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
                    myVertexTypeDefinition.AddIncomingEdge(prop);


                }
                else
                {

                    if (unknown.DefaultValue != null)
                        throw new Exception("A default value is not allowed on a binary property.");

                    var prop = new OutgoingEdgePredefinition(unknown.AttributeName)
                        .SetAttributeType(unknown.AttributeType)
                        .SetEdgeType(unknown.EdgeType)
                        .SetComment(unknown.Comment);

                    switch (unknown.Multiplicity)
                    {
                        case UnknownAttributePredefinition.SetMultiplicity:
                            prop.SetAsHyperEdge();
                            break;
                        default:
                            throw new Exception("Unknown multiplicity for edges.");
                    }

                    myVertexTypeDefinition.AddOutgoingEdge(prop);
                }
            }
            myVertexTypeDefinition.ResetUnknown();
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
            foreach (var prop in myVertexTypeDefinition.BinaryProperties)
            {
                prop.CheckNull("Binary Property in vertex type predefinition " + myVertexTypeDefinition.VertexTypeName);
                if (myUniqueNameSet.Add(prop.AttributeName))
                    throw new DuplicatedAttributeNameException(myVertexTypeDefinition, prop.AttributeName);
            }
        }

        /// <summary>
        /// Checks the uniqueness of property names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        /// <param name="myUniqueNameSet">A set of attribute names defined on this vertex type predefinition.</param>
        private static void CheckPropertiesUniqueName(VertexTypePredefinition vertexTypeDefinition, ISet<string> myUniqueNameSet)
        {
            foreach (var prop in vertexTypeDefinition.Properties)
            {
                prop.CheckNull("Property in vertex type predefinition " + vertexTypeDefinition.VertexTypeName);
                if (myUniqueNameSet.Add(prop.AttributeName))
                    throw new DuplicatedAttributeNameException(vertexTypeDefinition, prop.AttributeName);

                CheckPropertyType(vertexTypeDefinition, prop);
            }
        }

        /// <summary>
        /// Checks if a given property definition has a valid type.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition that defines the property.</param>
        /// <param name="myProperty">The property to be checked.</param>
        private static void CheckPropertyType(VertexTypePredefinition myVertexTypeDefinition, PropertyPredefinition myProperty)
        {
            if (String.IsNullOrWhiteSpace(myProperty.AttributeType))
            {
                throw new EmptyPropertyTypeException(myVertexTypeDefinition, myProperty.AttributeName);
            }

            if (!IsBaseType(myProperty.AttributeType))
            {
                //it is not one of the base types
                //TODO: check if it is a user defined data type
                throw new UnknownPropertyTypeException(myVertexTypeDefinition, myProperty.AttributeType);
            }
        }

        /// <summary>
        /// Gets whether a property predefinition has a basic c# type as type.
        /// </summary>
        /// <param name="myProperty">The property to be checked.</param>
        /// <returns>True, if the property has a type that is in the list of supported c# types, otherwise false.</returns>
        private static bool IsBaseType(String myType)
        {
            BasicTypes result;
            return Enum.TryParse(myType, false, out result);
        }

        /// <summary>
        /// Checks the uniqueness of property names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        /// <param name="myUniqueNameSet">A set of attribute names defined on this vertex type predefinition.</param>
        private static void CheckOutgoingEdgesUniqueName(VertexTypePredefinition vertexTypeDefinition, ISet<string> myUniqueNameSet)
        {
            foreach (var edge in vertexTypeDefinition.OutgoingEdges)
            {
                edge.CheckNull("Outgoing myEdge in vertex type predefinition " + vertexTypeDefinition.VertexTypeName);
                if (myUniqueNameSet.Add(edge.AttributeName))
                    throw new DuplicatedAttributeNameException(vertexTypeDefinition, edge.AttributeName);

                CheckEdgeType(vertexTypeDefinition, edge);
            }
        }

        /// <summary>
        /// Checks whether the edge type property on an outgoing edge definition contains anything.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition that defines the outgoing edge.</param>
        /// <param name="myEdge">The outgoing edge to be checked.</param>
        private static void CheckEdgeType(VertexTypePredefinition myVertexTypeDefinition, OutgoingEdgePredefinition myEdge)
        {
            if (string.IsNullOrWhiteSpace(myEdge.EdgeType))
            {
                throw new EmptyEdgeTypeException(myVertexTypeDefinition, myEdge.AttributeName);
            }
        }

        /// <summary>
        /// Checks the uniqueness of incoming edge names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        /// <param name="myUniqueNameSet">A set of attribute names defined on this vertex type predefinition.</param>
        private static void CheckIncomingEdgesUniqueName(VertexTypePredefinition vertexTypeDefinition, ISet<String> myUniqueNameSet)
        {
            foreach (var edge in vertexTypeDefinition.IncomingEdges)
            {
                edge.CheckNull("Incoming myEdge in vertex type predefinition " + vertexTypeDefinition.VertexTypeName);
                if (myUniqueNameSet.Add(edge.AttributeName))
                    throw new DuplicatedAttributeNameException(vertexTypeDefinition, edge.AttributeName);
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
                return false;

            return type == BaseTypes.Vertex;
        }

        /// <summary>
        /// Checks a list of VertexTypePredefinitions for duplicate vertex names.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">A list of vertex type predefinitions.</param>
        /// <returns>A dictionary of vertex name to VertexTypePredefinition.</returns>
        private static Dictionary<String, VertexTypePredefinition> CanAddCheckDuplicates(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions)
        {
            var result = (myVertexTypeDefinitions is ICollection)
                ? new Dictionary<String, VertexTypePredefinition>((myVertexTypeDefinitions as ICollection).Count)
                : new Dictionary<String, VertexTypePredefinition>(ExpectedVertexTypes);

            foreach (var predef in myVertexTypeDefinitions)
            {
                if (result.ContainsKey(predef.VertexTypeName))
                    throw new DuplicatedVertexTypeNameException(predef.VertexTypeName);

                result.Add(predef.VertexTypeName, predef);
            }
            return result;
        }

        /// <summary>
        /// Sorts a list of vertex type predefinitions topologically regarding their parentPredef type name.
        /// </summary>
        /// <param name="myDefsByVertexName"></param>
        /// <param name="myDefsByParentVertexName"></param>
        /// <returns> if the vertex type predefinition can be sorted topologically regarding their parentPredef type, otherwise false.</returns>
        private static LinkedList<VertexTypePredefinition> CanAddSortTopolocically(
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
                //All predefinitions, that has the current predefintion as parent vertex type.
                var corrects = myDefsByParentVertexName[current.Value.VertexTypeName];

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

        private IEnumerable<IVertexType> Add(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            //Perf: count is necessary, fast if it is an ICollection
            var count = myVertexTypeDefinitions.Count();

            //This operation reserves #count ids for this operation.
            var lastTypeID = Interlocked.Add(ref _LastTypeID, count);
            var firstTypeID = lastTypeID - count;

            //Contains dictionary of vertex name to vertex predefinition.
            var defsByVertexName = CanAddCheckDuplicates(myVertexTypeDefinitions);

            //Contains dictionary of parent vertex name to list of vertex predefinitions.
            var defsByParentVertexName = myVertexTypeDefinitions
                .GroupBy(def => def.SuperVertexTypeName)
                .ToDictionary(group => group.Key, group => group.AsEnumerable());

            //Contains list of vertex predefinitions sorted topologically.
            var defsTopologically = CanAddSortTopolocically(defsByVertexName, defsByParentVertexName);

            var typeInfos = GenerateTypeInfos(defsTopologically, defsByVertexName, firstTypeID, myTransaction, mySecurity);

            //we can add each type separately
            var creationDate = DateTime.UtcNow.ToBinary();
            var resultPos = 0;

            var result = new IVertexType[count];

            //now we store each vertex type
            for (var current = defsTopologically.First;current != null; current = current.Next)
            {
                result[resultPos] = new VertexType(BaseGraphStorageManager.StoreVertexType(
                    _vertexManager.VertexStore, 
                    typeInfos[current.Value.VertexTypeName].VertexInfo,
                    current.Value.VertexTypeName,
                    current.Value.Comment,
                    creationDate,
                    current.Value.IsAbstract,
                    current.Value.IsSealed,
                    typeInfos[current.Value.SuperVertexTypeName].VertexInfo,
                    null, //TODO uniques
                    mySecurity,
                    myTransaction));

                _vertexIDs.Add(result[resultPos].ID, new UniqueID());

            }

            #region Store Attributes

            //The order of adds is important. First property, then outgoing edges (that might point to properties) and finally incoming edges (that might point to outgoing edges)
            //Do not try to merge it into one for block.

            #region Store properties

            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                var lastAttrID = Interlocked.Add(ref _LastAttrID, current.Value.PropertyCount);
                var firstAttrID = lastAttrID - current.Value.PropertyCount;
                var currentExternID = typeInfos[current.Value.VertexTypeName].AttributeCountWithParents - current.Value.PropertyCount - 1;    

                foreach (var prop in current.Value.Properties)
                {
                    BaseGraphStorageManager.StoreProperty(
                        _vertexManager.VertexStore,
                        new VertexInformation((long)BaseTypes.Property, firstAttrID++ ),
                        currentExternID++,
                        prop.AttributeName,
                        prop.Comment,
                        creationDate,
                        prop.IsMandatory,
                        prop.Multiplicity,
                        prop.DefaultValue,
                        typeInfos[current.Value.VertexTypeName].VertexInfo,
                        ConvertBasicType(prop.AttributeType),
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion

            #region Store binary properties

            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                var lastAttrID = Interlocked.Add(ref _LastAttrID, current.Value.BinaryPropertyCount);
                var firstAttrID = lastAttrID - current.Value.BinaryPropertyCount;
                var currentExternID = typeInfos[current.Value.VertexTypeName].AttributeCountWithParents - current.Value.PropertyCount - current.Value.BinaryPropertyCount - 1;

                foreach (var prop in current.Value.Properties)
                {
                    BaseGraphStorageManager.StoreBinaryProperty(
                        _vertexManager.VertexStore,
                        new VertexInformation((long)BaseTypes.BinaryProperty, firstAttrID++),
                        currentExternID++,
                        prop.AttributeName,
                        prop.Comment,
                        creationDate,
                        typeInfos[current.Value.VertexTypeName].VertexInfo,
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion

            #region Store outgoing edges

            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                var lastAttrID = Interlocked.Add(ref _LastAttrID, current.Value.OutgoingEdgeCount);
                var firstAttrID = lastAttrID - current.Value.OutgoingEdgeCount;
                var currentExternID = typeInfos[current.Value.VertexTypeName].AttributeCountWithParents - current.Value.PropertyCount - current.Value.OutgoingEdgeCount - current.Value.BinaryPropertyCount - 1;

                foreach (var edge in current.Value.OutgoingEdges)
                {

                    BaseGraphStorageManager.StoreOutgoingEdge(
                        _vertexManager.VertexStore,
                        new VertexInformation((long)BaseTypes.OutgoingEdge, firstAttrID++),
                        currentExternID++,
                        edge.AttributeName,
                        edge.Comment,
                        creationDate,
                        edge.Multiplicity,
                        typeInfos[current.Value.VertexTypeName].VertexInfo,
                        new VertexInformation((long)BaseTypes.EdgeType, _edgeManager.GetEdgeType(edge.EdgeType, myTransaction, mySecurity).ID),
                        typeInfos[edge.AttributeType].VertexInfo,
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion

            #region Store incoming edges

            for (var current = defsTopologically.First; current != null; current = current.Next)
            {
                var lastAttrID = Interlocked.Add(ref _LastAttrID, current.Value.IncomingEdgeCount);
                var firstAttrID = lastAttrID - current.Value.IncomingEdgeCount;
                var currentExternID = typeInfos[current.Value.VertexTypeName].AttributeCountWithParents - current.Value.PropertyCount - current.Value.BinaryPropertyCount - current.Value.OutgoingEdgeCount - current.Value.IncomingEdgeCount - 1;

                foreach (var edge in current.Value.IncomingEdges)
                {

                    BaseGraphStorageManager.StoreIncomingEdge(
                        _vertexManager.VertexStore,
                        new VertexInformation((long)BaseTypes.IncomingEdge, firstAttrID++),
                        currentExternID++,
                        edge.AttributeName,
                        edge.Comment,
                        creationDate,
                        typeInfos[current.Value.VertexTypeName].VertexInfo,
                        GetOutgoingEdgeVertexInformation(GetTargetVertexTypeFromAttributeType(edge.AttributeType), GetTargetEdgeNameFromAttributeType(edge.AttributeType), myTransaction, mySecurity),
                        mySecurity,
                        myTransaction);
                }

            }

            #endregion


            #endregion

            #region Add Indices

            var uniqueIdx = _indexManager.GetBestMatchingIndexName(true, false, false);
            var indexIdx =  _indexManager.GetBestMatchingIndexName(false, false, false);

            resultPos = 0;
            for (var current = defsTopologically.First; current != null; current = current.Next, resultPos++)
            {
                #region Uniqueness

                #region own uniques 

                if (current.Value.Uniques != null)
                {
                    var indexPredefs = current.Value.Uniques.Select(unique =>
                        new IndexPredefinition().AddProperty(unique.Properties).SetIndexType(uniqueIdx).SetVertexType(current.Value.VertexTypeName));

                    var indexDefs = indexPredefs.Select(indexPredef=>_indexManager.CreateIndex(indexPredef, mySecurity, myTransaction, false)).ToArray();

                    //only own unique indices are connected to the vertex type on the UniquenessDefinitions attribute
                    ConnectVertexToUniqueIndex(typeInfos[current.Value.VertexTypeName], indexDefs, mySecurity, myTransaction);
                }

                #endregion

                #region parent uniques

                foreach (var unique in result[resultPos].GetParentVertexType.GetUniqueDefinitions(true))
                {
                    _indexManager.CreateIndex(
                        new IndexPredefinition().AddProperty(unique.UniquePropertyDefinitions.Select(x=>x.Name)).SetIndexType(uniqueIdx).SetVertexType(unique.DefiningVertexType.Name), 
                        mySecurity, 
                        myTransaction, 
                        false);
                }

                #endregion

                #endregion

                #region Indices

                foreach (var index in current.Value.Indices)
                {
                    _indexManager.CreateIndex(index, mySecurity, myTransaction);
                }

                foreach (var index in result[resultPos].GetParentVertexType.GetIndexDefinitions(true))
                {
                    _indexManager.CreateIndex(
                        new IndexPredefinition(index.Name).AddProperty(index.IndexedProperties.Select(x=>x.Name)).SetVertexType(current.Value.VertexTypeName).SetIndexType(index.IndexTypeName), 
                        mySecurity, 
                        myTransaction);
                }

                #endregion

            }

            #endregion

            return result;
        }

        private static string GetTargetEdgeNameFromAttributeType(string myAttributeType)
        {
            return myAttributeType.Split(IncomingEdgePredefinition.TypeSeparator)[1];
        }

        private static string GetTargetVertexTypeFromAttributeType(string myAttributeType)
        {
            return myAttributeType.Split(IncomingEdgePredefinition.TypeSeparator)[0];
        }

        private void ConnectVertexToUniqueIndex(TypeInfo myTypeInfo, IIndexDefinition[] myIndexDefinitions, SecurityToken mySecurity, TransactionToken myTransaction)
        {
            _vertexManager.VertexStore.UpdateVertex(
                            mySecurity,
                            myTransaction,
                            myTypeInfo.VertexInfo.VertexID,
                            myTypeInfo.VertexInfo.VertexTypeID,
                            new VertexUpdateDefinition(
                                myHyperEdgeUpdate: new HyperEdgeUpdate(
                                    myUpdated: new Dictionary<long, HyperEdgeUpdateDefinition>
                                {
                                    {
                                        (long)AttributeDefinitions.UniquenessDefinitions, 
                                        new HyperEdgeUpdateDefinition((long)BaseTypes.Edge, myToBeUpdatedSingleEdges: myIndexDefinitions.Select(x=>IndexDefinitionToSingleEdgeUpdate(myTypeInfo.VertexInfo, x)))
                                    }
                                }
                            )));
        }

        private static SingleEdgeUpdateDefinition IndexDefinitionToSingleEdgeUpdate(VertexInformation mySourceVertex, IIndexDefinition myDefinition)
        {
            return new SingleEdgeUpdateDefinition(mySourceVertex, new VertexInformation((long)BaseTypes.Index, myDefinition.ID), (long)BaseTypes.Edge);
        }

        private VertexInformation GetOutgoingEdgeVertexInformation(string myVertexType, string myEdgeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertices = _vertexManager.GetVertices(new BinaryExpression(new SingleLiteralExpression(myEdgeName), BinaryOperator.Equals, _attributeNameExpression), false, myTransaction, mySecurity);
            var vertex = vertices.First(x => IsAttributeOnVertexType(myVertexType, x));
            return new VertexInformation(vertex.VertexTypeID, vertex.VertexID);
        }

        private static bool IsAttributeOnVertexType(String myVertexTypeName, IVertex myAttributeVertex)
        {
            var vertexTypeName = myAttributeVertex.GetOutgoingEdge((long)AttributeDefinitions.DefiningType).GetPropertyAsString((long)AttributeDefinitions.Name);
            return myVertexTypeName.Equals(vertexTypeName);
        }

        private static  VertexInformation ConvertBasicType(string myBasicTypeName)
        {
            BasicTypes resultType;
            if (!Enum.TryParse(myBasicTypeName, out resultType))
                throw new NotImplementedException("User defined base types are not implemented yet.");

            return DBCreationManager.BasicTypesVertices[resultType];
        }

        private struct TypeInfo
        {
            public VertexInformation VertexInfo;
            public long AttributeCountWithParents;
        }

        private Dictionary<String, TypeInfo> GenerateTypeInfos(
            LinkedList<VertexTypePredefinition> myDefsSortedTopologically,
            Dictionary<String, VertexTypePredefinition> myDefsByName, 
            long myFirstID,
            TransactionToken myTransaction, 
            SecurityToken mySecurity)
        {
            //At most all vertex types are needed.
            HashSet<String> neededVertexTypes = new HashSet<string>();

            foreach (var def in myDefsByName)
            {
                neededVertexTypes.Add(def.Value.SuperVertexTypeName);
                foreach (var edge in def.Value.OutgoingEdges)
                {
                    neededVertexTypes.Add(edge.AttributeType);
                }
            }

            var result = new Dictionary<String, TypeInfo>((int)(_LastTypeID - Int64.MinValue));
            foreach (var vertexType in neededVertexTypes)
            {
                if (myDefsByName.ContainsKey(vertexType))
                {
                    result.Add(vertexType, new TypeInfo
                        { 
                            AttributeCountWithParents = myDefsByName[vertexType].AttributeCount, 
                            VertexInfo = new VertexInformation((long)BaseTypes.VertexType, myFirstID++)
                        });
                }
                else
                {
                    var vertex = _vertexManager.GetSingleVertex(new BinaryExpression(new Expression.SingleLiteralExpression(vertexType), BinaryOperator.Equals, _vertexTypeNameExpression), myTransaction, mySecurity);
                    result.Add(vertexType, new TypeInfo
                        {
                            AttributeCountWithParents = vertex.GetIncomingVertices((long)BaseTypes.Attribute, (long)AttributeDefinitions.DefiningType).Max(x => GetID(x)),
                            VertexInfo = new VertexInformation((long)BaseTypes.VertexType, GetID(vertex))
                        });
                }
            }

            //accumulate attribute counts
            for(var current = myDefsSortedTopologically.First; current != null; current = current.Next)
            {
                if (myDefsByName.ContainsKey(current.Value.VertexTypeName))
                {
                    var info = result[current.Value.VertexTypeName];
                    info.AttributeCountWithParents = info.AttributeCountWithParents + result[current.Value.SuperVertexTypeName].AttributeCountWithParents;
                }
            }



            return result;
        }

        private static long GetID(IVertex myAttributeVertex)
        {
            return myAttributeVertex.GetProperty<long>((long)AttributeDefinitions.ID);
        }

        #endregion

        #region Remove

        private bool CanRemove(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        private void Remove(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Update

        private bool CanUpdate(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        private void Update(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region public methods

        private long GetMaxID(long myTypeID, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertices = _vertexManager.GetVertices(myTypeID, myTransaction, mySecurity);
            if (vertices == null)
                //TODO better exception here
                throw new Exception("The base vertex types are not available.");

            return (vertices.CountIsGreater(0))
                ? vertices.Max(x => x.VertexID)
                : Int64.MinValue;
        }

        /// <summary>
        /// Initializes this manager.
        /// </summary>
        /// <param name="myIndexManager">The index manager that should be used within the vertex type manager</param>
        /// <param name="myVertexManager">The vertex manager that should be used within the vertex type manager</param>
        public void Initialize(IIndexManager myIndexManager, IVertexManager myVertexManager, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            _indexManager = myIndexManager;
            _vertexManager = myVertexManager;

            _LastTypeID = GetMaxID((long)BaseTypes.VertexType, myTransaction, mySecurity);
            _LastAttrID = Math.Max(
                            GetMaxID((long)BaseTypes.Property, myTransaction, mySecurity),
                            Math.Max(
                                GetMaxID((long)BaseTypes.OutgoingEdge, myTransaction, mySecurity),
                                Math.Max(
                                    GetMaxID((long)BaseTypes.IncomingEdge, myTransaction, mySecurity),
                                    GetMaxID((long)BaseTypes.BinaryProperty, myTransaction, mySecurity))));


            LoadBaseType(
                myTransaction, 
                mySecurity, 
                BaseTypes.Attribute, 
                BaseTypes.BaseType, 
                BaseTypes.BinaryProperty, 
                BaseTypes.EdgeType, 
                BaseTypes.IncomingEdge, 
                BaseTypes.Index,
                BaseTypes.OutgoingEdge,
                BaseTypes.Property,
                BaseTypes.VertexType);
        }

        private void LoadBaseType(TransactionToken myTransaction, SecurityToken mySecurity, params BaseTypes[] myBaseTypes)
        {
            foreach (var baseType in myBaseTypes)
            {
                var vertex = _vertexManager.VertexStore.GetVertex(mySecurity, myTransaction, (long)baseType, (long)BaseTypes.VertexType, String.Empty);
                if (vertex == null)
                    //TODO: better exception
                    throw new Exception("Could not load base type.");
                _baseTypes.Add(baseType.ToString(), new VertexType(vertex));

            }
        }

        #endregion


        #region IVertexTypeManager Members


        public long GetUniqueVertexID(IVertexType myVertexType)
        {
            myVertexType.CheckNull("myVertexType");
            return _vertexIDs[myVertexType.ID].GetNextID();
        }

        public long GetUniqueVertexID(long myVertexTypeID)
        {
            return _vertexIDs[myVertexTypeID].GetNextID();
        }

        #endregion
    }

}

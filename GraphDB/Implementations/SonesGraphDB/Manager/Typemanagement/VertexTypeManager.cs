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
using sones.GraphDB.ErrorHandling.VertexTypeErrors;
using System.Collections;
using sones.GraphDB.Manager.Vertex;
using sones.GraphDB.Manager.Index;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.VertexStore.Definitions;
using System.Threading;

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
        /// Stores the last vertex type id.
        /// </summary>
        private long lastID;

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

        #endregion

        #region IVertexTypeManager Members

        #region Retrieving

        public IVertexType GetVertexType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
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

            #region get from fs

            var vertex = Get(myTypeName, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeName));

            return new VertexType(vertex);

            #endregion
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

            CanAddCheckWithFS(defsTopologically, defsByVertexName, myTransaction, mySecurity);

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
            var grouped = myVertexTypePredefinition.IncomingEdges.GroupBy(x => x.SourceTypeName);
            foreach (var group in grouped)
            {
                if (!myDefsByName.ContainsKey(group.Key))
                {
                    var vertex = Get(group.Key, myTransaction, mySecurity);
                    if (vertex == null)
                        throw new TargetVertexTypeNotFoundException(myVertexTypePredefinition, group.Key, group.Select(x=>x.EdgeName));

                    var attributes = vertex.GetIncomingVertices((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.DefiningType);
                    foreach (var edge in group)
                    {
                        if (!attributes.Any(outgoing => edge.SourceEdgeName.Equals(outgoing.GetPropertyAsString((long)AttributeDefinitions.Name))))
                            throw new OutgoingEdgeNotFoundException(myVertexTypePredefinition, edge);
                    }
                }
                else
                {
                    var target = myDefsByName[group.Key];

                    foreach (var edge in group)
                    {
                        if (!target.OutgoingEdges.Any(outgoing => edge.SourceEdgeName.Equals(outgoing.EdgeName)))
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
            var grouped = myVertexTypePredefinition.OutgoingEdges.GroupBy(x => x.TargetVertexType);
            foreach (var group in grouped)
            {
                if (!myDefsByName.ContainsKey(group.Key))
                {
                    var vertex = Get(group.Key, myTransaction, mySecurity);
                    if (vertex == null)
                        throw new TargetVertexTypeNotFoundException(myVertexTypePredefinition, group.Key, group.Select(x => x.EdgeName));

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
            var parent = myCurrent.Previous;

            while (parent != null)
            {
                if (parent.Value.VertexTypeName.Equals(myCurrent.Value.SuperVertexTypeName))
                    break;
                parent = parent.Previous;
            }
            return parent;
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

                CheckSealedAndAbstract(vertexTypeDefinition);
                CheckVertexTypeName(vertexTypeDefinition);
                CheckParentTypeAreNoBaseTypes(vertexTypeDefinition);
                CheckAttributes(vertexTypeDefinition);
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
                if (myUniqueNameSet.Add(prop.PropertyName))
                    throw new DuplicatedAttributeNameException(vertexTypeDefinition, prop.PropertyName);

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
            if (String.IsNullOrWhiteSpace(myProperty.TypeName))
            {
                throw new EmptyPropertyTypeException(myVertexTypeDefinition, myProperty.PropertyName);
            }

            if (!IsBaseType(myProperty))
            {
                //it is not one of the base types
                //TODO: check if it is a user defined data type
                throw new UnknownPropertyTypeException(myVertexTypeDefinition, myProperty.TypeName);
            }
        }

        /// <summary>
        /// Gets whether a property predefinition has a basic c# type as type.
        /// </summary>
        /// <param name="myProperty">The property to be checked.</param>
        /// <returns>True, if the property has a type that is in the list of supported c# types, otherwise false.</returns>
        private static bool IsBaseType(PropertyPredefinition myProperty)
        {
            BasicTypes result;
            return Enum.TryParse(myProperty.TypeName, false, out result);
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
                if (myUniqueNameSet.Add(edge.EdgeName))
                    throw new DuplicatedAttributeNameException(vertexTypeDefinition, edge.EdgeName);

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
                throw new EmptyEdgeTypeException(myVertexTypeDefinition, myEdge.EdgeName);
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
                if (myUniqueNameSet.Add(edge.EdgeName))
                    throw new DuplicatedAttributeNameException(vertexTypeDefinition, edge.EdgeName);
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
                throw new EmptyVertexTypeNameException(myVertexTypeDefinition);
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
            Interlocked.Add(ref lastID, count);

            //we can add each type separately
            throw new NotImplementedException();
            
        }

        private IVertexType Add(VertexTypePredefinition vertexTypePredefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
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

        /// <summary>
        /// Sets the index manager
        /// </summary>
        /// <param name="myIndexManager">The index manager that should be used within the vertex type manager</param>
        public void SetIndexManager(IIndexManager myIndexManager)
        {
            _indexManager = myIndexManager;
        }

        /// <summary>
        /// Sets the vertex manager
        /// </summary>
        /// <param name="myVertexManager">The vertex manager that should be used within the vertex type manager</param>
        public void SetVertexManager(IVertexManager myVertexManager)
        {
            _vertexManager = myVertexManager;
        }

        #endregion

    }

}

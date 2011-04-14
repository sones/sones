using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Expression;
using sones.GraphDB.Request;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BaseTypes;
using sones.GraphDB.TypeSystem;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.Library.LanguageExtensions;
using sones.GraphDB.ErrorHandling.VertexTypeErrors;
using System.Collections;
using sones.GraphDB.Manager.Vertex;
using sones.GraphDB.Manager.Index;
using sones.Library.PropertyHyperGraph;
using sones.Library.VertexStore.Definitions;

/*
 * IncomingEdge cases:
 *   - if someone changes the super type of an vertex or IncomingEdge type 
 *     - Henning, Timo 
 *       - that this isn't a required feature for version 2.0
 * 
 *   - undoability of the typemanager 
 *     - Henning, Timo 
 *       - the type manager is only responsible for converting type changing request into filesystem requests
 *       - the ability to undo an request should be implemented in the corresponding piplineable request
 * 
 *   - unique attributes
 *     - Henning, Timo
 *       - the type manager creates unique indices on attributes on the type that declares the uniqness attribute and all deriving types
 * 
 *   - load 
 *     - Timo
 *       - will proove if the main vertex types are available
 *       - will load the main vertex types
 *       - looks for the maximum vertex type id
 * 
 *   - create
 *     - Timo
 *       - will add the main vertex types 
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

        #endregion

        #region C'tor

        /// <summary>
        /// Create a new VertexTypeManager.
        /// </summary>
        /// <param name="myIndexManager">
        /// An instance of IIndexManager.
        /// </param>
        /// <param name="myVertexManager">
        /// An instance of IVertexManager.
        /// </param>
        public VertexTypeManager()
        {
            
            _vertexTypeNameExpression = new PropertyExpression(BaseVertexType.VertexType.ToString(), AttributeDefinitions.Name.Item2);
            _vertexTypeIDExpression = new PropertyExpression(BaseVertexType.VertexType.ToString(), AttributeDefinitions.ID.Item2);
        }

        #endregion
        
        #region Constants

        private const int ExpectedVertexTypes = 100;

        #region base c# types

        private static readonly String[] BaseTypes = 
        {
            // ordered by assumed usage, to speed up contains
            TypeInt32,
            TypeString,
            TypeBoolean,
            TypeInt64,
            TypeChar,
            TypeByte,
            TypeSingle,
            TypeSByte,
            TypeInt16,
            TypeUInt32,
            TypeUInt64,
            TypeUInt16,
        };

        #region value types

        public const string TypeBoolean = "System.Boolean";
        public const string TypeByte    = "System.Byte";
        public const string TypeChar    = "System.Char";
        public const string TypeSingle  = "System.Single";
        public const string TypeInt32   = "System.Int32";
        public const string TypeInt64   = "System.Int64";
        public const string TypeSByte   = "System.SByte";
        public const string TypeInt16   = "System.Int16";
        public const string TypeUInt32  = "System.UInt32";
        public const string TypeUInt64  = "System.UInt64";
        public const string TypeUInt16  = "System.UInt16";

        #endregion

        #region reference types

        public const string TypeString = "System.String";

        #endregion

        #endregion

        private readonly IVertexType _vertexTypeVertexType;
        private readonly IExpression _vertexTypeNameExpression;
        private readonly IExpression _vertexTypeIDExpression;

        #endregion

        #region IVertexTypeManager Members

        #region Retrieving

        public IVertexType GetVertexType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
/*            #region check if it is a base type

            if (Enum.IsDefined(typeof(BaseVertexType), myTypeId))
            {
                return BaseVertexTypeFactory.GetInstance((BaseVertexType) myTypeId);
            }

            #endregion
            */
            #region get from fs

            var vertex = Get(myTypeId, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeId));

            return new VertexType(vertex);
        
            #endregion
        }

        public IVertexType GetVertexType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            /*
            #region check if it is a base type

            BaseVertexType baseType;
            if (Enum.TryParse(myTypeName, out baseType))
            {
                return BaseVertexTypeFactory.GetInstance(baseType);
            }

            #endregion
            */
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

        public void CanAddVertexType(ref IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region check arguments

            myVertexTypeDefinitions.CheckNull("myVertexTypeDefinitions");
            
            #endregion

            CheckAdd(ref myVertexTypeDefinitions, myTransaction, mySecurity);
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

        #region IStorageUsingManager Members

        public void Load(IMetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        public void Create(IMetaManager myMetaManager)
        {
            
            throw new NotImplementedException();
            
        }

        #endregion

        #region private members

        #region Get


        private IVertex Get(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region get the type from fs

            return _vertexManager.GetSingleVertex(new BinaryExpression(_vertexTypeNameExpression, BinaryOperator.Equals, new ConstantExpression(myTypeName)), myTransaction, mySecurity);

            #endregion
        }

        private IVertex Get(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            #region get the type from fs

            return _vertexManager.GetSingleVertex(new BinaryExpression(_vertexTypeIDExpression, BinaryOperator.Equals, new ConstantExpression(myTypeId)), myTransaction, mySecurity);

            #endregion
        }


        #endregion

        #region Add

        private void CheckAdd(
            ref IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, 
            TransactionToken myTransaction, SecurityToken mySecurity)
        {
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
            // TODO - check that unique constraints and indices definition contains existing attributes


            #region Checks without IVertexManager
            
            CanAddCheckBasics(myVertexTypeDefinitions);

            //Contains dictionary of vertex name to vertex predefinition.
            var defsByVertexName = CanAddCheckDuplicates(myVertexTypeDefinitions);
            
            //Contains dictionary of parentPredef vertex name to list of vertex predefinitions.
            var defsByParentVertexName = myVertexTypeDefinitions.GroupBy(def=>def.SuperVertexTypeName).ToDictionary(group => group.Key, group=>group.AsEnumerable());

            //Contains list of vertex predefinitions sorted topologically.
            var defsTopologically = CanAddSortTopolocically(defsByVertexName, defsByParentVertexName);
            
            #endregion

            #region Checks with IVertexManager 
            //Here we know that the VertexTypePredefinitions are syntactical correct.

            CanAddCheckWithFS(defsTopologically, defsByVertexName, myTransaction, mySecurity);

            #endregion

            //Perf: We assign the topologically sorted list to the caller (request manager) to avoid a second sort in Add.
            myVertexTypeDefinitions = defsTopologically;
        }

        /// <summary>
        /// Does the necessary checks for can add with the use of the FS.
        /// </summary>
        /// <param name="myDefsTopologically"></param>
        /// <param name="myDefsByName"></param>
        /// <param name="myTransaction"></param>
        /// <param name="mySecurity"></param>
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
                 CanAddCheckAttributeNameUniquenessWithFS(current, myTransaction, mySecurity, attributes);
                 CanAddCheckOutgoingEdgeTargets(current.Value, myDefsByName, myTransaction, mySecurity);
                 CanAddCheckIncomingEdgeSources(current.Value, myDefsByName, myTransaction, mySecurity);
             }
        }

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

                    var attributes = vertex.GetIncomingVertices((long)BaseVertexType.OutgoingEdge, AttributeDefinitions.DefiningTypeOnAttribute.Item1);
                    foreach (var edge in group)
                    {
                        if (!attributes.Any(outgoing => edge.SourceEdgeName.Equals(outgoing.GetPropertyAsString(AttributeDefinitions.Name.Item1))))
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

        private void CanAddCheckVertexNameUniqueWithFS(VertexTypePredefinition current, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            if (Get(current.VertexTypeName, myTransaction, mySecurity) != null)
                throw new DuplicatedVertexTypeNameException(current.VertexTypeName);
        }

        private void CanAddCheckAttributeNameUniquenessWithFS(LinkedListNode<VertexTypePredefinition> current, TransactionToken myTransaction, SecurityToken mySecurity, IDictionary<string, HashSet<string>> attributes)
        {
            var parentPredef = GetParentPredefinitionOnTopologicallySortedList(current, current.Previous);

            if (parentPredef == null)
            {
                //Get the parent type from FS.
                var parent = Get(current.Value.SuperVertexTypeName, myTransaction, mySecurity);

                if (parent == null)
                    //No parent type was found.
                    throw new InvalidBaseVertexTypeException(current.Value.SuperVertexTypeName);

                if (parent.GetProperty<bool>(AttributeDefinitions.IsSealedOnBaseType.Item1))
                    //The parent type is sealed.
                    throw new SealedBaseVertexTypeException(current.Value.VertexTypeName, parent.GetPropertyAsString(AttributeDefinitions.Name.Item1));

                var attributeNames = parent.GetIncomingVertices(
                    (long)BaseVertexType.Attribute,
                    AttributeDefinitions.DefiningTypeOnAttribute.Item1).Select(vertex => vertex.GetPropertyAsString(AttributeDefinitions.Name.Item1));

                attributes[current.Value.VertexTypeName] = new HashSet<string>(attributeNames);
            }
            else 
            {
                attributes[current.Value.VertexTypeName] = new HashSet<string>(attributes[parentPredef.Value.VertexTypeName]);
            }

            var attributeNamesSet = attributes[current.Value.VertexTypeName];

            CheckIncomingEdgesUniqueName(current.Value, attributeNamesSet);
            CheckOutgoingEdgesUniqueName(current.Value, attributeNamesSet);
            CheckPropertiesUniqueName(current.Value, attributeNamesSet);
        }

        private static LinkedListNode<VertexTypePredefinition> GetParentPredefinitionOnTopologicallySortedList(LinkedListNode<VertexTypePredefinition> current, LinkedListNode<VertexTypePredefinition> parent)
        {
            while (parent != null)
            {
                if (parent.Value.VertexTypeName.Equals(current.Value.SuperVertexTypeName))
                    break;
                parent = parent.Previous;
            }
            return parent;
        }

        /// <summary>
        /// Checks for errors in a list of VertexTypePredefinitions without using the FS
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The list of VertexTypePredefinitions to be checked.</param>
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

        private static void CheckAttributes(VertexTypePredefinition vertexTypeDefinition)
        {
            var uniqueNameSet = new HashSet<string>();

            CheckIncomingEdgesUniqueName(vertexTypeDefinition, uniqueNameSet);
            CheckOutgoingEdgesUniqueName(vertexTypeDefinition, uniqueNameSet);
            CheckPropertiesUniqueName(vertexTypeDefinition, uniqueNameSet);
        }

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

        private static void CheckPropertyType(VertexTypePredefinition vertexTypeDefinition, PropertyPredefinition prop)
        {
            if (String.IsNullOrWhiteSpace(prop.TypeName))
            {
                throw new EmptyPropertyTypeException(vertexTypeDefinition, prop.PropertyName);
            }

            if (!IsBaseType(prop))
            {
                //it is not one of the base types
                //TODO: check if it is a user defined data type
                throw new UnknownPropertyTypeException(vertexTypeDefinition, prop.TypeName);
            }
        }

        private static bool IsBaseType(PropertyPredefinition prop)
        {
            return BaseTypes.Contains(prop.TypeName);
        }

        private static void CheckOutgoingEdgesUniqueName(VertexTypePredefinition vertexTypeDefinition, ISet<string> myUniqueNameSet)
        {
            foreach (var edge in vertexTypeDefinition.OutgoingEdges)
            {
                edge.CheckNull("Outgoing edge in vertex type predefinition " + vertexTypeDefinition.VertexTypeName);
                if (myUniqueNameSet.Add(edge.EdgeName))
                    throw new DuplicatedAttributeNameException(vertexTypeDefinition, edge.EdgeName);

                CheckEdgeType(vertexTypeDefinition, edge);
            }
        }

        private static void CheckEdgeType(VertexTypePredefinition vertexTypeDefinition, OutgoingEdgePredefinition edge)
        {
            if (string.IsNullOrWhiteSpace(edge.EdgeType))
            {
                throw new EmptyEdgeTypeException(vertexTypeDefinition, edge.EdgeName);
            }
        }


        private static void CheckIncomingEdgesUniqueName(VertexTypePredefinition vertexTypeDefinition, ISet<String> myUniqueNameSet)
        {
            foreach (var edge in vertexTypeDefinition.IncomingEdges)
            {
                edge.CheckNull("Incoming edge in vertex type predefinition " + vertexTypeDefinition.VertexTypeName);
                if (myUniqueNameSet.Add(edge.EdgeName))
                    throw new DuplicatedAttributeNameException(vertexTypeDefinition, edge.EdgeName);
            }
        }

        private static void CheckVertexTypeName(VertexTypePredefinition myVertexTypeDefinition)
        {
            if (string.IsNullOrWhiteSpace(myVertexTypeDefinition.VertexTypeName))
            {
                throw new EmptyVertexTypeNameException(myVertexTypeDefinition);
            }
        }

        private static void CheckSealedAndAbstract(VertexTypePredefinition predef)
        {
            if (predef.IsSealed && predef.IsAbstract)
            {
                throw new UselessVertexTypeException(predef);
            }
        }


        private static void CheckParentTypeAreNoBaseTypes(VertexTypePredefinition myVertexTypeDefinition)
        {
            if (IsBaseVertexType(myVertexTypeDefinition.SuperVertexTypeName))
            {
                throw new InvalidBaseVertexTypeException(myVertexTypeDefinition.VertexTypeName);
            }
        }

        private static bool IsBaseVertexType(string myTypeName)
        {
            BaseVertexType type;
            if (!Enum.TryParse(myTypeName, out type))
                return true;

            return type != BaseVertexType.Vertex;
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
            //In this step, we assume that parentPredef types, that are not in the list of predefinitons are correct.
            //Correct means: either they are in fs or they are not in fs but then they are not defined. (this will be detected later)
            var correctRoots = myDefsByParentVertexName.Where(parent => !myDefsByVertexName.ContainsKey(parent.Key)).SelectMany(x => x.Value);
            var result = new LinkedList<VertexTypePredefinition>(correctRoots);
            

            //Here we step throught the list of topolocically sorted predefinitions.
            //Each predefinition that is in this list, is a valid parentPredef type for other predefinitions.
            //Thus we can add all predefinitions, that has parentPredef predefinition in the list to the end of the list.
            var current = result.First;
            while (current != null) 
            {
                //All predefinitions, that has the current predefintion as parentPredef vertex type.
                var corrects = myDefsByParentVertexName[current.Value.VertexTypeName];

                //They go from toBeChecked into vertex.
                foreach (var correct in corrects)
                {
                    result.AddLast(correct);
                }

                current = current.Next;
            }


            if (myDefsByVertexName.Count > result.Count)
                //There are some defintions that are not in the vertex...so they must contain a circle.
                throw new CircularTypeHierarchyException(myDefsByVertexName.Values.Except(result));

            return result;
        }

        private IEnumerable<IVertexType> Add(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            // there are two ways to add the vertex types
            // 1. We add a vertex per definition without setting the parentPredef type. After that we update these vertices to set the IncomingEdge to the parentPredef type.
            // 2. We built up a derivation forest (list of trees) and insert the types in order with setting the base type
            // we assume, that 1. must visit the vertices more often in FS and 2. also generates a spanning tree, so we know that we do not have an inheritance problem

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

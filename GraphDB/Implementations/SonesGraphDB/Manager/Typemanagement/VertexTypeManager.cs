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
using sones.Library.PropertyHyperGraph;

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
        #region nested class

        #region VertexTypePredefinitionComparer

        private class VertexTypePredefinitionComparer: IComparer<VertexTypePredefinition>
        {
            #region IComparer<VertexTypePredefinition> Members

            int IComparer<VertexTypePredefinition>.Compare(VertexTypePredefinition x, VertexTypePredefinition y)
            {
                return String.Compare(x.VertexTypeName, y.VertexTypeName);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Constants

        private const UInt64 VertexTypeID = UInt64.MinValue;
        private const UInt64 EdgeTypeID = UInt64.MinValue + 1;
        private const int ExpectedVertexTypes = 100;

        #region base c# types

        private static readonly String[] baseTypes = 
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

        private static readonly IExpression VertexTypeNameExpression = new PropertyExpression("VertexType", "Name");

        private static readonly IComparer<VertexTypePredefinition> VertexTypePredefinitionComparerInstance = new VertexTypePredefinitionComparer();

        #endregion

        #region IVertexTypeManager Members

        #region Retrieving

        public IVertexType GetVertexType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            #region check if it is a base type

            BaseVertexType baseType;
            if (Enum.TryParse(myTypeName, out baseType))
            {
                return BaseVertexTypeFactory.GetInstance(baseType);
            }

            #endregion

            var vertex = Get(myTypeName, myTransaction, mySecurity, myMetaManager);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeName));

            return new VertexType(vertex);

        }

        #endregion

        #region Updates

        #region Add

        public void CanAddVertexType(ref IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            #region check arguments

            myMetaManager.CheckNull("MyMetaManager");
            myVertexTypeDefinitions.CheckNull("myVertexTypeDefinitions");
            
            #endregion

            CanAdd(ref myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        public IEnumerable<IVertexType> AddVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return Add(myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        #endregion

        #region Remove

        public void CanRemoveVertexType(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            CanRemove(myVertexTypes, myTransaction, mySecurity, myMetaManager);
        }

        public void RemoveVertexType(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            Remove(myVertexTypes, myTransaction, mySecurity, myMetaManager);
        }

        #endregion

        #region Update

        public void CanUpdateVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            CanUpdate(myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        public void UpdateVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            Update(myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        #endregion

        #endregion
        
        #endregion

        #region IStorageUsingManager Members

        public void Load(MetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        public void Create(MetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private members

        #region Get


        private static IVertex Get(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            #region get the type from fs

            return myMetaManager.VertexManager.GetSingleVertex(new BinaryExpression(VertexTypeNameExpression, BinaryOperator.Equals, new ConstantExpression(myTypeName)), myTransaction, mySecurity);

            #endregion
        }

        #endregion

        #region Add

        private void CanAdd(ref IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
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

            CanAddCheckWithFS(defsTopologically, defsByVertexName, myTransaction, mySecurity, myMetaManager);

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
        /// <param name="myMetaManager"></param>
        /// The predefinitions are checked one by one in topologically order. 
        private static void CanAddCheckWithFS(
            LinkedList<VertexTypePredefinition> myDefsTopologically, 
            Dictionary<String, VertexTypePredefinition> myDefsByName,
            TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {

            //Contains the vertex type name to the attribute names of the vertex type.
            var attributes = new Dictionary<String, HashSet<String>>(myDefsTopologically.Count);

            for (var current = myDefsTopologically.First; current != null; current = current.Next)
            {
                 CanAddCheckVertexNameWithFS(current.Value, myTransaction, mySecurity, myMetaManager);
                 CanAddCheckAttributeNameUniquenessWithFS(current, myTransaction, mySecurity, myMetaManager, attributes);
                 CanAddCheckOutgoingEdgeTargets(current.Value, myDefsByName, myTransaction, mySecurity, myMetaManager);
                 CanAddCheckIncomingEdgeSources(current.Value, myDefsByName, myTransaction, mySecurity, myMetaManager);
                 current = current.Next;
             }
        }

        private static void CanAddCheckIncomingEdgeSources(VertexTypePredefinition myVertexTypePredefinition, Dictionary<string, VertexTypePredefinition> myDefsByName, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            var grouped = myVertexTypePredefinition.IncomingEdges.GroupBy(x => x.SourceTypeName);
            foreach (var group in grouped)
            {
                if (!myDefsByName.ContainsKey(group.Key))
                {
                    var vertex = Get(group.Key, myTransaction, mySecurity, myMetaManager);
                    if (vertex == null)
                        throw new TargetVertexTypeNotFoundException(myVertexTypePredefinition, group.Key, group.Select(x=>x.EdgeName));

                    var attributes = vertex.GetIncomingVertices((long)BaseVertexType.OutgoingEdge, AttributeDefinitions.DefiningTypeOnAttribute.AttributeID);
                    foreach (var edge in group)
                    {
                        if (!attributes.Any(outgoing => edge.SourceEdgeName.Equals(outgoing.GetPropertyAsString(AttributeDefinitions.Name.AttributeID))))
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

        private static void CanAddCheckOutgoingEdgeTargets(VertexTypePredefinition myVertexTypePredefinition, Dictionary<string, VertexTypePredefinition> myDefsByName, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            var grouped = myVertexTypePredefinition.OutgoingEdges.GroupBy(x => x.TargetVertexType);
            foreach (var group in grouped)
            {
                if (!myDefsByName.ContainsKey(group.Key))
                {
                    var vertex = Get(group.Key, myTransaction, mySecurity, myMetaManager);
                    if (vertex == null)
                        throw new TargetVertexTypeNotFoundException(myVertexTypePredefinition, group.Key, group.Select(x => x.EdgeName));

                }
            }
        }

        private static void CanAddCheckVertexNameWithFS(VertexTypePredefinition current, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            if (Get(current.VertexTypeName, myTransaction, mySecurity, myMetaManager) != null)
                throw new DuplicatedVertexTypeNameException(current.VertexTypeName);
        }

        private static void CanAddCheckAttributeNameUniquenessWithFS(LinkedListNode<VertexTypePredefinition> current, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager, Dictionary<string, HashSet<string>> attributes)
        {
            var parentPredef = GetParentPredefinitionOnTopologicallySortedList(current, current.Previous);

            if (parentPredef == null)
            {
                //Get the parent type from FS.
                var parent = Get(current.Value.SuperVertexTypeName, myTransaction, mySecurity, myMetaManager);

                if (parent == null)
                    //No parent type was found.
                    throw new InvalidBaseVertexTypeException(current.Value.SuperVertexTypeName);

                if (parent.GetProperty<bool>(AttributeDefinitions.IsSealedOnBaseType.AttributeID))
                    //The parent type is sealed.
                    throw new SealedBaseVertexTypeException(current.Value.VertexTypeName, parent.GetPropertyAsString(AttributeDefinitions.Name.AttributeID));

                var attributeNames = parent.GetIncomingVertices(
                    (long)BaseVertexType.Attribute,
                    AttributeDefinitions.DefiningTypeOnAttribute.AttributeID).Select(vertex => vertex.GetPropertyAsString(AttributeDefinitions.Name.AttributeID));

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
            HashSet<String> uniqueNameSet = new HashSet<string>();

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
            return baseTypes.Contains(prop.TypeName);
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
            Dictionary<String, VertexTypePredefinition> result = (myVertexTypeDefinitions is ICollection)
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
        /// <param name="myVertexTypeDefinitions">A set of vertex type predefinitions sorted by their names.</param>
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

        private IEnumerable<IVertexType> Add(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            // there are two ways to add the vertex types
            // 1. We add a vertex per definition without setting the parentPredef type. After that we update these vertices to set the IncomingEdge to the parentPredef type.
            // 2. We built up a derivation forest (list of trees) and insert the types in order with setting the base type
            // we assume, that 1. must visit the vertices more often in FS and 2. also generates a spanning tree, so we know that we do not have an inheritance problem

            throw new NotImplementedException();
        }

        #endregion

        #region Remove

        private bool CanRemove(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        private void Remove(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Update

        private bool CanUpdate(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        private void Update(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

    }

}

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
using sones.GraphDB.Request.CreateVertexTypes;
using System.Collections;

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
 *     - if one of the base vertex types is requested, return a predefined result.
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
            return Get(myTypeName, myTransaction, mySecurity, myMetaManager);
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


        private static IVertexType Get(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            #region check if it is a base type

            BaseVertexType baseType;
            if (Enum.TryParse(myTypeName, out baseType))
            {
                return BaseVertexTypeFactory.GetInstance(baseType);
            }

            #endregion

            #region get the type from fs

            var vertex = myMetaManager.VertexManager.GetSingleVertex(new BinaryExpression(VertexTypeNameExpression, BinaryOperator.Equals, new ConstantExpression(myTypeName)), myTransaction, mySecurity);
            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeName));

            return new VertexType(vertex);

            #endregion
        }

        #endregion

        #region Add

        private void CanAdd(ref IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            // Basically first check the pre-definitions itself without asking the IVertexManager. 
            // If these checks are okay, proof everything concerning the types stored in the fs using the IVertexManager.

            // These are the necessary checks:
            // - vertex type names are unique
            // - attribute names are unique for each type pre-definition
            // - parent types are none of the base vertex types
            // - check that no vertex type has the flags sealed and abstract at the same time
            // TODO: - check that unique constraints and indices definition contains existing attributes
            // ---- now with IVertexManager ---- (This means we can assume, that the vertex types are created, so we have a list of all vertex types containing the 'to-be-added-types'.)
            // - check if the type names are unique
            // - check if the derviation is circle free
            // - check if the attribute names are unique regarding the derivation
            // - check if all parent types exists and are not sealed
            // - check if all outgoing edges have existing targets
            // - check if all incoming edges have existing outgoing edges


            #region Checks without IVertexManager
            
            CheckBasics(myVertexTypeDefinitions);

            //Checks if vertex type names are duplicated.
            Dictionary<String, VertexTypePredefinition> uniqueDefs = CheckDuplicates(myVertexTypeDefinitions);

            //Perf: We assign the topologically sorted list to the caller (request manager) to avoid a second sort in Add.
            myVertexTypeDefinitions = SortTopolocically(uniqueDefs);
            
            #endregion

            #region Checks with IVertexManager 
            //Here we know the the VertexTypePredefinitions are syntactical correct.



            
            #endregion


            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks for errors in a list of VertexTypePredefinitions without using the FS
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The list of VertexTypePredefinitions to be checked.</param>
        private static void CheckBasics(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions)
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
            SortedSet<String> uniqueNameSet = new SortedSet<string>();
            CheckIncomingEdges(vertexTypeDefinition, uniqueNameSet);
            CheckOutgoingEdges(vertexTypeDefinition, uniqueNameSet);
            CheckProperties(vertexTypeDefinition, uniqueNameSet);
        }

        private static void CheckProperties(VertexTypePredefinition vertexTypeDefinition, SortedSet<string> myUniqueNameSet)
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

        private static void CheckOutgoingEdges(VertexTypePredefinition vertexTypeDefinition, SortedSet<string> myUniqueNameSet)
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


        private static void CheckIncomingEdges(VertexTypePredefinition vertexTypeDefinition, SortedSet<String> myUniqueNameSet)
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
        private static Dictionary<String, VertexTypePredefinition> CheckDuplicates(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions)
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
        /// Sorts a list of vertex type predefinitions topologically regarding their parent type name.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">A set of vertex type predefinitions sorted by their names.</param>
        /// <returns> if the vertex type predefinition can be sorted topologically regarding their parent type, otherwise false.</returns>
        private static IEnumerable<VertexTypePredefinition> SortTopolocically(Dictionary<String, VertexTypePredefinition> myVertexTypeDefinitions)
        {
            //Vertex type predefinitions goes from toBeChecked into result.
            var toBeChecked = myVertexTypeDefinitions;

            //Group predefinitions by their parent type names and convert it into a dictionary.
            var grouped = myVertexTypeDefinitions.Values.GroupBy(predef => predef.SuperVertexTypeName).ToDictionary(group => group.Key, x => x.AsEnumerable());

            //The list of topolocically sorted vertex types
            //In this step, we assume that parent types, that are not in the list of predefinitons are correct.
            //Correct means: either they are in fs or they are not in fs but then they are not defined. (this will be detected later)
            var correctRoots = grouped.Where(parent => !toBeChecked.ContainsKey(parent.Key)).SelectMany(x => x.Value);
            var result = new LinkedList<VertexTypePredefinition>(correctRoots);
            

            //Here we step throught the list of topolocically sorted predefinitions.
            //Each predefinition that is in this list, is a valid parent type for other predefinitions.
            //Thus we can add all predefinitions, that has parent predefinition in the list to the end of the list.
            var current = result.First;
            while (current != null) 
            {
                //All predefinitions, that has the current predefintion as parent vertex type.
                var corrects = grouped[current.Value.VertexTypeName];

                //They go from toBeChecked into result.
                foreach (var correct in corrects)
                {
                    result.AddLast(correct);
                }

                current = current.Next;
            }

            
            if (toBeChecked.Count > result.Count)
                //There are some defintions that are not in the result...sothey must contain a circle.
                throw new CircularTypeHierarchyException(toBeChecked.Values.Except(result));

            return result;
        }

        private IEnumerable<IVertexType> Add(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            // there are two ways to add the vertex types
            // 1. We add a vertex per definition without setting the parent type. After that we update these vertices to set the edge to the parent type.
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

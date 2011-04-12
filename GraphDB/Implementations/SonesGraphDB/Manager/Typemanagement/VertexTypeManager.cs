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

        public const UInt64 VertexTypeID = UInt64.MinValue;
        public const UInt64 EdgeTypeID = UInt64.MinValue + 1;

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

            var vertex = myMetaManager.VertexManager.GetSingleVertex(new BinaryExpression(VertexTypeNameExpression, BinaryOperator.Equals, new ConstantExpression(myTypeName)), myTransaction, mySecurity, myMetaManager);
            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeName));

            return new VertexType(vertex);

            #endregion
        }

        #endregion

        #region Add

        private void CanAdd(ref IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            // basically first check the pre-definitions itself without asking the IVertexManager. If these checks are okay, proof everything concerning the types stored in the fs using the IVertexManager
            // These are the necessary checks:
            // - vertex type names are unique
            // - attribute names are unique for each type pre-definition
            // - parent types are none of the base types
            // - check that no vertex type has the flags sealed and abstract at the same time
            // - check that unique constraints and indices definition contains existing attributes
            // ---- now with IVertexManager ---- (This means we can assume, that the vertex types are created, so we have a list of all vertex types containing the 'to-be-added-types'.)
            // - check if the type names are unique
            // - check if the derviation is circle free
            // - check if the attribute names are unique regarding the derivation
            // - check if all parent types exists and are not sealed
            // - check if all outgoing edges have existing targets
            // - check if all incoming edges have existing outgoing edges

            CheckBasics(myVertexTypeDefinitions);

            //checks if vertex type names are duplicated.
            SortedSet<VertexTypePredefinition> sortedDefs = SortByName(myVertexTypeDefinitions);


            myVertexTypeDefinitions = SortTopolocically(sortedDefs);


            throw new NotImplementedException();
        }

        private static void CheckBasics(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions)
        {
            foreach (var predef in myVertexTypeDefinitions)
            {
                predef.CheckNull("Element in myVertexTypeDefinitions");
                CheckParentTypeAreNoBaseTypes(predef);
                CheckSealedAndAbstract(predef);
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
            if (IsBaseType(myVertexTypeDefinition.SuperVertexTypeName))
            {
                throw new InvalidBaseVertexTypeException(myVertexTypeDefinition.VertexTypeName);
            }
        }

        private static bool IsBaseType(string myTypeName)
        {
            BaseVertexType type;
            if (!Enum.TryParse(myTypeName, out type))
                return true;

            return type != BaseVertexType.Vertex;
        }

        /// <summary>
        /// Sorts a list of vertex type predefinitions according to the vertex type name.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">A list of vertex type predefinitions.</param>
        /// <returns>A sorted set</returns>
        private static SortedSet<VertexTypePredefinition> SortByName(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions)
        {
            SortedSet<VertexTypePredefinition> sortedDefs = new SortedSet<VertexTypePredefinition>(VertexTypePredefinitionComparerInstance);
            foreach (var predef in myVertexTypeDefinitions)
            {
                if (!sortedDefs.Add(predef))
                {
                    throw new DuplicatedVertexTypeNameException(predef.VertexTypeName);
                }
            }
            return sortedDefs;
        }

        /// <summary>
        /// Sorts a list of vertex type predefinitions topologically regarding their parent type name.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">A set of vertex type predefinitions sorted by their names.</param>
        /// <returns> if the vertex type predefinition can be sorted topologically regarding their parent type, otherwise false.</returns>
        private static IEnumerable<VertexTypePredefinition> SortTopolocically(SortedSet<VertexTypePredefinition> myVertexTypeDefinitions)
        {
            //vertex type predefinitions goes from toBeChecked into result.
            var toBeChecked = myVertexTypeDefinitions;
            var result = new List<VertexTypePredefinition>(myVertexTypeDefinitions.Count());
            
            //group predefinitions by their name and convert it into a Dictionary<String, List<VertexTypePredefinition>>
            var grouped = myVertexTypeDefinitions.GroupBy(predef => predef.SuperVertexTypeName).ToDictionary(group => group.Key);
            
            String nextRoot = String.Empty;

            while (toBeChecked.Count > 0 && nextRoot != null) 
            {
                //here we assume, parent types that are not on the list of definitions are correct parents
                //so nextroot contains the name of a correct parent type
                nextRoot = grouped.Keys.FirstOrDefault(parentType => !toBeChecked.Any(def => parentType.Equals(def.VertexTypeName)));
                
                if (nextRoot == null)
                {
                    //we did not find a correct parent, so the types in toBeChecked contains a circle
                    throw new CircularTypeHierarchyException(toBeChecked);
                }

                toBeChecked.ExceptWith(grouped[nextRoot]);

            }

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

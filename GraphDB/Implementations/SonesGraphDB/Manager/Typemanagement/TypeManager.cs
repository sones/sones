using System.Collections.Generic;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.Transaction;
using sones.Library.LanguageExtensions;
using sones.Library.VertexStore;
using System.Threading;
using System;
using sones.GraphDB.Manager.TypeManagement.Exceptions;

/*
 * edge cases:
 *   - if someone changes the super type of an vertex or edge type 
 *     - Henning, Timo 
 *       * that this isn't a required feature for version 2.0
 *     
 *   - undoability of the typemanager 
 *     - Henning, Timo 
 *       * the type manager is only responsible for converting type changing request into filesystem requests
 *       * the ability to undo an request should be implemented in the corresponding piplineable request
 *   
 *   - load 
 *     - Timo
 *       * will proove if the five main vertex types are available
 *       * will load the five main vetex types
 *       * looks for the maximum vertex type id
 *       
 *   - create
 *     - Timo
 *       * will add the five main vertex types 
 *   
 *   
 */

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed partial class TypeManager
    {
        #region Data

        private long _TypeID = Int64.MinValue;

        #endregion


        #region c'tor

        //TODO: here we get a slim version of IGraphDB. So we can ask for types by name and are sure, that the index is used.
        public TypeManager()
        {
            
        }

        #endregion

        #region public methods regarding type manager itself

        /// <summary>
        /// Loads data from the underlying vertex store
        /// </summary>
        public void Load()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates the basic vertex type definitions.
        /// </summary>
        //TODO: here we get a VertexStore(no security, no transaction) and an IndexManager, so we can create the five base vertex types, that are used to store the type manager knowlegde.
        public void Create()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region public methods regarding edge type

        #region update edge types

        /// <summary>
        /// Adds a given edge type.
        /// </summary>
        /// <param name="myEdgeTypeDefinition">Defines the edge type to be added.</param>
        /// <returns>An instance of IEdgeType, that represents the just now added edge type.</returns>
        public IEdgeType Add(EdgeTypeDefinition myEdgeTypeDefinition)
        {
            //we delegate the work to the edge manager 
            return _EdgeManager.Add(myEdgeTypeDefinition);
        }

        #endregion

        #endregion

        #region public methods regarding vertex type

        #region get vertex types

        /// <summary>
        /// Gets a vertex type by name.
        /// </summary>
        /// <param name="myTypeName">
        /// The name of the vertex type.
        /// </param>
        /// <returns>An instance of IVertexType, that represents the vertex type.</returns>
        public IVertexType Get(string myTypeName)
        {
            return _VertexManager.Get(myTypeName);
        }

        #endregion

        #region update vertex types


        /// <summary>
        /// Validates a command.
        /// </summary>
        /// <param name="myCommand">The command to be validated by the type manager</param>
        /// <returns>True, if the command is executable, otherwise false.</returns>
        /// This method must be called on every command, before it can be executed.
        /// Here we could also proceed work regarding the transaction manager, for example to lock resources within a two phases locking protocol.
        public bool Validate(ATypeManagerCommand myCommand)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Executes a type manager changing command.
        /// </summary>
        /// <param name="myCommand">A type manager changing command. Must be validated before.</param>
        public void Execute(ATypeManagerCommand myCommand)
        {
            if (!myCommand.IsValidated)
                throw new NotValidatedException();

            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Undos a type manager changing command.
        /// </summary>
        /// <param name="myCommand">A type manager changing command. Must be executed before.</param>
        public void Undo(ATypeManagerCommand myCommand)
        {
            if (!myCommand.IsExecuted)
                throw new NotValidatedException();

            throw new System.NotImplementedException();
        }

        /* will be removed, if the DO and UNDO pattern is feasible
         * 
        /// <summary>
        /// Adds a new vertex type to the type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        public void Add(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction)
        {
            _VertexManager.Add(myVertexTypeDefinition, myTransaction);
        }

        /// <summary>
        /// Adds a bunch of new vertex types to the type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definition of the new vertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        public void Add(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction)
        {
            _VertexManager.Add(myVertexTypeDefinitions, myTransaction);
        }

        /// <summary>
        /// Removes a vertex type from the type manager.
        /// </summary>
        /// <param name="myVertexType">The vertex type that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// The vertex type will be removed unless there are no edges that point to this type.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove both type simultaneously using <see cref="Add(IEnumerable<IVertexType>, TransactionToken)"/>.
        public void Remove(IVertexType myVertexType, TransactionToken myTransaction)
        {
            _VertexManager.Remove(myVertexType, myTransaction);
        }

        /// <summary>
        /// Removes a bunch of vertex types from the type manager.
        /// </summary>
        /// <param name="myVertexTypes">The vertex types that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// All types will be removed unless there are no edges that point to at least one of the given types.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove this type too.
        /// All types are removed simultaneously. This means that edges between the types are not need to be removed before.
        public void Remove(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction)
        {
            _VertexManager.Remove(myVertexTypes, myTransaction);
        }

        /// <summary>
        /// Updates an existing vertex type.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The definition of the vertex. The VertexTypeDefinition.VertexTypeName identifies the vertex type to change.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// 
        public void Update(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction)
        {
            _VertexManager.Update(myVertexTypeDefinition, myTransaction);
        }

        public void Update(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction)
        {
            _VertexManager.Update(myVertexTypeDefinitions, myTransaction);
        }

        */
        #endregion

        #endregion
    }
}

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
using sones.GraphDB.Expression;
using sones.GraphDB.Manager.Vertex;
using sones.GraphDB.Request;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Manager.BaseGraph;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class ExecuteEdgeTypeManager: IEdgeTypeHandler
    {
        private readonly IDictionary<string, IEdgeType> _baseTypes = new Dictionary<String, IEdgeType>();
        private IDManager _idManager;
        private IManagerOf<IVertexHandler> _vertexManager;
        private BaseGraphStorageManager _baseStorageManager;
        
        /// <summary>
        /// A property expression on EdgeType.Name
        /// </summary>
        private readonly IExpression _vertexTypeNameExpression = new PropertyExpression(BaseTypes.EdgeType.ToString(), "Name");

        /// <summary>
        /// A property expression on VertexID
        /// </summary>
        private readonly IExpression _vertexTypeIDExpression = new PropertyExpression(BaseTypes.EdgeType.ToString(), "VertexID");


        public ExecuteEdgeTypeManager(IDManager myIDManager)
        {
            _idManager = myIDManager;
        }

        #region IEdgeTypeManager Members

        IEdgeType IEdgeTypeHandler.GetEdgeType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
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
                throw new KeyNotFoundException(string.Format("A edge type with name {0} was not found.", myTypeId));

            return new EdgeType(vertex, _baseStorageManager);

            #endregion

        }

        IEdgeType IEdgeTypeHandler.GetEdgeType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            if (String.IsNullOrWhiteSpace(myTypeName))
                throw new ErrorHandling.EmptyEdgeTypeNameException();

            #region get static types

            if (_baseTypes.ContainsKey(myTypeName))
            {
                return _baseTypes[myTypeName];
            }

            #endregion

            #region get from fs

            var vertex = Get(myTypeName, myTransaction, mySecurity);

            if (vertex == null)
                throw new KeyNotFoundException(string.Format("A edge type with name {0} was not found.", myTypeName));

            return new EdgeType(vertex, _baseStorageManager);

            #endregion

        }

        IEnumerable<IEdgeType> IEdgeTypeHandler.GetAllEdgeTypes(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertices = _vertexManager.ExecuteManager.GetVertices(BaseTypes.EdgeType.ToString(), myTransaction, mySecurity, false);

            return vertices == null ? Enumerable.Empty<IEdgeType>() : vertices.Select(x => new EdgeType(x, _baseStorageManager));
        }

        IEdgeType IEdgeTypeHandler.AddEdgeType(IEnumerable<EdgeTypePredefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        void IEdgeTypeHandler.RemoveEdgeTypes(IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        void IEdgeTypeHandler.UpdateEdgeType(IEnumerable<EdgeTypePredefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Initialize(IMetaManager myMetaManager)
        {
            _vertexManager = myMetaManager.VertexManager;
            _baseStorageManager = myMetaManager.BaseGraphStorageManager;
        }

        public void Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            LoadBaseType(
                myTransaction,
                mySecurity,
                BaseTypes.Edge,
                BaseTypes.Weighted,
                BaseTypes.Orderable);
        }

        #region GetEdgeType

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

            return _vertexManager.ExecuteManager.GetSingleVertex(new BinaryExpression(_vertexTypeNameExpression, BinaryOperator.Equals, new SingleLiteralExpression(myTypeName)), myTransaction, mySecurity);

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

            return _vertexManager.ExecuteManager.GetSingleVertex(new BinaryExpression(_vertexTypeIDExpression, BinaryOperator.Equals, new SingleLiteralExpression(myTypeId)), myTransaction, mySecurity);

            #endregion
        }

        #endregion

        #region Load

        private void LoadBaseType(TransactionToken myTransaction, SecurityToken mySecurity, params BaseTypes[] myBaseTypes)
        {
            foreach (var baseType in myBaseTypes)
            {
                var vertex = _vertexManager.ExecuteManager.VertexStore.GetVertex(mySecurity, myTransaction, (long)baseType, (long)BaseTypes.EdgeType, String.Empty);
                if (vertex == null)
                    //TODO: better exception
                    throw new Exception("Could not load base edge type.");
                _baseTypes.Add(baseType.ToString(), new EdgeType(vertex, _baseStorageManager));
            }
        }

        #endregion
    }
}

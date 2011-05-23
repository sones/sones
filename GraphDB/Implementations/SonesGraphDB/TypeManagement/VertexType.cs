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

using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Manager.BaseGraph;

namespace sones.GraphDB.TypeManagement
{
    /// <summary>
    /// This class transforms an IVertex (came from FS) into a vertex type.
    /// </summary>
    /// <remarks>
    /// An object of this class caches the intermediate results for the live time of the object itself. 
    /// This means equal queries to the FS are only done one time per object.
    /// The object also does most of its queries lazy. This means the queries are executed only if the result is needed.
    /// </remarks>
    internal class VertexType: BaseType, IVertexType
    {
        #region Data

        private IEnumerable<IUniqueDefinition> _uniques;
        private readonly bool _hasOwnUniques;
        private IEnumerable<IIndexDefinition> _indices;
        private readonly bool _hasOwnIndices;
        private IEnumerable<IVertexType> _childs;
        private readonly bool _hasChilds;

        #endregion

        #region c'tor

        /// <summary>
        /// Creates a new instance of VertexType.
        /// </summary>
        /// <param name="myVertexTypeVertex">An IVertex that represents the vertex type.</param>
        internal VertexType(IVertex myVertexTypeVertex)
            : base(myVertexTypeVertex)
        {
            _hasOwnUniques = HasOutgoingEdge(AttributeDefinitions.VertexTypeDotUniquenessDefinitions);
            _hasOwnIndices = HasIncomingVertices(BaseTypes.Index, AttributeDefinitions.IndexDotDefiningVertexType);
            _hasChilds = HasIncomingVertices(BaseTypes.VertexType, AttributeDefinitions.VertexTypeDotParent);
        }

        #endregion

        #region BaseType members

        public override bool HasParentType
        {
            get { return ID != (long)BaseTypes.Vertex; }
        }

        public override bool HasChildTypes
        {
            get { return _hasChilds; }
        }

        protected override BaseType GetParentType()
        {
            return HasParentType ? new VertexType(GetOutgoingSingleEdge(AttributeDefinitions.VertexTypeDotParent).GetTargetVertex()) : null;
        }

        protected override IDictionary<string, IAttributeDefinition> RetrieveAttributes()
        {
            return BaseGraphStorageManager.GetBinaryPropertiesFromFS(Vertex, this).Cast<IAttributeDefinition>()
                .Union(BaseGraphStorageManager.GetPropertiesFromFS(Vertex, this))
                .Union(BaseGraphStorageManager.GetOutgoingEdgesFromFS(Vertex, this))
                .Union(BaseGraphStorageManager.GetIncomingEdgesFromFS(Vertex, this))
                .ToDictionary(x => x.Name);
        }

        #endregion

        #region IVertexType Members

        #region Inheritance

        public IVertexType ParentVertexType
        {
            get 
            {
                return GetParentType() as IVertexType;
            }
        }

        public IEnumerable<IVertexType> GetChildVertexTypes(bool myRecursive = true, bool myIncludeSelf = false)
        {
            if (myIncludeSelf)
                yield return this;

            foreach (var aChildVertexType in GetChilds())
            {
                yield return aChildVertexType;

                if (!myRecursive) 
                    continue;

                foreach (var aVertex in aChildVertexType.GetChildVertexTypes())
                {
                    yield return aVertex;
                }
            }

            yield break;

        }

        #endregion

        #region Binary Properties

        public bool HasBinaryProperty(string myEdgeName)
        {
            return HasTypedAttribute<IBinaryPropertyDefinition>(myEdgeName);
        }

        public IBinaryPropertyDefinition GetBinaryPropertyDefinition(string myEdgeName)
        {
            return GetTypedAttributeDefinition<IBinaryPropertyDefinition>(myEdgeName);
        }

        public bool HasBinaryProperties(bool myIncludeAncestorDefinitions)
        {
            return HasTypedAttributes<IBinaryPropertyDefinition>(myIncludeAncestorDefinitions);
        }

        public IEnumerable<IBinaryPropertyDefinition> GetBinaryProperties(bool myIncludeAncestorDefinitions)
        {
            return GetTypedAttributeDefinitions<IBinaryPropertyDefinition>(myIncludeAncestorDefinitions);
        }

        #endregion

        #region Incoming Edges

        public bool HasIncomingEdge(string myEdgeName)
        {
            return HasTypedAttribute<IIncomingEdgeDefinition>(myEdgeName);
        }

        public IIncomingEdgeDefinition GetIncomingEdgeDefinition(string myEdgeName)
        {
            return GetTypedAttributeDefinition<IIncomingEdgeDefinition>(myEdgeName);
        }

        public bool HasIncomingEdges(bool myIncludeAncestorDefinitions)
        {
            return HasTypedAttributes<IIncomingEdgeDefinition>(myIncludeAncestorDefinitions);
        }

        public IEnumerable<IIncomingEdgeDefinition> GetIncomingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return GetTypedAttributeDefinitions<IIncomingEdgeDefinition>(myIncludeAncestorDefinitions);
        }

        #endregion

        #region Outgoing Edges

        public bool HasOutgoingEdge(string myEdgeName)
        {
            return HasTypedAttribute<IOutgoingEdgeDefinition>(myEdgeName);
        }

        public IOutgoingEdgeDefinition GetOutgoingEdgeDefinition(string myEdgeName)
        {
            return GetTypedAttributeDefinition<IOutgoingEdgeDefinition>(myEdgeName);
        }

        public bool HasOutgoingEdges(bool myIncludeAncestorDefinitions)
        {
            return HasTypedAttributes<IOutgoingEdgeDefinition>(myIncludeAncestorDefinitions);
        }

        public IEnumerable<IOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return GetTypedAttributeDefinitions<IOutgoingEdgeDefinition>(myIncludeAncestorDefinitions);
        }

        #endregion

        #region Uniques

        public bool HasUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            return (myIncludeAncestorDefinitions && HasParentType)
                ? _hasOwnUniques || ParentVertexType.HasUniqueDefinitions(true)
                : _hasOwnUniques;
        }

        public IEnumerable<IUniqueDefinition> GetUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            return (myIncludeAncestorDefinitions && HasParentType)
                ? GetUniques().Union(ParentVertexType.GetUniqueDefinitions(true))
                : GetUniques();
        }

        #endregion

        #region Indices

        public bool HasIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
            return (myIncludeAncestorDefinitions && HasParentType)
                ? _hasOwnIndices || ParentVertexType.HasIndexDefinitions(true)
                : _hasOwnIndices;
        }

        public IEnumerable<IIndexDefinition> GetIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
            return (myIncludeAncestorDefinitions && HasParentType)
                ? GetIndices().Union(ParentVertexType.GetIndexDefinitions(true))
                : GetIndices();
        }

        #endregion

        #endregion

        #region private members

        private IEnumerable<IVertexType> GetChilds()
        {
            if (_childs == null)
                lock (LockObject)
                {
                    if (_childs == null)
                        _childs = RetrieveChilds();
                }

            return _childs;
        }

        private IEnumerable<IVertexType> RetrieveChilds()
        {
            if (!HasChildTypes)
                return Enumerable.Empty<IVertexType>();

            var vertices = GetIncomingVertices(BaseTypes.VertexType, AttributeDefinitions.VertexTypeDotParent);

            return vertices.Select(vertex => new VertexType(vertex)).ToArray();
        }

        private IEnumerable<IUniqueDefinition> GetUniques()
        {
            if (_uniques == null)
                lock (LockObject)
                {
                    if (_uniques == null)
                        _uniques = RetrieveUniques();
                }

            return _uniques;
        }

        private IEnumerable<IUniqueDefinition> RetrieveUniques()
        {
            if (HasOutgoingEdge(AttributeDefinitions.VertexTypeDotUniquenessDefinitions))
            {
                var edge = GetOutgoingHyperEdge(AttributeDefinitions.VertexTypeDotUniquenessDefinitions);
                var vertices = edge.GetTargetVertices();
                var indices = vertices.Select(x => BaseGraphStorageManager.CreateIndexDefinition(x, this));
                return indices.Select(ConvertIIndexDefinitionToIUniqueDefinition).ToArray();
            }
            return Enumerable.Empty<IUniqueDefinition>();
        }

        private IUniqueDefinition ConvertIIndexDefinitionToIUniqueDefinition(IIndexDefinition myIndexDefinition)
        {
            return new UniqueDefinition
            {
                DefiningVertexType = this,
                UniquePropertyDefinitions = myIndexDefinition.IndexedProperties,
                CorrespondingIndex = myIndexDefinition,
            };
        }

        private IEnumerable<IIndexDefinition> GetIndices()
        {
            if (_indices == null)
                lock (LockObject)
                {
                    if (_indices == null)
                        _indices = RetrieveIndices();
                }

            return _indices;
        }

        private IEnumerable<IIndexDefinition> RetrieveIndices()
        {
            if (_hasOwnIndices)
            {
                var vertices = GetIncomingVertices(BaseTypes.Index, AttributeDefinitions.IndexDotDefiningVertexType);
                return vertices.Select(x => BaseGraphStorageManager.CreateIndexDefinition(x, this)).ToArray();
            }
            return Enumerable.Empty<IIndexDefinition>();

        }

        #endregion

    }
}

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
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.Manager.BaseGraph;

namespace sones.GraphDB.TypeManagement
{
    internal class EdgeType: BaseType, IEdgeType
    {
        #region Data

        //private IEnumerable<IEdgeType> _childs;
        private readonly bool _hasChilds;        

        #endregion

        #region c'tor

        public EdgeType(IVertex myVertex, BaseGraphStorageManager myBaseStorageManager)
            : base(myVertex, myBaseStorageManager)
        {
            _hasChilds = HasIncomingVertices(BaseTypes.EdgeType, AttributeDefinitions.EdgeTypeDotParent);
        }

        #endregion

        #region BaseType Members

        public override bool HasParentType
        {
            get { return ID != (long)BaseTypes.Edge; }
        }

        public override bool HasChildTypes
        {
            get { return _hasChilds; }
        }

        protected override BaseType RetrieveParentType()
        {
            return HasParentType ? new EdgeType(GetOutgoingSingleEdge(AttributeDefinitions.EdgeTypeDotParent).GetTargetVertex(), _baseStorageManager) : null;
        }

        protected override IDictionary<string, IAttributeDefinition> RetrieveAttributes()
        {
            return _baseStorageManager.GetPropertiesFromFS(Vertex, this).Cast<IAttributeDefinition>().ToDictionary(x => x.Name);
        }

        protected override IEnumerable<BaseType> RetrieveChildrenTypes()
        {
            if (!HasChildTypes)
                return Enumerable.Empty<EdgeType>();

            var vertices = GetIncomingVertices(BaseTypes.EdgeType, AttributeDefinitions.EdgeTypeDotParent);

            return vertices.Select(vertex => new EdgeType(vertex, _baseStorageManager)).ToArray();
        }

        #endregion

        #region IEdgeType Members

        #region Inheritance

        public IEnumerable<IEdgeType> GetDescendantEdgeTypes()
        {
            return GetDescendantTypes().Cast<IEdgeType>().ToArray();
        }

        public IEnumerable<IEdgeType> GetDescendantEdgeTypesAndSelf()
        {
            return GetDescendantTypesAndSelf().Cast<IEdgeType>().ToArray();
        }

        public IEnumerable<IEdgeType> GetAncestorEdgeTypes()
        {
            return GetAncestorTypes().Cast<IEdgeType>().ToArray();
        }

        public IEnumerable<IEdgeType> GetAncestorEdgeTypesAndSelf()
        {
            return GetAncestorTypesAndSelf().Cast<IEdgeType>().ToArray();
        }

        public IEnumerable<IEdgeType> GetKinsmenEdgeTypes()
        {
            return GetKinsmenTypes().Cast<IEdgeType>().ToArray();
        }

        public IEnumerable<IEdgeType> GetKinsmenEdgeTypesAndSelf()
        {
            return GetKinsmenTypesAndSelf().Cast<IEdgeType>().ToArray();
        }

        public IEdgeType ParentEdgeType
        {
            get { return ParentType as IEdgeType; }
        }

        public IEnumerable<IEdgeType> ChildrenEdgeTypes
        {
            get { return ChildrenTypes.Cast<IEdgeType>().ToArray(); }
        }

        #endregion

        #endregion

    }
}

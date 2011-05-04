using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using sones.Library.LanguageExtensions;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.Manager.BaseGraph;
using System.Collections;

namespace sones.GraphDB.TypeManagement
{
    internal class EdgeType: BaseType, IEdgeType
    {

        #region Constants

        /// <summary>
        /// This is the initialization count of the result list of a GetChildVertices method
        /// </summary>
        private const int ExpectedChildTypes = 50;

        /// <summary>
        /// This is the initialization count of the result list of a GetAttributes method
        /// </summary>
        private const int ExpectedAttributes = 50;

        #endregion

        #region Data

        private IEnumerable<IEdgeType> _childs;
        private bool _hasChilds;

        #endregion

        #region c'tor

        public EdgeType(IVertex myVertex): base(myVertex)
        {
            _hasChilds = HasIncomingVertices(BaseTypes.EdgeType, AttributeDefinitions.EdgeTypeDotParent);
        }

        #endregion


        public override bool HasParentType
        {
            get { return ID != (long)BaseTypes.Edge; }
        }

        public override bool HasChildTypes
        {
            get { return _hasChilds; }
        }

        protected override BaseType GetParentType()
        {
            if (HasParentType)
                return new EdgeType(GetOutgoingSingleEdge(AttributeDefinitions.EdgeTypeDotParent).GetTargetVertex());

            return null;
        }

        protected override IDictionary<string, IAttributeDefinition> RetrieveAttributes()
        {
            return BaseGraphStorageManager.GetPropertiesFromFS(_vertex, this).Cast<IAttributeDefinition>().ToDictionary(x => x.Name);
        }

        #region IEdgeType Members

        public IEdgeType ParentEdgeType
        {
            get 
            {
                return GetParentType() as IEdgeType;
            }
        }

        public IEnumerable<IEdgeType> GetChildEdgeTypes(bool myRecursive = true, bool myIncludeSelf = false)
        {
            if (myIncludeSelf)
                yield return this;

            foreach (var aChildEdgeType in GetChilds())
            {
                yield return aChildEdgeType;

                if (myRecursive)
                {
                    foreach (var aVertex in aChildEdgeType.GetChildEdgeTypes(true))
                    {
                        yield return aVertex;
                    }
                }
            }

            yield break;
        }

        #endregion

        #region private methods

        private IEnumerable<IEdgeType> GetChilds()
        {
            if (_childs == null)
                lock (_lock)
                {
                    if (_childs == null)
                        _childs = RetrieveChilds();
                }

            return _childs;
        }

        private IEnumerable<IEdgeType> RetrieveChilds()
        {
            if (!HasChildTypes)
                return Enumerable.Empty<IEdgeType>();

            var vertices = GetIncomingVertices(BaseTypes.EdgeType, AttributeDefinitions.EdgeTypeDotParent);

            return vertices.Select(vertex => new EdgeType(vertex)).ToArray();
        }


        #endregion
    }
}

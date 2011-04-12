using System;
using System.Collections.Generic;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement
{
    internal class EdgeType: IEdgeType
    {
        private Library.PropertyHyperGraph.IVertex vertex;

        public EdgeType(Library.PropertyHyperGraph.IVertex vertex)
        {
            // TODO: Complete member initialization
            this.vertex = vertex;
        }


        #region IEdgeType Members

        string IEdgeType.Name
        {
            get { throw new NotImplementedException(); }
        }

        IBehaviour IEdgeType.Behaviour
        {
            get { throw new NotImplementedException(); }
        }

        string IEdgeType.Comment
        {
            get { throw new NotImplementedException(); }
        }

        bool IEdgeType.IsAbstract
        {
            get { throw new NotImplementedException(); }
        }

        bool IEdgeType.IsSealed
        {
            get { throw new NotImplementedException(); }
        }

        bool IEdgeType.HasParentEdgeType
        {
            get { throw new NotImplementedException(); }
        }

        IEdgeType IEdgeType.GetParentEdgeType
        {
            get { throw new NotImplementedException(); }
        }

        bool IEdgeType.HasChildEdgeTypes
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerable<IEdgeType> IEdgeType.GetChildEdgeTypes
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerable<IPropertyDefinition> IEdgeType.GetProperties
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}

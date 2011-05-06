using System.Collections.Generic;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Index
{
    internal class IndexDefinition: IIndexDefinition
    {
        #region IIndexDefinition Members

        public string Name { get; internal set; }

        public string IndexTypeName { get; internal set; }

        public string Edition { get; internal set; }

        public bool IsUserdefined { get; internal set; }

        public IList<IPropertyDefinition> IndexedProperties { get; internal set; }

        public IVertexType VertexType { get; internal set; }

        public long ID { get; internal set; }

        public bool IsSingle { get; internal set; }

        public bool IsRange { get; internal set; }

        public bool IsVersioned { get; internal set; }

        public IIndexDefinition SourceIndex { get; internal set; }

        #endregion
    }
}

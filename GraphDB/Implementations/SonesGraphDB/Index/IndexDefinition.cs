using System.Collections.Generic;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Index
{
    internal class IndexDefinition: IIndexDefinition
    {
        public string Name { get; internal set; }

        public string IndexTypeName { get; internal set; }

        public string Edition { get; internal set; }

        public bool IsUserdefined { get; internal set; }

        public IEnumerable<IPropertyDefinition> IndexedProperties { get; internal set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.TypeSystem
{
    public interface IIndexDefinition
    {
        String Name { get; }

        String IndexTypeName { get; }

        IEnumerable<IAttributeDefinition> IndexedProperties { get; }
    }
}

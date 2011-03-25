using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.TypeSystem
{
    public interface IUniqueDefinition
    {
        UInt16 Count { get; }

        IEnumerable<IAttributeDefinition> GetUniqueAttributeDefinitions();
    }
}

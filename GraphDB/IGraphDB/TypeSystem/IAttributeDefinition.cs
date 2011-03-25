using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.TypeSystem
{
    public interface IAttributeDefinition
    {
        String Name { get; }

        Int64 AttributeID { get; }

        AttributeType Kind { get; }
    }
}

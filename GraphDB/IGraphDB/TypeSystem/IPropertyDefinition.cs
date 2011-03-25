using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.TypeSystem
{
    public interface IPropertyDefinition : IAttributeDefinition
    {
        Boolean IsMandatory { get; }

        Type BaseType { get; }
    }
}

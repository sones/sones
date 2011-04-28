using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Helper.Enums;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    public class EdgeTypeParamDefinition
    {
        public Object Param { get; private set; }
        public ParamType Type { get; private set; }

        public EdgeTypeParamDefinition(ParamType type, object param)
        {
            // TODO: Complete member initialization
            this.Type = type;
            this.Param = param;
        }
    }
}

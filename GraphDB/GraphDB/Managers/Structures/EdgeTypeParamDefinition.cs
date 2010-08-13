using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Managers.Structures
{

    public enum ParamType
    {
        GraphType,
        Value,
        DefaultValueDef,
        Sort
    }

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

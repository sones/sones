using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    /// <summary>
    /// This is the type definition of the type of an attribute. e.g. List&lt;String&gt;
    /// </summary>
    public sealed class DBTypeOfAttributeDefinition
    {
        /// <summary>
        /// The name of the type
        /// </summary>
        public String Name { get; set; }

        public String Type { get; set; }

        public TypeCharacteristics TypeCharacteristics { get; set; }
        public String EdgeType { get; set; }
        public EdgeTypeParamDefinition[] Parameters { get; set; }

        public DBTypeOfAttributeDefinition()
        {
            TypeCharacteristics = new TypeCharacteristics();
        }
    }
}

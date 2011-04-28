using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    public sealed class AttributeDefinition
    {
        #region Properties

        public String AttributeName { get; private set; }
        public DBTypeOfAttributeDefinition AttributeType { get; private set; }
        public object DefaultValue { get; private set; }

        #endregion

        #region Ctor

        public AttributeDefinition(DBTypeOfAttributeDefinition attributeType, String attributeName, Object defaultValue = null)
        {

            AttributeType = attributeType;
            AttributeName = attributeName;
            DefaultValue = defaultValue;

        }

        #endregion
    }
}

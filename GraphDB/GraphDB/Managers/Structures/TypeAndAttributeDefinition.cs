using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Managers.Structures
{
    public class TypeAndAttributeDefinition
    {
        private TypeAttribute _attributeDefinition;
        private GraphDBType _typeOfAttribute;

        public TypeAttribute Definition { get { return _attributeDefinition; } }
        public GraphDBType TypeOfAttribute { get { return _typeOfAttribute; } }

        public TypeAndAttributeDefinition(TypeAttribute attributeDefinition, GraphDBType typeOfAttribute)
        {
            _attributeDefinition = attributeDefinition;
            _typeOfAttribute = typeOfAttribute;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_RemoveTypeAttribute : GraphDBAttributeError
    {
        public GraphDBType Type { get; private set; }
        public TypeAttribute Attribute { get; private set; }

        public Error_RemoveTypeAttribute(GraphDBType myType, TypeAttribute myAttribute)
        {
            Type = myType;
            Attribute = myAttribute;
        }

        public override string ToString()
        {
            return String.Format("The attribute " + Attribute.Name + " from type " + Type.Name + " could not be removed.");
        }
    }
}

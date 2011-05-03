using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request.CreateVertexTypes
{

    /// <summary>
    /// This class represents an attribute predefinition, that can be used, if it is unclear whether the attribute will become a property, binary property, outgoing edge or incoming edge.
    /// </summary>
    /// This class is internally converted into one of the other attribute predefinitions depending on the <see cref="AttributeType"/>.
    /// Properties that are not needed from this predefinitions will be ignored, e.g. a property predefinition will ignore the edge type.
    public class UnknownAttributePredefinition: AttributePredefinition
    {
        public const String LISTMultiplicity = "LIST";

        public const String SETMultiplicity = "SET";

        public String Multiplicity { get; private set; }

        public String DefaultValue { get; private set; }

        public String EdgeType { get; private set; }

        public String InnerEdgeType { get; private set; }

        public UnknownAttributePredefinition(String myAttributeName)
            : base(myAttributeName)
        {

        }

        public UnknownAttributePredefinition SetMultiplicity(String myMultiplicity)
        {
            Multiplicity = myMultiplicity;

            return this;
        }

        public UnknownAttributePredefinition SetMultiplicityAsList()
        {
            Multiplicity = LISTMultiplicity;

            return this;
        }

        public UnknownAttributePredefinition SetMultiplicityAsSet()
        {
            Multiplicity = SETMultiplicity;

            return this;
        }

        public UnknownAttributePredefinition SetDefaultValue(String myDefaultValue)
        {
            DefaultValue = myDefaultValue;

            return this;
        }

        public UnknownAttributePredefinition SetInnerEdgeType(String myEdgeType)
        {
            InnerEdgeType = myEdgeType;

            return this;
        }

        public UnknownAttributePredefinition SetEdgeType(String myEdgeType)
        {
            EdgeType = myEdgeType;

            return this;
        }

        public UnknownAttributePredefinition SetAttributeType(String myAttributeType)
        {
            AttributeType = myAttributeType;

            return this;
        }


    }
}

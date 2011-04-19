using System;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDB.TypeManagement
{
    /// <summary>
    /// This class represents a property definition
    /// </summary>
    internal class PropertyDefinition: IPropertyDefinition
    {
        public bool IsMandatory { get; internal set; }

        public Type BaseType { get; internal set; }

        public PropertyMultiplicity Multiplicity { get; internal set; }

        public string Name { get; internal set; }

        public long AttributeID { get; internal set; }

        public AttributeType Kind { get { return AttributeType.Property; } }

        #region IPropertyDefinition Members

        public IComparable ExtractValue(IVertex aVertex)
        {
            //A usual property like Age or Name...
            return aVertex.GetProperty(AttributeID);
        }

        #endregion
    }
}

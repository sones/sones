using System;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using System.Collections.Generic;
using sones.GraphDB.TypeManagement.Base;

namespace sones.GraphDB.TypeManagement
{
    /// <summary>
    /// This class represents a property definition
    /// </summary>
    internal class PropertyDefinition: IPropertyDefinition
    {

        #region IPropertyDefinition Members

        public IComparable ExtractValue(IVertex aVertex)
        {
            if (RelatedType.ID == (long)BaseTypes.Vertex)
            {
                switch (AttributeID)
                {
                    case (long)AttributeDefinitions.UUID:
                        return aVertex.VertexID;
                    case (long)AttributeDefinitions.CreationDate:
                        return aVertex.CreationDate;
                    case (long)AttributeDefinitions.ModificationDate:
                        return aVertex.ModificationDate;
                    case (long)AttributeDefinitions.Comment:
                        return aVertex.Comment;
                    case (long)AttributeDefinitions.Edition:
                        return aVertex.EditionName;
                    case (long)AttributeDefinitions.Revision:
                        return aVertex.VertexRevisionID;
                    case (long)AttributeDefinitions.TypeID:
                        return aVertex.VertexTypeID;
                    default:
                        return null;
                }
            }
            else
            {
                //A usual property like Age or Name...
                if (aVertex.HasProperty(AttributeID))
                {
                    return aVertex.GetProperty(AttributeID);                    
                }
                
                return null;
            }
        }

        public bool IsMandatory { get; internal set; }

        public bool IsUserDefinedType { get; internal set; }

        public Type BaseType { get; internal set; }

        public PropertyMultiplicity Multiplicity { get; internal set; }

        public IEnumerable<IIndexDefinition> InIndices { get; internal set; }

        public IComparable DefaultValue { get; internal set; }

        #endregion

        #region IAttributeDefinition Members

        public long ID { get; internal set; }

        public string Name { get; internal set; }

        public long AttributeID { get; internal set; }

        public AttributeType Kind { get { return AttributeType.Property; } }

        public IBaseType RelatedType { get; internal set; }

        #endregion


        #region IEquatable<IAttributeDefinition> Members

        public bool Equals(IAttributeDefinition myOther)
        {
            return myOther != null && myOther.AttributeID == AttributeID && EqualityComparer<IBaseType>.Default.Equals(RelatedType, myOther.RelatedType);
        }

        #endregion
    }
}

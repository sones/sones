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
                switch (ID)
                {
                    case (long)AttributeDefinitions.VertexDotUUID:
                        return aVertex.VertexID;
                    case (long)AttributeDefinitions.VertexDotCreationDate:
                        return aVertex.CreationDate;
                    case (long)AttributeDefinitions.VertexDotModificationDate:
                        return aVertex.ModificationDate;
                    case (long)AttributeDefinitions.VertexDotComment:
                        return aVertex.Comment;
                    case (long)AttributeDefinitions.VertexDotEdition:
                        return aVertex.EditionName;
                    case (long)AttributeDefinitions.VertexDotRevision:
                        return aVertex.VertexRevisionID;
                    case (long)AttributeDefinitions.VertexDotTypeID:
                        return aVertex.VertexTypeID;
                    default:
                        return null;
                }
            }
            else
            {
                //A usual property like Age or Name...
                if (aVertex.HasProperty(ID))
                {
                    return aVertex.GetProperty(ID);                    
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

        public bool IsUserDefined { get; internal set; }

        #endregion

        #region IAttributeDefinition Members

        public long ID { get; internal set; }

        public string Name { get; internal set; }

        public AttributeType Kind { get { return AttributeType.Property; } }

        public IBaseType RelatedType { get; internal set; }

        #endregion


        #region IEquatable<IAttributeDefinition> Members

        public bool Equals(IAttributeDefinition myOther)
        {
            return myOther != null && myOther.ID == ID;
        }

        #endregion
    }
}

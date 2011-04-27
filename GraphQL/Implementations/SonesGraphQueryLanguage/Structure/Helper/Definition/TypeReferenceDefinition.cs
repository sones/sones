using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public sealed  class TypeReferenceDefinition
    {

        public TypeReferenceDefinition(string typeName, string reference)
        {
            TypeName = typeName;
            Reference = reference;
        }

        public String TypeName { get; private set; }
        public String Reference { get; private set; }

        public override int GetHashCode()
        {
            return TypeName.GetHashCode() ^ Reference.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj as TypeReferenceDefinition == null)
            {
                return false;
            }
            return TypeName.Equals((obj as TypeReferenceDefinition).TypeName) && Reference.Equals((obj as TypeReferenceDefinition).Reference);
        }

    }
}

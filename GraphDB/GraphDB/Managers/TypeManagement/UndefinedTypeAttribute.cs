using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.TypeManagement
{
    public class UndefinedTypeAttribute : TypeAttribute
    {

        public static AttributeUUID AttributeUUID = new AttributeUUID(99);

        public String UndefinedAttributeName { get; private set; }

        public UndefinedTypeAttribute()
            : base(AttributeUUID)
        { }

        public UndefinedTypeAttribute(String myUndefinedAttributeName)
            : base(AttributeUUID)
        {
            UndefinedAttributeName = myUndefinedAttributeName;
            Name = myUndefinedAttributeName;
            TypeCharacteristics = new TypeCharacteristics();
        }

        public override bool Equals(object obj)
        {

            if (!(obj is UndefinedTypeAttribute))
            {
                return false;
            }

            return UndefinedAttributeName == UndefinedAttributeName;
        }

        public override int GetHashCode()
        {
            return UndefinedAttributeName.GetHashCode();
        }


    }
}

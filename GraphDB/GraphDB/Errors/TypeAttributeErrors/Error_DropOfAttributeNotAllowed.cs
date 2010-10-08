using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_DropOfAttributeNotAllowed : GraphDBAttributeError
    {
        public String AttributeName { get; private set; }
        public String TypeName { get; private set; }
        public Dictionary<TypeAttribute, GraphDBType> ConflictingAttributes { get; private set; }

        public Error_DropOfAttributeNotAllowed(String myType, String myAttributeName, Dictionary<TypeAttribute, GraphDBType> myConflictingAttributes)
        {
            ConflictingAttributes = myConflictingAttributes;
            TypeName = myType;
            AttributeName = myAttributeName;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var aConflictingAttribute in ConflictingAttributes)
            {
                sb.Append(String.Format("{0} ({1}),", aConflictingAttribute.Key.Name, aConflictingAttribute.Value.Name));
            }

            sb.Remove(sb.Length - 1, 1);

            return String.Format("It is not possible to drop {0} of type {1} because there are remaining references from the following attributes: {2}" + Environment.NewLine + "Please remove them in previous.", AttributeName, TypeName, sb.ToString());

        }


    }
}

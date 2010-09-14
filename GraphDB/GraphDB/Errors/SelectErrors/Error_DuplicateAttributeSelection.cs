using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_DuplicateAttributeSelection : GraphDBSelectError
    {
        public String SelectionAlias { get; private set; }

        public Error_DuplicateAttributeSelection(String mySelectionAlias)
        {
            SelectionAlias = mySelectionAlias;
        }

        public override string ToString()
        {
            return String.Format("You cannot select \"{0}\" more than one time. Try to use an alias.", SelectionAlias);
        }
    }
}

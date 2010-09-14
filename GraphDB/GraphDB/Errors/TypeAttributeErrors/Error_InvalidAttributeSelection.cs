using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidAttributeSelection : GraphDBAttributeError
    {
        public String SelectedAttribute { get; private set; }

        public Error_InvalidAttributeSelection(String mySelectedAttribute)
        {
            SelectedAttribute = mySelectedAttribute;
        }

        public override string ToString()
        {
            return String.Format("The selected attribute \"{0}\" is not valid!", SelectedAttribute);
        }
    }
}

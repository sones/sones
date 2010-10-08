using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Warnings
{
    public class Warning_UndefinedAttribute : GraphDBWarning
    {
        public Warning_UndefinedAttribute()
        { 
        }

        public override string ToString()
        {
            return "Warning insert or update of an undefined attribute.";
        }
    }
}

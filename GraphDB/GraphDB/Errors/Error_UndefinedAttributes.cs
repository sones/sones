using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_UndefinedAttributes : GraphDBError
    {
        public Error_UndefinedAttributes()
        { 
        }

        public override string ToString()
        {
            return "Undefined attributes can't inserted nor updated. Use the setting SETUNDEFBEHAVE to change this behaviour.";
        }
    }
}

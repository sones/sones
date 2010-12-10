using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Warnings
{
    public class Warning_UndefinedAttribute : GraphDBWarning
    {
        private Managers.Structures.IDChainDefinition iDChainDefinition;

        public Warning_UndefinedAttribute(Managers.Structures.IDChainDefinition iDChainDefinition)
        {
            this.iDChainDefinition = iDChainDefinition;
        }

        public override string ToString()
        {
            return string.Format("Warning insert or update of an undefined attribute {0}.", this.iDChainDefinition.ContentString);
        }
    }
}

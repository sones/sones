using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidGroupByLevel : GraphDBSelectError
    {
        public IDChainDefinition IDChainDefinition { get; private set; }

        public Error_InvalidGroupByLevel(IDChainDefinition myIDChainDefinition)
        {
            IDChainDefinition = myIDChainDefinition;
        }

        public override string ToString()
        {
            return String.Format("The level ({0}) greater than 1 is not allowed: '{1}'", IDChainDefinition.Edges.Count, IDChainDefinition.ContentString);
        }
    }
}

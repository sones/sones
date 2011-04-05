using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement
{
    internal class IncomingEdgeDefinition: IIncomingEdgeDefinition
    {
        public IOutgoingEdgeDefinition RelatedEdgeDefinition { get; internal set; }

        public string Name { get; internal set; }

        public long AttributeID { get; internal set; }

        public AttributeType Kind { get; internal set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement
{
    internal class UniqueDefinition: IUniqueDefinition
    {
        #region IUniqueDefinition Members

        public IEnumerable<IPropertyDefinition> UniquePropertyDefinitions { get; internal set; }

        public long ID { get; internal set; }

        public IVertexType DefiningVertexType { get; internal set; }

        #endregion
    }
}

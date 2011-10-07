using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI;
using sones.GraphDS.GraphDSRemoteClient.GraphElements;

namespace sones.GraphDS.GraphDSRemoteClient.TypeManagement
{
    internal class RemoteUniqueDefinition : IUniqueDefinition
    {
        #region Constructor

        internal RemoteUniqueDefinition(ServiceUniqueDefinition myUniqueDefinition, IServiceToken myServiceToken)
        {
            this.UniquePropertyDefinitions = myUniqueDefinition.UniquePropertyDefinition.Select(x => new RemotePropertyDefinition(x, myServiceToken));
            this.DefiningVertexType = new RemoteVertexType(myUniqueDefinition.DefiningVertexType, myServiceToken);
            this.CorrespondingIndex = new RemoteIndexDefinition(myUniqueDefinition.CorrespondingIndex, myServiceToken);
        }

        #endregion
        

        #region IUniqueDefinition Members

        public IEnumerable<IPropertyDefinition> UniquePropertyDefinitions { get; internal set; }

        public IVertexType DefiningVertexType { get; internal set; }

        public IIndexDefinition CorrespondingIndex { get; internal set; }

        #endregion
    }
}

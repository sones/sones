using System;
using System.Collections.Generic;
using sones.GraphDB.NewAPI;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.NewAPI
{

    public interface IEdge : IEquatable<IEdge>
    {

        #region IEdge Properties

        ObjectUUID        UUID          { get; set; }
        String            TYPE          { get; }
        String            EDITION       { get; set; }
        ObjectRevisionID  REVISIONID    { get; set; }
        String            Comment       { get; set; }

        #endregion

        String EdgeTypeName { get; set; }

        IVertex SourceVertex { get; }
        IVertex TargetVertex { get; }
        IEnumerable<IVertex> TargetVertices { get; }
        
    }

}

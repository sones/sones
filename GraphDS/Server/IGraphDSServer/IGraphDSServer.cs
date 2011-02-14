using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS;

namespace sones.GraphDSServer
{
    /// <summary>
    /// The interface for all GraphDS server
    /// </summary>
    public interface IGraphDSServer : IGraphDSREST, IGraphDS
    {
    }
}

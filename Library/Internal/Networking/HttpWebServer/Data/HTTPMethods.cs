using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Networking.HTTP
{

    /// <summary>
    /// WebDAV valid Methods
    /// </summary>
    public enum HTTPMethods
    {

        UNKNOWN,
        OPTIONS,
        PROPFIND,
        PROPPATCH,
        MKCOL,
        GET,
        HEAD,
        POST,
        DELETE,
        PUT,
        COPY,
        MOVE,
        LOCK,
        UNLOCK,
        TRACE

    }

}

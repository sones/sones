/*
 * GraphFS - ISessionInfo
 * (c) Achim Friedland, 2009 - 2010
 */

using System;
using System.Text;
using System.Collections.Generic;

namespace sones.GraphFS.Session
{

    public interface ISessionInfo
    {
        SessionUUID SessionUUID         { get; }
        String      Username            { get; }
        Boolean     ThrowExceptions     { get; set; }
    }

}

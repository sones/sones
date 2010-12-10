/*
 * IGraphDBInterface
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;

using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;
using System.Collections.Generic;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB
{

    public interface IGraphDB
    {
        ObjectLocation DatabaseRootPath { get; }
        void Shutdown(SessionToken mySessionToken);
    }

}

/*
 * IGraphDBExport
 * Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Net.Mime;

using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphIO
{

    /// <summary>
    /// An interface to export data from the graph database
    /// </summary>

    public interface IGraphDBExport
    {

        ContentType ExportContentType { get; }

        Byte[]      ExportQueryResult (QueryResult myQueryResult);
        Object      ExportVertex      (Vertex      myVertex);

    }

}

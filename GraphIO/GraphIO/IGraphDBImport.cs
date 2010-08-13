/*
 * IGraphDBImport
 * Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Net.Mime;
using sones.GraphDB.Structures.Result;

#endregion

namespace sones.GraphIO
{

    /// <summary>
    /// An interface to import data into the graph database
    /// </summary>

    public interface IGraphDBImport
    {

        ContentType     ImportContentType   { get; }

        QueryResult     ParseQueryResult    (String myInput);
        DBObjectReadout ParseDBObject       (String myInput);

    }

}

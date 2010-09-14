/*
 * IGraphDBImport
 * Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Net.Mime;

using sones.GraphDB.Result;
using sones.Lib.ErrorHandling;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphIO
{

    /// <summary>
    /// An interface to import data into the graph database
    /// </summary>

    public interface IGraphDBImport
    {

        ContentType         ImportContentType           { get; }

        QueryResult         ParseQueryResult            (String myInput);
        Vertex              ParseVertex                 (String myInput);

        UnspecifiedWarning  GenerateUnspecifiedWarning  (Object myWarningXElement);
        UnspecifiedError    GenerateUnspecifiedError    (Object myErrorXElement);

    }

}

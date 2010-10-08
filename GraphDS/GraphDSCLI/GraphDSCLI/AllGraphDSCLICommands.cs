/*
 * AllGraphDSCLICommands
 * Achim Friedland, 2010
 */

#region Usings

using System;

using sones.GraphDB;
using sones.GraphDB.GraphQL;
using sones.GraphDB.Structures;

using sones.GraphDS.Connectors.CLI;
using sones.GraphDB.Result;
using sones.GraphDS.API.CSharp;

#endregion

namespace sones.GraphFS.Connectors.GraphDSCLI
{

    /// <summary>
    /// The abstract class for all GraphDS commands of the grammar-based
    /// command line interface.
    /// </summary>
    public abstract class AllGraphDSCLICommands : AllCLICommands
    {

        #region QueryDB(myQueryString, IGraphDBSession, myWithOutput = true)

        protected QueryResult QueryDB(AGraphDSSharp myGraphDSSharp, String myQueryString, Boolean myWithOutput = true)
        {

            if (myWithOutput)
                Write(myQueryString + " => ");

            var GQLQuery = new GraphQLQuery(myGraphDSSharp.IGraphDBSession.DBPluginManager);
            var _QueryResult = GQLQuery.Query(myQueryString, myGraphDSSharp.IGraphDBSession);

            if (myWithOutput)
                WriteLine(_QueryResult.ResultType.ToString());

            if (_QueryResult == null)
                WriteLine("The QueryResult is invalid!\n\n");

            else if (_QueryResult.ResultType != ResultType.Successful)
                foreach (var aError in _QueryResult.Errors)
                    WriteLine(aError.GetType().ToString() + ": " + aError.ToString() + "\n\n");

            return _QueryResult;

        }

        #endregion
    
    }

}

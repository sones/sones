/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/



#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.ImportExport;


#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Dump
{

    public class DumpNode : AStatement
    {

        private List<String> _TypesToDump;
        private DumpFormats _DumpFormat;
        private DumpTypes   _DumpType;
        private IDumpable   _DumpableGrammar;
        private String      _DumpDestination;

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region Get the optional type list

            if (parseNode.ChildNodes[1].HasChildNodes())
            {
                _TypesToDump = ((parseNode.ChildNodes[1].ChildNodes[1].AstNode as TypeListNode).Types).Select(tlnode => tlnode.TypeName).ToList();
            }

            #endregion

            _DumpType           = (parseNode.ChildNodes[2].AstNode as DumpTypeNode).DumpType;
            _DumpFormat         = (parseNode.ChildNodes[3].AstNode as DumpFormatNode).DumpFormat;
            _DumpableGrammar    = context.Compiler.Language.Grammar as IDumpable;

            if (_DumpableGrammar == null)
            {
                throw new GraphDBException(new Error_NotADumpableGrammar(context.Compiler.Language.Grammar.GetType().ToString()));
            }

            if (parseNode.ChildNodes[4].HasChildNodes())
            {
                _DumpDestination = parseNode.ChildNodes[4].ChildNodes[1].Token.ValueString;
            }

        }

        public override String StatementName
        {
            get { return "DUMP"; }
        }

        public override Enums.TypesOfStatements TypeOfStatement
        {
            get { return Enums.TypesOfStatements.Readonly; }
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="myIGraphDBSession">The DBSession to start new transactions</param>
        /// <param name="myDBContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="myIGraphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession myIGraphDBSession, DBContext myDBContext)
        {

            var dumpReadout = new Dictionary<String, Object>();
            AGraphDBExport exporter;

            switch (_DumpFormat)
            {
                case DumpFormats.GQL:
                    exporter = new GraphDBExport_GQL();
                    break;
                default:
                    return new QueryResult(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

            }

            return exporter.Export(_DumpDestination, myDBContext, _DumpableGrammar, _TypesToDump, _DumpType);


        }

        /*

#region WriteToLocation - Wrap this into classes

private QueryResult WriteToLocation(Dictionary<String, Object> GDDLandGDML)
{

    #region Read querie lines from location

    try
    {
        if (_DumpDestination.ToLower().StartsWith(@"file:\\"))
        {
            return new QueryResult(new SelectionResultSet(new List<DBObjectReadout>() { new DBObjectReadout(WriteFile(_DumpDestination.Substring(@"file:\\".Length), GDDLandGDML)) }));
        }
        else if (_DumpDestination.ToLower().StartsWith("http://"))
        {
            return new QueryResult(new SelectionResultSet(new List<DBObjectReadout>() { new DBObjectReadout(UploadToHttp(_DumpDestination, GDDLandGDML)) }));
        }
    }
    catch (Exception ex)
    {
        return new QueryResult(new Exceptional(new Error_ImportFailed(ex)));
    }

    #endregion

    return new QueryResult(new Exceptional(new Error_InvalidDumpLocation(_DumpDestination, @"file:\\", "http://")));

}

private Dictionary<String, Object> WriteFile(string filename, Dictionary<String, Object> GDDLandGDML)
{

    var output = new Dictionary<String, Object>();

    using (var file = File.Create(filename))
    { }

    foreach (var vals in GDDLandGDML)
    {
        File.AppendAllLines(filename, vals.Value as List<String>);
        output.Add(vals.Key, new List<String>(new[] { filename }));
    }

    return output;
}

private Dictionary<String, Object> UploadToHttp(string destination, Dictionary<String, Object> GDDLandGDML)
{

    var output = new Dictionary<String, Object>();

    var request = (HttpWebRequest)WebRequest.Create(destination);
    request.Method = "PUT";
    request.Timeout = 1000;
    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
    {
        foreach (var vals in GDDLandGDML)
        {
            foreach (var line in vals.Value as List<String>)
            {
                streamWriter.WriteLine(line);
            }
            output.Add(vals.Key, new List<String>(new[] { destination }));
        }
    }

    var response = request.GetResponse();
    var stream = new StreamReader(response.GetResponseStream());

    var errors = stream.ReadToEnd();

    if (!String.IsNullOrEmpty(errors))
    {
        throw new Exception(errors);
    }

    return output;
}

#endregion
*/

    }

}

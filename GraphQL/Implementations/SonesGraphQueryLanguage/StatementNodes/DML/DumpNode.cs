/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.GQL.Structure.Helper.Enums;
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.ErrorHandling;
using sones.Library.DataStructures;
using System.Diagnostics;
using sones.Plugins.SonesGQL.DBExport;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class DumpNode : AStatement, IAstNodeInit
    {
        #region properties

        private IEnumerable<String> _TypesToDump;
        private DumpFormats _DumpFormat;
        private DumpTypes _DumpType;
        private IDumpable _DumpableGrammar;
        private String _DumpDestination;
        private String _Query = "";

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region Get the optional type list

            if (HasChildNodes(parseNode.ChildNodes[1]))
            {
                _TypesToDump = ((parseNode.ChildNodes[1].ChildNodes[1].AstNode as VertexTypeListNode).Types).Select(tlnode => tlnode.TypeName).ToList();
            }

            #endregion

            _DumpType = (parseNode.ChildNodes[2].AstNode as DumpTypeNode).DumpType;
            _DumpFormat = (parseNode.ChildNodes[3].AstNode as DumpFormatNode).DumpFormat;
            _DumpableGrammar = context.Parser.Language.Grammar as IDumpable;

            if (_DumpableGrammar == null)
            {
                throw new NotADumpableGrammarException(context.Parser.Language.Grammar.GetType().ToString(), "");
            }

            if (HasChildNodes(parseNode.ChildNodes[4]))
            {
                _DumpDestination = parseNode.ChildNodes[4].ChildNodes[1].Token.ValueString;
            }
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "DUMP"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            var sw = Stopwatch.StartNew();

            QueryResult result = null;

            if (_DumpFormat.ToString().ToUpper().Equals("GQL"))
            {
                var plugin = myPluginManager.GetAndInitializePlugin<IGraphDBExport>("GQLEXPORT");

                if (plugin != null)
                {
                    result = plugin.Export(_DumpDestination, _DumpableGrammar, myGraphDB, myGraphQL, mySecurityToken, myTransactionToken, _TypesToDump, _DumpType);
                }
            }

            sw.Stop();

            if (result != null)
            {
                return new QueryResult(myQuery, _DumpFormat.ToString(), (ulong)sw.ElapsedMilliseconds, result.TypeOfResult, result.Vertices, result.Error);
            }
            else
                return null;
        }

        #endregion

        private QueryResult GenerateOutput(IRequestStatistics myStats)
        {
            return new QueryResult(_Query, "GQL", Convert.ToUInt64(myStats.ExecutionTime.Milliseconds), ResultType.Successful, new List<IVertexView>());
        }
    }
}

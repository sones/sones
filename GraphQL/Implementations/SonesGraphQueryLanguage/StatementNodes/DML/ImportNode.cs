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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphDB;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Nodes.DML;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.DataStructures;
using sones.Plugins.SonesGQL.DBImport;
using sones.GraphQL.Structure.Nodes.Misc;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class ImportNode : AStatement, IAstNodeInit
    {
        #region Properties

        public String ImportFormat { get; private set; }
        public String SourceLocation { get; private set; }
        public UInt32 ParallelTasks { get; private set; }
        public List<String> Comments { get; private set; }
        public UInt64? Offset { get; private set; }
        public UInt64? Limit { get; private set; }
        public VerbosityTypes VerbosityType { get; private set; }
		public Dictionary<string, string> Options { get; private set; }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            // parseNode.ChildNodes[0] - import symbol
            // parseNode.ChildNodes[1] - from symbol
            SourceLocation = parseNode.ChildNodes[2].Token.ValueString;
            // parseNode.ChildNodes[3] - format symbol
            ImportFormat = parseNode.ChildNodes[4].Token.Text;

            ParallelTasks = (parseNode.ChildNodes[5].AstNode as ParallelTasksNode).ParallelTasks;
            Comments = (parseNode.ChildNodes[6].AstNode as CommentsNode).Comments;
            Offset = (parseNode.ChildNodes[7].AstNode as OffsetNode).Count;
            Limit = (parseNode.ChildNodes[8].AstNode as LimitNode).Count;
            VerbosityType = (parseNode.ChildNodes[9].AstNode as VerbosityNode).VerbosityType;
			Options = (parseNode.ChildNodes[10].AstNode as OptionsNode).Options;				
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Import"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, 
			IGraphQL myGraphQL,
			GQLPluginManager myPluginManager,
			String myQuery,
			SecurityToken mySecurityToken,
			TransactionToken myTransactionToken)
        {
            var sw = Stopwatch.StartNew();

            QueryResult result = null;

            var plugin = myPluginManager.GetAndInitializePlugin<IGraphDBImport>(ImportFormat.ToUpper());

            if (plugin != null)
            {
                result = plugin.Import(SourceLocation, 
					myGraphDB,
					myGraphQL,
					mySecurityToken,
					myTransactionToken,
					ParallelTasks,
					Comments,
					Offset,
					Limit,
					VerbosityType,
					Options);
            }

            sw.Stop();

            if(result != null)
            {
                return new QueryResult(myQuery,
					ImportFormat,
					(ulong)sw.ElapsedMilliseconds,
					result.TypeOfResult,
					result.Vertices,
					result.Error);
            }
            else
                return null;
        }

        #endregion
    }
}

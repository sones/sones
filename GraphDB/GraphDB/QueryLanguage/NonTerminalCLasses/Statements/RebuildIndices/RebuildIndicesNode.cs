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


/* <id name="sones GraphDB – RebuildIndicesNode Node" />
 * <copyright file="RebuildIndicesNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary>Rebuild indices after an insert.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class RebuildIndicesNode : AStatement
    {
        #region Data

        private HashSet<String> _Types;

        #endregion

        #region constructors

        public RebuildIndicesNode()
        { }

        #endregion

        public override string StatementName
        {
            get { return "REBUILD INDICES"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _Types = new HashSet<string>();

            var dbTypeContext = (DBContext)context.IContext;

            if (parseNode.ChildNodes[2].HasChildNodes())
            {
                parseNode.ChildNodes[2].ChildNodes[0].ChildNodes.ForEach(item => _Types.Add(((ATypeNode)item.AstNode).ReferenceAndType.TypeName));
            }
        }

        public override QueryResult Execute(IGraphDBSession graphDBSession, DBContext dbContext)
        {
            using (var transaction = graphDBSession.BeginTransaction())
            {
                Exceptional<Boolean> rebuildResult = null;

                IEnumerable<GraphDBType> typesToRebuild;
                QueryResult result = new QueryResult();

                if (_Types.Count == 0)
                {
                    typesToRebuild = dbContext.DBTypeManager.GetAllTypes(false);
                }
                else
                {

                    #region Get types by name and return on error

                    typesToRebuild = new HashSet<GraphDBType>();
                    foreach (var typeName in _Types)
                    {
                        var type = transaction.GetDBContext().DBTypeManager.GetTypeByName(typeName);
                        if (type == null)
                        {
                            return new QueryResult(new Errors.Error_TypeDoesNotExist(typeName));
                        }
                        (typesToRebuild as HashSet<GraphDBType>).Add(type);
                    }

                    #endregion

                }

                rebuildResult = transaction.GetDBContext().DBIndexManager.RebuildIndices(typesToRebuild);

                if (!rebuildResult.Success)
                {
                    result = new QueryResult(rebuildResult.Errors);

                    result.AddErrorsAndWarnings(transaction.Rollback());

                    return result;
                }
                else
                {
                    return new QueryResult(transaction.Commit());
                }
            }
        }
    }
}

/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="GraphDB – RebuildIndicesNode Node" />
 * <copyright file="RebuildIndicesNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary>Rebuild indices after an insert.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.GraphDB.TypeManagement;

using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.RebuildIndices
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

            if (parseNode.ChildNodes[2].HasChildNodes())
            {
                parseNode.ChildNodes[2].ChildNodes[0].ChildNodes.ForEach(item => _Types.Add(((ATypeNode)item.AstNode).ReferenceAndType.TypeName));
            }
        }

        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {

            var qresult = graphDBSession.RebuildIndices(_Types);
            qresult.PushIExceptional(ParsingResult);
            return qresult;

        }
    
    }

}

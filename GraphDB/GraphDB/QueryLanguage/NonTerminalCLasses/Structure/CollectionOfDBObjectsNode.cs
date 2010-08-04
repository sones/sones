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


/* <id name="sones GraphDB – CollectionOfDBObjectsNode node" />
 * <copyright file="CollectionOfDBObjectsNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of CollectionOfDBObjectsNode statement (SETOF or LISTOF).</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;

using sones.Lib.Frameworks.Irony.Parsing;

using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.DataStructures;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;

using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Objects;
using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{

    /// <summary>
    /// This node is requested in case of CollectionOfDBObjectsNode statement (SETOF or LISTOF).
    /// </summary>
    public class CollectionOfDBObjectsNode : AStructureNode, IAstNodeInit
    {

        public CollectionDefinition CollectionDefinition { get; private set; }

        #region constructor

        public CollectionOfDBObjectsNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var multiplyString = parseNode.ChildNodes[0].Token.Text.ToUpper();
            CollectionType collectionType;

            switch (multiplyString)
            {
                case DBConstants.LISTOF:

                    collectionType = CollectionType.List;

                    break;

                case DBConstants.SETOF:

                    collectionType = CollectionType.Set;

                    break;

                case DBConstants.SETOFUUIDS:

                    collectionType = CollectionType.SetOfUUIDs;

                    break;
                default:

                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            var tupleNode = (TupleNode)parseNode.ChildNodes[1].AstNode;
            if (tupleNode == null)
            {
                CollectionDefinition = new CollectionDefinition(collectionType, new TupleDefinition());
            }
            else
            {
                CollectionDefinition = new CollectionDefinition(collectionType, tupleNode.TupleDefinition);
            }

        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    }
}

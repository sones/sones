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

/* <id name="sones GraphDB – RemoveFromListAttrUpdate Node" />
 * <copyright file="RemoveFromListAttrUpdateNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an RemoveFromListAttrUpdate node.</summary>
 */

#region usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Enums;

using sones.GraphDB.QueryLanguage.Operators;

using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.Operator;

using sones.Lib;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// 
    /// </summary>
    public class RemoveFromListAttrUpdateNode : AStructureNode, IAstNodeInit
    {
        #region properties

        protected TupleNode _tupleNode;
        protected TypeAttribute _Attribute;
        protected String _AttrName = String.Empty;

        #endregion

        #region constructor

        public RemoveFromListAttrUpdateNode()
        {

        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var content = parseNode.FirstChild.AstNode as RemoveFromListAttrUpdateNode;
            _tupleNode = content.TupleNode;
            _Attribute = content.Attribute;
            _AttrName = content.AttributeName;
        }

        #region Accessessors

        public TupleNode TupleNode { get { return _tupleNode; } }
        public TypeAttribute Attribute { get { return _Attribute; } }
        public String AttributeName { get { return _AttrName; } }

        #endregion


        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    }//class
}//namespace

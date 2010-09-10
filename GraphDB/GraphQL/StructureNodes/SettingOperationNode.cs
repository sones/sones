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


#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class SettingOperationNode : AStructureNode, IAstNodeInit
    {

        TypesOfSettingOperation _OperationType;
        public TypesOfSettingOperation OperationType
        {
            get { return _OperationType; }
        }

        Dictionary<String, String> _Settings = null;
        public Dictionary<String, String> Settings
        {
            get { return _Settings; }
        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            _Settings = new Dictionary<String, String>();

            foreach (var _ParseTreeNode1 in parseNode.ChildNodes)
            {

                if (_ParseTreeNode1.HasChildNodes())
                {

                    switch (_ParseTreeNode1.ChildNodes[0].Token.Text.ToUpper())
                    {
                        case "GET":
                            _OperationType = TypesOfSettingOperation.GET;
                            break;
                        case "SET":
                            _OperationType = TypesOfSettingOperation.SET;
                            break;
                        case "REMOVE":
                            _OperationType = TypesOfSettingOperation.REMOVE;
                            break;
                    }

                    foreach (var _ParseTreeNode2 in _ParseTreeNode1.ChildNodes[1].ChildNodes)
                    {

                        if (_ParseTreeNode2 != null)
                        {
                            if (_ParseTreeNode2.HasChildNodes())
                            {
                                if (_ParseTreeNode2.ChildNodes[0] != null)
                                {
                                    if (_ParseTreeNode2.ChildNodes[2] != null)
                                    {
                                        if (_ParseTreeNode2.ChildNodes[0].Token != null && _ParseTreeNode2.ChildNodes[2].Token != null)
                                        {
                                            var Temp = _ParseTreeNode2.ChildNodes[2].Token.Text.ToUpper();

                                            if (Temp.Contains("DEFAULT"))
                                                _Settings.Add(_ParseTreeNode2.ChildNodes[0].Token.ValueString.ToUpper(), Temp);
                                            else
                                                _Settings.Add(_ParseTreeNode2.ChildNodes[0].Token.ValueString.ToUpper(), _ParseTreeNode2.ChildNodes[2].Token.ValueString.ToUpper());
                                        }
                                    }
                                }
                            }

                            else
                            {
                                if (_ParseTreeNode2.Token != null)
                                    _Settings.Add(_ParseTreeNode2.Token.ValueString, "");
                            }
                        }

                    }

                }

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

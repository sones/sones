
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

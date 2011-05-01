using System;
using Irony.Ast;
using Irony.Parsing;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class IndexDropOnAlterType : AStructureNode, IAstNodeInit
    {
        #region Data

        private Dictionary<String, String> _DropIndexList;

        #endregion

        #region Accessors

        public Dictionary<String, String> DropIndexList
        {
            get { return _DropIndexList; }
        }

        #endregion

        #region constructors

        public IndexDropOnAlterType()
        { }

        #endregion


        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _DropIndexList = new Dictionary<String, String>();


            if (parseNode.ChildNodes[1].ChildNodes.Count == 2 && parseNode.ChildNodes[1].ChildNodes.TrueForAll(item => !HasChildNodes(item)))
            {
                var idxName = parseNode.ChildNodes[1].ChildNodes[0].Token.Text;
                var idxEdition = ((EditionOptNode)parseNode.ChildNodes[1].ChildNodes[1].AstNode).IndexEdition;

                if (!_DropIndexList.ContainsKey(idxName))
                {
                    _DropIndexList.Add(idxName, idxEdition);
                }
            }
            else
            {
                foreach (var nodes in parseNode.ChildNodes[1].ChildNodes)
                {
                    var idxName = nodes.ChildNodes[0].Token.Text;
                    var idxEdition = ((EditionOptNode)nodes.ChildNodes[1].AstNode).IndexEdition;

                    if (!_DropIndexList.ContainsKey(idxName))
                    {
                        _DropIndexList.Add(idxName, idxEdition);
                    }
                }
            }
        }

        #endregion
    }
}

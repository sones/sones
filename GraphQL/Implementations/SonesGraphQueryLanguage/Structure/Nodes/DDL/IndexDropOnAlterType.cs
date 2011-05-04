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

            if (HasChildNodes(parseNode.ChildNodes[1]) &&
                HasChildNodes(parseNode.ChildNodes[1].ChildNodes[0]))
            {
                foreach (var aIndexNameToken in parseNode.ChildNodes[1].ChildNodes[0].ChildNodes)
                {
                    _DropIndexList.Add(aIndexNameToken.Token.ValueString, String.Empty);
                }
            }
        }

        #endregion
    }
}

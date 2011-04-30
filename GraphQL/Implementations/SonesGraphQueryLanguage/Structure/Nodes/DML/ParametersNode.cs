using System;
using Irony.Ast;
using Irony.Parsing;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// A list of parameters passed to a add method of a AListEdgeType.
    /// </summary>
    public sealed class ParametersNode : AStructureNode, IAstNodeInit
    {
        private Dictionary<string, object> _ParameterValues;
        public Dictionary<string, object> ParameterValues
        {
            get { return _ParameterValues; }
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _ParameterValues = new Dictionary<string, object>();
            if (HasChildNodes(parseNode) && HasChildNodes(parseNode.ChildNodes[1]))
            {
                foreach (var child in parseNode.ChildNodes[1].ChildNodes)
                {
                    _ParameterValues.Add(child.FirstChild.Token.ValueString, child.LastChild.Token.Value);
                }
            }
        }

        #endregion
    }
}

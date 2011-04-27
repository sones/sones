using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.StatementNodes.DML;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class PartialSelectStmtNode : AStructureNode, IAstNodeInit
    {
        #region Data

        QueryResult _queryResult = null;

        #endregion

        #region Properties
        
        public SelectDefinition SelectDefinition { get; private set; }

        #endregion

        #region constructor

        public PartialSelectStmtNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var aSelectNode = (SelectNode)parseNode.ChildNodes[0].AstNode;

            SelectDefinition = new SelectDefinition(aSelectNode.TypeList, aSelectNode.SelectedElements, aSelectNode.WhereExpressionDefinition,
                aSelectNode.GroupByIDs, aSelectNode.Having, aSelectNode.Limit, aSelectNode.Offset, aSelectNode.OrderByDefinition, aSelectNode.ResolutionDepth);

        }

        #endregion
    }
}
